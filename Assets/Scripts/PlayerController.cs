using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    private Animator animator;
    [SerializeField] MobileJoystick joystick;
    public float speed = 5f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //Application.targetFrameRate = 60;
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

        if (rb.velocity != Vector2.zero)
        {
            SetAnimate("Move");
        }
        else
        {
            //等待上个动画播放完毕后，再设置空闲动画, 在不是HIt动画状态下才设置空闲动画
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                SetAnimate("Idle");
            }  
        }
    }

    public void SetAnimate(string stateName)
    {
        if (animator != null)
        {
            animator.Play(stateName);
        }
        else
        {
            Debug.LogWarning("Animator组件未设置，无法设置动画状态");
        }
    }
}
