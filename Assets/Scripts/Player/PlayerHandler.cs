using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] private JumpHandler jump;
    [SerializeField] private MovementHandler move;
    [SerializeField] private GroundedCheck ground;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D hurtbox;
    [SerializeField] private Transform moteHolder;

    [Header("ActionData")]
    [SerializeField] private AnimationCurve rollCurve;
    [SerializeField] private AnimationCurve attackCurve;
    [SerializeField] private GameObject moteTemplate;
    private List<GameObject> motes = new List<GameObject>();
    private int facing = 1;
    private bool grounded;
    private bool acting;
    private float jumpLockout;
    private const float JUMP_LOCK = 0.1f;
    private HurtboxMode hurtboxMode = HurtboxMode.HURT;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int prevFacing = facing;
        grounded = ground.CheckGrounded();

        if (InputHandler.Instance.move.pressed && !acting) {
            move.StartAcceleration(InputHandler.Instance.dir);
            if (InputHandler.Instance.dir != 0)
                facing = (int)InputHandler.Instance.dir;
        } else if (InputHandler.Instance.move.down && !acting) {
            move.UpdateMovement(InputHandler.Instance.dir);
            if (InputHandler.Instance.dir != 0)
                facing = (int)InputHandler.Instance.dir;
        } else if (InputHandler.Instance.move.released && !acting) {
            move.StartDeceleration();
        }

        if (InputHandler.Instance.dodge.pressed && grounded && !acting) {
            if (InputHandler.Instance.dir == 0) {
                hurtboxMode = HurtboxMode.PARRY;
                animator.SetTrigger("parry");
                acting = true;
                move.StartDeceleration();
            } else {
                hurtboxMode = HurtboxMode.DODGE;
                animator.SetTrigger("roll");
                move.OverrideCurve(30, rollCurve, InputHandler.Instance.dir);
                acting = true;
            }
        } else if (InputHandler.Instance.primary.pressed && !acting) {
            animator.SetTrigger("attack");
            if (InputHandler.Instance.dir != 0 && grounded)
                move.OverrideCurve(30, attackCurve, InputHandler.Instance.dir);
            acting = true;
        } else if (InputHandler.Instance.secondary.pressed && !acting) {
            animator.SetTrigger("cast");
            if (grounded)
                move.StartDeceleration();
            acting = true;
        } else if (InputHandler.Instance.jump.pressed && grounded && !acting) {
            jump.StartJump();
            jumpLockout = Time.time + JUMP_LOCK;
            animator.SetTrigger("jump");
        }

        animator.SetBool("grounded", Time.time >= jumpLockout && grounded);
        animator.SetBool("running", InputHandler.Instance.dir != 0 && !acting);

        if (prevFacing != facing) {
            StartCoroutine(MoveMote(facing * -1));
            sprite.flipX = facing == -1;
        }
    }

    public IEnumerator MoveMote(int targetPos) {
        for (int i = 1; i <= 30; i++) {
            yield return  new WaitForSeconds(0.02f);
            moteHolder.localPosition = Mathf.Lerp(moteHolder.localPosition.x, targetPos, i / 30f) * Vector3.right;
        }
    }


    public void EndAction() {
        acting = false;
        if (InputHandler.Instance.move.down) {
            move.ResetCurves();
        } else {
            move.ForceStop();
        }
    }

    public void EndParryFrames() {
        hurtboxMode = HurtboxMode.DODGE;
    }

    public void OnAttack() {
        GameHandler.Instance.AttackBuilder(owner: hurtbox, parent: transform, destroyDelay: (1/6f), position: new Vector2(2 * facing, 0));
    }

    public void OnCast() {
        if (motes.Count > 0) {
            Destroy(motes[0]);
            motes.RemoveAt(0);

            VFXHandler.Instance.ParticleBuilder(ParticleType.SPELLBRAND_FIRE, moteHolder.transform.position, 5/12f);
        }
    }

    public void OnHit(HitboxData attackData) {
        switch (hurtboxMode) {
            case HurtboxMode.HURT:
            break;
            case HurtboxMode.PARRY:
                TryAddMote();
            break;
            case HurtboxMode.DODGE:
            break;
        }
    }

    public void TryAddMote() {
        if (motes.Count >= 3)
            return;
        
        motes.Add(Instantiate(moteTemplate, moteHolder));
    }

    private enum HurtboxMode {
        HURT, PARRY, DODGE
    }
}
