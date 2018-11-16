using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DT_Machine : MonoBehaviour
{
    public GameObject projectile;
    public float launchVelocity = 50.0f;
    public float launchFreq = 5.0f;

    public Transform launchPosition;

    public Animator turrentRig;

    private float count = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (count >= launchFreq - 1.0f && count < launchFreq)
        {
            turrentRig.SetBool("BuildAndFire", true);
        }
        else if (count > launchFreq)
        {
            turrentRig.SetBool("BuildAndFire", false);
            
            GameObject newProjectile = Instantiate(projectile) as GameObject;
            newProjectile.transform.position = launchPosition.position;
            newProjectile.transform.localRotation = launchPosition.transform.localRotation;
            newProjectile.GetComponent<Rigidbody>().AddRelativeForce(launchPosition.forward * launchVelocity);

            count = 0.0f;
        }

        count += Time.deltaTime;
    }
}
