using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    /************************************************************************/
    /* Instance Management                                                  */
    /************************************************************************/

    private static GameManager instance;

    private void Awake()
    {
        if (instance)
        {
            Debug.LogError("Two game instances exists in the game at once! This shouldn't be possible! WTF?!?! \n" +
                "Instance 1: " + this + "\n" +
                "Instance 2: " + instance
            );

            Debug.LogAssertion("Quitting game so you can fix the mess you've made...");

            Application.Quit();
        }
        else
            instance = this;
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    /************************************************************************/
    /* Variables                                                            */
    /************************************************************************/

    public float WORLD_SPAWN_HEIGHT = -100.0f;
    public float defaultViewableRadius = 10.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
