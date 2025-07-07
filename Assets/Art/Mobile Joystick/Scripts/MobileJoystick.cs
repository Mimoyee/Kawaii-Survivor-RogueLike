using UnityEngine;
using UnityEngine.UI;

public class MobileJoystick : MonoBehaviour
{
    [Header(" UI 摇杆Transform ")]
    [SerializeField] private RectTransform joystickOutline;
    [SerializeField] private RectTransform joystickKnob;

    [Header(" 移动速度倍率 ")]
    [SerializeField] private float moveFactor;
    private Vector3 clickedPosition;
    private Vector3 move_offset;
    private bool canControl;
    [SerializeField] float canvasScale = 1f;
    Canvas canvas;
    Camera uiCamera;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.worldCamera;

        canvasScale = canvas.GetComponent<RectTransform>().localScale.x;

        HideJoystick();
    }

    private void OnDisable()
    {
        HideJoystick();
    }


    void Update()
    {
        if (canControl)
            ControlJoystick();
    }

    //面板点击事件PointerDown, EventTrigger
    public void ClickedOnJoystickZoneCallback()
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            //获取点击位置
            clickedPosition = Input.mousePosition;
            //设置joystickOutline外框的位置
            joystickOutline.position = clickedPosition;
        }
        else //如果Canvas不是Overlay类型，= ScreenSpace Camera
        {
            // 将鼠标屏幕坐标转换为UI的RectTransform局部坐标
            Vector2 mousePos2D;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                joystickOutline.parent as RectTransform,
                Input.mousePosition,
                uiCamera,
                out mousePos2D);

            // 更新位置
            clickedPosition = mousePos2D;
            joystickOutline.localPosition = clickedPosition;
        }

        ShowJoystick();
    }

    private void ShowJoystick()
    {
        joystickOutline.gameObject.SetActive(true);
        canControl = true;
    }

    private void HideJoystick()
    {
        joystickOutline.gameObject.SetActive(false);
        canControl = false;
        //重置位置
        move_offset = Vector3.zero;
    }

    private void ControlJoystick()
    {
        Vector3 currentPosition;
        Vector3 direction;
        Vector3 targetPosition;
        float moveMagnitude;
        float absoluteWidth;
        float realWidth;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            currentPosition = Input.mousePosition;
            //Debug.Log("当前位置 = " + currentPosition);
            direction = currentPosition - clickedPosition; //获取鼠标位置与点击位置的方向

            absoluteWidth = joystickOutline.rect.width / 2;

            realWidth = absoluteWidth * canvasScale;

            moveMagnitude = direction.magnitude * moveFactor * canvasScale; ;
            //限制移动距离 = 取两者最小值
            moveMagnitude = Mathf.Min(moveMagnitude, realWidth);
            //最终移动方向 = 方向向量 * 移动距离
            move_offset = direction.normalized * moveMagnitude;
            //圆点目标位置 = 点击位置 + 最终移动方向
            targetPosition = clickedPosition + move_offset;
            //限制内部圆点位置在屏幕范围内
            joystickKnob.position = targetPosition;
        }
        else //如果Canvas类型 = ScreenSpace Camera
        {
            // 将鼠标屏幕坐标转换为UI的RectTransform局部坐标
            Vector2 mousePos2D;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                joystickOutline.parent as RectTransform,
                Input.mousePosition,
                uiCamera,
                out mousePos2D);

            currentPosition = mousePos2D;//拖拽鼠标移动的位置

            direction = currentPosition - clickedPosition;//获取鼠标位置与点击位置的方向
            moveMagnitude = direction.magnitude;//方向的长度
            absoluteWidth = joystickOutline.rect.width / 2;//限制圆点在外圈范围内
            //限制移动距离 = 最小值是拖动方向的长度, 最大值 = 外圈范围 / 2
            moveMagnitude = Mathf.Min(moveMagnitude, absoluteWidth);
            //移动的偏移量 = 方向向量 * 移动距离
            move_offset = direction.normalized * moveMagnitude;
            //圆点起始位置 = 外圈中心位置
            targetPosition = joystickOutline.transform.position;
            //圆点目标移动位置 = 圆点起始位置 + 偏移量
            joystickKnob.localPosition = targetPosition + move_offset;
        }

        //松开鼠标时隐藏摇杆
        if (Input.GetMouseButtonUp(0))
            HideJoystick();
    }

    public Vector3 GetMoveVector()
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return move_offset / canvasScale;
        }
        else //如果Canvas类型 = ScreenSpace Camera
        {
            return move_offset;
        }
    }

}
