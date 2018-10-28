using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarManager : MonoBehaviour
{

    public static PillarManager instance;

    public float zOffsetOdd = 1.492f;
    public float zOffsetEven = 1.492f;
    public float xOffsetOdd = 1.72f;
    public float xOffsetEven = 1.72f;

    public List<Pillar> generatedMap = new List<Pillar>();
    public GameObject[] pillars;

    [System.Serializable]
    public class Pillar
    {
        public Transform transform;
        public int x;
        public int y;
        public float yOffset;
    }

    private string mapToGenerate =
        "111000000111." +
        "111000000111." +
        "111111111111." +
        "111111111111." +
        "001111111100." +
        "001111111100." +
        "001111111100." +
        "001111111100." +
        "111111111111." +
        "111111111111." +
        "111000000111." +
        "111000000111.";

    private string[] stages =
    {
        "111000000111." +
        "111000000111." +
        "111111111111." +
        "111114111111." +
        "001113111100." +
        "003333333300." +
        "001111111100." +
        "001111111100." +
        "111111111111." +
        "111111111111." +
        "111000000111." +
        "111000000111."
    };

    private void Awake()
    {
        instance = this;
        GenerateMap();
        SetYOffset(stages[0]);
    }

    void Start()
    {

    }

    void Update()
    {
        UpdatePillarOffset();
    }

    public void GenerateMap()
    {
        string[] split = mapToGenerate.Split('.');

        float yOffset = 0, xOffset = 0;

        // rows
        for(int y = 0; y < split.Length; y++)
        {
            yOffset += (y % 2 == 0 ? zOffsetEven : zOffsetOdd);
            if (y % 2 == 0)
                xOffset = xOffsetOdd;
            else
                xOffset = 0;
            // cols
            for (int x = 0; x < split[y].Length; x++)
            {
                xOffset += (xOffsetEven);
                Pillar newPillar = new Pillar();
                GameObject newPillarObj = Instantiate(pillars[Random.Range(0, pillars.Length)], transform);
                newPillar.transform = newPillarObj.transform;
                newPillar.transform.position = new Vector3(xOffset, 0, yOffset);
                newPillar.transform.localPosition = new Vector3(newPillar.transform.localPosition.x, 0, newPillar.transform.localPosition.z);
                newPillar.x = x;
                newPillar.y = y;
                float rotation = 60 * (int)Random.Range(0, 5);
                newPillar.transform.eulerAngles = new Vector3(-90, rotation, 0);
                generatedMap.Add(newPillar);
            }
        }
    }

    public void SetYOffset(string stage)
    {
        string[] split = stage.Split('.');

        // rows
        for (int y = 0; y < split.Length; y++)
        {
            // cols
            for (int x = 0; x < split[y].Length; x++)
            {
                float yOffset = int.Parse(split[y][x].ToString()) * 0.5f;
                if (yOffset == 0)
                    yOffset -= 100;
                else
                    yOffset += UnityEngine.Random.Range(-0.1f, 0.1f);
                generatedMap.Find(p => p.x == x && p.y == y).yOffset = yOffset;
            }
        }
    }

    public void UpdatePillarOffset()
    {
        for(int i = 0; i < generatedMap.Count; i++)
        {
            generatedMap[i].transform.localPosition = new Vector3(generatedMap[i].transform.localPosition.x, generatedMap[i].yOffset, generatedMap[i].transform.localPosition.z);
        }
    }
}
