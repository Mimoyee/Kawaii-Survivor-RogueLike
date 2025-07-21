using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("玩家属性")]
    public int currentHealth = 100; // 玩家生命值
    public int maxHealth = 100; // 玩家最大生命值
    public int currentLevelXP = 100;
    public int NextLevelXP = 100;

    void Start()
    {

    }

    public void Init()
    {
        currentHealth = maxHealth; // 玩家初始生命值
    }
}
