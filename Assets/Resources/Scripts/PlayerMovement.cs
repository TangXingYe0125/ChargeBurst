using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerMovement : MonoBehaviour
{
    Vector3 _currentInput;
    public float _speed;
    private float TempX = 0.0f;
    private float TempY = 0.0f;
    [SerializeField] private float EdgeX;
    [SerializeField] private float EdgeY;

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
    }
}
