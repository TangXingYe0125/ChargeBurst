using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossEnemyInstantiate : InstantiateEnemy
{
    public int _instantiateHP;
    private float _fadeDuration = 1.5f;

    private void Awake()
    {
        foreach (var point in _spawnPoints)
        {
            var sr = point.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 0f; 
                sr.color = c;
            }
        }
    }
    protected override void Update()
    {
        
    }
    protected override void Instantiate()
    {

    }
    public IEnumerator BossInstantiateEnemy()
    {
        foreach(var point in _spawnPoints)
        {
            var sr = point.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                StartCoroutine(Fade(sr, 0f, 1f, _fadeDuration));
            }
        }
        float t = 0;
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        for(int i = 0; i < _spawnPoints.Count; i ++)
        {
            _enemyType = Random.Range(0, _enemyPrefab.Length);
            GameObject enemys = Instantiate(_enemyPrefab[_enemyType], _spawnPoints[i].position, Quaternion.identity);
        }
        foreach (var point in _spawnPoints)
        {
            var sr = point.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                StartCoroutine(Fade(sr, 1f, 0f, 0.5f * _fadeDuration));
            }
        }
    }

    private IEnumerator Fade(SpriteRenderer sr, float from, float to, float duration)
    {
        float t = 0f;
        Color c = sr.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            sr.color = c;
            yield return null;
        }
        c.a = to;
        sr.color = c;
    }
}
