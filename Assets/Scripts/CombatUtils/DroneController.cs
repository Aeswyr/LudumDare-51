using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    private float nextFire;
    [SerializeField] private float fireDelay;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D hurtbox;
    [SerializeField] private SpriteRenderer sprite;

    void Start() {
        nextFire = Time.time + 1f;
    }
    bool acting = false;
    int facing = 1;
    void FixedUpdate() {
        if (Time.time > nextFire) {
            acting = true;
            animator.SetBool("acting", acting);
            animator.SetTrigger("ready");
            
            nextFire = Time.time + fireDelay + animator.GetCurrentAnimatorStateInfo(0).length;
        }

        if (!acting) {
            transform.position += (Vector3)(facing * 0.03f * Vector2.right);
            if (Utils.Raycast(transform.position, facing * Vector2.right, 1.5f, LayerMask.GetMask(new []{"World"}))) {
                facing *= -1;
                sprite.flipX = facing < 0;
            }
        } else {
            facing = (int)Mathf.Sign(GameHandler.Instance.GetPlayer().transform.position.x - transform.position.x);
            sprite.flipX = facing < 0;
        }
    }

    void EndAction() {
        acting = false;
        animator.SetBool("acting", acting);
    }

    void FireAttack() {
        AudioHandler.Instance.Play(AudioType.DRONEATTACK);
        GameHandler.Instance.AttackBuilder(AttackType.Drone, new Vector2(2, 2), transform.position, owner: hurtbox, destroyOnHit: true, speed: 15, direction: (GameHandler.Instance.GetPlayer().transform.position - transform.position).normalized, ignoreEnemies: true);
    }
}
