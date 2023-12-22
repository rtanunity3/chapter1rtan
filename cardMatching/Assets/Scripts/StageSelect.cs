using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    [Header("Object")]
    public GameObject[] selects;

    private void Start()
    {

        InitBestScore();
        InitSelectButton();
    }

    private void InitBestScore()
    {
        for (int i = 0; i < selects.Length; i++)
        {
            if (!PlayerPrefs.HasKey($"MaxScore_{i}"))
            {
                // 최고점수 기록이 없으면 초기화
                PlayerPrefs.SetInt($"MaxScore_{i}", 0);
            }
            selects[i].transform.Find("BestScoreText").GetComponent<Text>().text = PlayerPrefs.GetInt($"MaxScore_{i}").ToString();
        }
    }

    private void InitSelectButton()
    {
        for (int i = 0; i < selects.Length; i++)
        {
            if (!PlayerPrefs.HasKey($"Unlock_{i}"))
            {
                if(i == 0)
                {
                    PlayerPrefs.SetInt($"Unlock_{i}", 1);
                }
                else
                {
                    // 최고점수 기록이 없으면 초기화
                    PlayerPrefs.SetInt($"Unlock_{i}", 0);
                }
                    
            }
            
            if(PlayerPrefs.GetInt($"Unlock_{i}") == 0)
            {
                selects[i].transform.GetComponent<Button>().interactable = false;
                selects[i].transform.Find("LockImage").gameObject.SetActive(true);
            }
            else
            {
                selects[i].transform.GetComponent<Button>().interactable = true;
                selects[i].transform.Find("LockImage").gameObject.SetActive(false);
            }
        }
    }

    // 스테이지 선택 시 해당 스테이지에서 시작
    public void startSelectedStage(int stageIndex)
    {
        DataManager.Instance.level = stageIndex;
        SceneManager.LoadScene("MainScene");
    }
}
