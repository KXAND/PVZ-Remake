using Plant;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    float CDTime = 10;
    bool isReadyAtStart = false;
    Coroutine coldDown;

    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] TextMeshProUGUI plantName;
    [SerializeField] GameObject nameBackground;
    [SerializeField] Image plantImage;
    [SerializeField] Image cardFlash;
    [SerializeField] RectTransform cardMask1;
    [SerializeField] RectTransform cardMask2;

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
        }
    }
    public bool IsReady => coldDown == null;

    public void Init(string plants)
    {
        plantName.text = plants;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.currentState == GameManager.GMState.Ilde)
        {
            nameBackground.SetActive(false);
            cardMask1.gameObject.SetActive(true);
            cardMask2.gameObject.SetActive(true);
            GameManager.Instance.ToSelectingPlantState(plantImage.sprite, this);
        }
    }

    public void CancelPlanting()
    {
        StopAllCoroutines();
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
        coldDown = StartCoroutine(ColdDown());
    }

    public IEnumerator ColdDown()
    {
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / CDTime;
            cardMask2.offsetMin = Vector2.Lerp(Vector2.zero, new(0, 56), t);
            yield return null;
        }
        cardMask1.gameObject.SetActive(false);
        cardMask2.gameObject.SetActive(false);
        cardMask2.offsetMin = Vector2.zero;

        cardFlash.enabled = true;
        yield return new WaitForSeconds(0.1f);
        cardFlash.enabled = false;

    }
}
