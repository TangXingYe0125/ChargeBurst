using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static CinemachineBasicMultiChannelPerlin _noise;
    private static float _shakeTime;

    void Awake()
    {
        var vcam = GetComponent<CinemachineVirtualCamera>();
        _noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public static void Shake(float amplitude, float time)
    {
        _shakeTime = time;
        _noise.m_AmplitudeGain = amplitude;
    }

    void Update()
    {
        if (_shakeTime > 0)
        {
            _shakeTime -= Time.deltaTime;
            if (_shakeTime <= 0)
            {
                _noise.m_AmplitudeGain = 0;
            }
        }
    }
}
