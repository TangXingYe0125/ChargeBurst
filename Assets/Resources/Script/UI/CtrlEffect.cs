using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class CtrlEffect : MonoBehaviour
{
    public float blinkMinAlpha = 0.4f;
    public float blinkMaxAlpha = 1f;
    public float blinkDuration;

    public float scaleMin = 0.9f;
    public float scaleMax = 1.1f;
    public float scaleDuration;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Tween alphaTween;
    private Tween scaleTween;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        canvasGroup.alpha = blinkMaxAlpha; 
        rectTransform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }

    void Start()
    {
        // ÉÁË¸
        alphaTween = canvasGroup.DOFade(blinkMinAlpha, blinkDuration / 2)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // ·Å´óËõÐ¡
        scaleTween = rectTransform.DOScale(scaleMax, scaleDuration / 2)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StopEffect();
        }
    }

    public void StopEffect()
    {
        alphaTween?.Kill();
        scaleTween?.Kill();

        canvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false); 
            Destroy(gameObject);
        });
    }
}
