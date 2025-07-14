using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitNum : MonoBehaviour
{
    [SerializeField]Animator anim;
    [SerializeField]TMP_Text numText;
    void Start()
    {
        anim = GetComponent<Animator>();
        numText = transform.Find("A/Text").GetComponent<TMP_Text>();
    }

    public void PlayAnim(int num)
    {
        if (anim == null || numText == null)
        {
            Debug.LogWarning("找不到 Animator 或 TMP_Text 组件");
            return;
        }

        numText.text = num.ToString();
        anim.SetTrigger("Show");
    }


}
