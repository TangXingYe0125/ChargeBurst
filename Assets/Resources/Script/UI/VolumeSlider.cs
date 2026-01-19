using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _seSlider;

    private void OnEnable()
    {
        _bgmSlider.SetValueWithoutNotify(VolumeController.instance.bgmVolume);
        _seSlider.SetValueWithoutNotify(VolumeController.instance.seVolume);

        _bgmSlider.onValueChanged.RemoveAllListeners();
        _seSlider.onValueChanged.RemoveAllListeners();

        _bgmSlider.onValueChanged.AddListener(VolumeController.instance.SetBGMVolume);
        _seSlider.onValueChanged.AddListener(VolumeController.instance.SetSEVolume);
    }
}
