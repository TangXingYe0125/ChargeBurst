using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRing : MonoBehaviour
{
    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _waitTime;
    private SpriteRenderer _sr;
    [SerializeField] private SpriteRenderer _bossBody;
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        Color c = _sr.color;
        c.a = 0;
        _sr.color = c;
        _bossBody.color = c;
    }

    // Update is called once per frame
    private void OnEnable()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(_waitTime);
        yield return StartCoroutine(Fade(0.0f,1.0f,_fadeDuration,_sr));
        yield return StartCoroutine(Fade(0.0f,1.0f,2 * _fadeDuration,_bossBody));//not the best way
        yield return new WaitForSeconds(_waitTime);
        yield return StartCoroutine(Fade(1.0f, 0.0f, _fadeDuration, _sr));
        Destroy(this.gameObject);
    }

    private IEnumerator Fade(float from, float to, float duration,SpriteRenderer sr)
    {
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / duration);
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
            yield return null;
        }
    }
}
