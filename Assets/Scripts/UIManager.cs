using System.Collections.Generic;
using UnityEngine;

// 用户界面管理器类，负责管理排行榜的用户界面显示
public class UIManager : MonoBehaviour
{
    // 排行榜单个条目的预制体
    [Header("排行榜列表")]
    private RankItem rankItemTemplate;
    // 排行榜条目父物体的Transform，用于组织排行榜条目的布局
    public Transform Grid;
    // 用于存储已创建的排行榜条目实例的列表
    private List<RankItem> rankItem_List = new List<RankItem>();

    // 在对象启动时调用，用于获取排行榜单个条目的预制体引用
    private void Start()
    {
        // 在场景中查找名为"RankItem"的对象，并获取其RankItem组件，赋值给rankItemTemplate
        rankItemTemplate = transform.Find("RankItem").GetComponent<RankItem>();
        // 这里注释掉了Initate函数的调用，可能是为了根据实际需求在其他地方触发排行榜的初始化
        // Initate();
    }

    // 初始化排行榜界面，根据排行榜数据创建和更新排行榜条目显示
    public void Initate()
    {
        // 如果已经创建了排行榜条目，则将它们全部设置为非激活状态
        if (rankItem_List.Count > 0)
        {
            foreach (RankItem temp in rankItem_List)
            {
                temp.gameObject.SetActive(false);
            }
        }
        // 从GameManager获取排行榜数据列表
        List<GameManager.Save> saves = GameManager.instance.GetInfo();
        // 如果排行榜数据列表为空，则直接返回，不进行界面更新
        if (saves.Count <= 0)
        {
            return;
        }
        else
        {
            // 遍历排行榜数据列表
            for (int i = 0; i < saves.Count; i++)
            {
                // 获取或创建一个排行榜条目实例
                RankItem rankItem = GetRankItem();
                // 更新排行榜条目的显示内容，传入排名、姓名和分数
                rankItem.Initiate(i + 1, saves[i].name, saves[i].score);
            }
        }
    }

    // 创建一个新的排行榜条目实例
    private RankItem CreateNewRankItem()
    {
        // 如果排行榜单个条目预制体为空，则在场景中查找并获取其组件
        if (rankItemTemplate == null)
        {
            rankItemTemplate = transform.Find("RankItem").GetComponent<RankItem>();
        }
        // 克隆排行榜单个条目预制体
        GameObject gameObject = Instantiate(rankItemTemplate.gameObject);
        // 获取克隆后的对象的RankItem组件
        RankItem newRankItem = gameObject.GetComponent<RankItem>();
        // 设置克隆后的对象的父物体为Grid
        newRankItem.transform.SetParent(Grid);
        // 激活克隆后的游戏物体
        newRankItem.gameObject.SetActive(true);
        // 设置克隆后的对象的本地缩放为单位向量（保持原始大小）
        newRankItem.transform.localScale = Vector3.one;
        // 将新创建的排行榜条目实例添加到列表中
        rankItem_List.Add(newRankItem);
        // 返回新创建的排行榜条目实例
        return newRankItem;
    }

    // 获取一个可用的排行榜条目实例，如果没有可用的则创建一个新的
    private RankItem GetRankItem()
    {
        // 遍历已创建的排行榜条目列表
        foreach (RankItem temp in rankItem_List)
        {
            // 如果某个条目在层级中处于非激活状态，则激活它并返回
            if (!temp.gameObject.activeInHierarchy)
            {
                temp.gameObject.SetActive(true);
                return temp;
            }
        }
        // 如果所有条目都处于激活状态，则创建一个新的排行榜条目实例
        RankItem rankItem = CreateNewRankItem();
        return rankItem;
    }
}