using System.Collections.Generic;
using UnityEngine;

// 游戏管理器类，负责管理游戏的整体状态和排行榜相关数据
public class GameManager : MonoBehaviour
{
    // 单例模式，用于在整个游戏中获取唯一的GameManager实例
    public static GameManager instance;

    // 玩家分数
    [Header("参数")]
    public int m_Points = 0;
    // 游戏是否结束
    public bool m_GameOver = false;
    // 玩家姓名
    public string m_PlayerName = null;

    // 在对象被唤醒时调用，用于初始化单例模式
    private void Awake()
    {
        // 如果GameManager实例为空，则将当前对象赋值给instance，并确保在场景切换时不被销毁
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // 如果已经存在GameManager实例且不是当前对象，则销毁当前对象
        else
        {
            Destroy(gameObject);
        }
    }

    // 可序列化的类，用于存储排行榜中的单个玩家数据（姓名和分数）
    [System.Serializable]
    public class Save
    {
        public string name;
        public int score;
    }

    // 用于存储排行榜数据的列表，每个元素是一个Save对象
    public List<Save> saves = new List<Save>();
    // 排行榜显示的最大条目数
    private int rankLimit = 10;

    // 从本地存储中解析排行榜数据字符串为Save对象列表
    public void ParseInfo()
    {
        // 清空当前的排行榜数据列表
        saves = new List<Save>();
        // 从PlayerPrefs中获取名为"PONGPI_SaveInfo"的字符串，该字符串存储了排行榜数据
        string saveString = PlayerPrefs.GetString("PONGPI_SaveInfo");
        // 如果获取到的字符串不为空，则进行解析
        if (!string.IsNullOrEmpty(saveString))
        {
            // 按'|'分割字符串，得到每个排名项的字符串数组
            string[] rankItemArry = saveString.Split('|');
            // 遍历每个排名项字符串
            for (int ident = 0; ident < rankItemArry.Length; ident++)
            {
                // 按'+'分割排名项字符串，得到姓名和分数的字符串数组
                string[] rankItem = rankItemArry[ident].Split('+');
                // 创建一个新的Save对象
                Save save = new()
                {
                    // 将分割得到的姓名赋值给Save对象的name属性
                    name = rankItem[0],
                    // 将分割得到的分数转换为整数并赋值给Save对象的score属性
                    score = int.Parse(rankItem[1])
                };
                // 将Save对象添加到排行榜数据列表中
                saves.Add(save);
            }
        }
        // 如果排行榜数据列表不为空，则对其进行排序
        if (saves.Count != 0)
        {
            saves.Sort(CompareScore);
        }
    }

    // 比较两个Save对象的分数，用于排行榜数据排序
    int CompareScore(Save player_1, Save player_2)
    {
        // 获取第一个玩家的分数
        int score_1 = player_1.score;
        // 获取第二个玩家的分数
        int score_2 = player_2.score;
        // 如果两个玩家分数相等，返回0
        if (score_1 == score_2)
        {
            return 0;
        }
        // 如果第一个玩家分数大于第二个玩家分数，返回 -1，表示第一个玩家排名靠前
        else if (score_1 > score_2)
        {
            return -1;
        }
        // 如果第一个玩家分数小于第二个玩家分数，返回1，表示第一个玩家排名靠后
        else
        {
            return 1;
        }
    }

    // 将当前玩家数据添加到排行榜数据中并保存到本地存储
    public void SaveInfo()
    {
        // 先解析本地存储中的排行榜数据，确保数据是最新的
        ParseInfo();
        // 创建一个新的Save对象，用于存储当前玩家数据
        Save save = new Save
        {
            name = m_PlayerName,
            score = m_Points
        };
        // 将当前玩家数据添加到排行榜数据列表中
        saves.Add(save);
        // 对排行榜数据列表进行排序
        saves.Sort(CompareScore);
        // 如果排行榜数据列表长度超过了最大条目数，则移除最后一个元素（即分数最低的那个）
        if (saves.Count > rankLimit)
        {
            saves.RemoveAt(saves.Count - 1);
        }
        // 用于构建要保存到本地存储的字符串
        string saveString = "";
        // 遍历排行榜数据列表
        for (int ident = 0; ident < saves.Count; ident++)
        {
            // 构建每个排名项的字符串，格式为"姓名+分数"
            string temp = "";
            temp += saves[ident].name;
            temp += "+";
            temp += saves[ident].score;
            // 如果不是最后一个排名项，则添加'|'作为分隔符
            if (ident != saves.Count - 1)
            {
                temp += "|";
            }
            // 将构建好的排名项字符串添加到总的保存字符串中
            saveString += temp;
        }
        // 将构建好的排行榜数据字符串保存到PlayerPrefs中，键为"PONGPI_SaveInfo"
        PlayerPrefs.SetString("PONGPI_SaveInfo", saveString);
    }

    /// <summary>
    /// 获取已保存到本地存储中的排行榜数据列表
    /// </summary>
    /// <returns>排行榜数据列表（Save List）</returns>
    public List<Save> GetInfo()
    {
        // 先解析排行榜数据，确保获取到的是最新数据
        ParseInfo();
        // 返回排行榜数据列表
        return saves;
    }
}