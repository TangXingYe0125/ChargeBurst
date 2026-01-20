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


    [SerializeField] private GameObject _guideControl;
    [SerializeField] private List<GameObject> _pages = new List<GameObject>();
    [SerializeField] private RectTransform _backPauseButtonRect;
    [SerializeField] private RectTransform _nextPageButtonRect;
    [SerializeField] private RectTransform _prevPageButtonRect;
    [SerializeField] private Animator _guideCanvasAnimator;
    [SerializeField] private Animator _attackGIFAnimator;
    [SerializeField] private Animator _chargeGIFAnimator;
    private int _currentPage = 0;
    private bool _isAnimating = false;
    void Start()
    {
        _menu.SetActive(true);
        _volumeControl.SetActive(false);
        _guideControl.SetActive(false);
        foreach (var page in _pages)
            page.SetActive(false);
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

    public void ShowGuide()
    {
        if (_isAnimating) return;
        _guideControl.SetActive(true);
        _currentPage = 0;
        UpdatePage();
        ResetButtonScales();
    }
    public void BackToPause()
    {
        if (_isAnimating) return;
        StartCoroutine(CloseGuideCanvas());
        ResetButtonScales();
    }
    public void NextPage()
    {
        if (_isAnimating) return;
        _currentPage++;
        UpdatePage();
        ResetButtonScales();
    }

    public void PrevPage()
    {
        if (_isAnimating) return;
        _currentPage--;
        UpdatePage();
        ResetButtonScales();
    }
    private void UpdatePage()
    {
        for (int i = 0; i < _pages.Count; i++)
        {
            bool isActive = (i == _currentPage);
            _pages[i].SetActive(isActive);

            if (isActive)
            {
                var gifPlayers = _pages[i].GetComponentsInChildren<GifImporter.GifPlayer>(true);
                foreach (var gif in gifPlayers)
                    gif.Restart();
            }
        }
    }

    private IEnumerator CloseGuideCanvas()
    {
        _isAnimating = true;

        _guideCanvasAnimator.SetTrigger("CloseGuideCanvas");
        _attackGIFAnimator.SetTrigger("CloseGIF");
        _chargeGIFAnimator.SetTrigger("CloseGIF");

        yield return new WaitForSecondsRealtime(1.0f);

        _guideControl.SetActive(false);

        _isAnimating = false;
    }
}
