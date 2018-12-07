using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    /************************************************************************/
    /* Variables                                                            */
    /************************************************************************/

    [SerializeField]
    float movementSpeed = 750.0f;

    [SerializeField]
    float movementDrag = 2.0f;

    [SerializeField]
    float jumpForce = 5.0f;

    [SerializeField][Tooltip("The downward force applied when jump is released")]
    float fallForce = 20.0f;

    [SerializeField]
    float groundDistance = 1.0f;

    [SerializeField]
    float dashDistance = 1.0f;

    [SerializeField]
    [Tooltip("Double tap detection")]
    float buttonCooldown = 0.5f;

    /************************************************************************/
    /* References                                                           */
    /************************************************************************/

    Rigidbody rb;

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/

    bool isDoubleJumped = false;
    Vector3 playerVelocity;
    bool isGrounded;
    [SerializeField]
    Camera currentCamera;

	// Use this for initialization
	void Start () {
		
        rb = transform.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        isGrounded = IsGrounded();

        if (isDoubleJumped && isGrounded)
        {
            isDoubleJumped = false;
        }

        if (!isGrounded && !Input.GetButton("Jump"))
        {
            rb.AddForce(new Vector3(0.0f, -fallForce, 0.0f));
        }

        UpdateInput();
    }

    private void LateUpdate()
    {
        UpdateDrag(movementDrag);
    }

    bool IsGrounded()
    {
        // Calculate bottom point of player
        Vector3 groundPosition = transform.position;
        groundPosition.y -= transform.localScale.y * 0.5f;

        return Physics.CheckSphere(groundPosition, groundDistance, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);
    }

    // Applies linear drag to only the X and Z axis
    void UpdateDrag(float _dragAmount)
    {
        Vector3 vel = rb.velocity;
        vel.x = -_dragAmount * vel.x;
        vel.z = -_dragAmount * vel.z;
        rb.AddForce(new Vector3(vel.x, 0.0f, vel.z));
    }

    /************************************************************************/
    /* Input                                                                */
    /************************************************************************/

    void UpdateInput()
    {
        ////////////////////////////////////////////////////////////////////////// Movement
        if (Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f)
            MovePlayer();

        ////////////////////////////////////////////////////////////////////////// Jump
        if(Input.GetButtonDown("Jump") && isGrounded)
            JumpPlayer();
        else if (Input.GetButtonDown("Jump") && !isDoubleJumped)
        {
            Vector3 vel = rb.velocity;
            vel.y = 0.0f;
            rb.velocity = vel;

            JumpPlayer();
            isDoubleJumped = true;
        }

        ////////////////////////////////////////////////////////////////////////// Dash
        if (Input.GetButtonDown("Dash"))
            DashPlayer();
    }

    void MovePlayer()
    {
        // Get move inputs
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Convert movement to be relative to camera
        Vector3 relativeMove = currentCamera.transform.TransformVector(move);
        relativeMove.Scale(Vector3.forward + Vector3.right);

        // Move player
        rb.AddForce(relativeMove * Time.deltaTime * movementSpeed);
    }

    void JumpPlayer()
    {
        rb.AddForce(new Vector3(0, jumpForce, 0));
    }

    void DashPlayer()
    {
        // TODO: Add dash
    }
}
