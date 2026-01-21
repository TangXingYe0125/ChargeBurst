using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private Animator _attackGIFAnimator; 
    [SerializeField] private Animator _chargeGIFAnimator;
    [SerializeField] private Animator _guideCanvasAnimator;
    [SerializeField] private Animator _volumeCanvasAnimator;

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

        yield return new WaitForSeconds(1.0f);

        _guideCanvas.SetActive(false);

        _guideCanvasAnimator.SetBool("IsClosing", false);
        _attackGIFAnimator.SetBool("IsClosing", false);
        _chargeGIFAnimator.SetBool("IsClosing", false);

        _isAnimating = false;
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

    private IEnumerator CloseVolumeCanvas()
    {
        _isAnimating = true;

        _volumeCanvasAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        _volumeCanvasAnimator.SetTrigger("CloseVolumeCanvas");

        yield return new WaitForSeconds(1.0f);

        _volumeCanvas.SetActive(false);
        _isAnimating = false;
    }
    private void ResetButtonScales()
    {
        _backTitleButtonRect.localScale = defaultScale;
        _nextPageButtonRect.localScale = defaultScale;
        _prevPageButtonRect.localScale = defaultScale;
        _closeButtonRect.localScale = defaultScale;
        _soundButtonRect.localScale = Vector3.one;
    }
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
