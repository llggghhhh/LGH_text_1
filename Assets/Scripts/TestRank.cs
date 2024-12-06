using UnityEngine;

public class TestRank : MonoBehaviour
{
    public UIManager uiManager;
    public GameManager gameManager;

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            // 模拟游戏结束，设置玩家分数和姓名
            gameManager.m_Points = 50 - i;
            gameManager.m_PlayerName = "Player"+i;
            // 保存排行榜数据
            gameManager.SaveInfo();

            // 初始化排行榜UI
            uiManager.Initate();
        }
    }
}