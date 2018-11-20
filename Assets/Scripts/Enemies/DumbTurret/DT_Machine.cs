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

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/
    
    private float count = 0.0f;
    private bool isBuildingUp = false;
    private bool isFired = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* Keep animation normal when the frequency has enough time to allow the animations to run for their full duration.
         * Run a different command set if it's too short
         */
        if (launchFreq >= 2.0f)
        {
            turrentRig.SetFloat("AnimationSpeed", 1.0f);

            isFired = false;

            // Start building animation before 2 seconds of frequency
            if (count >= launchFreq - 1.0f && count < launchFreq && isBuildingUp == false)
            {
                isBuildingUp = true;
                turrentRig.SetTrigger("Build");
            }

            // Launch projectile before 1 second of frequency
            else if (count > launchFreq && isFired == false)
            {
                turrentRig.SetTrigger("Fire");
                LaunchProjectile();

                isBuildingUp = false;

                count = 0.0f;
            }
        }
        else
        {
            float divider = launchFreq * 0.5f;

            // Adjust animation speed relative to launch frequency (Else case just for in case it tries to divide by 0)
            if (divider != 0.0f)
            {
                turrentRig.SetFloat("AnimationSpeed", launchFreq / divider);
            }
            else
            {
                turrentRig.SetFloat("AnimationSpeed", 1.0f);
            }
            
            // Start build animation at start
            if (count >= divider && isBuildingUp == false)
            {
                isBuildingUp = true;
                isFired = false;
                turrentRig.SetTrigger("Build");
            }

            // Launch after half the time starts
            if (isFired == false)
            {
                turrentRig.SetTrigger("Fire");
                LaunchProjectile();

                isBuildingUp = false;
                isFired = true;

                count = 0.0f;
            }
        }

        count += Time.deltaTime;
    }

    void LaunchProjectile()
    {
        GameObject newProjectile = Instantiate(projectile) as GameObject;

        // Set projectile's properties relative to enemy's
        newProjectile.transform.localScale = transform.localScale * 0.5f;

        // Set other properties for launch
        newProjectile.transform.position = launchPosition.position;
        newProjectile.transform.localRotation = launchPosition.transform.localRotation;
        newProjectile.GetComponent<Rigidbody>().AddRelativeForce(launchPosition.forward * launchVelocity);
    }
}
