using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : Singleton<GameHandler>
{
    [SerializeField] private GameObject templateAttack;
    [SerializeField] private RuntimeAnimatorController[] attacks;
    public GameObject AttackBuilder(AttackType type = AttackType.Default, Vector3 position = default, Transform parent = null, Collider2D owner = null, float destroyDelay = 0f, bool destroyOnHit = false, float speed = 0, Vector2 direction = default, ParticleType particle = ParticleType.DEFAULT, bool ignoreEnemies = false) {
        GameObject attack;
        if (parent != null) {
            attack = Instantiate(templateAttack, parent);
            attack.transform.localPosition = position;
        } else {
            attack = Instantiate(templateAttack, position, Quaternion.identity);
        }

        if (owner != null)
            attack.GetComponent<HitboxController>().Init(owner, destroyOnHit, particle, ignoreEnemies);

        if (destroyDelay != 0) {
            attack.AddComponent<DestroyAfterDelay>().Init(destroyDelay);
        }

        if (type != AttackType.Default) {
            attack.AddComponent<SpriteRenderer>().sortingLayerName = "VFX";
            attack.AddComponent<Animator>().runtimeAnimatorController = attacks[(int)type - 1];
        }

        if (speed > 0) {
            attack.AddComponent<ProjectileController>().Init(speed, direction);
        }

        return attack;
    }

    GameObject player;
    public GameObject GetPlayer() {
        if (player == null) {
            player = FindObjectOfType<PlayerHandler>().gameObject;
        }
        return player;
    }
}



public enum AttackType {
    Default, Fire, Drone
}
