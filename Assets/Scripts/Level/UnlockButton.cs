using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockButton : MonoBehaviour
{
    [Tooltip("This sync key must match all the keys in the attached locks")]
    public string syncKey = "1111";

    public List<GameObject> attachedLocks = new List<GameObject>();

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/
    bool isActive = false;

    void OnCollisionEnter(Collision _collision)
    {
        foreach (ContactPoint contact in _collision.contacts)
        {
            if (contact.otherCollider && !isActive)
            {
                isActive = true;
                Unlock();
            }
        }
    }

    void Unlock()
    {
        for(int l = 0; l < attachedLocks.Count; l++)
        {
            ILock levelLock = attachedLocks[l].GetComponent<ILock>();
            levelLock.UnlockLand(syncKey);
        }
    }
}
