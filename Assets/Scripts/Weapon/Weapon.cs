using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("攻击伤害")][SerializeField] int damage = 100; // 伤害值
    [Header("引用")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform hitDetectionTransform; // 用于碰撞检测的位置
    [Header("最近距离的敌人")][SerializeField] private Transform closestEnemy; // 最近敌人位置

    [Header("射线参数设置")]
    [SerializeField] private float _scanRadius = 2f;//射线查找敌人的范围
    [SerializeField] private float _hitDectectionRadius = 0.58f;//射线攻击敌人的范围
    private float scanMinDistance;
    [SerializeField] private LayerMask _enemyLayer; // 敌人层
    [Header("扫描间隔_单位秒")][SerializeField] private float _scanInterval = 0.2f; // 射线扫描间隔
    private float _scanTimer;
    [Header("扫描最近敌人数量")][SerializeField] private int _maxScanNum = 20;
    [SerializeField] private Collider2D[] _scanColliders; // 射线扫描检测结果
    [Header("检测最近攻击敌人数量")][SerializeField] private int _maxhitNum = 10;
    [SerializeField] private Collider2D[] _hitColliders; // 射线攻击检测结果
    [Header("方向转向速度")][SerializeField] private float _directionSpeed = 15f; // 转向速度
    [Header("显示Gizmos射线范围")][SerializeField] bool showGizmos = true;
    // 定义武器状态
    private State state = State.Idle;
    private List<Enemy> damagedEnemys = new List<Enemy>(); // 记录已经攻击过的敌人

    enum State
    {
        Idle, // 待机状态
        Attack, // 攻击状态

    }

    void Start()
    {
        animator = GetComponent<Animator>();
        _scanColliders = new Collider2D[_maxScanNum]; // 预分配数组空间
        _hitColliders = new Collider2D[_maxhitNum]; // 预分配数组空间
        hitDetectionTransform = transform.Find("A/Hit_Pos"); // 找到碰撞检测位置
        _enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
    }

    void Update()
    {
        // 统一管理扫描计时器
        _scanTimer += Time.deltaTime;
        if (_scanTimer >= _scanInterval)
        {
            ScanForEnemies();
            _scanTimer = 0f;
        }

        switch (state)
        {
            case State.Idle:
                AutoAim();
                break;
            case State.Attack:
                Attacking();
                break;
        }
    }

    private void ScanForEnemies()
    {
        closestEnemy = null;
        scanMinDistance = _scanRadius;
        int numColliders = Physics2D.OverlapCircleNonAlloc(transform.position, _scanRadius, _scanColliders, _enemyLayer);
        if (numColliders > 0)
        {
            for (int i = 0; i < numColliders; i++)
            {
                if (_scanColliders[i] == null) continue;
                float distance = Vector3.Distance(_scanColliders[i].transform.position, transform.position);
                if (distance < scanMinDistance)
                {
                    scanMinDistance = distance;
                    closestEnemy = _scanColliders[i].transform;
                }
            }
        }
    }

    private void HandleIdleState()
    {
        state = State.Idle;
        //animator.SetTrigger("Idle");
        // 缓慢转向默认方向
        transform.up = Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime * _directionSpeed);
    }

    private void AutoAim()
    {
        if (closestEnemy == null)
        {
            // 缓慢转向默认方向
            transform.up = Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime * _directionSpeed);
            return;
        }
        // 计算目标方向
        Vector3 targetDirection = (closestEnemy.position - transform.position).normalized;
        // 平滑朝向
        transform.up = Vector3.Lerp(transform.up, targetDirection, Time.deltaTime * _directionSpeed);
    }

    private void Attack()
    {
        // 检测攻击范围内的敌人
        int hitNumColliders = Physics2D.OverlapCircleNonAlloc(hitDetectionTransform.position, _hitDectectionRadius, _hitColliders, _enemyLayer);

        if (hitNumColliders <= 0) return;

        for (int i = 0; i < hitNumColliders; i++)
        {
            Enemy enemy = _hitColliders[i].GetComponent<Enemy>();

            if (enemy != null)
            {
                if (!damagedEnemys.Contains(enemy))
                {
                    enemy.TakeDamage(damage); // 攻击敌人
                    damagedEnemys.Add(enemy); // 添加敌人到已攻击列表
                }
            }

            StopAttack();
        }

    }

    [NaughtyAttributes.Button]//添加按钮到面板
    public void StartAttack()
    {
        // 开始攻击时触发攻击动画, 通过动画的检测武器碰撞到的敌人
        state = State.Attack;
        animator.SetTrigger("Attack");
        damagedEnemys.Clear(); // 清空已攻击的敌人列表
    }

    public void Attacking()
    {
        Attack();
    }

    public void StopAttack()
    {
        HandleIdleState(); // 切换到待机状态
        damagedEnemys.Clear(); // 清空已攻击的敌人列表
    }


    public void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // 绘制扫描范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _scanRadius);

        // 绘制攻击范围
        Gizmos.color = Color.red;
        if (hitDetectionTransform != null)
            Gizmos.DrawWireSphere(hitDetectionTransform.position, _hitDectectionRadius);

        // 绘制到最近敌人的连线
        if (closestEnemy != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, closestEnemy.position);
        }
    }
}
