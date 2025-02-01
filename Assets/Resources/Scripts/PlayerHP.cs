using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public static PlayerHP instance;
    [SerializeField] private DemoScript _loseScript;
    [SerializeField] private DemoScript _winScript;
    public int _HP; 
    public int _kills = 0;
    public int _explodes = 0;
    private AudioSource _hit;
    [SerializeField] private List<Image> _hearts = new List<Image>();
    private int index = 4;

    [SerializeField] private Animator _feedBack;
    [SerializeField] private Animator _heartFade;

    //[SerializeField] private float _flash;
    //[SerializeField] private float _duration;
    //[SerializeField] private SpriteRenderer _sr;
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
            GetDamage();
            StartCoroutine(invincibility());
            _explodes++;
            Destroy(collision.gameObject);
        }
    }
    private void GetDamage()
    {
        _feedBack.SetTrigger("isFeedBack");
        index = Mathf.Max(index, 0);
        _heartFade = _hearts[index].GetComponent<Animator>();
        _heartFade.SetTrigger("Fade");
        index--;
    }

    private IEnumerator invincibility()
    {
        Physics2D.IgnoreLayerCollision(9, 10, true);
        //for(int i = 0; i < _flash; i++)
        //{
        //    _sr.color = Color.red;
        //    yield return new WaitForSeconds(_duration / _flash);
        //    _sr.color = Color.white;
        //    yield return new WaitForSeconds(_duration / _flash);
        //}
        yield return new WaitForSeconds(1.0f);
        Physics2D.IgnoreLayerCollision(9, 10, false);
    }
}
