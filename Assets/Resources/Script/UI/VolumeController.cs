using UnityEngine;

public class VolumeController : MonoBehaviour
{
    public static VolumeController instance;

    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float seVolume = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadVolumeSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 1f);
        seVolume = PlayerPrefs.GetFloat("SE_VOLUME", 1f);
        ApplyVolume();
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        PlayerPrefs.SetFloat("BGM_VOLUME", value);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    public void SetSEVolume(float value)
    {
        seVolume = value;
        PlayerPrefs.SetFloat("SE_VOLUME", value);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    public void ApplyVolume()
    {
        foreach (var audio in FindObjectsOfType<AudioSource>())
        {
            if (audio.GetComponentInParent<Camera>() == Camera.main)
            {
                audio.volume = bgmVolume;
            }
            else
            {
                audio.volume = seVolume;
            }
        }
    }
}
