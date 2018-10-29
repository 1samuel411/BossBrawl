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

    public float intensity = 12;
    public float intensityDef = 12;
    public float intensityMultiplier = 12;
    public float falloff = 1.5f;
    public float intensityReduction = 12;
    public float period = 12;
    public float yInitOffset;

    public List<Pillar> generatedMap = new List<Pillar>();
    public GameObject[] pillars;

    [System.Serializable]
    public class Pillar
    {
        public Transform transform;
        public int x;
        public int y;
        public float yOffset;
        public bool enabled;
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
        "003333113300." +
        "001111111100." +
        "001111311100." +
        "111122111111." +
        "111211111111." +
        "111000000111." +
        "111000000111.",

        "111000000111." +
        "111000000111." +
        "111111112111." +
        "111311131111." +
        "001112111100." +
        "001112111100." +
        "001112111100." +
        "001212311100." +
        "112122111111." +
        "111211111111." +
        "111000000111." +
        "111000000111.",

        "111000000111." +
        "112000000111." +
        "112111113111." +
        "112114111111." +
        "002113111100." +
        "001113111300." +
        "001111121100." +
        "001111311100." +
        "113122111111." +
        "141211111111." +
        "121000000111." +
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
                bool enabled = true;
                float yOffset = int.Parse(split[y][x].ToString()) * 0.5f;
                if (yOffset == 0)
                {
                    yOffset = -10;
                    enabled = false;
                }
                else
                    yOffset += UnityEngine.Random.Range(-0.1f, 0.1f);
                generatedMap.Find(p => p.x == x && p.y == y).yOffset = yOffset;
                generatedMap.Find(p => p.x == x && p.y == y).enabled = enabled;
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

    public void Impact(int x, int y, float intensity)
    {
        this.intensity = intensity;
        StartCoroutine(ImpactCoroutine(new Vector2(x, y)));
    }

    public void Impact(GameObject hexagon)
    {
        this.intensity = intensityDef;
        Pillar pillar = generatedMap.Find(x => x.transform == hexagon.transform);
        StartCoroutine(ImpactCoroutine(new Vector2(pillar.x, pillar.y)));
    }

    IEnumerator ImpactCoroutine(Vector2 a)
    {
        while (intensity > 0f)
        {
            string[] map = mapToGenerate.Split('.');
            int width = map[0].Length;
            int height = map.Length;

            //period += Time.deltaTime * 3;
            intensity -= Time.deltaTime * intensityReduction;

            for(int i = 0; i < generatedMap.Count; i++)
            {
                if (!generatedMap[i].enabled)
                    continue;
                float difference = (a - new Vector2(generatedMap[i].x, generatedMap[i].y)).magnitude;

                generatedMap[i].yOffset = yInitOffset + ((falloff/difference) * (intensity * intensityMultiplier) * Mathf.Sin(Time.time * period * difference) * Time.deltaTime);
            }

            yield return null;
        }
        SetYOffset(stages[Random.Range(0, stages.Length)]);
    }

    void SetOffset(float newOffset, int x, int y)
    {
        for(int i =0; i < generatedMap.Count; i++)
        {
            if(generatedMap[i].x == x && generatedMap[i].y == y && generatedMap[i].enabled)
            {
                generatedMap[i].yOffset = newOffset;
            }
        }
    }

}
