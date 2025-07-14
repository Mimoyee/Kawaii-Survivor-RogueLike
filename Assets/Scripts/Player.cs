using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("玩家属性")]
    public int currentHealth = 100; // 玩家生命值
    public int maxHealth = 100; // 玩家最大生命值
    public int currentLevelXP = 100;
    public int NextLevelXP = 100;
    public int attackDamage = 10; // 玩家攻击力
    public float attackRange = 1f; // 玩家攻击范围
    [HideInInspector] public bool isDead = false; // 玩家是否死亡
    private HitNum hitNum; // HitNum组件引用

    void Start()
    {
        Init();
    }


    void Update()
    {

    }

    void Init()
    {
        // 初始化玩家相关设置
        currentHealth = maxHealth;
        hitNum = transform.Find("Hit_Num").GetComponent<HitNum>();

    }

    public void Attack()
    {
        // 攻击逻辑
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > 100)
        {
            currentHealth = 100;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        GameManager.Instance.playerController.SetAnimate("Hit");
        hitNum.PlayAnim(damage); // 播放伤害数字动画

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.uiManager.UpdateUI();
            Die();
        }
        GameManager.Instance.uiManager.UpdateUI(); // 更新UI显示
    }

    private void Die()
    {
        isDead = true;
        GameManager.Instance.playerController.SetAnimate("Dead");
        GameManager.Instance.playerController.StopMovement(); // 停止玩家移动

        //动画播放完毕后，Destory(gameObject)
        Destroy(gameObject, 1.5f);
        // 执行死亡逻辑，比如播放死亡动画，禁用玩家控制等
        StartCoroutine(GameManager.Instance.uiManager.DelaySHowPanel("GameOverPanel", 1.4f));

    }
}
