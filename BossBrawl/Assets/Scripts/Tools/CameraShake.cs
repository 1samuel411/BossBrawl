using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public float intensity;
    public float intensityDecay;

    public Health health;

    private Vector3 initPos;

    void Start()
    {
        initPos = transform.localPosition;
        health.onDamage += OnDamage;
    }

    void OnDamage(int dmg)
    {
        intensity = (dmg * 0.5f) * 1;
    }

    void Update()
    {
        transform.localPosition = initPos + (((Vector3.up * Random.Range(-intensity, intensity)) + (Vector3.left * Random.Range(-intensity, intensity))) * Time.deltaTime);
        intensity -= Time.deltaTime * intensityDecay;
        intensity = Mathf.Clamp(intensity, 0, int.MaxValue);
    }

    public void SetIntensity(float newIntensity)
    {
        intensity = newIntensity;
    }
}
