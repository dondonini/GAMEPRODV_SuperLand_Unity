using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAllEnemiesDeath : MonoBehaviour
{
    [Tooltip("This sync key must match all the keys in the attached locks")]
    public string syncKey = "1111";

    public List<GameObject> attachedLocks = new List<GameObject>();

    public List<GameObject> enemies = new List<GameObject>();

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/

    bool isAllDead = false;

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
