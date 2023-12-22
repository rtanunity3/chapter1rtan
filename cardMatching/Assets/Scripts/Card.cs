using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Animator anim;
    public AudioClip flip;
    public AudioSource audioSource;

    public void OpenCard()
    {
        audioSource.PlayOneShot(flip);

        anim.SetBool("isOpen", true);
        transform.Find("Front").gameObject.SetActive(true);
        transform.Find("Back").gameObject.SetActive(false);

        if (GameManager.Instance.firstCard == null)
        {
            GameManager.Instance.firstCard = gameObject;
        }
        else
        {
            GameManager.Instance.secondCard = gameObject;
            GameManager.Instance.IsMatched();
        }
    }


    public void DestroyCard()
    {
        Invoke(nameof(DestroyCardInvoke), 1.0f);
    }

    void DestroyCardInvoke()
    {
        Destroy(gameObject);
    }

    public void CloseCard()
    {
        Invoke(nameof(CloseCardInvoke), 1.0f);
    }

    void CloseCardInvoke()
    {
        anim.SetBool("isOpen", false);
        transform.Find("Back").gameObject.SetActive(true);
        transform.Find("Front").gameObject.SetActive(false);
        // card 뒷면 회색으로 변경
        transform.Find("Back").gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        // card 앞면 회색으로 변경
        transform.Find("Front/Front_Bg").gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
    }
}
