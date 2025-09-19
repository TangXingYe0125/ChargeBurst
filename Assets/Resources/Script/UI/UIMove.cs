using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIMove : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform _rectTransform;
    private float _scale;
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _scale = _rectTransform.localScale.x;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _rectTransform.DOKill();
        _rectTransform.DOScale(_scale * new Vector3(1.2f, 1.2f, 0f), 0.5f).SetEase(Ease.InOutCirc);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _rectTransform.DOKill();
        _rectTransform.DOScale(_scale * new Vector3(1.0f, 1.0f, 0f), 0.5f).SetEase(Ease.InOutCirc);
    }
    
}
