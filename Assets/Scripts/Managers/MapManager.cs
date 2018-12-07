﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    /************************************************************************/
    /* Instance Management                                                  */
    /************************************************************************/
    private static MapManager instance;

    private void Awake()
    {
        // Initialize instance and destroy self if there is already one available.
        if (!instance)
            instance = this;
        else
            Destroy(this);
    }

    /// <summary>
    /// Return existing instance of MapManager
    /// </summary>
    /// <returns>MapManager instance</returns>
    public static MapManager GetInstance()
    {
        return instance;
    }

    /************************************************************************/
    /* Map Data Struct                                                      */
    /************************************************************************/

    public struct MapSegmentData
    {
        public MapMovement move;
        public Transform tran;
    }

    /************************************************************************/
    /* Variables                                                            */
    /************************************************************************/

    [SerializeField]
    private List<MapSegmentData> mapSegments;

    /************************************************************************/
    /* Caching                                                              */
    /************************************************************************/

    GameManager gameManager;

    

    // Use this for initialization
    void Start () 
    {
        // Caching variables
        gameManager = GameManager.GetInstance();

        // Initializing variables
        mapSegments = new List<MapSegmentData>();

		// Collecting all map segments
        foreach (GameObject mapSeg in GameObject.FindGameObjectsWithTag("Land"))
        {
            MapSegmentData newData = new MapSegmentData
            {
                move = mapSeg.GetComponent<MapMovement>(),
                tran = mapSeg.transform,
            };

            mapSegments.Add(newData);
        }
        Debug.Log("Segments found: " + mapSegments.Count);
    }
	
	// Update is called once per frame
	void Update () 
    {
        UpdateMap();
    }

    void UpdateMap()
    {
        for (int seg = 0; seg < mapSegments.Count; seg++)
        {
            MapSegmentData currentData = mapSegments[seg];

            // Check if segment is still alive
            // and remove it if dead
            if (currentData.tran == null || currentData.move.IsIgnored())
            {                     
                mapSegments.Remove(currentData);
                continue;
            }

            bool result = false;

            for (int inst = 0; inst < gameManager.GetAllSubjectCount(); inst++)
            {
                Transform currentInstance = gameManager.allSubjects[inst];

                // Dead link found! Remove from subject list
                if (!currentInstance)
                {
                    gameManager.RemoveSubject (currentInstance);
                    continue;
                }

                SubjectData foundSubjectData = currentInstance.GetComponent<SubjectData>();

                float distanceSqr = Vector3.Distance(currentInstance.position, currentData.move.GetRootPosition());

                // Check if valid distance to spawn
                if (foundSubjectData && distanceSqr < foundSubjectData.viewingRadius)
                {
                    result = true;
                }
                else if (!foundSubjectData && distanceSqr < gameManager.defaultViewableRadius)
                {
                    result = true;
                }
                
            }

            if (result)
                currentData.move.SegmentEnabled(true);
            else
                currentData.move.SegmentEnabled(false);
        }
    }

}
