using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerControllor : MonoBehaviour
{
    Vector3 _currentInput;
    public float _speed;
    private float TempX = 0.0f;
    private float TempY = 0.0f;
    [SerializeField] private float EdgeX;
    [SerializeField] private float EdgeY;

    public GameObject Enemys;
    public float CreatTime = 3.0f;
    public int EnemyAmount;

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        _currentInput = new Vector3(horizontal, vertical, 0);

        transform.position += _currentInput * _speed;

        TempX = Mathf.Clamp(transform.position.x, -EdgeX, EdgeX);
        TempY = Mathf.Clamp(transform.position.y, -EdgeY, EdgeY);
        transform.position = new Vector3(TempX, TempY, transform.position.z);

        var pos = Camera.main.WorldToScreenPoint(transform.localPosition);
        var rotation = Quaternion.LookRotation(Vector3.forward, Input.mousePosition - pos);
        transform.localRotation = rotation;

        //if (EnemyAmount != 0)
        //{
        //    CreatTime -= Time.deltaTime;
        //    if (CreatTime <= 0)
        //    {
        //        CreatTime = Random.Range(0.1f, 2f);
        //        GameObject obj = (GameObject)Resources.Load("Prefab/Enemy");
        //        Enemys = Instantiate <GameObject>(obj);
        //        Enemys.transform.position = new Vector3(Random.Range(-15,15), Random.Range(-5.5f,5.5f));
        //        EnemyAmount -= 1;
        //    }
        //}
        //else if (EnemyAmount == 0) 
        //{
        //    SceneManager.LoadScene("Win");
        //}
    }
}
