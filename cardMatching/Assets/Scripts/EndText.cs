using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndText : MonoBehaviour
{
    public void ReGame()
    {
        AdsManager.Instance.ShowRewardAd();
    }

    // 다음 스테이지 시작
    public void NextGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
