using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class VolumeController : MonoBehaviour
{
    public static VolumeController instance;

    [SerializeField] private AudioMixer audioMixer;

    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float seVolume = 1f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolumeSettings();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyVolume(); 
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
        ApplyVolume();
    }

    public void SetSEVolume(float value)
    {
        seVolume = value;
        PlayerPrefs.SetFloat("SE_VOLUME", value);
        ApplyVolume();
    }

    public void ApplyVolume()
    {
        if (audioMixer == null) return;

        audioMixer.SetFloat("BGM_VOLUME", LinearToDb(bgmVolume));
        audioMixer.SetFloat("SE_VOLUME", LinearToDb(seVolume));
    }

    private float LinearToDb(float value)
    {
        return value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;
    }
}
