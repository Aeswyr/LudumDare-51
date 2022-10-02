using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitboxController : MonoBehaviour
{
    [SerializeField] HitboxData attackData;
    private bool destroyOnHit;
    Collider2D owner;
    public void Init(Collider2D owner, bool destroyOnHit) {
        this.owner = owner;
        this.destroyOnHit = destroyOnHit;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other == owner)
            return;

        if (other.transform.parent.TryGetComponent(out PlayerHandler player))
            player.OnHit(attackData);
        else if (other.transform.parent.TryGetComponent(out CombatController target))
            target.OnHit(attackData);
        
        if (destroyOnHit)
            Destroy(gameObject);
    }
}

[Serializable] public struct HitboxData {
    int damage;

}
