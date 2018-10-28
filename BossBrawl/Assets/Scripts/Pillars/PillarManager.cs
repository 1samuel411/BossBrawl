using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            generatedMap[i].transform.localPosition = Vector3.Lerp(generatedMap[i].transform.localPosition, new Vector3(generatedMap[i].transform.localPosition.x, generatedMap[i].yOffset, generatedMap[i].transform.localPosition.z), 5 * Time.deltaTime);
        }
    }

    public void Test()
    {
        Impact(Random.Range(0, 12), Random.Range(0, 12), 4.0f);   
    }

    public void Impact(int x, int y, float intensity)
    {
        Vector2[] p = new Vector2[4];
        Vector2[] k = new Vector2[4];
        Vector2[] t = new Vector2[12];

        p[0] = new Vector2(x - 1, y);
        p[1] = new Vector2(x + 1, y);
        p[2] = new Vector2(x, y + 1);
        p[3] = new Vector2(x, y - 1);

        k[0] = new Vector2(x + 1, y+1);
        k[1] = new Vector2(x - 1, y+1);
        k[2] = new Vector2(x + 1, y-1);
        k[3] = new Vector2(x - 1, y-1);

        t[0] = new Vector2(x - 2, y + 1);
        t[1] = new Vector2(x - 2, y);
        t[2] = new Vector2(x - 2, y - 1);

        t[3] = new Vector2(x + 2, y + 1);
        t[4] = new Vector2(x + 2, y);
        t[5] = new Vector2(x + 2, y - 1);

        t[6] = new Vector2(x - 1, y - 2);
        t[7] = new Vector2(x, y - 2);
        t[8] = new Vector2(x + 1, y - 2);

        t[9] = new Vector2(x - 1, y + 2);
        t[10] = new Vector2(x, y + 2);
        t[11] = new Vector2(x + 1, y + 2);

        // Get radius
        //   t t t  
        // t k p k t
        // t p o p t
        // t k p k t
        //   t t t  


        StartCoroutine(ImpactCoroutine(p, k, t, intensity));
    }

    IEnumerator ImpactCoroutine(Vector2[] p, Vector2[] k, Vector2[] t, float intensity)
    {
        Debug.Log("Testing");
        float period = 2 * Mathf.PI;

        while (intensity <= 0f)
        {
            string[] map = mapToGenerate.Split('.');
            int width = map[0].Length;
            int height = map.Length;

            period += Time.time * 3;
            intensity -= Time.time * 3;
            for (int i = 0; i < p.Length; i++)
            {
                // valid cord
                if (p[i].x >= 0 && p[i].x <= width && p[i].y >= 0 && p[i].y <= height)
                {
                    generatedMap.Single(x => x.x == p[i].x && x.y == p[i].y).yOffset = intensity * Mathf.Sin(period * Time.deltaTime);
                }
            }

            for (int i = 0; i < k.Length; i++)
            {
                // valid cord
                if (k[i].x >= 0 && k[i].x <= width && k[i].y >= 0 && k[i].y <= height)
                {
                    generatedMap.Single(x => x.x == k[i].x && x.y == k[i].y).yOffset = intensity * Mathf.Sin(period * Time.deltaTime);
                }
            }

            for (int i = 0; i < t.Length; i++)
            {
                // valid cord
                if (t[i].x >= 0 && t[i].x <= width && t[i].y >= 0 && t[i].y <= height)
                {
                    generatedMap.Single(x => x.x == t[i].x && x.y == t[i].y).yOffset = intensity * Mathf.Sin(period * Time.deltaTime);
                }
            }

            yield return null;
        }
    }

}
