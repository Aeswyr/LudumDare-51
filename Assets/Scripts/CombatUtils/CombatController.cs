using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] private ParticleType onDeathParticle;
    public void OnHit(HitboxData attackData, Transform other) {
        VFXHandler.Instance.ScreenShake(0.1f, 0.05f);
        Destroy(gameObject);
        VFXHandler.Instance.ParticleBuilder(onDeathParticle, transform.position, true, "Entity");

        if (other != default) {
            GameHandler.Instance.GetLevel().GetComponent<LevelController>().AddScore(500);
            AudioHandler.Instance.Play(AudioType.HURT);
        }
    }
}
