using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public Text nameTxt;

    [Header("■ Object")]
    public GameObject endText;
    public GameObject card;

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
    float nameTime;
    float effectTime; // 경고등 깜빡임을 조절하기 위한 시간

    enum Difficulty { easy, normal, hard } // 난이도
    List<int> rtans = new List<int>(); // 카드패
    int gameScore;
    string[] names = { "황문규", "황문규", "김관철", "김관철", "권순성", "이주환", "이주환", "김상민" };


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
        int cardObjectCount = 16;
        gameScore = 0;

        switch (difficulty)
        {
            case (int)Difficulty.easy:
                // 쉬움
                break;
            case (int)Difficulty.normal:
                cardObjectCount = 20;
                // 보통
                break;
            case (int)Difficulty.hard:
                cardObjectCount = 24;
                // 어려움
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

            float x = (i / 4) * 1.4f - 2.1f;
            float y = (i % 4) * 1.4f - 3.0f;

            StartCoroutine(SpiralEffect(newCard, 1f, new Vector3(x, y, 0)));
            //newCard.transform.position = new Vector3(x, y, 0);

            string rtanName = "rtan" + rtans[i].ToString();
            newCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(rtanName);
        }
    }

    void Start()
    {
        Time.timeScale = 1.0f;
        time = maxTime;

        InitGame(0);// 파라미터는 난이도
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

        WarningUI();


        if (time <= 0)                      // 시간이 0이 되었을때 게임이 끝나도록 변경
        {
            endText.SetActive(true);
            bgmSource.Stop();
            Time.timeScale = 0.0f;

            // 시간 종료 점수 계산 및 저장

        }

        nameTime += Time.deltaTime;
        if (nameTime >= 1f)
        {
            nameTxt.gameObject.SetActive(false);
            nameTime = 0f;
        }
    }

    public void IsMatched()
    {
        string firstCardImage = firstCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite.name;
        string secondCardImage = secondCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite.name;

        // 매칭 시도 횟수 카운트

        if (firstCardImage == secondCardImage)
        {
            //audioSource.PlayOneShot(match);
            audioSource.PlayOneShot(correct);

            nameTime = 0f;
            // 텍스트 켜기
            nameTxt.gameObject.SetActive(true);

            // 스프라이트에서 번호 추출하여 해당하는 이름으로 세팅하기
            nameTxt.text = names[ExtractNumber(firstCardImage)];


            firstCard.GetComponent<Card>().DestroyCard();
            secondCard.GetComponent<Card>().DestroyCard();

            // 맞춘 점수 +10
            gameScore += 10;

            Card[] leftCards = GameObject.Find("Cards").transform.GetComponentsInChildren<Card>();
            foreach (Card card in leftCards)
            {
                card.transform.Find("Back").GetComponent<SpriteRenderer>().color = Color.white;
            }

            int cardsLeft = GameObject.Find("Cards").transform.childCount;
            if (cardsLeft == 2)
            {
                endText.SetActive(true);
                Time.timeScale = 0.0f;

                // 점수계산 및 저장
                gameScore += Mathf.FloorToInt(maxTime - time) * 10;

            }
        }
        else
        {
            nameTime = 0f;
            // 텍스트 켜기
            nameTxt.gameObject.SetActive(true);

            nameTxt.text = "실패";

            audioSource.PlayOneShot(incorrect);
            firstCard.GetComponent<Card>().CloseCard();
            secondCard.GetComponent<Card>().CloseCard();

            // 틀려서 감점 -1
            gameScore -= 1;
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

    // 스트링에서 숫자만 추출
    public int ExtractNumber(string spriteName)
    {
        string numberString = "";

        foreach (char c in spriteName)
        {
            if (char.IsDigit(c))
            {
                numberString += c;
            }
        }

        if (numberString.Length > 0)
        {
            return int.Parse(numberString);
        }

        return -1;
    }

    IEnumerator SpiralEffect(GameObject card, float duration, Vector3 endPosition)
    {
        float time = 0;
        Vector3 startPosition = new Vector3(0, 0, 0);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // 나선형 경로 계산
            float theta = t * 2 * Mathf.PI; // 각도
            float radius = (1 - t) * 5; // 반지름
            Vector3 spiralPos = new Vector3(Mathf.Cos(theta) * radius, Mathf.Sin(theta) * radius, 0);

            card.transform.position = Vector3.Lerp(startPosition + spiralPos, endPosition, t);

            yield return null;
        }

        card.transform.position = endPosition;
    }
}
