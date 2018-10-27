using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVScroll : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    public float scrollRate;
    public Vector2 scroll;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        scroll += Vector2.one * scrollRate * Time.deltaTime;
        meshRenderer.material.SetTextureOffset("_MainTex", scroll);
    }
}
