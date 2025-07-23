using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("引用")]
    EnemyMovement enemyMovement;
    private Slider healthSlider; // 生命值滑动条
    private TMP_Text healthText; // 生命值文本
    [Header("敌人生命")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [HideInInspector] public bool isDead = false;
    [Header("敌人参数设置")]
    private float nextAttackTime = 0f;
    [SerializeField, Tooltip("敌人攻击间隔")] private float attackInterval = 1f;
    public int damage = 5; //敌人攻击玩家造成的伤害

    void Start()
    {
        Init();
    }

    public void Init()
    {
        // 初始化敌人状态
        isDead = false;
        currentHealth = maxHealth; // 重置当前生命值

        enemyMovement = GetComponent<EnemyMovement>();
        healthSlider = transform.Find("OtherObject/Canvas/HP_Bar/Slider").GetComponent<Slider>();
        healthText = transform.Find("OtherObject/Canvas/HP_Bar/Text_HP").GetComponent<TMP_Text>();
        healthText.text = currentHealth.ToString(); // 更新生命值文本显示     
        healthSlider.value = 1; // 设置滑动条当前值     
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

    public void TakeDamage(int damage)
    {
        if (isDead) return; // 如果敌人已经死亡，直接返回
        enemyMovement.SetAnimate("Hit");
        currentHealth -= damage; // 减少当前生命值
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 确保当前生命值不小于0
        // 处理敌人受到伤害逻辑
        Debug.Log($"敌人受到 {damage} 点伤害");
        // 这里可以添加受伤动画或其他逻辑

        // 检查敌人是否死亡
        if (currentHealth <= 0)
        {
            enemyMovement.Die();
        }

        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth; // 更新滑动条值
        }
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString(); // 更新生命值文本显示
        }
    }



}
