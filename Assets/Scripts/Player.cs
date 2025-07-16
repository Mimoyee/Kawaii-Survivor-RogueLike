using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public PlayerController playerController;

    [Header("玩家属性")]
    public int attackDamage = 10; // 玩家攻击力
    public float attackRange = 1f; // 玩家攻击范围
    [HideInInspector] public bool isDead = false; // 玩家是否死亡
    private HitNum hitNum; // HitNum组件引用

    // void Start()
    // {
    //     Init();
    // }


    public void Init()
    {
        hitNum = transform.Find("Hit_Num").GetComponent<HitNum>();
        playerController = GetComponent<PlayerController>();
        playerHealth = GetComponent<PlayerHealth>();
        playerHealth.Init(); // 初始化玩家生命值
        playerController.isInitDone = true; // 初始化完成
        Debug.Log("Player 初始化完成");
    }

    public void Attack()
    {
        // 攻击逻辑
    }

    public void Heal(int amount)
    {
        playerHealth.currentHealth += amount;
        if (playerHealth.currentHealth > 100)
        {
            playerHealth.currentHealth = 100;
        }
    }

    public void TakeDamage(int damage)
    {
        playerHealth.currentHealth -= damage;
        playerController.SetAnimate("Hit");
        hitNum.PlayAnim(damage); // 播放伤害数字动画

        if (playerHealth.currentHealth <= 0)
        {
            playerHealth.currentHealth = 0;
            GameManager.Instance.uiManager.UpdateUI();
            Die();
        }
        GameManager.Instance.uiManager.UpdateUI(); // 更新UI显示
    }

    private void Die()
    {
        isDead = true;
        playerController.SetAnimate("Dead");
        playerController.StopMovement(); // 停止玩家移动

        //动画播放完毕后，Destory(gameObject)
        Destroy(gameObject, 1.5f);
        // 执行死亡逻辑，比如播放死亡动画，禁用玩家控制等
        StartCoroutine(GameManager.Instance.uiManager.DelaySHowPanel("GameOverPanel", 1.4f));

    }
}
