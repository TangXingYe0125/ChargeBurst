using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class PlayerHP : MonoBehaviour
{
    public static PlayerHP instance;
    public int _HP; 
    public int _maxHP = 5; 

    private AudioSource _hit;
    [SerializeField] private List<Image> _hearts = new List<Image>();
    private int _index = 4;

    [SerializeField] private Animator _feedBack;
    private Animator _heartFade;
    [HideInInspector] public bool _isReady = true;
    [HideInInspector] public bool _isEnemy = false;
    [HideInInspector] public int _damage;

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
        }
    }
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
    private void Start()
    {
        _hit = GetComponent<AudioSource>();
        _HP = _maxHP;
    }
    private void Update()
    {
        EnemyDetect();
    }
    private void EnemyDetect()
    {
        if (_isEnemy)
        {
            _hit.PlayOneShot(_hit.clip);
            HandleEnemyHit();
            StartCoroutine(invincibility());
            Kills.instance._explodes++;
            _isEnemy = false;
        }
    }
    private async Task GetDamageAsync()
    {
        _HP -= _damage;
        for (int i = 0; i < _damage; i++)
        {
            _index = Mathf.Max(_index, 0);
            _heartFade = _hearts[_index].GetComponent<Animator>();
            _heartFade.SetTrigger("Fade");
            _index--;
        }
        _feedBack.SetTrigger("isFeedBack");
        if (_HP <= 0)
        {
            GameStateManager.instance.SetState(GameState.GameOver);
            return;
        }        
        await Task.Delay(700);
        _damage = 0;
        _isReady = true;
    }
    private IEnumerator invincibility()
    {
        Physics2D.IgnoreLayerCollision(9, 10, true);
        yield return new WaitForSeconds(1.0f);
        Physics2D.IgnoreLayerCollision(9, 10, false);
    }

    private async void HandleEnemyHit()
    {
        await GetDamageAsync();
    }
}
