using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXHandler : Singleton<VFXHandler>
{
    [SerializeField] private Animator fade;
    [SerializeField] private GameObject particleTemplate;
    [SerializeField] private AnimationClip[] particles;

    Vector3 cameraPos;
    private void Start() {
        cameraPos = Camera.main.transform.position;
    }
    
    public void ParticleBuilder(ParticleType type, Vector3 position = default, bool destroyOnExpire = false, string layer = "VFX", bool flipX = false) {
        GameObject particle = Instantiate(particleTemplate, position, Quaternion.identity);

        var animator = particle.GetComponent<Animator>();

        AnimatorOverrideController anim = new AnimatorOverrideController(animator.runtimeAnimatorController);
        anim["particle"] = particles[(int)(type - 1)];

        animator.runtimeAnimatorController = anim;

        if (destroyOnExpire) {
            particle.AddComponent<DestroyAfterDelay>().Init(particles[(int)(type - 1)].length);
        }

        var sprite = particle.GetComponent<SpriteRenderer>();
        sprite.sortingLayerName = layer;
        sprite.flipX = flipX;
    }

    public void ScreenShake(float duration, float magnitude) {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude) {
        int scale = 60;
        int scaledDuration = (int)(duration * scale);
        for (int i = scaledDuration; i > 0; i--) {
            Camera.main.transform.position = cameraPos + (Vector3) (magnitude * scaledDuration * Random.insideUnitCircle);

            yield return new WaitForSeconds(1f/scale);
        }

        Camera.main.transform.position = cameraPos;
    }

    public void FadeIn() {
        fade.SetTrigger("fadeOut");
    }

    public void FadeOut() {
        fade.SetTrigger("fadeIn");
    }
}

public enum ParticleType {
    DEFAULT, SPELLBRAND_FIRE, VFX_PARRY, VFX_DEFLECT, VFX_HITSPARK,
    DEATH_DRONE, VFX_EXPLOSION, DUST_ROLL, DUST_SMALL, DUST_LAUNCH,
    VFX_SPAWN,
}
