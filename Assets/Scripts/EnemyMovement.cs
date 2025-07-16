using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Enemy enemy;

    [SerializeField] private GameObject fx_dead;
    GameObject canvasObject;
    private Vector3 centerPosition;

    [Header("敌人参数设置")]
    //[SerializeField] private float health = 100f;
    [SerializeField] private float moveSpeed = 2f;
    //[SerializeField] private float moveSpeedFast = 2f;
    private float startSpeed = 0f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float warnningRange = 3f;


    //动画
    private Animator animator;
    private GameObject otherObject;
    private GameObject enemySprite;
    private GameObject spawnIndicator;
    private Rigidbody2D rb;

    bool startToMove = false;

    [Header("Gizmos设置")]
    [SerializeField] bool showGizmos = true;

    void Start()
    {
        Init();
        StartCoroutine(ShowEnemySprite()); //延迟显示敌人Sprite
    }

    void Update()
    {
        if (enemy.isDead || !startToMove) return; //如果敌人已经死亡或尚未出生，则不执行任何操作
        if (GameManager.Instance.player == null) return;

        if (GameManager.Instance.player.isDead)
        {
            SetAnimate("Idle");
            return;
        }

        if (InAttackRange())//在攻击范围内
        {
            enemy.Attack();
        }
        else //不在攻击范围
        {
            FollowPlayer();
        }
    }

    void Init()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        canvasObject = transform.Find("OtherObject/Canvas").gameObject;
        canvasObject.SetActive(false); // 确保Canvas对象未激活
        otherObject = transform.Find("OtherObject")?.gameObject;
        otherObject?.SetActive(false);
        enemySprite = transform.Find("A/Sprite")?.gameObject;
        enemySprite?.SetActive(false); // 确保敌人Sprite未激活

        spawnIndicator = transform.Find("SpawnIndicator")?.gameObject;
        spawnIndicator?.SetActive(true); // 确保出生点指示器未激活
        fx_dead = transform.Find("FX_Dead")?.gameObject;//加载死亡特效
        fx_dead?.SetActive(false); // 确保死亡特效未激活

        moveSpeed = Random.Range(moveSpeed * 0.8f, moveSpeed * 1.8f); //随机设置敌人移动速度
        //moveSpeedFast = moveSpeed * 1.3f; //设置快速移动速度
        startSpeed = moveSpeed;

    }

    IEnumerator ShowEnemySprite()
    {
        yield return new WaitForSeconds(GameManager.Instance.delayStartTime);
        enemySprite?.SetActive(true); // 显示敌人Sprite  
        otherObject?.SetActive(true); // 激活其他对象
        canvasObject.SetActive(false); // 激活Canvas对象

        spawnIndicator?.SetActive(false); // 隐藏出生点指示器
        rb.velocity = Vector2.zero; //清空速度

        SetAnimate("Born"); // 播放出生动画

        yield return new WaitForSeconds(0.3f); // 等待动画播放完成
        startToMove = true;// 敌人开始移动
        //GameManager.Instance.player.playerController.startToMove = true; // 开始玩家移动
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

    public void FollowPlayer()
    {
        if (GameManager.Instance.player != null)
        {
            Vector2 targetPosition = Vector2.MoveTowards(transform.position, GameManager.Instance.player.transform.position, moveSpeed * Time.deltaTime);
            transform.position = targetPosition;

            if (InWarnninRange()) //是不是在Warnning范围内
            {
                SetAnimate("Warnning");
            }
            else // 距离大于Warnning范围内
            {
                SetAnimate("Move");
                moveSpeed = startSpeed; //恢复速度
            }
        }
        else
        {
            Debug.LogWarning("Player对象未设置，无法追逐玩家");
        }
    }

    public bool InWarnninRange()
    {
        //centerPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z); //获取敌人中心位置
        var enemyDistanceToPlayer = Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
        return enemyDistanceToPlayer <= warnningRange; //如果敌人与玩家之间的距离小于等于指定的距离，则警告玩家
    }

    public bool InAttackRange()
    {
        //centerPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z); //获取敌人中心位置
        var enemyDistanceToPlayer = Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
        return enemyDistanceToPlayer <= attackRange;
    }


    public void Die()
    {
        enemy.isDead = true;
        SetAnimate("Dead");
        fx_dead.transform.parent = null; //将死亡特效从敌人对象中分离出来
        fx_dead.SetActive(true); //激活死亡特效
        rb.velocity = Vector2.zero; //清空速度
        otherObject?.SetActive(false);
        Destroy(gameObject, 2f); //销毁敌人
    }

    public bool GetCurrentAnimatorStateInfo(string stateName)
    {
        if (animator != null)
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }
        else
        {
            Debug.LogWarning("Animator组件未设置，无法获取当前动画状态");
            return false;
        }
    }



    public void OnDrawGizmosSelected()
    {
        //centerPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z); //获取敌人中心位置
        if (!showGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, warnningRange);
    }


}
