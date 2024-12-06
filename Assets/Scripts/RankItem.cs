using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankItem : MonoBehaviour
{
    [Header("Child")]
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;


    public void Initiate(int rank, string name, int score)
    {
        try
        {
            rankText.text = rank.ToString();
            nameText.text = name;
            scoreText.text = score.ToString();
        }
        catch
        {
            Debug.Log(111);
        }
    }
}
