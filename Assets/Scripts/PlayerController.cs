using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] MobileJoystick joystick;
    public float speed = 5f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Application.targetFrameRate = 60;
        if (joystick == null) Debug.LogWarning("Joystick脚本未添加到PlayerController脚本上");
    }


    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        // float moveHorizontal = Input.GetAxis("Horizontal");
        // float moveVertical = Input.GetAxis("Vertical");
        // Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        //从摇杆获取移动向量
        rb.velocity = joystick.GetMoveVector() * speed * Time.deltaTime;
    }
}
