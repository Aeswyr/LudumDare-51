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
    private int facing = 1;
    private bool grounded;
    private bool acting;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = ground.CheckGrounded();
        

        if (InputHandler.Instance.jump.pressed && grounded) {
            jump.StartJump();
        }

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
                move.OverrideCurve(20, rollCurve, InputHandler.Instance.dir);
                acting = true;
            }
        }

        sprite.flipX = facing == -1;


        animator.SetBool("grounded", grounded);
        animator.SetBool("running", InputHandler.Instance.dir != 0 && !acting);
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
