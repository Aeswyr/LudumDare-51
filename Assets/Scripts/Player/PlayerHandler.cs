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

    [Header("ActionData")]
    [SerializeField] private AnimationCurve rollCurve;
    [SerializeField] private AnimationCurve attackCurve;
    private int facing = 1;
    private bool grounded;
    private bool acting;
    private float jumpLockout;
    private const float JUMP_LOCK = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
                animator.SetTrigger("parry");
                acting = true;
                move.StartDeceleration();
            } else {
                animator.SetTrigger("roll");
                move.OverrideCurve(30, rollCurve, InputHandler.Instance.dir);
                acting = true;
            }
        } else if (InputHandler.Instance.primary.pressed && !acting) {
            animator.SetTrigger("attack");
            if (InputHandler.Instance.dir != 0)
                move.OverrideCurve(30, attackCurve, InputHandler.Instance.dir);
            acting = true;
        } else if (InputHandler.Instance.secondary.pressed && !acting) {
            animator.SetTrigger("cast");
            move.StartDeceleration();
            acting = true;
        } else if (InputHandler.Instance.jump.pressed && grounded && !acting) {
            jump.StartJump();
            jumpLockout = Time.time + JUMP_LOCK;
            animator.SetTrigger("jump");
        }

        animator.SetBool("grounded", Time.time >= jumpLockout && grounded);
        animator.SetBool("running", InputHandler.Instance.dir != 0 && !acting);

        sprite.flipX = facing == -1;
    
    }


    public void EndAction() {
        acting = false;
        if (InputHandler.Instance.move.down) {
            move.ResetCurves();
        } else {
            move.ForceStop();
        }

    }
}
