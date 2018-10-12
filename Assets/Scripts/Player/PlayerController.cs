using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    /************************************************************************/
    /* Variables                                                            */
    /************************************************************************/

    [SerializeField]
    float movementSpeed = 10.0f;

    [SerializeField]
    float jumpForce = 5.0f;

    [SerializeField]
    float groundDistance = 1.0f;

    [SerializeField]
    float jumpHeight = 0.5f;

    [SerializeField]
    float dashDistance = 1.0f;

    [SerializeField]
    Vector3 linearDrag = Vector3.zero;

    /************************************************************************/
    /* References                                                           */
    /************************************************************************/

    [SerializeField]
    CharacterController cc;
    [SerializeField]
    Transform groundChecker;

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/

    Vector3 playerVelocity;
    bool isGrounded;
    [SerializeField]
    Camera currentCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        isGrounded = IsGrounded();

        UpdateInput();

        // Adjust player velocity if in air
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = 0.0f;

        UpdatePlayerGravity();

        UpdateLinearDrag();
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundChecker.position, groundDistance, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);
        
    }

    void UpdatePlayerGravity()
    {
        playerVelocity.y += Physics.gravity.y * Time.deltaTime;
        cc.Move(playerVelocity * Time.deltaTime);
    }

    void UpdateLinearDrag()
    {
        playerVelocity.x /= 1 + linearDrag.x * Time.deltaTime;
        playerVelocity.y /= 1 + linearDrag.y * Time.deltaTime;
        playerVelocity.z /= 1 + linearDrag.z * Time.deltaTime;
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
        cc.Move(relativeMove * Time.deltaTime * movementSpeed);

        // Turn player according to move direction
        if (relativeMove != Vector3.zero)
            transform.forward = relativeMove;

        
    }

    void JumpPlayer()
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
    }

    void DashPlayer()
    {
        Debug.Log("Dash");
        playerVelocity += Vector3.Scale(transform.forward,
                                   dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * linearDrag.x + 1)) / -Time.deltaTime),
                                                              0,
                                                              (Mathf.Log(1f / (Time.deltaTime * linearDrag.z + 1)) / -Time.deltaTime)));
    }
}
