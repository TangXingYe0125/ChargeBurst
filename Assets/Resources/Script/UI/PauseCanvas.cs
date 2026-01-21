using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : MonoBehaviour
{
    [SerializeField] private GameObject _volumeControl;
    [SerializeField] private GameObject _menu;

    private Vector3 defaultScale = Vector3.one;
    [SerializeField] private RectTransform _backButtonRect;
    [SerializeField] private RectTransform _soundButtonRect;
    [SerializeField] private RectTransform _guideButtonRect;
    [SerializeField] private RectTransform _nextPageButtonRect;
    [SerializeField] private RectTransform _prevPageButtonRect;
    [SerializeField] private RectTransform _closeButtonRect;

    [SerializeField] private GameObject _guideControl;
    [SerializeField] private List<GameObject> _pages = new List<GameObject>();

    [SerializeField] private Animator _attackGIFAnimator;
    [SerializeField] private Animator _chargeGIFAnimator;
    [SerializeField] private Animator _guideCanvasAnimator;
    private int _currentPage = 0;
    private bool _isAnimating = false;

    void Start()
    {
        _menu.SetActive(true);
        _volumeControl.SetActive(false);
        _guideControl.SetActive(false);

        foreach (var page in _pages)
            page.SetActive(false);

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
        _guideButtonRect.localScale = defaultScale;
        _nextPageButtonRect.localScale = defaultScale;
        _prevPageButtonRect.localScale = defaultScale;
        _closeButtonRect.localScale = defaultScale;
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
        if (_currentPage >= _pages.Count - 1) return;

        _currentPage++;
        UpdatePage();
        ResetButtonScales();
    }

    public void PrevPage()
    {
        if (_isAnimating) return;
        if (_currentPage <= 0) return;

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
                var animators = _pages[i].GetComponentsInChildren<Animator>(true);
                foreach (var animator in animators)
                {
                    animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                    animator.Play("GIF_Start", 0, 0f);
                }
            }
        }
    }

    private IEnumerator CloseGuideCanvas()
    {
        _isAnimating = true;

        _guideCanvasAnimator.SetBool("IsClosing", true);
        _attackGIFAnimator.SetBool("IsClosing", true);
        _chargeGIFAnimator.SetBool("IsClosing", true);

        yield return new WaitForSecondsRealtime(1.0f);

        _guideControl.SetActive(false);

        _guideCanvasAnimator.SetBool("IsClosing", false);
        _attackGIFAnimator.SetBool("IsClosing", false);
        _chargeGIFAnimator.SetBool("IsClosing", false);


        _isAnimating = false;
        ResetButtonScales();
    }
    void OnEnable()
    {
        _menu.SetActive(true);
        _volumeControl.SetActive(false);
        _guideControl.SetActive(false);

        _isAnimating = false;
        ResetButtonScales();
    }
}
