using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Stats")]
    public int health = 3;
    public int maxHealth = 3;
    public float heathCooldown = 2.0f;

    [Header("Scoring Stats")]
    public int coinScore = 0;
    public int starScore = 0;

    GameManager gm;
    float coolDown = 0.0f;

    private void OnValidate()
    {
        coinScore = Mathf.Clamp(coinScore, 0, int.MaxValue);
        starScore = Mathf.Clamp(starScore, 0, int.MaxValue);
    }

    private void Start()
    {
        gm = GameManager.GetInstance();
    }

    private void Update()
    {
        // Cool down health if it goes over maxHealth
        if (health > maxHealth)
        {
            if (coolDown >= heathCooldown)
            {
                health--;
                coolDown = 0.0f;
            }
            else
            {
                coolDown += Time.deltaTime;
            }
        }
    }
}
