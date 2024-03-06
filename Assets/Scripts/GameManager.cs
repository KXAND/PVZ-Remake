using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GMState
    {
        Ilde,
        Selectingplant
    }

    [SerializeField] Card card;
    List<Card> cards = new();
    [SerializeField] Transform seedBank;
    public Sun sun;
    public GMState currentState = GMState.Ilde;
    public TextMeshProUGUI sunNumText;
    public Transform plantGridParent;
    public GameObject plantSelecting;
    public GameObject plantPreview;
    public Vector2[][] plantGrids;
    [SerializeField] RectTransform SunIcon;

    Card selectingCard;
    private int sunNum = 50;

    public Vector3 SunIconPosition => Camera.main.ScreenToWorldPoint(SunIcon.position);
    public static GameManager Instance { get; private set; }
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
            }
        }
    }

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
    }

    void Start()
    {
        StartCoroutine(ThrowSun());

        for (int i = 0; i < 5; i++)
        {
            Card item = Instantiate(card, seedBank);
            Debug.Log(item.name);

            cards.Add(item);

        }
        cards[0].Seed = Resources.Load<Seed>("Seeds/SunFlower");
        cards[1].Seed = Resources.Load<Seed>("Seeds/PeaShooter");

    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == GMState.Selectingplant)
        {
            plantSelecting.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Vector2 newPreviewPos = GetPlantPreviewPosition();
            if (Vector2.Distance(plantSelecting.transform.position, newPreviewPos) < 1)
            {
                plantPreview.transform.position = newPreviewPos;
                if (Input.GetMouseButtonDown(0))
                {
                    selectingCard.StartCoroutine(selectingCard.ColdDown());
                    Instantiate(selectingCard.Seed.seedInstance, plantPreview.transform.position, Quaternion.identity);
                    sunNum -= selectingCard.Seed.costSun;
                }
            }
            else
            {
                plantPreview.transform.position = new(10, 10);
            }

            if (Input.GetMouseButtonDown(1))
            {
                currentState = GMState.Ilde;
                plantSelecting.SetActive(false);
                plantPreview.SetActive(false);
                selectingCard.CancelPlanting();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))//idle
        {
            {
                LayerMask sunMask = 1 << 8;
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    Vector3.back, Mathf.Infinity, sunMask);
                if (hit)
                {
                    StartCoroutine(hit.transform.GetComponent<Sun>().BeCollected());
                }
            }

        }
    }

    public void SelectingPlant(Sprite plantSprite, Card selectingCard)
    {
        this.selectingCard = selectingCard;
        currentState = GMState.Selectingplant;
        plantSelecting.SetActive(!plantSelecting.activeSelf);
        plantSelecting.GetComponent<SpriteRenderer>().sprite = plantSprite;
        plantPreview.SetActive(!plantPreview.activeSelf);
        plantPreview.GetComponent<SpriteRenderer>().sprite = plantSprite;
    }

    Vector2 GetPlantPreviewPosition()
    {
        int y = 0;
        int x = 0;
        float distance = float.MaxValue;

        for (int i = 0; i < plantGrids.Length; i++)//row
        {
            var newDistance = Vector2.Distance(plantSelecting.transform.position, plantGrids[i][0]);

            if (newDistance > distance) break;

            distance = newDistance;
            y = i;
        }

        distance = float.MaxValue;
        for (int i = 0; i < plantGrids[y].Length; i++)
        {
            var newDistance = Vector2.Distance(plantSelecting.transform.position, plantGrids[y][i]);

            if (newDistance > distance) break;

            distance = newDistance;
            x = i;
        }
        return plantGrids[y][x];
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
