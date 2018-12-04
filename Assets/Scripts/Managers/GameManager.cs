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

    public float despawnHeight = -100.0f;
    public float defaultViewableRadius = 10.0f;

    [SerializeField]
    private Transform mainSubject;
    [SerializeField]
    private List<Transform> subjects;

    [Header("Points")]

    [SerializeField]
    private int localCoins = 0;

    [SerializeField]
    private int localStars = 0;

    bool isWon = false;
    bool isLost = false;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		DespawnDeadSubjects();
	}

    public void SetMainSubject(Transform _newMainSubject)
    {
        if (mainSubject == _newMainSubject)
        {
            Debug.LogWarning(_newMainSubject + " is already the main subject... what are you doing?");
            return;
        }

        // Set new main subject
        mainSubject =_newMainSubject;
    }

    public void AddSubject(Transform _newSubject)
    {
        // Prevent duplicates
        if (mainSubject && mainSubject == _newSubject) return;
        for (int s = 0; s < subjects.Count; s++)
        {
            if (subjects[s] == mainSubject) return;
        }

        subjects.Add(_newSubject);
    }

    public void RemoveMainSubject()
    {
        mainSubject = null;
    }

    public void RemoveSubject(Transform _targetSubject)
    {
        if (mainSubject == _targetSubject)
        {
            subjects.Remove(_targetSubject);
            return;
        }

        for (int s = 0; s < subjects.Count; s++)
        {
            if (subjects[s] == _targetSubject)
            {
                subjects.RemoveAt(s);
                return;
            }
        }

        // If it gets to hear, it means that the targeted subject was not found
        Debug.LogWarning("Subject " + _targetSubject + " is not in the subject list.");
    }

    public Transform[] GetAllSubjects()
    {
        Transform[] tempArray;

        if (mainSubject)
        {        
            tempArray = new Transform[subjects.Count + 1];

            // Add main subject as first
            tempArray[0] = mainSubject;

            // Add the rest of the subjects
            for (int s = 1; s < subjects.Count + 1; s++)
            {
                tempArray[s] = subjects[s - 1];
            }
        }
        else
        { 
            // Just set it as the subject array
            tempArray = subjects.ToArray();
        }

        return tempArray;
    }

    public Transform GetMainSubject()
    {
        return mainSubject;
    }

    public int GetAllSubjectCount()
    {
        if (mainSubject)
            return subjects.Count + 1;
        else
            return subjects.Count;
    }

    void DespawnDeadSubjects()
    {
        for (int s = 0; s < subjects.Count; s++)
        {
            if (subjects[s].position.y <= despawnHeight)
            {
                if (subjects[s].CompareTag("Player"))
                {
                    LoseState();
                }
                else if (subjects[s].CompareTag("Enemies"))
                {
                    // TODO: Kill enemies here
                }
                else
                {
                    Destroy(subjects[s].gameObject);
                }

                // Remove current subject in subject list
                subjects.RemoveAt(s);
            }
        }
    }

    /************************************************************************/
    /* Scoring                                                              */
    /************************************************************************/

    public void AddCoins(int _coins)
    {
        localCoins += _coins;
    }

    public void AddCoin()
    {
        localCoins++;
    }

    public void AddStars(int _stars)
    {
        localStars += _stars;
    }

    public void AddStar()
    {
        localStars++;
    }

    public void UploadData()
    {
        PlayerData.masterStars = localStars;
        PlayerData.coins = localCoins;
    }

    /************************************************************************/
    /* States                                                               */
    /************************************************************************/

    public void WinState()
    {
        if (isWon) return;

        Debug.Log("Player won!");
    }

    public void LoseState()
    {
        if (isLost) return;
        // TODO: Make a lost state

        Debug.Log("Player lost!");
    }
}
