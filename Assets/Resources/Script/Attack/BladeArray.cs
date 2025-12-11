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
    [SerializeField] private int layers = 3;               // ≤„ ˝
    [SerializeField] private int swordCountPerLayer = 12;  // √ø»¶Ω£ ˝
    [SerializeField] private float baseRadius = 2f;        // µ⁄“ª»¶∞ÅE∂
    [SerializeField] private float radiusStep = 1f;        // √ø»¶∞ÅE∂µ›‘ÅE
    [SerializeField] private float rotationSpeed = 90f;    // Ω«∂»/√ÅE
    [SerializeField] private float fadeInDuration = 0.3f;  // Ω•»ÅEØª≠ ±ºÅE
    [SerializeField] private float layerDelay = 0.5f;      // √ø≤„…˙≥…—”≥Ÿ

    [Header("Attack Settings")]
    [SerializeField] private Transform _lastBOSSBody;     // Canvas …œ BOSS µƒ body Transform
    [SerializeField] private float attackDuration = 2.0f; // ∑…œÅEBOSS  ±ºÅE
    [SerializeField] private float stopDuration = 1.0f;   // Õ£¡Ù ±ºÅE
    [SerializeField] private float backDistance = 0.5f;   // ∫ÛÕÀæ‡¿ÅE
    [SerializeField] private float backDuration = 0.2f;   // ∫ÛÕÀ∂Øª≠ ±ºÅE

    private List<List<Transform>> allSwords = new List<List<Transform>>();
    private List<float[]> allAngles = new List<float[]>();
    public bool isAttackingBoss = false;
    [SerializeField] private Boss boss;

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
    [SerializeField] private int triggerHpValue = 40;
    private bool _patternTriggered = false;

    public float ctrlTriggerHp = 25f;
    private bool ctrlAttackUnlocked = false;
    private bool ctrlAttackUsed = false;

    private async void Update()
    {
        if (!_patternTriggered && boss._hp <= triggerHpValue)
        {
            _patternTriggered = true; 
            StartCoroutine(SpawnMultiLayerBlades());
            StartCoroutine(FollowPlayerCircle());
        }

        if (!ctrlAttackUnlocked && boss._hp <= ctrlTriggerHp)
        {
            ctrlAttackUnlocked = true;
        }
        if(ctrlAttackUnlocked && !ctrlAttackUsed && !isAttackingBoss)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ctrlAttackUsed = true;
                await BladesAttackBossAsync();
            }
        }
    }
    // ----------------- …˙≥…∂‡≤„Ω£’ÅE-----------------
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

                // Ω•»ÅE∏√˜∂»
                SpriteRenderer sr = sword.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = 0f;
                    sr.color = c;
                    StartCoroutine(FadeIn(sr, fadeInDuration));
                }

                // Ω£º‚≥ØÕÅE
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

    // ----------------- Ω£’Ûª∑»∆ÕÊº“ -----------------
    private IEnumerator FollowPlayerCircle()
    {
        while (allSwords.Count > 0 && !isAttackingBoss)
        {
            for (int layer = 0; layer < allSwords.Count; layer++)
            {
                List<Transform> layerSwords = allSwords[layer];
                float[] angles = allAngles[layer];
                float direction = (layer % 2 == 0) ? 1f : -1f; // ª˘ ˝À≥ ±’ÅE¨≈º ˝ƒÊ ±’ÅE

                for (int i = 0; i < layerSwords.Count; i++)
                {
                    angles[i] += direction * rotationSpeed * Time.deltaTime;
                    Vector2 targetPos = (Vector2)player.position + AngleToVector(angles[i]) * (baseRadius + layer * radiusStep);
                    layerSwords[i].position = targetPos;

                    // Ω£º‚ º÷’≥ØÕÅE
                    layerSwords[i].up = (targetPos - (Vector2)player.position).normalized;
                }
            }
            yield return null;
        }
    }

    // ----------------- Ω£’Ûπ•ª˜BOSS -----------------
    public async UniTask BladesAttackBossAsync()
    {
        // ¥•∑¢ø™ º ¬º˛
        OnStartBossAttack?.Invoke();

        // œ»øÿ÷∆Ω£’Û…˙≥…∫Õ“∆∂ØÕÅE…
        await ControlBladesAsync(_lastBOSSBody.position);

        // …Ë÷√π•ª˜◊¥Ã¨
        isAttackingBoss = true;

        // Õ£÷πª∑»∆£¨±£÷§“ª÷°À¢–¬Œª÷√
        await UniTask.Yield();

        List<UniTask> attackTasks = new List<UniTask>();

        // ±È¿˙√ø≤„Ω£
        for (int layer = 0; layer < allSwords.Count; layer++)
        {
            List<Transform> layerSwords = allSwords[layer];
            for (int i = 0; i < layerSwords.Count; i++)
            {
                Transform sword = layerSwords[i];
                Vector2 dirToBOSS = (_lastBOSSBody.position - sword.position).normalized;
                Vector2 startPos = sword.position;
                Vector2 backPos = startPos - dirToBOSS * backDistance;

                // Ω£º‚÷∏œÚBOSS
                sword.up = dirToBOSS;

                // DOTween À≥–Ú∂Øª≠
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(stopDuration); // Õ£¡ÅE
                seq.Append(sword.DOMove(backPos, backDuration).SetEase(Ease.OutCubic)); // ∫ÛÕÀ
                seq.Append(sword.DOMove(_lastBOSSBody.position, attackDuration).SetEase(Ease.InCubic));
                seq.AppendCallback(() =>
                {
                    CameraShake.Shake(2.0f,0.5f);
                });
                seq.OnComplete(() => Destroy(sword.gameObject));
                seq.Play();

                // Ω´ DOTween ∂Øª≠∞ÅE∞Œ™ UniTask£¨º”»ÅEŒŒÒ¡–±ÅE
                attackTasks.Add(UniTask.Create(async () => await seq.AsyncWaitForCompletion()));
            }
        }

        // µ»¥˝À˘”–Ω£µƒπ•ª˜∂Øª≠ÕÅE…
        await UniTask.WhenAll(attackTasks);

        // «Âø’ ˝æ›
        allSwords.Clear();
        allAngles.Clear();

        // ÷ÿ÷√π•ª˜◊¥Ã¨
        isAttackingBoss = false;

        // ¥•∑¢Ω· ¯ ¬º˛
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

        // À˘”–Ω£µƒ»ŒŒÒ¡–±ÅE
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
            seq.AppendInterval((i + 1) / 2f * _delayTime);                  // —”≥Ÿ
            seq.Append(DOTween.To(() => sword.up, x => sword.up = x, dir, 0.3f)); // Ω£º‚–˝◊™
            seq.Join(sword.DOMove(targetPos, 0.3f));                         // “∆∂ØµΩ∆´“∆Œª÷√
            seq.AppendInterval(1.0f);                                        // Õ£¡ÅE

            seq.AppendCallback(() =>
            {
                foreach (var col in bossCols)
                {
                    Physics2D.IgnoreCollision(swordCol, col, false);
                }
            });

            seq.Append(sword.DOMove(targetPos - dir, 0.4f));                 // ∫ÛÕÀ
            seq.Append(sword.DOMove(attackPos, 0.5f));                        // ∑…œÚƒø±ÅE
            seq.AppendCallback(() =>
            {
                CameraShake.Shake(2.0f,0.2f);
            });
            seq.OnComplete(() => Destroy(sword.gameObject));
            seq.Play();

            // √ø∞—Ω£∂Øª≠∞ÅE∞Œ™ UniTask£¨º”»ÅE–±ÅE
            swordTasks.Add(UniTask.Create(async () => await seq.AsyncWaitForCompletion()));
        }

        // µ»¥˝À˘”–Ω£∂Øª≠ÕÅE…
        await UniTask.WhenAll(swordTasks);

    }

    // ----------------- π§æﬂ∑Ω∑® -----------------
    private Vector2 AngleToVector(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
