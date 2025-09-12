using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideButton : MonoBehaviour
{
    [SerializeField] private GameObject _guideCanvas;
    //[SerializeField] private GameObject _movementGuide;
    //[SerializeField] private GameObject _attackGuide;
    [SerializeField] private List<GameObject> _pages = new List<GameObject>();
    private int _currentPage = 0;
    [SerializeField] private RectTransform _backTitleButtonRect;
    [SerializeField] private RectTransform _nextPageButtonRect;
    [SerializeField] private RectTransform _prevPageButtonRect;
    private Vector3 defaultScale;

    [SerializeField] private Animator _guideCanvasanimator;
    [SerializeField] private Animator _attackGIFanimator;
    [SerializeField] private Animator _chargeGIFanimator;
    void Start()
    {
        _guideCanvas.SetActive(false);
        foreach (var page in _pages)
        {
            page.SetActive(false);
        }
        defaultScale = _backTitleButtonRect.localScale;
        ResetButtonScales();
    }
    public void ShowGuide()
    {
        _guideCanvas.SetActive(true);
        _currentPage = 0;
        UpdatePage();
        ResetButtonScales();
    }
    public void BackToTitle()
    {
        StartCoroutine(CloseCanvas());
        ResetButtonScales();
    }
    public void NextPage()
    {
        _currentPage++;
        UpdatePage();
        ResetButtonScales();
    }
    public void PrevPage()
    {
        _currentPage--;
        UpdatePage();
        ResetButtonScales();
    }
    private void ResetButtonScales()
    {
        _backTitleButtonRect.localScale = defaultScale;
        _nextPageButtonRect.localScale = defaultScale;
        _prevPageButtonRect.localScale = defaultScale;
    }
    private void UpdatePage()
    {
        for (int i = 0; i < _pages.Count; i++)
        {
            bool isActive = (i == _currentPage);
            _pages[i].SetActive(isActive);

            if (isActive)
            {
                // 找到这一页下的所有 GifPlayer，让它们重新从头播放
                var gifPlayers = _pages[i].GetComponentsInChildren<GifImporter.GifPlayer>(true);
                foreach (var gif in gifPlayers)
                {
                    gif.Restart();
                }
            }
        }
    }
    private IEnumerator CloseCanvas()
    {
        _guideCanvasanimator.SetTrigger("CloseGuideCanvas");
        _attackGIFanimator.SetTrigger("CloseGIF");
        _chargeGIFanimator.SetTrigger("CloseGIF");
        yield return new WaitForSeconds(1.0f);
        _guideCanvas.SetActive(false);
    }
}
