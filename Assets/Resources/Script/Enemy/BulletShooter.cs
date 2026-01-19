using UnityEngine;
using System.Collections;

public class BulletShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    private float _fireRate;
    public float _waveDuration;
    public float _waveCooldown; 
    public enum Pattern { None, Aiming, Spread, Circle, Rotate }
    public Pattern currentPattern;

    private Transform player;
    private float _rotateAngle;
    private float _circleGapAngle = 0f;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(ShootLoop());
    }

    private IEnumerator ShootLoop()
    {
        Pattern[] patterns = { Pattern.Aiming, Pattern.Spread, Pattern.Circle, Pattern.Rotate };
        Pattern lastPattern = Pattern.None; 

        while (true)
        {
            Pattern newPattern;
            do{newPattern = patterns[Random.Range(0, patterns.Length)];} 
            while (newPattern == lastPattern);

            currentPattern = newPattern;
            lastPattern = newPattern; 

            float timer = 0f;
            while (timer < _waveDuration)
            {
                ShootPattern();
                yield return new WaitForSeconds(_fireRate);
                timer += _fireRate;
            }

            yield return new WaitForSeconds(_waveCooldown);
        }
    }

    private void ShootPattern()
    {
        switch (currentPattern)
        {
            case Pattern.Aiming:
                _fireRate = 0.2f;
                ShootAiming(); 
                break;
            case Pattern.Spread:
                _fireRate = 0.5f;
                ShootSpread(); 
                break;
            case Pattern.Circle:
                _fireRate = 0.5f;
                ShootCircle();
                break;
            case Pattern.Rotate:
                _fireRate = 0.5f;
                ShootRotate(); 
                break;
        }
    }

    void ShootAiming()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        FireBullet(dir);
    }

    void ShootSpread()
    {
        int count = 8;
        float spreadAngle = 150f;
        Vector2 toPlayer = (player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        for (int i = 0; i < count; i++)
        {
            float angle = baseAngle - spreadAngle / 2 + (spreadAngle / (count - 1)) * i;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            FireBullet(dir);
        }
    }

    void ShootCircle()
    {
        int bulletCount = 14;
        float gapAngle = 50f;

        _circleGapAngle += 25f; 
        if (_circleGapAngle >= 360f)
            _circleGapAngle -= 360f;

        float startAngle = _circleGapAngle + gapAngle / 2f;
        float endAngle = _circleGapAngle - gapAngle / 2f + 360f;

        float step = (endAngle - startAngle) / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + step * i;
            FireBullet(AngleToVector(angle));
        }
    }

    void ShootRotate()
    {
        int count = 12;
        _rotateAngle += 10f;

        for (int i = 0; i < count; i++)
        {
            float angle = _rotateAngle + i * (360f / count);
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            FireBullet(dir);
        }
    }

    void FireBullet(Vector2 dir)
    {
        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        b.GetComponent<Bullet>().SetDirection(dir);
    }
    private Vector2 AngleToVector(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
