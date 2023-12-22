using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;


    [Header("■ Options")]
    public float maxTime;
    public float paneltyTime;

    [Header("■ UI")]
    public Text timeText;
    public Text highScoreText;
    public Text curScoreText;
    public Text curTryText;

    [Header("■ Object")]
    public GameObject endText;
    public GameObject card;
    public GameObject nextGameText;

    public GameObject firstCard;
    public GameObject secondCard;

    [Header("■ AudioSource")]
    public AudioSource audioSource;
    public AudioSource bgmSource;

    [Header("■ AudioClip")]
    public AudioClip match;
    public AudioClip correct;
    public AudioClip incorrect;

    float time;
    float effectTime; // 경고등 깜빡임을 조절하기 위한 시간
    int matchCount; // 매칭 횟수 저장

    enum Difficulty { easy, normal, hard } // 난이도
    List<int> rtans = new List<int>(); // 카드패
    int maxScore;
    int curScore;
    int tryCount;


    /**************************************************************/
    //레벨마다 필요한 변수
    int col; // Column
    int row; // Row
    float scale; // Cards 오브젝트 스케일 변경 값


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    void InitGame(int difficulty)
    {
        InitScore();
        int cardObjectCount = 16;

        switch (difficulty)
        {
            case (int)Difficulty.easy:
                // 쉬움
                cardObjectCount = 12;
                col = 4;
                row = 3;
                scale = 1f;
                break;
            case (int)Difficulty.normal:
                cardObjectCount = 16;
                // 보통
                col = 4;
                row = 4;
                scale = 0.95f;
                break;
            case (int)Difficulty.hard:
                cardObjectCount = 24;
                // 어려움
                col = 6;
                row = 4;
                scale = 0.9f;
                break;
            default:
                // 에러
                break;
        }


        for (int i = 0; i < cardObjectCount / 2; i++)
        {
            rtans.Add(i);
            rtans.Add(i);
        }
    }

    void ShuffleCard()
    {
        Debug.Log("== Shuffle ==");
        int cardCount = rtans.Count;
        for (int i = 0; i < cardCount; i++)
        {
            // 위치 바꿀 자리 선택
            int shuffleIndex = Random.Range(i, cardCount);

            // 위치 교환
            int tmp = rtans[shuffleIndex];
            rtans[shuffleIndex] = rtans[i];
            rtans[i] = tmp;
        }
    }

    void GenCard()
    {
        for (int i = 0; i < rtans.Count; i++)
        {
            GameObject newCard = Instantiate(card);
            newCard.transform.parent = GameObject.Find("Cards").transform;

            float x = (i / col) * (float)(6.0 / row) - ((float)(6.0 / row) * (row - 1)) / 2;
            float y = (i % col) * (float)(8.0 / col) - ((float)(8.0 / col) * (col - 1)) / 2 - 1f;
            newCard.transform.position = new Vector3(x, y, 0);

            string rtanName = "rtan" + rtans[i].ToString();
            newCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(rtanName);
        }
        GameObject.Find("Cards").transform.localScale = new Vector3(scale, scale, 1f);
    }

    void Start()
    {
        Time.timeScale = 1.0f;
        time = maxTime;

        InitGame(DataManager.Instance.level);// 파라미터는 난이도
        ShuffleCard();
        GenCard();

        //int[] rtans = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7 };
        //rtans = rtans.OrderBy(x => Random.Range(-1.0f, 1.0f)).ToArray();

        // 카드 생성
        //for (int i = 0; i < 16; i++)
        //{
        //    GameObject newCard = Instantiate(card);
        //    newCard.transform.parent = GameObject.Find("Cards").transform;

        //    float x = (i / 4) * 1.4f - 2.1f;
        //    float y = (i % 4) * 1.4f - 3.0f;
        //    newCard.transform.position = new Vector3(x, y, 0);

        //    string rtanName = "rtan" + rtans[i].ToString();
        //    newCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(rtanName);
        //}

    }

    // Update is called once per frame
    void Update()
    {

        time -= Time.deltaTime;
        timeText.text = time.ToString("N2");

        if (maxScore < curScore)
        {
            maxScore = curScore;
        }
        highScoreText.text = maxScore.ToString();
        curScoreText.text = curScore.ToString();
        curTryText.text = tryCount.ToString();

        WarningUI();


        if (time <= 0)                      // 시간이 0이 되었을때 게임이 끝나도록 변경
        {
            DataManager.Instance.level = 0;
            endText.SetActive(true);
            bgmSource.Stop();
            Time.timeScale = 0.0f;

            // 시간 종료 점수 계산 및 저장
            SaveScore();
        }
    }

    public void IsMatched()
    {
        string firstCardImage = firstCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite.name;
        string secondCardImage = secondCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite.name;

        // 매칭 시도 횟수 카운트
        tryCount++;
        if (tryCount % 10 == 0)
        {
            // 시도 10회마다 -10점
            curScore -= 5;
        }

        if (firstCardImage == secondCardImage)
        {
            //audioSource.PlayOneShot(match);
            audioSource.PlayOneShot(correct);

            firstCard.GetComponent<Card>().DestroyCard();
            secondCard.GetComponent<Card>().DestroyCard();

            // 맞춘 점수 +10
            curScore += 10;

            Card[] leftCards = GameObject.Find("Cards").transform.GetComponentsInChildren<Card>();
            foreach (Card card in leftCards)
            {
                card.transform.Find("Back").GetComponent<SpriteRenderer>().color = Color.white;
            }

            int cardsLeft = GameObject.Find("Cards").transform.childCount;
            if (cardsLeft == 2)
            {
                if(DataManager.Instance.level < 2)
                {
                    DataManager.Instance.level++;
                    nextGameText.SetActive(true);
                }
                else
                {
                    DataManager.Instance.level = 0;
                    endText.SetActive(true);
                }
                
                Time.timeScale = 0.0f;

                // 점수계산 및 저장
                curScore += Mathf.FloorToInt(time * 10);
                SaveScore();
            }
        }
        else
        {
            audioSource.PlayOneShot(incorrect);
            firstCard.GetComponent<Card>().CloseCard();
            secondCard.GetComponent<Card>().CloseCard();

            // 틀려서 감점 -1
            curScore -= 1;
        }

        firstCard = null;
        secondCard = null;
    }

    public void ReGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void WarningUI()   // 시간이 촉박할때!
    {
        if (time <= maxTime * 0.5)  //남은시간이 절반 이하일때
        {
            bgmSource.pitch = 1.1f;
            timeText.color = Color.yellow;
        }
        if (time <= maxTime * 0.2)  // 남은시간이 20% 이하일때
        {
            bgmSource.pitch = 1.2f;

            effectTime += Time.deltaTime;
            if (effectTime < 1f)
            {
                timeText.color = new Color(1, 0, 0, 1 - effectTime);
            }
            else
            {
                timeText.color = new Color(1, 0, 0, effectTime);
                if (effectTime > 1f)
                {
                    effectTime = 0f;
                }
            }

        }

    }


    void InitScore()
    {
        if (!PlayerPrefs.HasKey("MaxScore"))
        {
            // 최고점수 기록이 없으면 초기화
            PlayerPrefs.SetInt("MaxScore", 0);
        }

        maxScore = PlayerPrefs.GetInt("MaxScore");
        curScore = 0;
        tryCount = 0;

        highScoreText.text = maxScore.ToString();
        curScoreText.text = curScore.ToString();
        curTryText.text = tryCount.ToString();
    }


    void SaveScore()
    {
        //한번 더 검증 후 저장
        if (maxScore < curScore)
        {
            maxScore = curScore;
        }
        // 이전기록 비교
        PlayerPrefs.SetInt("MaxScore", maxScore);
    }

}

