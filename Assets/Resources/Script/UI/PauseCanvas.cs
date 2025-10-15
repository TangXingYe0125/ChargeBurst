using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PauseCanvas : MonoBehaviour
{
    [SerializeField] private GameObject _volumeControl;
    [SerializeField] private GameObject _menu;
    private Vector3 defaultScale;
    [SerializeField] private RectTransform _backButtonRect;
    [SerializeField] private RectTransform _soundButtonRect;
    void Start()
    {
        _menu.SetActive(true);
        _volumeControl.SetActive(false);
        defaultScale = _backButtonRect.localScale;
        ResetButtonScales();
    }
    public void ShowVolume()
    {
        _menu.SetActive(false);
        _volumeControl.SetActive(true);
        ResetButtonScales();
    }
    public void CloseVolume()
    {
        _volumeControl.SetActive(false);
        _menu.SetActive(true);
        ResetButtonScales();
    }
    private void ResetButtonScales()
    {
        _backButtonRect.localScale = defaultScale;
        _soundButtonRect.localScale = defaultScale;
    }
}
