using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    private Vector2 input;
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        input = new Vector2(horizontal, vertical).normalized;
    }
    void FixedUpdate()
    {
        rb.velocity = input * speed;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f; 

        Vector2 direction = new Vector2(mouseWorld.x, mouseWorld.y) - rb.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
        rb.MoveRotation(angle);
    }
}
