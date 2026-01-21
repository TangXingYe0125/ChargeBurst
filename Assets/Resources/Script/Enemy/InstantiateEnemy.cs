using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstantiateEnemy : MonoBehaviour
{
    [SerializeField] protected GameObject[] _enemyPrefab;
    protected int _enemyType;
    [SerializeField] protected List<Transform> _spawnPoints = new List<Transform>();

    private float _createTime = 3.0f;
    private float _createTimeMin = 0.1f;
    private float _createTimeMax = 1.0f;

    public int _enemyAmount;
    public int _totalAmount;
    public int _enemyLeft;

    protected virtual void Start()
    {
        _totalAmount = _enemyAmount;
        _enemyLeft = _totalAmount;
    }
    protected virtual void Update()
    {
        _enemyLeft = _totalAmount - Kills.instance._kills - Kills.instance._explodes;
        if (PlayerHP.instance._HP <= 0)
        {
            return;
        }
        Instantiate();
        if (_enemyLeft <= 0)
        {
            GameStateManager.instance.SetState(GameState.Victory);
        }
    }
    protected virtual void Instantiate()
    {
        if (_enemyAmount > 0)
        {
            _createTime -= Time.deltaTime;
            if (_createTime <= 0)
            {
                _createTime = Random.Range(_createTimeMin, _createTimeMax);
                _enemyType = Random.Range(0, _enemyPrefab.Length);

                Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];

                GameObject enemys = Instantiate(_enemyPrefab[_enemyType], spawnPoint.position, Quaternion.identity);

                _enemyAmount--;
            }
        }
    }
}
