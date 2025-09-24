using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _impactParticle;
    public void TriggerParticle()
    {
        _impactParticle.Play();
    }
}
