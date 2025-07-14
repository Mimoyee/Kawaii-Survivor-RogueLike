using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Enemy enemy;
    [SerializeField] private GameObject fx_dead;
    GameObject canvasObject;

    [Header("敌人参数设置")]
    //[SerializeField] private float health = 100f;
    [SerializeField] private float delayShowTime = 1f; //延迟显示Sprite的时间
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveSpeedFast = 2f;
    private float startSpeed = 0f;
    [SerializeField] private float stopDistanceToPlayer = 1f;
    [SerializeField] private float warnningDistanceToPlayer = 3f;


    //动画
    private Animator animator;
    private GameObject otherObject;
    private GameObject enemySprite;
    private GameObject spawnIndicator;
    //Rigidbody2D rb;

    bool hasSpawned = false;

    [Header("Gizmos设置")]
    [SerializeField] bool showGizmos = true;

    void Start()
    {
        Init();
        StartCoroutine(ShowEnemySprite()); //延迟显示敌人Sprite
        moveSpeed = Random.Range(moveSpeed * 0.8f, moveSpeed * 1.8f); //随机设置敌人移动速度
        moveSpeedFast = moveSpeed * 1.3f; //设置快速移动速度
        startSpeed = moveSpeed;
    }

    void Update()
    {
        if (enemy.isDead || !hasSpawned) return; //如果敌人已经死亡，则不执行任何操作
        if (GameManager.Instance.player == null)
        {
            //Debug.LogWarning("Player对象未设置，无法追逐玩家");
            return;
        }
        ; //如果玩家对象为空，则不执行任何操作
        if (GameManager.Instance.player.isDead)
        {
            SetAnimate("Idle");
            return;
        }

        if (!CanChasePlayer())
        {
            if(enemy.isDead) return; //如果敌人已经死亡，则不执行任何操作
            enemy.Attack(enemy.damage); //如果敌人可以追逐玩家，则攻击玩家
        }
        else
        {
            FollowPlayer();
        }
    }

    void Init()
    {
        animator = GetComponent<Animator>();
        //rb = GetComponent<Rigidbody2D>();
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
    }

    IEnumerator ShowEnemySprite()
    {
        yield return new WaitForSeconds(delayShowTime);
        enemySprite?.SetActive(true); // 显示敌人Sprite  
        otherObject?.SetActive(true); // 激活其他对象
        canvasObject.SetActive(false); // 激活Canvas对象
        spawnIndicator?.SetActive(false); // 隐藏出生点指示器
        //rb.velocity = Vector2.zero; //清空速度
        SetAnimate("Born"); // 播放出生动画
        yield return new WaitForSeconds(0.3f); // 等待动画播放完成
        hasSpawned = true;
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

            if (WarnningPlayer())
            {
                SetAnimate("Warnning");
                //Debug.Log("警告玩家!");
                moveSpeed = moveSpeedFast; //加快敌人的速度
            }
            else
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

    public bool WarnningPlayer()
    {
        var enemyDistanceToPlayer = Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
        return enemyDistanceToPlayer <= warnningDistanceToPlayer; //如果敌人与玩家之间的距离小于等于指定的距离，则警告玩家
    }

    public bool CanChasePlayer()
    {
        var enemyDistanceToPlayer = Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
        return enemyDistanceToPlayer > stopDistanceToPlayer; //如果敌人与玩家之间的距离大于指定的距离，则可以追逐玩家
        //反之, 靠近Player进行攻击
    }

    public void Die()
    {
        enemy.isDead = true;
        SetAnimate("Dead");
        fx_dead.transform.parent = null; //将死亡特效从敌人对象中分离出来
        fx_dead.SetActive(true); //激活死亡特效
        //rb.velocity = Vector2.zero; //清空速度
        otherObject?.SetActive(false);
        Destroy(gameObject, 2f); //销毁敌人
    }



    public void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistanceToPlayer);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, warnningDistanceToPlayer);
    }


}
