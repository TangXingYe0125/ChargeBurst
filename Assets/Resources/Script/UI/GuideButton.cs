using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GuideButton : MonoBehaviour
{
    [SerializeField] private GameObject _guideCanvas;
    [SerializeField] private GameObject _volumeCanvas;
    [SerializeField] private List<GameObject> _pages = new List<GameObject>();
    [SerializeField] private RectTransform _backTitleButtonRect;
    [SerializeField] private RectTransform _nextPageButtonRect;
    [SerializeField] private RectTransform _prevPageButtonRect;
    [SerializeField] private RectTransform _closeButtonRect;
    [SerializeField] private RectTransform _soundButtonRect;

    [SerializeField] private Animator _guideCanvasAnimator;
    [SerializeField] private Animator _volumeCanvasAnimator;
    [SerializeField] private Animator _attackGIFAnimator;
    [SerializeField] private Animator _chargeGIFAnimator;

    private Vector3 defaultScale;
    private int _currentPage = 0;
    private bool _isAnimating = false; 

    void Start()
    {
        _guideCanvas.SetActive(false);
        _volumeCanvas.SetActive(false);
        foreach (var page in _pages)
            page.SetActive(false);

        defaultScale = _backTitleButtonRect.localScale;
        ResetButtonScales();
    }

    public void ShowGuide()
    {
        if (_isAnimating) return; 
        _guideCanvas.SetActive(true);
        _currentPage = 0;
        UpdatePage();
        ResetButtonScales();
    }

    public void BackToTitle()
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

    public void ShowVolume()
    {
        if (_isAnimating) return;
        _volumeCanvas.SetActive(true);
        ResetButtonScales();
    }
    public void CloseVolume()
    {
        if (_isAnimating) return;
        StartCoroutine(CloseVolumeCanvas());
        ResetButtonScales();
    }

    private void ResetButtonScales()
    {
        _backTitleButtonRect.localScale = defaultScale;
        _nextPageButtonRect.localScale = defaultScale;
        _prevPageButtonRect.localScale = defaultScale;
        _closeButtonRect.localScale = defaultScale;
        _soundButtonRect.localScale = Vector2.one;
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

        yield return new WaitForSeconds(1.0f);

        _guideCanvas.SetActive(false);

        _isAnimating = false; 
    }

    public IEnumerator CloseVolumeCanvas()
    {
        _isAnimating = true;
        _volumeCanvasAnimator.SetTrigger("CloseVolumeCanvas");
        yield return new WaitForSeconds(1.0f);
        _volumeCanvas.SetActive(false);
        _isAnimating = false;
    }
}
