using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] int maxPlayerHealth = 100;
    [SerializeField] int playerCurrency = 0;

    float damageTimer = 0;
    int playerHealth;
    [Range(0, 0.5f)]
    [SerializeField] float maxDamageDelay = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = maxPlayerHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        damageTimer -= Time.deltaTime;
    }

    public int getPlayerHealth()
    {
        return playerHealth;
    }

    public void takeDamage(int damage)
    {
        if(damageTimer <= 0)
        {
            playerHealth -= damage;
            playerHealth = Mathf.Clamp(playerHealth, 0, maxPlayerHealth);
            damageTimer = maxDamageDelay;
        }
    }

    public int getPlayerCurrency()
    {
        return playerCurrency;
    }
}
