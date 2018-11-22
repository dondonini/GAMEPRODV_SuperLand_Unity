using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerButton : MonoBehaviour
{
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.GetInstance();
    }

    void OnCollisionEnter()
    {
        gm.WinState();
    }
}
