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
    Card selectingCard;

    [Header("Planting")]
    GraphicRaycaster raycaster;
    EventSystem eventSystem;
    public GameObject plantSelecting;
    public GameObject plantPreview;
    public Transform[][] plantGrids;
    Vector3 PlantPreviewWorldPos => Camera.main.ScreenToWorldPoint(plantPreview.transform.position);
    Vector3 PlantSelectingWorldPos => Camera.main.ScreenToWorldPoint(plantSelecting.transform.position);

    [Header("Sun")]
    [SerializeField] RectTransform SunIcon;
    private int sunNum = 0;
    public Sun sun;
    public TextMeshProUGUI sunNumText;
    [HideInInspector] public UnityEvent<int> OnSunChanged;

    public LevelConfig config;
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


        plantGrids = new Transform[plantGridParent.childCount][];
        for (int i = 0; i < plantGridParent.childCount; i++)
        {
            var row = plantGridParent.GetChild(i);
            plantGrids[i] = new Transform[row.childCount];
            for (int j = 0; j < row.childCount; j++)
            {
                plantGrids[i][j] = row.GetChild(j);
            }
        }

        eventSystem = FindObjectOfType<EventSystem>();
        raycaster = FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();
        //PlantingManager.Instance.Init();

    }

    void Start()
    {
        StartCoroutine(ThrowSun());

        for (int i = 0; i < 2; i++)
        {
            Card item = Instantiate(card, seedBank);
            cards.Add(item);
        }
        cards[0].Seed = Resources.Load<Seed>("Seeds/SunFlower");
        cards[1].Seed = Resources.Load<Seed>("Seeds/PeaShooter");

        SunNum = 500;

        //var a = ZombiesPools.Instance.GetFromPool("NormalZombie");
        PlantingManager.Instance.Init();
        LevelManager.Instance.Init(config);
        //var zombieF = new ZombieFactory();
        //zombieF.Produce(a);
    }

    // Update is called once per frame
    void Update()
    {
        plantSelecting.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        plantPreview.transform.position = Camera.main.WorldToScreenPoint(PlantingManager.Instance.GetPlantPreviewWorldPosition(PlantSelectingWorldPos));

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
                    if (selectingCard && selectingCard.IsReady)
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
                    CanclePlanting();
                }
                else
                {
                    PlantingManager.Instance.Plant(selectingCard);
                    SunNum -= selectingCard.Seed.costSun;
                    plantSelecting.SetActive(false);
                    plantPreview.SetActive(false);
                    selectingCard = null;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            CanclePlanting();
        }
    }
    void CanclePlanting()
    {
        if (selectingCard) selectingCard.OnCancelPlanting();
        plantSelecting.SetActive(false);
        plantPreview.SetActive(false);
        selectingCard = null;
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
