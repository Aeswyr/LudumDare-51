using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] private ParticleType onDeathParticle;
    public void OnHit(HitboxData attackData, Collider2D other) {
        VFXHandler.Instance.ScreenShake(0.1f, 0.05f);
        Destroy(gameObject);
        VFXHandler.Instance.ParticleBuilder(onDeathParticle, transform.position, true, "Entity");
    }
}
