using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _speed;

    private Vector2 input;

    [HideInInspector] public bool _isAttacking = false;
    private float _lockedAngle = 0f;

    [SerializeField] private float _rotationSpeed = 1080f;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        input = new Vector2(horizontal, vertical).normalized;
    }

    void FixedUpdate()
    {
        if (GameStateManager.instance.CurrentState != GameState.Playing)
        {
            _rb.velocity = Vector2.zero;
            return;
        }
        _rb.velocity = input * _speed;

        Vector3 _mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mouseWorld.z = 0f;
        Vector2 _direction = new Vector2(_mouseWorld.x, _mouseWorld.y) - _rb.position;
        float _targetAngle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90f;

        if (_isAttacking)
        {
            _rb.MoveRotation(_lockedAngle);
        }
        else
        {
            float _angle = Mathf.MoveTowardsAngle(_rb.rotation, _targetAngle, _rotationSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(_angle);
        }
    }
    public void StartAttack()
    {
        _isAttacking = true;
        _lockedAngle = _rb.rotation; 
    }
    public void EndAttack()
    {
        _isAttacking = false;
    }
}
