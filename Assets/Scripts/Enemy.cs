using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("引用")]
    EnemyMovement enemyMovement;
    [HideInInspector] public bool isDead = false;
    [Header("敌人参数设置")]
    private float nextAttackTime = 0f;
    [SerializeField, Tooltip("敌人攻击间隔")] private float attackInterval = 1f;
    public int damage = 5; //敌人攻击玩家造成的伤害
    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    public void Attack()
    {
        if (GameManager.Instance.player != null && !isDead)
        {

            if (Time.time > nextAttackTime)//攻击间隔
            {
                nextAttackTime = Time.time + attackInterval;
                //播放攻击动画
                enemyMovement.SetAnimate("Attack");
                //这里可以添加攻击逻辑，比如减少玩家的生命值等

                //自爆攻击
                //enemyMovement.Die(); 
                //Debug.Log("攻击玩家");
            }
            else //攻击间隔未到,待机
            {
                //如果当前动画状态不是攻击，才设置待机动画
                if (!enemyMovement.GetCurrentAnimatorStateInfo("Attack"))
                {
                    enemyMovement.SetAnimate("Idle");
                }
            }
        }
        else
        {
            Debug.LogWarning("Player对象未设置，无法攻击玩家");
        }
    }

    public void DamagePlayer()
    {
        GameManager.Instance.player.TakeDamage(damage);
    }



}
