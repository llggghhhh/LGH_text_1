using System.Collections.Generic;
using UnityEngine;

// �û�����������࣬����������а���û�������ʾ
public class UIManager : MonoBehaviour
{
    // ���а񵥸���Ŀ��Ԥ����
    [Header("���а��б�")]
    private RankItem rankItemTemplate;
    // ���а���Ŀ�������Transform��������֯���а���Ŀ�Ĳ���
    public Transform Grid;
    // ���ڴ洢�Ѵ��������а���Ŀʵ�����б�
    private List<RankItem> rankItem_List = new List<RankItem>();

    // �ڶ�������ʱ���ã����ڻ�ȡ���а񵥸���Ŀ��Ԥ��������
    private void Start()
    {
        // �ڳ����в�����Ϊ"RankItem"�Ķ��󣬲���ȡ��RankItem�������ֵ��rankItemTemplate
        rankItemTemplate = transform.Find("RankItem").GetComponent<RankItem>();
        // ����ע�͵���Initate�����ĵ��ã�������Ϊ�˸���ʵ�������������ط��������а�ĳ�ʼ��
        // Initate();
    }

    // ��ʼ�����а���棬�������а����ݴ����͸������а���Ŀ��ʾ
    public void Initate()
    {
        // ����Ѿ����������а���Ŀ��������ȫ������Ϊ�Ǽ���״̬
        if (rankItem_List.Count > 0)
        {
            foreach (RankItem temp in rankItem_List)
            {
                temp.gameObject.SetActive(false);
            }
        }
        // ��GameManager��ȡ���а������б�
        List<GameManager.Save> saves = GameManager.instance.GetInfo();
        // ������а������б�Ϊ�գ���ֱ�ӷ��أ������н������
        if (saves.Count <= 0)
        {
            return;
        }
        else
        {
            // �������а������б�
            for (int i = 0; i < saves.Count; i++)
            {
                // ��ȡ�򴴽�һ�����а���Ŀʵ��
                RankItem rankItem = GetRankItem();
                // �������а���Ŀ����ʾ���ݣ����������������ͷ���
                rankItem.Initiate(i + 1, saves[i].name, saves[i].score);
            }
        }
    }

    // ����һ���µ����а���Ŀʵ��
    private RankItem CreateNewRankItem()
    {
        // ������а񵥸���ĿԤ����Ϊ�գ����ڳ����в��Ҳ���ȡ�����
        if (rankItemTemplate == null)
        {
            rankItemTemplate = transform.Find("RankItem").GetComponent<RankItem>();
        }
        // ��¡���а񵥸���ĿԤ����
        GameObject gameObject = Instantiate(rankItemTemplate.gameObject);
        // ��ȡ��¡��Ķ����RankItem���
        RankItem newRankItem = gameObject.GetComponent<RankItem>();
        // ���ÿ�¡��Ķ���ĸ�����ΪGrid
        newRankItem.transform.SetParent(Grid);
        // �����¡�����Ϸ����
        newRankItem.gameObject.SetActive(true);
        // ���ÿ�¡��Ķ���ı�������Ϊ��λ����������ԭʼ��С��
        newRankItem.transform.localScale = Vector3.one;
        // ���´��������а���Ŀʵ����ӵ��б���
        rankItem_List.Add(newRankItem);
        // �����´��������а���Ŀʵ��
        return newRankItem;
    }

    // ��ȡһ�����õ����а���Ŀʵ�������û�п��õ��򴴽�һ���µ�
    private RankItem GetRankItem()
    {
        // �����Ѵ��������а���Ŀ�б�
        foreach (RankItem temp in rankItem_List)
        {
            // ���ĳ����Ŀ�ڲ㼶�д��ڷǼ���״̬���򼤻���������
            if (!temp.gameObject.activeInHierarchy)
            {
                temp.gameObject.SetActive(true);
                return temp;
            }
        }
        // ���������Ŀ�����ڼ���״̬���򴴽�һ���µ����а���Ŀʵ��
        RankItem rankItem = CreateNewRankItem();
        return rankItem;
    }
}