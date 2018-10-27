using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneManager : MonoBehaviour
{

    public enum Direction { North, East, South, West };

    public static SceneManager instance;

    [System.Serializable]
    public class BossPosition
    {
        public Direction dir;
        public Transform holder;
    }
    public BossPosition[] bossPositions;

    public Transform center;

    private void Awake()
    {
        instance = this;
    }

    public BossPosition GetBossPosition(Direction dir)
    {
        return bossPositions.First(x => x.dir == dir);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
