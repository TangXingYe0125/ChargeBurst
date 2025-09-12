using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public static PlayerHP instance;
    [SerializeField] private GoFade _loseScript;
    [SerializeField] private GoFade _winScript;
    public int _HP; 
    public int _kills = 0;
    public int _explodes = 0;

    private AudioSource _hit;
    [SerializeField] private List<Image> _hearts = new List<Image>();
    private int _index = 4;

    [SerializeField] private Animator _feedBack;
    [SerializeField] private Animator _heartFade;
    public bool _isReady = true;
    public bool _isEnemy = false;
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
            _loseScript.StartFade();
        }
        if(InstantiateEnemy.instance._enemyLeft == 0 || Timer._time <= 0.00f)
        {
            _winScript.StartFade();
        }
        EnemyDetect();
    }

    private void EnemyDetect()
    {
        if (_isEnemy)
        {
            _isReady = false;
            _hit.PlayOneShot(_hit.clip);
            StartCoroutine(GetDamage());
            StartCoroutine(invincibility());
            _explodes++;
            _isEnemy = false;
        }
    }
    private IEnumerator GetDamage()
    {
        _HP--;
        _feedBack.SetTrigger("isFeedBack");
        _index = Mathf.Max(_index, 0);
        _heartFade = _hearts[_index].GetComponent<Animator>();
        _heartFade.SetTrigger("Fade");
        _index--;
        yield return new WaitForSeconds(0.7f);
        _isReady = true;
    }
    private IEnumerator invincibility()
    {
        Physics2D.IgnoreLayerCollision(9, 10, true);
        yield return new WaitForSeconds(1.0f);
        Physics2D.IgnoreLayerCollision(9, 10, false);
    }
}
