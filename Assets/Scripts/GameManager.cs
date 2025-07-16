using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //单例模式
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [Header("引用")]
    public Player player;
    public UIManager uiManager;
    public SceneLoadManager sceneLoadManager;
    public float delayStartTime = 1.5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }

    void OnEnable()
    {
        StartCoroutine(Init());
    }

    void Start()
    {
        //Application.targetFrameRate = 60;
    }

    public IEnumerator Init()
    {
        // 等待直到找到Player对象
        while (player == null)
        {
            player = FindAnyObjectByType<Player>();
            if (player == null)
            {
                yield return null;
            }
        }

        player.Init(); // 初始化玩家

        uiManager = FindAnyObjectByType<UIManager>();
        sceneLoadManager = GetComponent<SceneLoadManager>();

        uiManager.Init();

        Debug.Log("GameManager 初始化完成");
        yield return null;
    }
}
