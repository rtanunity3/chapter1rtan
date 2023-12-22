using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance ;


    [Header("■ Options")]
    public float maxTime;
    public float paneltyTime;

    [Header("■ UI")]
    public Text timeText;

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
    float effectTime; // 경고등 깜빡임을 조절하기 위한 시간
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    void Start()
    {
        Time.timeScale = 1.0f;

        time = maxTime;

        int[] rtans = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7 };

        rtans = rtans.OrderBy(x => Random.Range(-1.0f, 1.0f)).ToArray();

        // 카드 생성
        for (int i = 0; i < 16; i++)
        {
            GameObject newCard = Instantiate(card);
            newCard.transform.parent = GameObject.Find("Cards").transform;

            float x = (i / 4) * 1.4f - 2.1f;
            float y = (i % 4) * 1.4f - 3.0f;
            newCard.transform.position = new Vector3(x, y, 0);

            string rtanName = "rtan" + rtans[i].ToString();
            newCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(rtanName);
        }

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
        }
    }

    public void IsMatched()
    {
        string firstCardImage = firstCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite.name;
        string secondCardImage = secondCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite.name;

        if (firstCardImage == secondCardImage)
        {
            //audioSource.PlayOneShot(match);
            audioSource.PlayOneShot(correct);

            firstCard.GetComponent<Card>().DestroyCard();
            secondCard.GetComponent<Card>().DestroyCard();

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
            }
        }
        else
        {
            audioSource.PlayOneShot(incorrect);
            firstCard.GetComponent<Card>().CloseCard();
            secondCard.GetComponent<Card>().CloseCard();
        }

        firstCard = null;
        secondCard = null;
    }

    public void ReGame()
    {
         //SceneManager.LoadScene("MainScene");
        AdsManager.Instance.ShowRewardAd();
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
            if(effectTime < 1f)
            {
                timeText.color = new Color(1, 0, 0, 1 - effectTime);
            }
            else
            {
                timeText.color = new Color(1, 0, 0, effectTime);
                if(effectTime > 1f)
                {
                    effectTime = 0f;
                }
            }
            
        }

    }

}
