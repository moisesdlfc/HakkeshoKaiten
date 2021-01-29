using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// Set that this script need RigidBody associated
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    #region REFERENCES
    // Camera
    public Transform m_Camera;

    // Rigidbody
    Rigidbody m_Rigidbody;

    // Animator
    private Animator m_Animator;
    #endregion

    #region FORCES
    // Physics speed
    public float jumpForce, moveSpeed, runSpeed;

    // Current speed
    private float currentJumpForce, currentMoveSpeed;
    #endregion

    #region PHYSICS
    // Movement axis
    float horizontal, vertical;

    private float turnAround, forwardAmount;

    // Max distance to define that the player is on the ground 
    [SerializeField] float groundCheckDistance = 0.1f;

    [SerializeField] float stationaryTurnAround = 180;
    [SerializeField] float movingTurnSpeed = 360;

    private Vector3 cameraForward;
    private Vector3 move; // Move direction, calculated by FixedUpdate

    private Vector3 groundNormal; // Vector orthogonal to ground
    #endregion

    #region STATUS
    // Save ground status
    private bool isGrounded;

    private bool jump;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Reference
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();

        currentMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGroundStatus();

        // Check movement value in H/V
        horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        vertical = CrossPlatformInputManager.GetAxis("Vertical");

        Debug.Log("H: " + horizontal + " - V: " + vertical);

        // Check jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Check run
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            currentMoveSpeed = runSpeed;
        }

        // Check walk
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentMoveSpeed = moveSpeed;
        }
    }

    private void FixedUpdate()
    {
        // Calculate move direction
        if (m_Camera != null)
        {
            // Calculates the direction of movement relative to where the camera is looking
            cameraForward = Vector3.Scale(m_Camera.forward, new Vector3(1, 0, 1)).normalized;
            move = vertical * cameraForward + horizontal * m_Camera.right;
        } else
        {
            // In case of not having a motion camera, calculate the relative coordinates of the world 
            move = vertical * Vector3.forward + horizontal * Vector3.right;
        }

        // Check move
        if (move.magnitude > 0)
        {
            Move(move);
        }
    }

    // Check if player is touching the ground
    private void CheckGroundStatus()
    {
        #if UNITY_EDITOR // Enable Gizmos
        Debug.DrawLine(transform.position + (Vector3.up * 1.0f),
                        transform.position + Vector3.down * 1.0f,
                        Color.red);
        #endif

        // Hit info
        RaycastHit hitInfo;

        // Trace the raycast about 10 cm above the player's sole 
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f,
            Vector3.down,
            out hitInfo,
            groundCheckDistance))
        {
            isGrounded = true;

            // Sets the direction normal to the ground(perpendicular)
            groundNormal = hitInfo.normal;
        } else
        {
            isGrounded = false;

            // Sets the direction normal to the ground(perpendicular)
            groundNormal = Vector3.up;
        }
    }

    // Move
    private void Move(Vector3 movement)
    {
        /* The motion vector must always be normalized
           to facilitate a possible future modification
           of the speed  */
        if (movement.magnitude > 1.0f)
        {
            movement.Normalize(); // Now vector have lenght 1
        }

        // Transform direction from world space to local space
        movement = transform.InverseTransformDirection(movement);

        CheckGroundStatus();

        // We modify the movement according to the normal vector to the surface on which it walks 
        movement = Vector3.ProjectOnPlane(movement, groundNormal);

        turnAround = Mathf.Atan2(move.x, move.z);

        forwardAmount = move.z;

        m_Rigidbody.velocity = transform.forward * currentMoveSpeed;

        ApplyExtraRotation();
    }

    // Jump
    private void Jump()
    {
        m_Rigidbody.AddForce(0, jumpForce, 0);
    }

    private void ApplyExtraRotation()
    {

    }
}
