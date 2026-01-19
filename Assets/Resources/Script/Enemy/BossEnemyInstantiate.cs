using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyInstantiate : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;
    private int _enemyType;
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    public int _instantiateHP;
    public void Instantiate()
    {
        for(int i = 0; i < _spawnPoints.Count; i ++)
        {
            _enemyType = Random.Range(0, _enemyPrefab.Length);
            GameObject enemys = Instantiate(_enemyPrefab[_enemyType], _spawnPoints[i].position, Quaternion.identity);
        }       
    }
}
