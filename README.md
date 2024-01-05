## Project
4주차 강의를 활용하여 팀원을 나타내는 이미지를 가지고 카드 뒤집기 게임을 만든다.

## Team Notion
https://www.notion.so/9-f2dc67aa98b447e484bd24ed6d2592e9

## Member
- 황문규(팀장)
- 김관철
- 권순성
- 이주환
- 김상민

## DevPeriod
- 2023.12.21 ~ 2023.12.27

## Dev Environment
- Engine : Unity 2020.3.48f1
- Language : C#



---

### keystore
```
Alias : `spartakey`  
Password : `123456`  
```

---

### 깃 사용 참고 링크
https://jjuke-brain.tistory.com/entry/Github%EB%A1%9C-Unity-%ED%94%84%EB%A1%9C%EC%A0%9D%ED%8A%B8-%EA%B4%80%EB%A6%AC%ED%95%98%EA%B8%B0


---

### 피드백

```
Git으로 공유할때 Assets, Packages, ProjectSettings 폴더를 제외한 폴더는 공유할 필요가 없습니다.
card_BackUpThisFolder_ButDontShipItWithYourGame, card_BurstDebugInformation_DoNotShip 폴더는 git ignore에 등록하여 제외 하는게 좋습니다.

Find 함수는 성능이 좋지 않아 사용을 지양 해야 합니다.
Find 보다는 SerializeField 를 이용하여 접근하는 방법이 좋습니다.
//transform.Find("back").gameObject.SetActive(true);
[SerializeField] private GameObject _backGameObject;
_backGameObject.SetActive(true);

GetComponent 함수를 사용할때 반환된 Component가 유요한지 항상 확인 해야합니다.
// selects[i].transform.GetComponent<Button>().interactable = false;
Button button = selects[i].transform.GetComponent<Button>();
if( button != null )
{
  button.interactable = false;
}
```
