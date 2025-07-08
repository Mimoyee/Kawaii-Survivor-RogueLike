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
    [SerializeField] float Min_X, Max_X, Min_Y, Max_Y; // 摄像机移动的最小/最大X、Y坐标
    [SerializeField, Header("摄像机跟随速度")] float cameraSpeed = 1f; // 摄像机跟随速度

    /// <summary>
    /// 在Awake中查找Player对象并获取其Transform
    /// </summary>
    void Awake()
    {
        // 通过标签找到Player对象，并获取其Transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        transform.position = new Vector3(0, 0, -10);
    }

    /// <summary>
    /// 在LateUpdate中让摄像机平滑跟随玩家，并限制摄像机移动范围
    /// </summary>
    void LateUpdate()
    {
        // 使用Lerp实现摄像机平滑跟随玩家
        transform.position = Vector3.Lerp(transform.position, playerTransform.position, cameraSpeed * Time.deltaTime);
        // 如果useCameraClame为true，则限制摄像机的X、Y坐标在指定范围内
        if (useCameraClame)
        {
            // 限制摄像机的X、Y坐标在指定范围内，Z轴固定为-10
            transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, Min_X, Max_X),
            Mathf.Clamp(transform.position.y, Min_Y, Max_Y),
            -10);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }

    }
}
