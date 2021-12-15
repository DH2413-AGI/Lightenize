using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColorChanger : MonoBehaviour
{
    ParticleSystem ps;

    // these lists are used to contain the particles which match
    // the trigger conditions each frame.
    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    private List<ParticleSystem.Particle> _insideTotal = new List<ParticleSystem.Particle>();
    List<ParticleSystem.Particle> exit = new List<ParticleSystem.Particle>();

    void OnEnable()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnParticleTrigger()
    {
        // get the particles which matched the trigger conditions this frame
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, enter, out var insideData);
        int numExit = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Outside, exit);

        // iterate through the particles which entered the trigger and make them red
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enter[i];
            // ApplyColorToParticle(ps, p);
            p.startColor = GetColorFromColliders(insideData, i);
            enter[i] = p;
        }

        // iterate through the particles which exited the trigger and make them green
        for (int i = 0; i < numExit; i++)
        {
            ParticleSystem.Particle p = exit[i];
            p.startColor = new Color32(255, 255, 255, 0);
            exit[i] = p;
        }

        // re-assign the modified particles back into the particle system
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, enter);
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Outside, exit);
    }

    private Color GetColorFromColliders(ParticleSystem.ColliderData colliderData, int particleIndex)
    {
        var color = new Color(0f, 0f, 0f, 1.0f);
        var numOfColliders = colliderData.GetColliderCount(particleIndex);

        for (int i = 0; i < numOfColliders; i++)
        {
            var collider =  colliderData.GetCollider(particleIndex, i);
            var colliderColor = collider.GetComponentInParent<ColoredLight>().Color;
            color += colliderColor;
        }

        if (color.r == 0.0f) color.r = 0.5f;
        if (color.b == 0.0f) color.b = 0.5f;
        if (color.g == 0.0f) color.g = 0.5f;
        color.a = 1.0f;
        return color;
    }
}
