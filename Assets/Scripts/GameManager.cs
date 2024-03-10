using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GMState
    {
        Ilde,
        SelectingPlant
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

    public Card selectingCard;
    private int sunNum = 50;

    public Vector3 SunIconPosition => Camera.main.ScreenToWorldPoint(SunIcon.position);

    Vector3 PlantPreviewWorldPos => Camera.main.ScreenToWorldPoint(plantPreview.transform.position);
    Vector3 PlantSelectingWorldPos => Camera.main.ScreenToWorldPoint(plantSelecting.transform.position);
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

    // preview correct :plant
    // preview correct not enough: plant cancel
    // preview wrong: cancel
    // right click:cancel

    // Update is called once per frame
    void Update()
    {
        //if (currentState == GMState.SelectingPlant)
        //{
        //    plantSelecting.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        //    plantPreview.transform.position = Camera.main.WorldToScreenPoint(GetPlantPreviewWorldPosition());
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        //var a = new(10, 10, 0);
        //        Debug.Log((PlantPreviewWorldPos == new Vector3(10, 10, 0)) + " " + PlantPreviewWorldPos + " " + new Vector3(10, 10, 0));
        //        if (PlantPreviewWorldPos != new Vector3(10f, 10f, 0f))// if planting place is available
        //        {
        //            selectingCard.OnPlanted();
        //            Instantiate(selectingCard.Seed.seedInstance, PlantPreviewWorldPos, Quaternion.identity);
        //            SunNum -= selectingCard.Seed.costSun;
        //            currentState = GMState.Ilde;
        //            plantSelecting.SetActive(false);
        //            plantPreview.SetActive(false);
        //        }
        //        else ToIdleState();
        //    }

        //    if (Input.GetMouseButtonDown(1))
        //    {
        //        ToIdleState();
        //    }
        //}
        //else if (currentState == GMState.Ilde)
        //{
        //    if (Input.GetKeyDown(KeyCode.Mouse0))
        //    {
        //        LayerMask sunMask = 1 << 8;
        //        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
        //            Vector3.back, Mathf.Infinity, sunMask);
        //        if (hit)
        //        {
        //            StartCoroutine(hit.transform.GetComponent<Sun>().BeCollected());
        //        }
        //    }
        //}
    }

    public void ToSelectingPlantState(Sprite plantSprite, Card selectingCard)
    {
        Debug.Log(!plantSelecting.activeSelf);
        currentState = GMState.SelectingPlant;
        this.selectingCard = selectingCard;
        plantSelecting.SetActive(!plantSelecting.activeSelf);
        plantSelecting.GetComponent<Image>().sprite = plantSprite;
        plantPreview.SetActive(!plantPreview.activeSelf);
        plantPreview.GetComponent<Image>().sprite = plantSprite;
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

    void ToIdleState()
    {
        currentState = GMState.Ilde;
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
