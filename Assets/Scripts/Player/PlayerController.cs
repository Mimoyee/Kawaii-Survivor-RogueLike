using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] MobileJoystick joystick;
    public float speed = 5f;
    public bool isInitDone = false;
    public bool startToMove = false; // 是否开始移动
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (joystick == null)
        {
            joystick = FindFirstObjectByType<MobileJoystick>();
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.player == null)
        {
            //Debug.LogWarning("玩家不存在，无法控制");
            return; //如果玩家不存在，则不执行任何操作
        }

        if (GameManager.Instance.player.isDead) return; //如果玩家不存在或已死亡，则不执行任何操作

        if (isInitDone && startToMove)
        {
            Move();
        }

    }

    void Move()
    {

        //从摇杆获取移动向量
        rb.velocity = joystick.GetMoveVector() * speed * Time.deltaTime;

        if (rb.velocity != Vector2.zero)
        {
            //当前动画不是HIt动画状态下，再设置移动动画
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                SetAnimate("Move");
            }
        }
        else
        {
            //在不是HIt动画状态下才设置空闲动画，再设置空闲动画, 
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

    public void StopMovement()
    {
        rb.velocity = Vector2.zero; // 停止玩家移动

    }

    public void StartMovement()
    {
        startToMove = true; // 开始移动
    }
}
