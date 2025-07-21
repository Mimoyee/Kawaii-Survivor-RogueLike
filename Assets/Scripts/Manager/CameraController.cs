using System.Collections; // 引入系统集合命名空间
using System.Collections.Generic; // 引入泛型集合命名空间
using UnityEngine; // 引入Unity引擎命名空间

/// <summary>
/// 控制摄像机跟随玩家并限制摄像机移动范围
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] Transform playerTransform; // 玩家Transform引用
    [SerializeField, Header("摄像机限制移动范围")] bool useCameraClame = false;
    [SerializeField] float MinMax_X, MinMax_Y; // 摄像机移动的最小/最大X、Y坐标
    [SerializeField, Header("摄像机跟随速度")] float cameraSpeed = 1f; // 摄像机跟随速度
    
    [Header("摄像机缩放")]
    [SerializeField] float zoomStartSize = 7f;
    [SerializeField]float zoomEndSize = 5f;
    [SerializeField] float zoomDuration = 2f;
    [SerializeField] float zoomSpeed = 2f;

    void Awake()
    {
        // 通过标签找到Player对象，并获取其Transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        transform.position = new Vector3(0, 0, -10);
        StartCoroutine(ZoomIn()); // 启动摄像机缩放协程
    }

    void LateUpdate()
    {
        if (playerTransform == null || GameManager.Instance.player.isDead) return; // 如果玩家Transform未设置，则不执行任何操作
        // 使用Lerp实现摄像机平滑跟随玩家
        transform.position = Vector3.Lerp(transform.position, playerTransform.position, cameraSpeed * Time.deltaTime);
        // 如果useCameraClame为true，则限制摄像机的X、Y坐标在指定范围内
        if (useCameraClame)
        {
            // 限制摄像机的X、Y坐标在指定范围内，Z轴固定为-10
            transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -MinMax_X, MinMax_X),
            Mathf.Clamp(transform.position.y, -MinMax_Y, MinMax_Y),
            -10);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }

    }

    IEnumerator ZoomIn()
    {
        Camera.main.orthographicSize = zoomStartSize;
        float elapsedTime = 0f;
        
        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime * zoomSpeed;
            float t = Mathf.Clamp01(elapsedTime / zoomDuration);
            Camera.main.orthographicSize = Mathf.Lerp(zoomStartSize, zoomEndSize, t);
            yield return null;
        }
        
        Camera.main.orthographicSize = zoomEndSize; // 确保最终精确值
    }
}
