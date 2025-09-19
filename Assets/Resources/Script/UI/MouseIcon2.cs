using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MouseIcon2 : MonoBehaviour
{
    [SerializeField] private Image _mouse;
    [SerializeField] private Sprite _default;
    [SerializeField] private Sprite _leftClick;

    private float _time = 0.0f;
    private float _chargePoint = 0.5f;
    private bool _isCharging = false;
    private Coroutine _blinkCoroutine;

    void Start()
    {
        _mouse.sprite = _default;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mouse.sprite = _leftClick;
            _isCharging = true;
        }

        if (_isCharging && Input.GetMouseButton(0))
        {
            _time += Time.deltaTime;

            if (_time >= _chargePoint && _blinkCoroutine == null)
            {
                _blinkCoroutine = StartCoroutine(BlinkEffect());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _mouse.sprite = _default;
            _isCharging = false;
            _time = 0f;

            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;
                StartCoroutine(ResetAlpha());             
            }
        }
    }
    private IEnumerator BlinkEffect()
    {
        while (true)
        {
            for (float t = 0; t < 0.3f; t += Time.deltaTime)
            {
                var c = _mouse.color;
                c.a = Mathf.Lerp(1f, 0.3f, t / 0.3f);
                _mouse.color = c;
                yield return null;
            }
            for (float t = 0; t < 0.3f; t += Time.deltaTime)
            {
                var c = _mouse.color;
                c.a = Mathf.Lerp(0.3f, 1f, t / 0.3f);
                _mouse.color = c;
                yield return null;
            }
        }
    }
    private IEnumerator ResetAlpha()
    {
        float startAlpha = _mouse.color.a;
        float duration = 0.2f; 
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            var c = _mouse.color;
            c.a = Mathf.Lerp(startAlpha, 1f, t / duration);
            _mouse.color = c;
            yield return null;
        }
    }
}
