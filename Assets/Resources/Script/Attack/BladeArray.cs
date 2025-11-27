using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BladeArray : MonoBehaviour
{
    [Header("Blade Settings")]
    [SerializeField] private GameObject _swordPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private int layers = 3;               // 层数
    [SerializeField] private int swordCountPerLayer = 12;  // 每圈剑数
    [SerializeField] private float baseRadius = 2f;        // 第一圈半径
    [SerializeField] private float radiusStep = 1f;        // 每圈半径递增
    [SerializeField] private float rotationSpeed = 90f;    // 角度/秒
    [SerializeField] private float fadeInDuration = 0.3f;  // 渐入动画时间
    [SerializeField] private float layerDelay = 0.5f;      // 每层生成延迟

    [Header("Attack Settings")]
    [SerializeField] private Transform _lastBOSSBody;     // Canvas 上 BOSS 的 body Transform
    [SerializeField] private float attackDuration = 2.0f; // 飞向 BOSS 时间
    [SerializeField] private float stopDuration = 1.0f;   // 停留时间
    [SerializeField] private float backDistance = 0.5f;   // 后退距离
    [SerializeField] private float backDuration = 0.2f;   // 后退动画时间

    private List<List<Transform>> allSwords = new List<List<Transform>>();
    private List<float[]> allAngles = new List<float[]>();
    public bool isAttackingBoss = false;

    [Header("ControlBlade")]
    [SerializeField] private int _swordCount;
    [SerializeField] private float _angle;
    [SerializeField] private float _radius;
    [SerializeField] private float _delayTime;
    [SerializeField] private float _waitTime;

    public System.Action OnStartBossAttack;
    public System.Action OnEndBossAttack;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SpawnMultiLayerBlades());
            StartCoroutine(FollowPlayerCircle());
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !isAttackingBoss)
        {
            StartCoroutine(BladesAttackBoss());
        }

        Debug.Log(isAttackingBoss);
    }

    // ----------------- 生成多层剑阵 -----------------
    private IEnumerator SpawnMultiLayerBlades()
    {
        allSwords.Clear();
        allAngles.Clear();

        for (int layer = 0; layer < layers; layer++)
        {
            float radius = baseRadius + layer * radiusStep;
            List<Transform> layerSwords = new List<Transform>();
            float[] angles = new float[swordCountPerLayer];
            float angleStep = 360f / swordCountPerLayer;

            for (int i = 0; i < swordCountPerLayer; i++)
            {
                angles[i] = angleStep * i;
                Vector2 targetPos = (Vector2)player.position + AngleToVector(angles[i]) * radius;

                GameObject sword = Instantiate(_swordPrefab, targetPos, Quaternion.identity);

                // 渐入透明度
                SpriteRenderer sr = sword.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = 0f;
                    sr.color = c;
                    StartCoroutine(FadeIn(sr, fadeInDuration));
                }

                // 剑尖朝外
                sword.transform.up = (targetPos - (Vector2)player.position).normalized;
                layerSwords.Add(sword.transform);
            }

            allSwords.Add(layerSwords);
            allAngles.Add(angles);

            yield return new WaitForSeconds(layerDelay);
        }
    }

    private IEnumerator FadeIn(SpriteRenderer sr, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(0f, 1f, timer / duration);
            sr.color = c;
            timer += Time.deltaTime;
            yield return null;
        }

        Color final = sr.color;
        final.a = 1f;
        sr.color = final;
    }

    // ----------------- 剑阵环绕玩家 -----------------
    private IEnumerator FollowPlayerCircle()
    {
        while (allSwords.Count > 0 && !isAttackingBoss)
        {
            for (int layer = 0; layer < allSwords.Count; layer++)
            {
                List<Transform> layerSwords = allSwords[layer];
                float[] angles = allAngles[layer];
                float direction = (layer % 2 == 0) ? 1f : -1f; // 基数顺时针，偶数逆时针

                for (int i = 0; i < layerSwords.Count; i++)
                {
                    angles[i] += direction * rotationSpeed * Time.deltaTime;
                    Vector2 targetPos = (Vector2)player.position + AngleToVector(angles[i]) * (baseRadius + layer * radiusStep);
                    layerSwords[i].position = targetPos;

                    // 剑尖始终朝外
                    layerSwords[i].up = (targetPos - (Vector2)player.position).normalized;
                }
            }
            yield return null;
        }
    }

    // ----------------- 剑阵攻击BOSS -----------------
    private IEnumerator BladesAttackBoss()
    {
        OnStartBossAttack?.Invoke();

        ControlBlades(_lastBOSSBody.position);
        yield return new WaitForSeconds(_waitTime);
        isAttackingBoss = true;

        // 停止环绕
        yield return new WaitForEndOfFrame();

        for (int layer = 0; layer < allSwords.Count; layer++)
        {
            List<Transform> layerSwords = allSwords[layer];
            for (int i = 0; i < layerSwords.Count; i++)
            {
                Transform sword = layerSwords[i];
                Vector2 dirToBOSS = (_lastBOSSBody.position - sword.position).normalized;

                Vector2 startPos = sword.position;
                Vector2 backPos = startPos - dirToBOSS * backDistance;

                // 剑尖指向BOSS
                sword.up = dirToBOSS;

                // DOTween 顺序动画
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(stopDuration); // 停留1秒
                seq.Append(sword.DOMove(backPos, backDuration).SetEase(Ease.OutCubic)); // 后退动作
                seq.Append(sword.DOMove(_lastBOSSBody.position, attackDuration).SetEase(Ease.InCubic));
                seq.OnComplete(() => Destroy(sword.gameObject));
                seq.Play();
            }
        }

        allSwords.Clear();
        allAngles.Clear();
        isAttackingBoss = false;

        OnEndBossAttack?.Invoke();
        yield return null;
    }

    public void ControlBlades(Vector2 attackPos)
    {
        List<Vector2> spawnPosList = new List<Vector2>();
        for (int i = 0; i < _swordCount; i++)
        {
            var dir = i % 2 == 0 ? -1 : 1;
            var p = Quaternion.Euler(0, 0, _angle * ((i + 1) / 2) * dir) * new Vector2(0, _radius);
            spawnPosList.Add(p);
        }

        var startPos = attackPos + new Vector2(0, _radius);
        for (int i = 0; i < spawnPosList.Count; i++)
        {
            var sword = Instantiate(_swordPrefab, startPos, Quaternion.identity);
            sword.transform.up = Vector2.down;

            var mSequence = DOTween.Sequence();
            var pos = attackPos + spawnPosList[i];
            var dir = (attackPos - pos).normalized;

            mSequence.AppendInterval((i + 1) / 2 * _delayTime);
            mSequence.Append(DOTween.To(() => sword.transform.up, x => sword.transform.up = x, (Vector3)dir, 0.3f));
            mSequence.Join(sword.transform.DOMove(pos, 0.3f));
            mSequence.AppendInterval(1.0f);
            mSequence.Append(sword.transform.DOMove(-dir + pos, 0.4f));
            mSequence.Append(sword.transform.DOMove(attackPos, 0.5f));
            mSequence.Play();
        }
    }

    // ----------------- 工具方法 -----------------
    private Vector2 AngleToVector(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
