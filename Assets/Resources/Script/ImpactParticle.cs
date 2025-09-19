using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _impactParticle;

    // 手动触发粒子发射
    public void TriggerParticle()
    {
        _impactParticle.Play();   // 播放粒子
        // rippleParticle.Clear(); // 清除已有粒子
    }
}
