using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制敌人移动行为的脚本
/// 主要功能：
/// - 管理敌人的移动逻辑
/// - 处理攻击和警告范围检测
/// - 控制敌人动画状态
/// - 处理敌人死亡逻辑
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Enemy enemy;  // 敌人基础组件引用
    [SerializeField] private GameObject fx_dead;  // 死亡特效对象
    private GameObject canvasObject;  // UI画布对象
    private Vector3 centerPosition;  // 敌人中心位置(用于范围检测)

    [Header("敌人参数设置")]
    [SerializeField] private float moveSpeed = 2f;  // 基础移动速度
    private float startSpeed = 0f;  // 初始移动速度备份
    [SerializeField] private float attackRange = 1f;  // 攻击触发范围
    [SerializeField] private float warnningRange = 3f;  // 警告触发范围

    // 动画相关组件
    public Animator animator;  // 动画控制器
    private GameObject otherObject;  // 其他附属对象
    private GameObject enemySprite;  // 敌人精灵对象
    private GameObject spawnIndicator;  // 出生指示器
    private Rigidbody2D rb;  // 刚体组件

    private bool startToMove = false;  // 是否开始移动的标志

    [Header("Gizmos设置")]
    [SerializeField] private bool showGizmos = true;  // 是否显示调试范围

    /// <summary>
    /// 初始化敌人移动组件
    /// </summary>
    void Start()
    {
        Init();
        StartCoroutine(ShowEnemySprite()); // 延迟显示敌人Sprite
    }

    /// <summary>
    /// 每帧更新敌人状态
    /// </summary>
    void Update()
    {
        if (enemy.isDead || !startToMove) return; // 如果敌人已经死亡或尚未出生，则不执行任何操作
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

    /// <summary>
    /// 初始化敌人移动组件
    /// </summary>
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
        startSpeed = moveSpeed;
    }

    /// <summary>
    /// 延迟显示敌人精灵的协程
    /// </summary>
    /// <returns>等待时间</returns>
    IEnumerator ShowEnemySprite()
    {
        yield return new WaitForSeconds(GameManager.Instance.delayStartTime);
        enemySprite?.SetActive(true); // 显示敌人Sprite  
        otherObject?.SetActive(true); // 激活其他对象
        canvasObject.SetActive(true); // 激活Canvas对象

        spawnIndicator?.SetActive(false); // 隐藏出生点指示器
        rb.velocity = Vector2.zero; //清空速度

        SetAnimate("Born"); // 播放出生动画

        yield return new WaitForSeconds(0.3f); // 等待动画播放完成
        startToMove = true;// 敌人开始移动
    }

    /// <summary>
    /// 设置敌人动画状态
    /// </summary>
    /// <param name="stateName">要播放的动画状态名称</param>
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

    /// <summary>
    /// 敌人跟随玩家移动的逻辑
    /// </summary>
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

    /// <summary>
    /// 检测敌人是否进入警告范围
    /// </summary>
    /// <returns>是否在警告范围内</returns>
    public bool InWarnninRange()
    {
        var enemyDistanceToPlayer = Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
        return enemyDistanceToPlayer <= warnningRange;
    }

    /// <summary>
    /// 检测敌人是否进入攻击范围
    /// </summary>
    /// <returns>是否在攻击范围内</returns>
    public bool InAttackRange()
    {
        var enemyDistanceToPlayer = Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
        return enemyDistanceToPlayer <= attackRange;
    }

    /// <summary>
    /// 处理敌人死亡逻辑
    /// </summary>
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

    /// <summary>
    /// 绘制调试范围
    /// </summary>
    public void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, warnningRange);
    }
}
