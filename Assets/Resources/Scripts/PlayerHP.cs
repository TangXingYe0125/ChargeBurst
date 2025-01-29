using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public static PlayerHP instance;
    [SerializeField] private DemoScript _loseScript;
    [SerializeField] private DemoScript _winScript;
    public int _HP; 
    public int _kills = 0;
    public int _explodes = 0;
    private AudioSource _hit;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        _hit = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (_HP <= 0)
        {
            _loseScript.GoFade();
        }
        if(InstantiateEnemy.instance._enemyLeft == 0 || Timer._time <= 0.00f)
        {
            _winScript.GoFade();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _hit.PlayOneShot(_hit.clip);
            _HP--;
            _explodes++;
            Destroy(collision.gameObject);
        }
    }
}
