using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : Singleton<GameHandler>
{
    [SerializeField] private GameObject templateAttack;
    [SerializeField] private RuntimeAnimatorController[] attacks;


    public void FixedUpdate() {
        if (InputHandler.Instance.menu.pressed)
            Application.Quit();
    }
    
    public GameObject AttackBuilder(AttackType type = AttackType.Default, Vector2 size = default, Vector3 position = default, Transform parent = null, Collider2D owner = null, float destroyDelay = 0f, bool destroyOnHit = false, float speed = 0, Vector2 direction = default, ParticleType particle = ParticleType.DEFAULT, bool ignoreEnemies = false) {
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

        if (size != default) {
            attack.GetComponent<BoxCollider2D>().size = size;
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

    GameObject level;
    public GameObject GetLevel() {
        if (level == null) {
            level = FindObjectOfType<LevelController>().gameObject;
        }
        return level;
    }
}



public enum AttackType {
    Default, Fire, Drone, SecGuard,
}
