using UnityEngine;

[ExecuteInEditMode] // ʹ�ýű��ڱ༭ģʽ��Ҳ������
public class SelectionOutline : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        // ��ȡ����İ�Χ��
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // ����������ɫΪ��ɫ
            Gizmos.color = Color.yellow;

            // ��������ı߽��
            Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.size);
        }
    }
}
