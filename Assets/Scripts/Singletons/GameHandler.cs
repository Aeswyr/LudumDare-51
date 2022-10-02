using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : Singleton<GameHandler>
{
    [SerializeField] private GameObject templateAttack;
    [SerializeField] private RuntimeAnimatorController[] attacks;
    public GameObject AttackBuilder(AttackType type = AttackType.Default, Vector3 position = default, Transform parent = null, Collider2D owner = null, float destroyDelay = 0f, bool destroyOnHit = false) {
        GameObject obj = templateAttack;

        if (owner != null)
            obj.GetComponent<HitboxController>().Init(owner, destroyOnHit);

        GameObject attack;
        if (parent != null) {
            attack = Instantiate(obj, parent);
            attack.transform.localPosition = position;
        } else {
            attack= Instantiate(obj, position, Quaternion.identity);
        }

        if (destroyDelay != 0) {
            attack.AddComponent<DestroyAfterDelay>().Init(destroyDelay);
        }

        if (type != AttackType.Default) {
            attack.AddComponent<Animator>().runtimeAnimatorController = attacks[(int)type - 1];
        }

        return attack;
    }
}

public enum AttackType {
    Default,
}
