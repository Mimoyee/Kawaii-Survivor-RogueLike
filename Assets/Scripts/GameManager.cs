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
    public PlayerController playerController;
    public UIManager uiManager;
    public SceneLoadManager sceneLoadManager;

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

        playerController = player.GetComponent<PlayerController>();
        uiManager = FindAnyObjectByType<UIManager>();
        sceneLoadManager = GetComponent<SceneLoadManager>();

        uiManager.Init();
        playerController.isInitDone = true;
        
        Debug.Log("GameManager 初始化完成");
        yield return null;
    }
}
