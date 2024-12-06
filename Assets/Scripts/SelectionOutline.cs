using UnityEngine;

[ExecuteInEditMode] // 使得脚本在编辑模式下也能运行
public class SelectionOutline : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        // 获取物体的包围盒
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // 设置轮廓颜色为黄色
            Gizmos.color = Color.yellow;

            // 绘制物体的边界框
            Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.size);
        }
    }
}
