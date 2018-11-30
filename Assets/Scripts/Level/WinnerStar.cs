using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerStar : MonoBehaviour
{
    GameManager gm;

    private void Start()
    {
        gm = GameManager.GetInstance();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gm.WinState();
        }
    }
}
