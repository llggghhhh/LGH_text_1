using UnityEngine;

public class TestRank : MonoBehaviour
{
    public UIManager uiManager;
    public GameManager gameManager;

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            // ģ����Ϸ������������ҷ���������
            gameManager.m_Points = 50 - i;
            gameManager.m_PlayerName = "Player"+i;
            // �������а�����
            gameManager.SaveInfo();

            // ��ʼ�����а�UI
            uiManager.Initate();
        }
    }
}