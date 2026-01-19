using UnityEngine;

public class EnemyHitSE : MonoBehaviour
{
    public static EnemyHitSE Instance;

    public AudioSource _enemyHit;

    private float lastHitTime;
    public float hitCooldown = 0.08f; // 80ms

    void Awake()
    {
        Instance = this;
    }

    public void PlayEnemyHit(AudioClip clip)
    {
        if (Time.time - lastHitTime < hitCooldown)
            return;

        lastHitTime = Time.time;
        _enemyHit.PlayOneShot(clip);
    }
}
