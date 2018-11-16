using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 rotateSpeed = new Vector3(1.0f, 0.0f, 0.0f);
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        // If target is not setup. The gameObject holding the script will become the target
        if (!target)
        {
            target = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        target.Rotate(rotateSpeed * Time.deltaTime);
    }
}
