using System.Collections; // 引入系统集合命名空间
using System.Collections.Generic; // 引入泛型集合命名空间
using UnityEngine; // 引入Unity引擎命名空间

//[ExecuteInEditMode] // 允许在编辑模式下执行脚本
/// <summary>
/// 根据物体的y坐标动态调整SpriteRenderer的sortingOrder，实现2D层级排序（y越小越靠前）
/// </summary>
public class SpriteSorter : MonoBehaviour
{
    SpriteRenderer spriteRenderer; // 该物体的SpriteRenderer组件

    private float lastY; // 上一次记录的y坐标

    void Start()
    {
        // 检查是否有SpriteRenderer组件
        if (GetComponent<SpriteRenderer>() == null)
        {
            Debug.LogWarning($"{gameObject.name} 没有 SpriteRenderer 组件，请添加一个 SpriteRenderer 组件。");
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastY = transform.position.y;
        // 初始化时设置一次sortingOrder
        spriteRenderer.sortingOrder = (int)(lastY * -10);
    }

    void Update()
    {
        float currentY = transform.position.y;
        // 只有y坐标发生变化时才更新sortingOrder，提升性能
        // 如果y坐标发生变化，则更新sortingOrder，实现动态层级排序
        if (!Mathf.Approximately(currentY, lastY))
        {
            // 根据y坐标设置sortingOrder，y越小越靠前
            spriteRenderer.sortingOrder = (int)(currentY * -10);
            // 记录本次y坐标，便于下次对比
            lastY = currentY;
            //Debug.Log($"Sprite层级排序:<color=yellow>{gameObject.name}</color> 的 y 坐标已更新为 {currentY}，排序顺序已更新为 {spriteRenderer.sortingOrder}");
        }
    }
}
