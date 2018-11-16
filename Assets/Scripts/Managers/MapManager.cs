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
        public float viewingRadius;

    }

    /************************************************************************/
    /* Variables                                                            */
    /************************************************************************/

    /* This array holds all of the instances to keep track of
     * when loading map info. */
    [SerializeField]
    public List<Transform> viewableInstances;

    [ReadOnly]
    public List<MapSegmentData> mapSegments;

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

            MapData findMapData = mapSeg.GetComponent<MapData>();

            if (findMapData != null)
                newData.viewingRadius = findMapData.viewingRadius;
            else
                newData.viewingRadius = gameManager.defaultViewableRadius;

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
        for (int inst = 0; inst < viewableInstances.Count; inst++)
        {
            Transform currentInstance = viewableInstances[inst];

            // Check if instance is still alive
            // and remove it is dead
            if (currentInstance == null)
            {
                viewableInstances.Remove(currentInstance);
                continue;
            }

            for (int seg = 0; seg < mapSegments.Count; seg++)
            {
                MapSegmentData currentData = mapSegments[seg];

                // Check if segment is still alive
                // and remove it is dead
                if (currentData.tran == null)
                {                     
                    mapSegments.Remove(currentData);
                    continue;
                }

                // Check if segment is in instance's radius
                float distanceSqr = (currentInstance.position - currentData.move.GetRootPosition()).sqrMagnitude;

                

                if (distanceSqr < currentData.viewingRadius)
                    currentData.move.SegmentEnabled(true);
                else
                    currentData.move.SegmentEnabled(false);
            }
        }
    }
}