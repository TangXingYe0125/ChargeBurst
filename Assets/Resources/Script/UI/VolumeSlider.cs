using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _seSlider;

    private void Start()
    {
        _bgmSlider.value = VolumeController.instance.bgmVolume;
        _seSlider.value = VolumeController.instance.seVolume;

        _bgmSlider.onValueChanged.AddListener((v) => VolumeController.instance.SetBGMVolume(v));
        _seSlider.onValueChanged.AddListener((v) => VolumeController.instance.SetSEVolume(v));
    }
}