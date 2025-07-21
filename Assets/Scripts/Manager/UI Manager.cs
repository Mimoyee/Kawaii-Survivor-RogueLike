using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI 引用")]
    [SerializeField] Transform canvasTransform;
    private GameObject selectWeaponPanel;
    private GameObject gameOverPanel;
    private GameObject gameWinPanel;
    [Header("UI 组件")]

    [SerializeField] Button restartButton;
    [SerializeField] Button mainMenuButton;
    private TMP_Text playerHPText;
    private Slider playerHPSlider;
    private Slider playerXPSlider;
    private TMP_Text waveText;
    private TMP_Text waveCountText;
    private TMP_Text lvText;
    private Player player;

    void Start()
    {
        //Init();
    }


    public void Init()
    {
        player = GameManager.Instance.player;

        if (player == null)
        {
            Debug.LogWarning("Player对象未找到，请确保GameManager已正确初始化");
            return;
        }

        canvasTransform = GameObject.FindWithTag("UI").GetComponent<Transform>();
        selectWeaponPanel = canvasTransform.Find("SelectWeaponPanel").gameObject;
        gameOverPanel = canvasTransform.Find("GameOverPanel").gameObject;
        gameWinPanel = canvasTransform.Find("GameWinPanel").gameObject;
        gameWinPanel.SetActive(false);
        selectWeaponPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        selectWeaponPanel.SetActive(false);
        playerHPText = canvasTransform.Find("Left/HP/Text_HP_Num").GetComponent<TMP_Text>();
        playerHPText.text = "100/100";
        lvText = canvasTransform.Find("Left/XP/Text_Lv").GetComponent<TMP_Text>();
        lvText.text = "Lv 0";
        waveText = canvasTransform.Find("Mid_Top/Text_Wave").GetComponent<TMP_Text>();
        waveText.text = "Wave 1/10";
        waveCountText = canvasTransform.Find("Mid_Top/Text_KillNum").GetComponent<TMP_Text>();
        waveCountText.text = "1";
        playerHPSlider = canvasTransform.Find("Left/HP/Slider").GetComponent<Slider>();
        playerHPSlider.value = (float)player.playerHealth.currentHealth / player.playerHealth.maxHealth;
        playerXPSlider = canvasTransform.Find("Left/XP/Slider").GetComponent<Slider>();
        playerXPSlider.value = (float)player.playerHealth.currentLevelXP / player.playerHealth.NextLevelXP;

        restartButton = canvasTransform.Find("GameOverPanel/Panel/RestartButton").GetComponent<Button>();
        restartButton.onClick.AddListener(GameManager.Instance.sceneLoadManager.LoadGameScene);
        mainMenuButton = canvasTransform.Find("GameWinPanel/Panel/MainMenuButton").GetComponent<Button>();
        mainMenuButton.onClick.AddListener(GameManager.Instance.sceneLoadManager.LoadMainMenu);

        Debug.Log("UIManager 初始化完成");
        UpdateUI();
    }

    public void UpdateUI()
    {
        playerHPText.text = $"{player.playerHealth.currentHealth}/{player.playerHealth.maxHealth}";
        playerHPSlider.value = (float)player.playerHealth.currentHealth / player.playerHealth.maxHealth;
        playerXPSlider.value = (float)player.playerHealth.currentLevelXP / player.playerHealth.NextLevelXP;
        playerHPSlider.DOValue((float)player.playerHealth.currentHealth / player.playerHealth.maxHealth, 1f).SetEase(Ease.OutBack);
        playerXPSlider.DOValue((float)player.playerHealth.currentLevelXP / player.playerHealth.NextLevelXP, 1f).SetEase(Ease.OutBack);
    }

    public void ShowPanel(string panelName)
    {
        switch (panelName)
        {
            case "SelectWeaponPanel":
                selectWeaponPanel.SetActive(true);
                DoScaleAni(selectWeaponPanel.transform.GetChild(0).gameObject);
                break;
            case "GameOverPanel":
                gameOverPanel.SetActive(true);
                DoScaleAni(gameOverPanel.transform.GetChild(0).gameObject);
                break;
            default:
                Debug.LogWarning("未找到指定的面板: " + panelName);
                break;
        }
    }

    public void HidePanel(string panelName)
    {
        switch (panelName)
        {
            case "SelectWeaponPanel":
                selectWeaponPanel.SetActive(false);
                break;
            case "GameOverPanel":
                gameOverPanel.SetActive(false);
                break;
            default:
                Debug.LogWarning("未找到指定的面板: " + panelName);
                break;
        }
    }

    void DoScaleAni(GameObject obj)
    {
        obj.transform.localScale = Vector3.zero; // 确保初始缩放为0
        obj.transform.DOScale(new Vector3(1, 1, 1), 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            obj.transform.localScale = Vector3.one; // 确保最终缩放为1
        });
    }

    public IEnumerator DelaySHowPanel(string panelName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        ShowPanel(panelName);
    }

}
