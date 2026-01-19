using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private AudioSource _bgm;

    private void Awake()
    {
        _bgm = GetComponent<AudioSource>();
    }

    private void Start()
    {
        VolumeController.instance.ApplyVolume();

        _bgm.Play();
    }
}
