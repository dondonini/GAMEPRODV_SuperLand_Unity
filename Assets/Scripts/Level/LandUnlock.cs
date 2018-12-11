using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LandUnlock : MonoBehaviour, ILoading, ILock
{
    #region Loading Data

    int l_loadedData = 0;
    int l_load = 0;

    public bool l_IsReady()
    {
        if (l_loadedData >= l_load) return true;

        return false;
    }

    public float l_LoadPercentage()
    {
        return Mathf.Clamp01(l_loadedData / l_load);
    }

    #endregion

    // Variables
    public bool unlockSegments = false;
    [Tooltip("This should match the unlock task")]
    public string syncKey = "1111";
    public GameObject[] lockedSegments;

    // Runtime Variables
    bool prevLockSet;
    List<MapMovement> segMapMovementRef;

    private void OnDrawGizmosSelected()
    {
        if (lockedSegments == null || lockedSegments.Length == 0) return;
        // Display all selected locked segments
        foreach(GameObject seg in lockedSegments)
        {
            MapMovement mapMove = seg.GetComponent<MapMovement>();

            if (mapMove.IsSegmentEnabled())
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawCube(seg.transform.position, Vector3.one * 0.1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        segMapMovementRef = new List<MapMovement>();

        // Calculate load
        l_load += lockedSegments.Length;

        // Collect all MapMovement scripts in all segments
        segMapMovementRef = GetAllMapMovementScripts();
    }

    List<MapMovement> GetAllMapMovementScripts()
    {
        List<MapMovement> tempArray = new List<MapMovement>();

        foreach(GameObject seg in lockedSegments)
        {
            MapMovement foundMapMove = seg.GetComponent<MapMovement>();

            if (foundMapMove)
            {
                tempArray.Add(foundMapMove);
                l_loadedData++;
            }
            else
            {
                Debug.LogWarning("There is no MapMovement script in " + seg + ". Could have been misplaced.");
            }
        }

        return tempArray;
    }

    // Update is called once per frame
    void Update()
    {
        if (unlockSegments != prevLockSet)
        {
            SetSegments(unlockSegments);
        }

        prevLockSet = unlockSegments;
    }

    public void UnlockLand(string _syncKey)
    {
        if (_syncKey == syncKey)
        {
            SetSegments(true);
        }
    }

    /// <summary>
    /// Unlocks all locked segments selected
    /// </summary>
    void SetSegments(bool _enabled)
    {
        foreach (MapMovement m in segMapMovementRef)
        {
            m.SegmentEnabled(_enabled);
        }
    }
}
