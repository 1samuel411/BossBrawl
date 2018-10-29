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
    public delegate void OnDeath();
    public OnDeath onDeath;

    public Image healthBar;

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if(onDamage != null)
            onDamage(amount);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            onDeath.Invoke();
        }

    }

    public void Update()
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
    }



}