using UnityEngine;
using UnityEngine.UI;

public class TestMousePos : MonoBehaviour
{
    private RectTransform rectTransform;
    public Camera uiCamera; // 拖拽你的UI相机到这里
    Image image;
    Canvas canvas;
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        image.enabled = false;
        // 如果没有指定相机，尝试获取Canvas的渲染相机
        if (uiCamera == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                uiCamera = canvas.worldCamera;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (uiCamera == null) return;
        if (Input.GetMouseButtonDown(0))
        {
            //gameObject.transform.position = Input.mousePosition;
            FollowMousePos();
            image.enabled = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            image.enabled = false;
        }

    }

    void FollowMousePos()
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            gameObject.transform.position = Input.mousePosition;
        }
        else // canvas.renderMode == ScreenSpaceCamera
        {

            Vector2 mousePos;
            // 将鼠标屏幕坐标转换为UI的RectTransform局部坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
                rectTransform.parent as RectTransform,
                Input.mousePosition,
                uiCamera,
                out mousePos
            );

            // 更新Image的位置
            rectTransform.localPosition = mousePos;
        }
    }
}
