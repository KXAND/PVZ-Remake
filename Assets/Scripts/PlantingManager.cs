using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantingManager : MonoBehaviour
{
    public Transform PlantsParent;
    public Transform[][] Grids = null;
    Transform PlantPreview;

    int RowNum => Grids.GetLength(0);
    int RowLength => Grids[0].Length;
    public static PlantingManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(Instance); }
    }
    public void Init()
    {
        Grids = new Transform[PlantsParent.childCount][];
        for (int i = 0; i < PlantsParent.childCount; i++)
        {
            var row = PlantsParent.GetChild(i);
            Grids[i] = new Transform[row.childCount];
            for (int j = 0; j < row.childCount; j++)
            {
                Grids[i][j] = row.GetChild(j);
            }
        }
    }

    /// <summary>
    ///  ���� plantPreview �����꣬�����ɼ�ʱ���ꡣ���ڽ�Ԥ��ͼƬ��Ϊ UI ������˲���ֱ�ӹ����� Gids �£����� Vector3 ����.
    /// </summary>
    /// <param name="PlantSelectingWorldPos"></param>
    /// <returns></returns>
    public Vector3 GetPlantPreviewWorldPosition(Vector3 PlantSelectingWorldPos)
    {
        int y = 0;
        int x = 0;
        float distance = float.MaxValue;

        for (int i = 0; i < Grids.Length; i++)//row
        {
            var newDistance = Vector2.Distance(PlantSelectingWorldPos, Grids[i][0].position);

            if (newDistance > distance) break;

            distance = newDistance;
            y = i;
        }

        distance = float.MaxValue;
        for (int i = 0; i < Grids[y].Length; i++)
        {
            var newDistance = Vector2.Distance(PlantSelectingWorldPos, Grids[y][i].position);

            if (newDistance > distance) break;

            distance = newDistance;
            x = i;
        }
        if (Vector2.Distance(PlantSelectingWorldPos, Grids[y][x].position) < 1)
        {
            PlantPreview = Grids[y][x];
            return Grids[y][x].position;
        }
        else
        {
            PlantPreview = null;
            return new(10, 10);
        }
    }

    public Vector3 GetRandomRowRightGridPosition()
    {
        return Grids[Random.Range(0, RowNum)][RowLength - 1].position;
        //return Vector3.zero;
    }

    public void Plant(Card PlantingCard)
    {
        PlantingCard.OnPlanted();
        Instantiate(PlantingCard.Seed.seedInstance, parent: PlantPreview);
    }
}
