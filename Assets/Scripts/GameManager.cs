using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
using Zombie;

public class GameManager : MonoBehaviour
{
    [Header("Cards")]
    [SerializeField] Card card;
    [SerializeField] Transform seedBank;
    readonly List<Card> cards = new();
    public Transform plantGridParent;
    public bool allowedClickCard = true;
    Card selectingCard;

    [Header("Planting")]
    GraphicRaycaster raycaster;
    EventSystem eventSystem;
    public GameObject plantSelecting;
    public GameObject plantPreview;
    public Vector2[][] plantGrids;
    Vector3 PlantPreviewWorldPos => Camera.main.ScreenToWorldPoint(plantPreview.transform.position);
    Vector3 PlantSelectingWorldPos => Camera.main.ScreenToWorldPoint(plantSelecting.transform.position);

    [Header("Sun")]
    [SerializeField] RectTransform SunIcon;
    private int sunNum = 0;
    public Sun sun;
    public TextMeshProUGUI sunNumText;
    [HideInInspector] public UnityEvent<int> OnSunChanged;
    public Vector3 SunIconPosition => Camera.main.ScreenToWorldPoint(SunIcon.position);
    public int SunNum
    {
        get => sunNum; set
        {
            int newSunNum = value;
            if (newSunNum < 0)
            {
                Debug.Log("Not Enough!");
            }
            else
            {
                if (newSunNum > 9999) { newSunNum = 9999; }
                sunNum = newSunNum;
                sunNumText.text = sunNum.ToString();
                OnSunChanged.Invoke(sunNum);
            }
        }
    }


    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        plantGrids = new Vector2[plantGridParent.childCount][];
        for (int i = 0; i < plantGridParent.childCount; i++)
        {
            var row = plantGridParent.GetChild(i);
            plantGrids[i] = new Vector2[row.childCount];
            for (int j = 0; j < row.childCount; j++)
            {
                plantGrids[i][j] = row.GetChild(j).position;
            }
        }

        eventSystem = FindObjectOfType<EventSystem>();
        raycaster = FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();
    }

    void Start()
    {
        StartCoroutine(ThrowSun());

        for (int i = 0; i < 2; i++)
        {
            Card item = Instantiate(card, seedBank);
            Debug.Log(item.name);

            cards.Add(item);

        }
        cards[0].Seed = Resources.Load<Seed>("Seeds/SunFlower");
        cards[1].Seed = Resources.Load<Seed>("Seeds/PeaShooter");

        SunNum = 50;

        ZombiesPools.Instance.GetFromPool("NormalZombie");
    }

    // Update is called once per frame
    void Update()
    {
        plantSelecting.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        plantPreview.transform.position = Camera.main.WorldToScreenPoint(GetPlantPreviewWorldPosition());

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            LayerMask sunMask = 1 << 8;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                Vector3.back, Mathf.Infinity, sunMask);
            if (hit)
            {
                StartCoroutine(hit.transform.GetComponent<Sun>().BeCollected());
            }

            if (!selectingCard)
            {
                List<RaycastResult> results = new();
                raycaster.Raycast(new(eventSystem) { position = Input.mousePosition }, results);
                foreach (RaycastResult result in results)// 循环事实上只会遍历到一个值
                {
                    selectingCard = result.gameObject.GetComponent<Card>();
                    if (selectingCard.IsReady)
                    {
                        selectingCard.OnSelected();
                        plantSelecting.SetActive(true);
                        plantPreview.SetActive(true);
                        plantSelecting.GetComponent<Image>().sprite = selectingCard.plantImage.sprite;
                        plantPreview.GetComponent<Image>().sprite = selectingCard.plantImage.sprite;
                    }
                    else
                        selectingCard = null;
                }
            }
            else if (selectingCard)
            {
                if (PlantPreviewWorldPos == new Vector3(10f, 10f, 0f))// 距离种植区过远，Preview已经被隐藏
                {
                    selectingCard.OnCancelPlanting();
                    allowedClickCard = true;
                    selectingCard = null;
                    plantSelecting.SetActive(false);
                    plantPreview.SetActive(false);
                }
                else
                {
                    selectingCard.OnPlanted();
                    Instantiate(selectingCard.Seed.seedInstance, PlantPreviewWorldPos, Quaternion.identity);
                    SunNum -= selectingCard.Seed.costSun;
                    allowedClickCard = true;
                    selectingCard = null;
                    plantSelecting.SetActive(false);
                    plantPreview.SetActive(false);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (selectingCard)
            {
                selectingCard.OnCancelPlanting();
                plantSelecting.SetActive(!plantSelecting.activeSelf);
                plantPreview.SetActive(!plantPreview.activeSelf);
                selectingCard = null;
                allowedClickCard = true;
            }
        }
    }

    public void SelectingAPlant(Sprite plantSprite, Card selectingCard)
    {
        this.selectingCard = selectingCard;
        plantSelecting.SetActive(!plantSelecting.activeSelf);
        plantSelecting.GetComponent<Image>().sprite = plantSprite;
        plantPreview.SetActive(!plantPreview.activeSelf);
        plantPreview.GetComponent<Image>().sprite = plantSprite;

        allowedClickCard = false;
    }

    /// <summary>
    /// 返回 plantPreview 的坐标，含不可见时坐标
    /// </summary>
    /// <returns></returns>
    Vector3 GetPlantPreviewWorldPosition()
    {
        int y = 0;
        int x = 0;
        float distance = float.MaxValue;

        for (int i = 0; i < plantGrids.Length; i++)//row
        {
            var newDistance = Vector2.Distance(PlantSelectingWorldPos, plantGrids[i][0]);

            if (newDistance > distance) break;

            distance = newDistance;
            y = i;
        }

        distance = float.MaxValue;
        for (int i = 0; i < plantGrids[y].Length; i++)
        {
            var newDistance = Vector2.Distance(PlantSelectingWorldPos, plantGrids[y][i]);

            if (newDistance > distance) break;

            distance = newDistance;
            x = i;
        }
        if (Vector2.Distance(PlantSelectingWorldPos, plantGrids[y][x]) < 1)
            return plantGrids[y][x];
        else return new(10, 10);
    }

    IEnumerator ThrowSun()
    {
        float throwSunBasicCD = 10f;
        float throwSunRandomCD = 5f;
        yield return new WaitForSeconds(Random.Range(0f, throwSunRandomCD));
        while (true)
        {
            Vector3 startPos = new(Random.Range(-7f, 4f), 8f, 0);
            Vector3 destPos = new(Random.Range(-7f, 4f), Random.Range(-4f, 0f), 0);

            var newSun = Instantiate(sun, startPos, Quaternion.identity);
            newSun.GetComponent<Sun>().StartCoroutine(newSun.SkySunThrow(destPos));

            yield return new WaitForSeconds(
                Random.Range(throwSunBasicCD - throwSunRandomCD, throwSunBasicCD + throwSunRandomCD));
        }
    }
}
