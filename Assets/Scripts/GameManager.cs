using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Sun sun;

    public Transform plantGridParent;
    public GameObject plantSelecting;
    public GameObject plantPreview;

    public Vector2[][] plantGrids;
    private void Awake()
    {
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
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ThrowSun());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            plantSelecting.SetActive(!plantSelecting.activeSelf);
            plantPreview.SetActive(!plantPreview.activeSelf);
            plantPreview.GetComponent<SpriteRenderer>().sprite = plantSelecting.GetComponent<SpriteRenderer>().sprite;
        }

        if (plantSelecting.activeSelf)
        {
            plantSelecting.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            plantPreview.transform.position = GetPlantPreviewPosition();
        }
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

    void CreateFallingSun()
    {
        Vector3 startPos = new(Random.Range(-7f, 4f), 8f, 0);
        Vector3 destPos = new(Random.Range(-7f, 4f), Random.Range(-4f, 0f), 0);

        Instantiate(sun);
        sun.transform.position = startPos;
        StartCoroutine(sun.SkySunThrow(destPos));
    }

    IEnumerator ThrowSun()
    {
        float throwSunBasicCD = 10f;
        float throwSunRandomCD = 5f;
        yield return new WaitForSeconds(Random.Range(0f, throwSunRandomCD));
        while (true)
        {
            CreateFallingSun();
            yield return new WaitForSeconds(
                Random.Range(throwSunBasicCD - throwSunRandomCD, throwSunBasicCD + throwSunRandomCD));
        }
    }

}
