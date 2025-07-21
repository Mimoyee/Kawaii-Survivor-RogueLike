using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [Header("引用")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform hitDetectionTransform; // 用于碰撞检测的位置
    [Header("最近距离的敌人")][SerializeField] private Transform closestEnemy; // 最近敌人位置

    [Header("射线参数设置")]
    [SerializeField] private float _scanRadius = 5f;//射线查找敌人的范围
    [SerializeField] private float _hitDectectionRadius = 3f;//射线攻击敌人的范围
    private float scanMinDistance;
    [SerializeField] private LayerMask _enemyLayer; // 敌人层
    [Header("扫描间隔_帧数")][SerializeField] private float _scanInterval = 0.2f; // 射线扫描间隔
    private float _scanTimer;

    [Header("扫描最近敌人数量")][SerializeField] private int _maxScanNum = 20;
    [SerializeField] private Collider2D[] _scanColliders; // 射线扫描检测结果
    [Header("检测最近攻击敌人数量")][SerializeField] private int _maxhitNum = 10;
    [SerializeField] private Collider2D[] _hitColliders; // 射线攻击检测结果
    [Header("方向转向速度")][SerializeField] private float _directionSpeed = 5f; // 射线扫描间隔
    [Header("显示Gizmos射线范围")][SerializeField] bool showGizmos = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        _scanColliders = new Collider2D[_maxScanNum]; // 预分配数组空间
        _hitColliders = new Collider2D[_maxhitNum]; // 预分配数组空间
    }

    void Update()
    {
        // 更新扫描计时器
        _scanTimer += Time.deltaTime;

        // 定期扫描敌人（性能优化）
        if (_scanTimer >= _scanInterval)
        {
            ScanForEnemies();
            _scanTimer = 0f;
        }

        // 如果有目标，进行瞄准和攻击
        if (closestEnemy != null)
        {
            AutoAim();
            Attack();
        }
        else
        {
            HandleIdleState();
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
        // 没有敌人时的待机处理
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            animator.SetTrigger("Idle");
        }

        // 缓慢转向默认方向
        transform.up = Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime * _directionSpeed);
    }

    private void AutoAim()
    {
        if (closestEnemy == null) return;

        // 计算目标方向
        Vector3 targetDirection = (closestEnemy.position - transform.position).normalized;

        // 平滑朝向
        transform.up = Vector3.Lerp(transform.up, targetDirection, Time.deltaTime * _directionSpeed);
    }

    private void Attack()
    {
        int hitNumColliders = Physics2D.OverlapCircleNonAlloc(hitDetectionTransform.position, _hitDectectionRadius, _hitColliders, _enemyLayer);

        if (hitNumColliders <= 0)
        {
            return;
        }

        bool attacked = false;
        for (int i = 0; i < hitNumColliders; i++)
        {
            if (_hitColliders[i] == null) continue;
            Enemy enemy = _hitColliders[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(100); // 对敌人造成伤害
                attacked = true;
                // 只攻击第一个敌人（如需群攻可去掉break）
                break;
            }
        }
        if (attacked)
        {
            animator.SetTrigger("Attack"); // 播放攻击动画
        }
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
