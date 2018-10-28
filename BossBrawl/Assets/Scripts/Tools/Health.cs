using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public const int maxHealth = 100;
    public int currentHealth = maxHealth;
    public delegate void OnDamage(int damageTaken);
    public OnDamage onDamage;
    public delegate void OnHealing(int currentHealth);
    public OnHealing onHealing;

    public Image healthBar;

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        onDamage(amount);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Dead!");
        }
    }

    public void Update()
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
    }



}