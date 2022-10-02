using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXHandler : Singleton<VFXHandler>
{
    [SerializeField] private GameObject particleTemplate;
    [SerializeField] private AnimationClip[] particles;
    
    public void ParticleBuilder(ParticleType type, Vector3 position = default, float destroyDelay = 0) {
        GameObject particle = Instantiate(particleTemplate, position, Quaternion.identity);

        var animator = particle.GetComponent<Animator>();

        AnimatorOverrideController anim = new AnimatorOverrideController(animator.runtimeAnimatorController);
        anim["particle"] = particles[(int)type];

        animator.runtimeAnimatorController = anim;

        if (destroyDelay != 0) {
            particle.AddComponent<DestroyAfterDelay>().Init(destroyDelay);
        }
    }
}

public enum ParticleType {
    SPELLBRAND_FIRE, 
}
