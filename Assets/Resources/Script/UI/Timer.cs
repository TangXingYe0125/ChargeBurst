using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static Timer instance;
    public float _time;
    [SerializeField] private TextMeshProUGUI _text;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }
    void FixedUpdate()
    {
        if(PlayerHP.instance._HP > 0)
        {
            _time -= Time.deltaTime;
        }
        _time = Mathf.Max(0.00f, _time);
        _text.text = "Time:" + (_time).ToString("F2");
    }
}
