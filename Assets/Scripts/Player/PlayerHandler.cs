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
    private int energy;
    private const int MAX_ENERGY = 10;

    bool paused = false;
    float pausedUntil;
    // Start is called before the first frame update
    void Start()
    {
        energy = MAX_ENERGY;
        for (int i = 0; i < energy; i++) {
            HUDHandler.Instance.AddEnergy();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time < pausedUntil)
            return;
        if (paused) {
            paused = false;
            animator.speed = 1;
        }

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

        if (InputHandler.Instance.dodge.pressed && grounded && !acting && energy > 0) {
            if (InputHandler.Instance.dir == 0) {
                hurtboxMode = HurtboxMode.PARRY;
                animator.SetTrigger("parry");
                acting = true;
                move.StartDeceleration();
            } else {
                hurtbox.enabled = false;
                hurtboxMode = HurtboxMode.DODGE;
                animator.SetTrigger("roll");
                move.OverrideCurve(30, rollCurve, InputHandler.Instance.dir);
                acting = true;
            }
            SpendEnergy();
        } else if (InputHandler.Instance.primary.pressed && !acting && energy > 0) {
            animator.SetTrigger("attack");
            if (InputHandler.Instance.dir != 0 && grounded)
                move.OverrideCurve(30, attackCurve, InputHandler.Instance.dir);
            acting = true;
        SpendEnergy();
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

    public void SpendEnergy() {
            energy--;
            HUDHandler.Instance.RemoveEnergy();
    }

    public void GainEnergy() {
        if (energy >= MAX_ENERGY)
            return;
        energy++;
        HUDHandler.Instance.AddEnergy();
    }

    public void RefillEnergy() {
        int diff = MAX_ENERGY - energy;
        for (int i = 0; i < diff; i++)
            HUDHandler.Instance.AddEnergy();
        energy = MAX_ENERGY;
    }

    public void EndAction() {
        acting = false;
        hurtbox.enabled = true;

        hurtboxMode = HurtboxMode.HURT;

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
        GameHandler.Instance.AttackBuilder(owner: hurtbox, parent: transform, destroyDelay: (1/6f), position: new Vector2(2 * facing, 0), particle: ParticleType.VFX_HITSPARK);
    }

    public void OnCast() {
        if (motes.Count > 0) {
            Destroy(motes[0]);
            motes.RemoveAt(0);
            GameHandler.Instance.AttackBuilder(type: AttackType.Fire, owner: hurtbox, destroyOnHit: true, position: (Vector2)transform.position + new Vector2(2 * facing, 0), speed: 40, direction: facing * Vector2.right, particle: ParticleType.VFX_EXPLOSION);
            VFXHandler.Instance.ParticleBuilder(ParticleType.SPELLBRAND_FIRE, moteHolder.transform.position, true, "Motes");
        }
    }

    public void OnHit(HitboxData attackData, Collider2D other) {
        switch (hurtboxMode) {
            case HurtboxMode.HURT:
            break;
            case HurtboxMode.PARRY:
                animator.Play("player_parry", 0, normalizedTime: 0.2f);
                VFXHandler.Instance.ParticleBuilder(ParticleType.VFX_PARRY, (Vector2)transform.position + new Vector2(facing * 1.5f, 0.5f), true);
                VFXHandler.Instance.ScreenShake(0.1f, 0.1f);
                TryAddMote();
                GainEnergy();
                HitPause(0.15f);
            break;
            case HurtboxMode.DODGE:
                VFXHandler.Instance.ParticleBuilder(ParticleType.VFX_DEFLECT, (Vector2)transform.position + new Vector2(facing * 1.5f, 0.5f), true);
                VFXHandler.Instance.ScreenShake(0.1f, 0.05f);
                HitPause(0.1f);
            break;
        }
    }

    public void HitPause(float duration) {
        if (paused && Time.time + duration > pausedUntil) {
            pausedUntil = Time.time + duration;
        } else {
            paused = true;
            pausedUntil = Time.time + duration;
            animator.speed = 0;
        }
        move.Pause(pausedUntil);
        jump.Pause(pausedUntil);
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
