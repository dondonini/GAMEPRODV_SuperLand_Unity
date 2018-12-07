﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAllEnemiesDeath : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();
    [SerializeField]
    public List<Transform> attachedLocks = new List<Transform>();

    public string syncKey = "1111";

    bool isAllDead = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForDeadEnemies();

        if (enemies.Count == 0 && !isAllDead)
        {
            for(int l = 0; l < attachedLocks.Count; l++)
            {
                ILock levelLock = attachedLocks[l].GetComponent<ILock>();
                levelLock.UnlockLand(syncKey);
            }

            isAllDead = true;
        }
    }

    void CheckForDeadEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i])
            {
                enemies.RemoveAt(i);
            }
        }
    }
}
