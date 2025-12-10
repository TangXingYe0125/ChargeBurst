using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class BladeArray : MonoBehaviour
{
    [Header("Blade Settings")]
    [SerializeField] private GameObject _bladeArray;
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
    [SerializeField] private GameObject _boss;
    [SerializeField] private GameObject _controlBlade;
    [SerializeField] private int _swordCount;
    [SerializeField] private float _angle;
    [SerializeField] private float _radius;
    [SerializeField] private float _delayTime;
    [SerializeField] private float _waitTime;

    public System.Action OnStartBossAttack;
    public System.Action OnEndBossAttack;

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SpawnMultiLayerBlades());
            StartCoroutine(FollowPlayerCircle());
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !isAttackingBoss)
        {
            await BladesAttackBossAsync();
        }

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

                GameObject sword = Instantiate(_bladeArray, targetPos, Quaternion.identity);

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
    public async UniTask BladesAttackBossAsync()
    {
        // 触发开始事件
        OnStartBossAttack?.Invoke();

        // 先控制剑阵生成和移动完成
        await ControlBladesAsync(_lastBOSSBody.position);

        // 设置攻击状态
        isAttackingBoss = true;

        // 停止环绕，保证一帧刷新位置
        await UniTask.Yield();

        List<UniTask> attackTasks = new List<UniTask>();

        // 遍历每层剑
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
                seq.AppendInterval(stopDuration); // 停留
                seq.Append(sword.DOMove(backPos, backDuration).SetEase(Ease.OutCubic)); // 后退
                seq.Append(sword.DOMove(_lastBOSSBody.position, attackDuration).SetEase(Ease.InCubic));
                seq.AppendCallback(() =>
                {
                    CameraShake.Shake(2.0f,0.5f);
                });
                seq.OnComplete(() => Destroy(sword.gameObject));
                seq.Play();

                // 将 DOTween 动画包装为 UniTask，加入任务列表
                attackTasks.Add(UniTask.Create(async () => await seq.AsyncWaitForCompletion()));
            }
        }

        // 等待所有剑的攻击动画完成
        await UniTask.WhenAll(attackTasks);

        // 清空数据
        allSwords.Clear();
        allAngles.Clear();

        // 重置攻击状态
        isAttackingBoss = false;

        // 触发结束事件
        OnEndBossAttack?.Invoke();
    }

    public async UniTask ControlBladesAsync(Vector3 attackPos)
    {
        List<Vector3> spawnPosList = new List<Vector3>();

        for (int i = 0; i < _swordCount; i++)
        {
            int dirMultiplier = i % 2 == 0 ? -1 : 1;
            Vector2 offset = Quaternion.Euler(0, 0, _angle * ((i + 1) / 2) * dirMultiplier) * new Vector2(0, _radius);
            spawnPosList.Add(offset);
        }

        Vector3 startPos = attackPos + new Vector3(0, _radius, 0);

        // 所有剑的任务列表
        List<UniTask> swordTasks = new List<UniTask>();

        for (int i = 0; i < spawnPosList.Count; i++)
        {
            Transform sword = Instantiate(_controlBlade, startPos, Quaternion.identity).transform;
            sword.up = Vector2.down;

            Collider2D swordCol = sword.GetComponent<Collider2D>();
            Collider2D[] bossCols = _boss.GetComponents<Collider2D>();

            Vector3 targetPos = attackPos + spawnPosList[i];
            Vector3 dir = (attackPos - targetPos).normalized;

            foreach (var col in bossCols)
            {
                Physics2D.IgnoreCollision(swordCol, col, true);
            }

            Sequence seq = DOTween.Sequence();
            seq.AppendInterval((i + 1) / 2f * _delayTime);                  // 延迟
            seq.Append(DOTween.To(() => sword.up, x => sword.up = x, dir, 0.3f)); // 剑尖旋转
            seq.Join(sword.DOMove(targetPos, 0.3f));                         // 移动到偏移位置
            seq.AppendInterval(1.0f);                                        // 停留

            seq.AppendCallback(() =>
            {
                foreach (var col in bossCols)
                {
                    Physics2D.IgnoreCollision(swordCol, col, false);
                }
            });

            seq.Append(sword.DOMove(targetPos - dir, 0.4f));                 // 后退
            seq.Append(sword.DOMove(attackPos, 0.5f));                        // 飞向目标
            seq.AppendCallback(() =>
            {
                CameraShake.Shake(2.0f,0.2f);
            });
            seq.OnComplete(() => Destroy(sword.gameObject));
            seq.Play();

            // 每把剑动画包装为 UniTask，加入列表
            swordTasks.Add(UniTask.Create(async () => await seq.AsyncWaitForCompletion()));
        }

        // 等待所有剑动画完成
        await UniTask.WhenAll(swordTasks);

    }




    // ----------------- 工具方法 -----------------
    private Vector2 AngleToVector(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
