using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    [Header("Object")]
    public GameObject select;

    private void Start()
    {
        
    }

    // 스테이지 선택 시 해당 스테이지에서 시작
    public void startSelectedStage(int stageIndex)
    {
        DataManager.Instance.level = stageIndex;
        SceneManager.LoadScene("MainScene");
    }
}
