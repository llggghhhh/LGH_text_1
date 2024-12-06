using System.Collections.Generic;
using UnityEngine;

// ��Ϸ�������࣬���������Ϸ������״̬�����а��������
public class GameManager : MonoBehaviour
{
    // ����ģʽ��������������Ϸ�л�ȡΨһ��GameManagerʵ��
    public static GameManager instance;

    // ��ҷ���
    [Header("����")]
    public int m_Points = 0;
    // ��Ϸ�Ƿ����
    public bool m_GameOver = false;
    // �������
    public string m_PlayerName = null;

    // �ڶ��󱻻���ʱ���ã����ڳ�ʼ������ģʽ
    private void Awake()
    {
        // ���GameManagerʵ��Ϊ�գ��򽫵�ǰ����ֵ��instance����ȷ���ڳ����л�ʱ��������
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // ����Ѿ�����GameManagerʵ���Ҳ��ǵ�ǰ���������ٵ�ǰ����
        else
        {
            Destroy(gameObject);
        }
    }

    // �����л����࣬���ڴ洢���а��еĵ���������ݣ������ͷ�����
    [System.Serializable]
    public class Save
    {
        public string name;
        public int score;
    }

    // ���ڴ洢���а����ݵ��б�ÿ��Ԫ����һ��Save����
    public List<Save> saves = new List<Save>();
    // ���а���ʾ�������Ŀ��
    private int rankLimit = 10;

    // �ӱ��ش洢�н������а������ַ���ΪSave�����б�
    public void ParseInfo()
    {
        // ��յ�ǰ�����а������б�
        saves = new List<Save>();
        // ��PlayerPrefs�л�ȡ��Ϊ"PONGPI_SaveInfo"���ַ��������ַ����洢�����а�����
        string saveString = PlayerPrefs.GetString("PONGPI_SaveInfo");
        // �����ȡ�����ַ�����Ϊ�գ�����н���
        if (!string.IsNullOrEmpty(saveString))
        {
            // ��'|'�ָ��ַ������õ�ÿ����������ַ�������
            string[] rankItemArry = saveString.Split('|');
            // ����ÿ���������ַ���
            for (int ident = 0; ident < rankItemArry.Length; ident++)
            {
                // ��'+'�ָ��������ַ������õ������ͷ������ַ�������
                string[] rankItem = rankItemArry[ident].Split('+');
                // ����һ���µ�Save����
                Save save = new()
                {
                    // ���ָ�õ���������ֵ��Save�����name����
                    name = rankItem[0],
                    // ���ָ�õ��ķ���ת��Ϊ��������ֵ��Save�����score����
                    score = int.Parse(rankItem[1])
                };
                // ��Save������ӵ����а������б���
                saves.Add(save);
            }
        }
        // ������а������б�Ϊ�գ�������������
        if (saves.Count != 0)
        {
            saves.Sort(CompareScore);
        }
    }

    // �Ƚ�����Save����ķ������������а���������
    int CompareScore(Save player_1, Save player_2)
    {
        // ��ȡ��һ����ҵķ���
        int score_1 = player_1.score;
        // ��ȡ�ڶ�����ҵķ���
        int score_2 = player_2.score;
        // ���������ҷ�����ȣ�����0
        if (score_1 == score_2)
        {
            return 0;
        }
        // �����һ����ҷ������ڵڶ�����ҷ��������� -1����ʾ��һ�����������ǰ
        else if (score_1 > score_2)
        {
            return -1;
        }
        // �����һ����ҷ���С�ڵڶ�����ҷ���������1����ʾ��һ�������������
        else
        {
            return 1;
        }
    }

    // ����ǰ���������ӵ����а������в����浽���ش洢
    public void SaveInfo()
    {
        // �Ƚ������ش洢�е����а����ݣ�ȷ�����������µ�
        ParseInfo();
        // ����һ���µ�Save�������ڴ洢��ǰ�������
        Save save = new Save
        {
            name = m_PlayerName,
            score = m_Points
        };
        // ����ǰ���������ӵ����а������б���
        saves.Add(save);
        // �����а������б��������
        saves.Sort(CompareScore);
        // ������а������б��ȳ����������Ŀ�������Ƴ����һ��Ԫ�أ���������͵��Ǹ���
        if (saves.Count > rankLimit)
        {
            saves.RemoveAt(saves.Count - 1);
        }
        // ���ڹ���Ҫ���浽���ش洢���ַ���
        string saveString = "";
        // �������а������б�
        for (int ident = 0; ident < saves.Count; ident++)
        {
            // ����ÿ����������ַ�������ʽΪ"����+����"
            string temp = "";
            temp += saves[ident].name;
            temp += "+";
            temp += saves[ident].score;
            // ����������һ������������'|'��Ϊ�ָ���
            if (ident != saves.Count - 1)
            {
                temp += "|";
            }
            // �������õ��������ַ�����ӵ��ܵı����ַ�����
            saveString += temp;
        }
        // �������õ����а������ַ������浽PlayerPrefs�У���Ϊ"PONGPI_SaveInfo"
        PlayerPrefs.SetString("PONGPI_SaveInfo", saveString);
    }

    /// <summary>
    /// ��ȡ�ѱ��浽���ش洢�е����а������б�
    /// </summary>
    /// <returns>���а������б�Save List��</returns>
    public List<Save> GetInfo()
    {
        // �Ƚ������а����ݣ�ȷ����ȡ��������������
        ParseInfo();
        // �������а������б�
        return saves;
    }
}