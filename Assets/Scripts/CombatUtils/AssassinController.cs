using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinController : MonoBehaviour
{
    private float nextFire;
    [SerializeField] private float fireDelay;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D hurtbox;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject afterImagePrefab;
    void Start() {
        nextFire = Time.time + 1f;
    }
    bool acting = false;
    bool attacking = false;
    int facing = 1;

    long tick;
    void FixedUpdate() {
        tick++;
        if (Time.time > nextFire) {
            acting = true;
            animator.SetBool("acting", acting);
            animator.SetTrigger("ready");

            AudioHandler.Instance.Play(AudioType.ASSASSINATTACK);

            nextFire = Time.time + fireDelay + animator.GetCurrentAnimatorStateInfo(0).length;
        }

        if (!acting) {
            attacking = false;
            facing = (int)Mathf.Sign(GameHandler.Instance.GetPlayer().transform.position.x - transform.position.x);
            sprite.flipX = facing < 0;
        } else if (!attacking) {
            if (tick % 5 == 0) {
                var afterImage = Instantiate(afterImagePrefab, transform.position, Quaternion.identity);
                var asprite = afterImage.GetComponent<SpriteRenderer>();
                asprite.sprite = sprite.sprite;
                asprite.flipX = sprite.flipX;
            }

            transform.position += (Vector3)(facing * 0.7f * Vector2.right);

            if (Mathf.Abs(GameHandler.Instance.GetPlayer().transform.position.x - transform.position.x) < 2
                || Utils.Raycast(transform.position, facing * Vector2.right, 3, LayerMask.GetMask(new []{"World"}))) {
                animator.Play("attack", 0);
            }
        }

    }

    void EndAction() {
        acting = false;
        animator.SetBool("acting", acting);
    }

    void FireAttack() {
        GameHandler.Instance.AttackBuilder(AttackType.SecGuard, new Vector2(3, 3), facing * 1.5f * Vector2.right, transform, owner: hurtbox, destroyDelay: 2/12f, ignoreEnemies: true);
        attacking = true;
    }
}
