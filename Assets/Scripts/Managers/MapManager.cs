using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    [SerializeField]
    bool showWholeMap = false;
    [SerializeField]
    float showSpeed = 1.0f;
    [SerializeField]
    Transform startPosition;

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

    public class MapSegmentData
    {
        public IMapMove move;
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

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/

    float showDistance = 1.0f;

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
            // Check if segment has a script that inherits from IMapMove
            if (mapSeg.GetComponent<IMapMove>() == null)
            {
                // Check if parent has a script that inherits from IMapMove
                if (mapSeg.GetComponentInParent<IMapMove>() == null)
                {
                    // Yeah... it's missing. Fix that!
                    Debug.LogWarning(mapSeg.name + " is missing a IMapMove script!");
                    continue;
                }
                else
                {
                    // It is a large segment
                    continue;
                }
            }

            MapSegmentData newData = new MapSegmentData
            {
                move = mapSeg.GetComponent<IMapMove>(),
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

        showDistance += showSpeed * Time.deltaTime;
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

                

                if (showWholeMap)
                {
                    float distanceSqr = Vector3.Distance(startPosition.position, currentData.move.GetRootPosition());

                    if (distanceSqr < showDistance)
                    {
                        result = true;
                    }
                }
                else
                {
                    SubjectData foundSubjectData = currentInstance.GetComponent<SubjectData>();

                    float internalDistance = Vector3.Distance(currentData.move.GetRootPosition(), currentData.move.GetSegmentBounds().ClosestPoint(currentInstance.position));

                    float distanceSqr = Vector3.Distance(currentInstance.position, currentData.move.GetRootPosition());

                    //distanceSqr -= internalDistance;

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
                
                
            }

            currentData.move.SegmentEnabled(result);
        }
    }

}
