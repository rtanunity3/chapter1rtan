using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Advertisements;
public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    string adType;
    string gameId;

    private void Awake()
    {
        Instance = this;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            adType = "Rewarded_iOS";
            gameId = "5505946";
        }
        else
        {
            adType = "Rewarded_Android";
            gameId = "5505947";
        }

        Advertisement.Initialize(gameId, true); // 상용은 false로 세팅. 프로젝트 세팅에서 테스트모드 off
    }

    public void ShowRewardAd()
    {
        if (Advertisement.IsReady())
        {
            ShowOptions options = new ShowOptions { resultCallback = ResultAds };
            Advertisement.Show(adType, options);
        }
    }

    void ResultAds(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Failed:
                Debug.LogError("광고 보기에 실패했습니다.");
                break;
            case ShowResult.Skipped:
                Debug.Log("광고를 스킵했습니다.");
                break;
            case ShowResult.Finished:
                // 광고 보기 보상 기능 
                GameManager.Instance.ReGame();
                break;
        }
    }
}
