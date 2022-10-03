using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    private float nextFire;
    [SerializeField] private float fireDelay;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D hurtbox;

    void Start() {
        nextFire = Time.time + fireDelay;
    }
    bool acting = false;
    void FixedUpdate() {
        if (Time.time > nextFire) {
            nextFire = Time.time + fireDelay;
            acting = true;
            animator.SetBool("acting", acting);
            animator.SetTrigger("ready");
        }

        if (!acting) {

        }
    }

    void EndAction() {
        acting = false;
        animator.SetBool("acting", acting);
    }

    void FireAttack() {
        GameHandler.Instance.AttackBuilder(AttackType.Drone, transform.position, owner: hurtbox, destroyOnHit: true, speed: 15, direction: (GameHandler.Instance.GetPlayer().transform.position - transform.position).normalized, ignoreEnemies: true);
    }
}
