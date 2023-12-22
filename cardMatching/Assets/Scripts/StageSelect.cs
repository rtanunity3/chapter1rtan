using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{

    public void startSelectedStage(int stageIndex)
    {
        DataManager.Instance.level = stageIndex;
        SceneManager.LoadScene("LevelTestScene");
    }
}
