using UnityEngine;

public class EnemyHitSE : MonoBehaviour
{
    public static EnemyHitSE Instance;

    public AudioSource _enemyHit;

    private float lastHitTime;
    public float hitCooldown;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_enemyHit == null)
        {
            _enemyHit = GetComponent<AudioSource>();
        }
    }

    public void PlayEnemyHit(AudioClip clip)
    {
        if (Time.time - lastHitTime < hitCooldown)
            return;

        lastHitTime = Time.time;
        _enemyHit.PlayOneShot(clip);
    }
}
