using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HitboxController : MonoBehaviour
{
    [SerializeField] HitboxData attackData;
    bool destroyOnHit;
    Collider2D owner;
    ParticleType particle;
    bool createsParticleOnHit;
    public void Init(Collider2D owner, bool destroyOnHit, ParticleType particle = ParticleType.DEFAULT) {
        this.owner = owner;
        this.destroyOnHit = destroyOnHit;
        this.particle = particle;
        if (particle != ParticleType.DEFAULT)
            createsParticleOnHit = true;;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other == owner)
            return;

        if (other.transform.parent.TryGetComponent(out PlayerHandler player))
            player.OnHit(attackData, other);
        else if (other.transform.parent.TryGetComponent(out CombatController target)) {
            target.OnHit(attackData, other);
            if (transform.parent != null && transform.parent.TryGetComponent(out PlayerHandler attackingPlayer))
                attackingPlayer.HitPause(0.15f);
        }

        if (createsParticleOnHit) {
            Vector2 dif = other.transform.position - transform.position;
                if (other.gameObject.layer == LayerMask.NameToLayer("World")) {
                    dif = transform.position.x - owner.transform.position.x  < 0 ? Vector2.left : Vector2.right;
                }
                    
            VFXHandler.Instance.ParticleBuilder(particle, (Vector2)transform.position + 0.5f * dif, true, flipX: dif.x < 0);
        }

        if (this.destroyOnHit) {
            Destroy(gameObject);
        }
    }
}

[Serializable] public struct HitboxData {
    int damage;

}
