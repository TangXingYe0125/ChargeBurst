using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WASD : MonoBehaviour
{
    [SerializeField]private List<RectTransform> _rects = new List<RectTransform>();
    private Dictionary<KeyCode, RectTransform> _keyRect;
    private float _scale;
    private Dictionary<KeyCode, Image> _keyBackGround;

    [Header("Highlight Settings")]
    [SerializeField] private Color highlightColor = new Color(1f, 0.8f, 0.2f); 
    [SerializeField] private Color normalColor = Color.white; 
    void Start()
    {
        _scale = _rects[0].localScale.x;
        _keyRect = new Dictionary<KeyCode, RectTransform>()
        {
            { KeyCode.W,_rects[0] },
            { KeyCode.A,_rects[1] },
            { KeyCode.S,_rects[2] },
            { KeyCode.D,_rects[3] },
        };

        _keyBackGround = new Dictionary<KeyCode, Image>
        {
            { KeyCode.W, _rects[0].GetComponent<Image>() },
            { KeyCode.A, _rects[1].GetComponent<Image>() },
            { KeyCode.S, _rects[2].GetComponent<Image>() },
            { KeyCode.D, _rects[3].GetComponent<Image>() }
        };
    }
    void Update()
    {
        foreach (KeyValuePair<KeyCode,RectTransform> kvp in _keyRect)
        {
            KeyCode keyCode = kvp.Key;
            RectTransform rect = kvp.Value;
            Image bgColor = _keyBackGround[keyCode];
            if(Input.GetKeyDown(keyCode))
            {
                ShowPressed(rect,1.2f,0.001f);
                PressedColor(_keyBackGround[keyCode],highlightColor,0.1f);
            }
            else if(Input.GetKeyUp(keyCode))
            {
                ShowPressed(rect, 1.0f, 0.5f);
                PressedColor(_keyBackGround[keyCode], normalColor, 0.2f);
            }
        }
    }
    private void ShowPressed(RectTransform rect, float targetScale, float duration)
    {
        rect.DOKill();
        rect.DOScale(_scale * Vector3.one * targetScale, duration).SetEase(Ease.InOutCirc);
    }
    private void PressedColor(Image image, Color targetColor, float duration)
    {
        image.DOKill();
        image.DOColor(targetColor, duration).SetEase(Ease.InSine);
    }
}
