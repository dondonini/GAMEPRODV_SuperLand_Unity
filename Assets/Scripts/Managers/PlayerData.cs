using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{

    public static PlayerData instance = null;

    public static PlayerData GetInstance()
    {
        if (instance == null)
        {
            instance = new PlayerData();
        }

        return instance;
    }

    
}
