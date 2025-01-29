using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstantiateEnemy : MonoBehaviour
{
    public static InstantiateEnemy instance;
    [SerializeField] private GameObject Enemys;
    private float _creatTime = 3.0f;
    public int EnemyAmount;
    public int _totalAmount;
    public int _enemyLeft;
    [SerializeField] private List<Transform> _factoryPos = new List<Transform>();
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
        _totalAmount = EnemyAmount;
    }
    void Update()
    {
        _enemyLeft = _totalAmount - PlayerHP.instance._kills - PlayerHP.instance._explodes;
        if (EnemyAmount != 0)
        {
            _creatTime -= Time.deltaTime;
            if (_creatTime <= 0)
            {
                _creatTime = Random.Range(0.1f, 1.0f);          
                Enemys.transform.position = _factoryPos[Random.Range(0,_factoryPos.Count)].position;
                Instantiate(Enemys);
                EnemyAmount -= 1;
            }
        }
    }
}
