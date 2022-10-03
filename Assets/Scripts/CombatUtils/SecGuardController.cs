using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecGuardController : MonoBehaviour
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

        facing = (int)Mathf.Sign(GameHandler.Instance.GetPlayer().transform.position.x - transform.position.x);
        sprite.flipX = facing < 0;
    }

    void EndAction() {
        acting = false;
        animator.SetBool("acting", acting);
    }

    void FireAttack() {
        AudioHandler.Instance.Play(AudioType.GUARDATTACK);
        GameHandler.Instance.AttackBuilder(AttackType.SecGuard, transform.position, owner: hurtbox, destroyOnHit: true, speed: 70, direction: Mathf.Sign((GameHandler.Instance.GetPlayer().transform.position - transform.position).x) * Vector2.right, ignoreEnemies: true);
    }
}
