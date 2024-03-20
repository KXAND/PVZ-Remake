using Plant;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler/*, IPointerDownHandler*/
{
    float CDTime = 10;
    bool isReadyAtStart = false;
    bool isColdingDown;
    bool haveEnoughSun;

    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] TextMeshProUGUI plantName;
    [SerializeField] GameObject nameBackground;
    [SerializeField] Image cardFlash;
    [SerializeField] RectTransform cardMask1;
    [SerializeField] RectTransform cardMask2;

    public Image plantImage;

    Seed seed;
    public Seed Seed
    {
        get => seed; set
        {
            seed = value;
            cost.text = seed.costSun.ToString();
            CDTime = seed.CardCDTime;
            isReadyAtStart = seed.isReadyAtStart;
            plantName.text = seed.name;
            plantImage.sprite = seed.seedSprite;

            if (!isReadyAtStart)
            {
                cardMask1.gameObject.SetActive(true); 
                cardMask2.gameObject.SetActive(true);
                StartCoroutine(ColdDown());
            }
        }
    }
    public bool IsReady => haveEnoughSun && (!isColdingDown);

    public void Awake()
    {
        GameManager.Instance.OnSunChanged.AddListener(OnSunChange);
    }

    public void Init(string plants)
    {
        plantName.text = plants;

    }

    public void OnSelected()
    {
        nameBackground.SetActive(false);
        cardMask1.gameObject.SetActive(true);
        cardMask2.gameObject.SetActive(true);
    }

    public void OnCancelPlanting()
    {
        cardMask1.gameObject.SetActive(false);
        cardMask2.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        nameBackground.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        nameBackground.SetActive(false);
    }

    public void OnPlanted()
    {
        StartCoroutine(ColdDown());
    }

    void OnSunChange(int nowSunNum)
    {
        if (nowSunNum < seed.costSun)
        {
            cardMask1.gameObject.SetActive(true);
            cardMask2.gameObject.SetActive(true);
            haveEnoughSun = false;
        }
        else
        {
            if (!isColdingDown)
            {
                cardMask1.gameObject.SetActive(false);
                cardMask2.gameObject.SetActive(false);
            }
            haveEnoughSun = true;
        }
    }

    public IEnumerator ColdDown()
    {
        isColdingDown = true;
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / CDTime;
            cardMask2.offsetMin = Vector2.Lerp(Vector2.zero, new(0, 56), t);
            yield return null;
        }
        isColdingDown = false;
        cardMask2.offsetMin = Vector2.zero;
        cardMask2.gameObject.SetActive(false);
        if (IsReady)
        {
            cardMask1.gameObject.SetActive(false);
            cardFlash.enabled = true;
            yield return new WaitForSeconds(0.1f);
            cardFlash.enabled = false;
        }
    }
}