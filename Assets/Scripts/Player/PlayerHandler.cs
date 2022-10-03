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
    [SerializeField] private GameObject dustTrailPrefab;
    [SerializeField] private GameObject sparkburstPrefab;
    private List<GameObject> motes = new List<GameObject>();
    private int facing = 1;
    private bool grounded;
    private bool acting;
    private float jumpLockout;
    private const float JUMP_LOCK = 0.1f;
    private HurtboxMode hurtboxMode = HurtboxMode.HURT;
    private int energy;
    private const int MAX_ENERGY = 10;

    bool paused, inputsDisabled;
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
        if (inputsDisabled)
            return;

        if (Time.time < pausedUntil)
            return;
        if (paused) {
            paused = false;
            animator.speed = 1;
        }

        int prevFacing = facing;
        bool prevGrounded = grounded;
        grounded = ground.CheckGrounded();

        if (!prevGrounded && grounded) {
            VFXHandler.Instance.ParticleBuilder(ParticleType.DUST_LAUNCH, transform.position, true, flipX: facing == -1);
        }

        if (InputHandler.Instance.move.pressed && !acting) {
            move.StartAcceleration(InputHandler.Instance.dir);
            if (InputHandler.Instance.dir != 0)
                facing = (int)InputHandler.Instance.dir;
            if (grounded)
                VFXHandler.Instance.ParticleBuilder(ParticleType.DUST_SMALL, (Vector2)transform.position - facing * 2f * Vector2.right, true, flipX: facing == -1);
        } else if (InputHandler.Instance.move.down && !acting) {
            move.UpdateMovement(InputHandler.Instance.dir);
            if (InputHandler.Instance.dir != 0)
                facing = (int)InputHandler.Instance.dir;
        } else if (InputHandler.Instance.move.released && !acting) {
            move.StartDeceleration();
        }

        if (InputHandler.Instance.dodge.pressed && grounded && !acting) {
            if (InputHandler.Instance.dir == 0 && TrySpendEnergy(2)) {
                hurtboxMode = HurtboxMode.PARRY;
                animator.SetTrigger("parry");
                acting = true;
                move.StartDeceleration();
            } else if (InputHandler.Instance.dir != 0 && TrySpendEnergy()) {
                VFXHandler.Instance.ParticleBuilder(ParticleType.DUST_ROLL, transform.position, true, flipX: facing == -1);
                hurtbox.enabled = false;
                hurtboxMode = HurtboxMode.DODGE;
                animator.SetTrigger("roll");
                move.OverrideCurve(30, rollCurve, InputHandler.Instance.dir);
                acting = true;
            }
        } else if (InputHandler.Instance.primary.pressed && !acting && TrySpendEnergy(2)) {
            animator.SetTrigger("attack");
            if (InputHandler.Instance.dir != 0 && grounded) {
                VFXHandler.Instance.ParticleBuilder(ParticleType.DUST_SMALL, transform.position, true, flipX: facing == -1);
                move.OverrideCurve(30, attackCurve, InputHandler.Instance.dir);
            }
            acting = true;
        } else if (InputHandler.Instance.secondary.pressed && !acting) {
            animator.SetTrigger("cast");
            if (grounded)
                move.StartDeceleration();
            acting = true;
        } else if (InputHandler.Instance.jump.pressed && grounded && !acting) {
            Instantiate(dustTrailPrefab, transform);
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

    public void SpendEnergy(int amount) {
            energy -= amount;
            for (int i = 0; i < amount; i++)
                HUDHandler.Instance.RemoveEnergy();
    }

    public void GainEnergy() {
        if (energy >= MAX_ENERGY)
            return;
        energy++;
        HUDHandler.Instance.AddEnergy();
    }

    public void RefillEnergy() {
        //AudioHandler.Instance.Play(AudioType.REFILL);
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

    bool TrySpendEnergy(int amount = 1) {
        if (energy >= amount) {
            SpendEnergy(amount);
            return true;
        }
        AudioHandler.Instance.Play(AudioType.NOENERGY);
        Instantiate(sparkburstPrefab, transform);
        return false;
    }

    public void OnAttack() {
        GameHandler.Instance.AttackBuilder(owner: hurtbox, parent: transform, destroyDelay: (1/6f), position: new Vector2(2 * facing, 0), particle: ParticleType.VFX_HITSPARK);
    }

    public void OnCast() {
        if (motes.Count > 0) {
            AudioHandler.Instance.Play(AudioType.FIREBALL);
            Destroy(motes[0]);
            motes.RemoveAt(0);
            GameHandler.Instance.AttackBuilder(type: AttackType.Fire, owner: hurtbox, destroyOnHit: true, position: (Vector2)transform.position + new Vector2(2 * facing, 0), speed: 40, direction: facing * Vector2.right, particle: ParticleType.VFX_EXPLOSION);
            VFXHandler.Instance.ParticleBuilder(ParticleType.SPELLBRAND_FIRE, moteHolder.transform.position, true, "Motes");
        } else {
            AudioHandler.Instance.Play(AudioType.NOENERGY);
        }
    }

    public void OnHit(HitboxData attackData, Transform other) {
        switch (hurtboxMode) {
            case HurtboxMode.HURT:
                AudioHandler.Instance.Play(AudioType.DEATH);
                DisableInputs();
                hurtbox.enabled = false;
                move.enabled = false;
                GameHandler.Instance.GetLevel().GetComponent<LevelController>().StopBlock();
                animator.SetTrigger("dying");
                transform.position += 0.1f * Vector3.up;
                transform.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(transform.position.x - other.position.x) * 20, 40);
                StartCoroutine(DieAndRestart());
            break;
            case HurtboxMode.PARRY:
                AudioHandler.Instance.Play(AudioType.PARRY);
                animator.Play("player_parry", 0, normalizedTime: 0.2f);
                VFXHandler.Instance.ParticleBuilder(ParticleType.VFX_PARRY, (Vector2)transform.position + new Vector2(facing * 1.5f, 0.5f), true);
                VFXHandler.Instance.ScreenShake(0.1f, 0.1f);
                TryAddMote();
                GainEnergy();
                HitPause(0.15f);
            break;
            case HurtboxMode.DODGE:
                AudioHandler.Instance.Play(AudioType.PARRY);
                VFXHandler.Instance.ParticleBuilder(ParticleType.VFX_DEFLECT, (Vector2)transform.position + new Vector2(facing * 1.5f, 0.5f), true);
                VFXHandler.Instance.ScreenShake(0.1f, 0.05f);
                HitPause(0.1f);
            break;
        }
    }

    public IEnumerator DieAndRestart() {

        yield return new WaitUntil(ground.CheckGrounded);
        animator.SetTrigger("dead");
        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        
        yield return new WaitForSeconds(0.5f);
        VFXHandler.Instance.FadeOut();
        yield return new WaitForSeconds(1f);
        
        GameHandler.Instance.GetLevel().GetComponent<LevelController>().EndBlock();
        transform.position = new Vector2(0, -4);
        animator.Play("player_idle");

        while (motes.Count > 0) {
            Destroy(motes[0]);
            motes.RemoveAt(0);
        }
        yield return new WaitForSeconds(0.5f);
        VFXHandler.Instance.FadeIn();
        yield return new WaitForSeconds(1f);
        move.enabled = true;
        GameHandler.Instance.GetLevel().GetComponent<LevelController>().StartBlock();
        EndAction();
        EnableInputs();
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

    public void DisableInputs() {
        move.ForceStop();
        jump.ForceLanding();
        inputsDisabled = true;
        animator.SetBool("grounded", true);
        animator.SetBool("running", false);
    }

    public void EnableInputs() {
        inputsDisabled = false;
    }

    private enum HurtboxMode {
        HURT, PARRY, DODGE
    }


}
