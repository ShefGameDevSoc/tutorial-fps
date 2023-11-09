using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is an attribute. In C#, attributes describe properties
// about a field, function or in this case a class
// The RequireComponent attribute makes it so that the editor
// forces the component in typeof(XXXXX) - in this case
// CharacterController - to be present on any GameObject
// PlayerMovement is present

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float gravity = 9.81f;

    [SerializeField] float walkAcceleration = 10f;
    [SerializeField] float walkSpeed = 5f;

    [SerializeField] Vector3 _velocity = Vector3.zero;

    CharacterController _characterController;

    [SerializeField] float jumpPower = 5f;
    [SerializeField] bool _grounded = false;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] LayerMask groundMask;


    bool _canJump = true;

    // Start is called before the first frame update
    void Start()
    {
        //just a getter component
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        Gravity();
        PlanarDecelerate();

        Walk();
        Jump();

        // We need to get the distance we travel this frame
        // so we multiply the velocity by the time step (Time.deltaTime)
        // and use the result to call CharacterController's Move method

        var motion = _velocity * Time.deltaTime;
        var collisionFlags = _characterController.Move(motion);
        CollisionResolution(collisionFlags);
    }

    void Gravity()
    {
        // Applying gravity is a simple operation
        // As gravity is a rate of change of velocity we need to multiply
        // the gravity variable by the timestep (Time.deltaTime)
        // and a Vector3 direction then add it to the _velocity variable
        // What direction would be appropriate?
        _velocity += Vector3.down * gravity * Time.deltaTime;
    }

    void Walk()
    {

        // First we get the movement direction using the Input class
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        // Then we calculate the directional speed and factor in
        // the walkSpeed field
        // We need to make sure this is relative to the direction
        // we are currently facing

        var moveWalk = Mathf.Approximately(vertical, 0) ? 0 : (int)Mathf.Sign(vertical);
        var moveStrafe = Mathf.Approximately(horizontal, 0) ? 0 : (int)Mathf.Sign(horizontal);

        var planarVelocity = Vector3.zero;
        planarVelocity.x = Vector3.Dot(_velocity, transform.right);
        planarVelocity.z = Vector3.Dot(_velocity, transform.forward);
        if (planarVelocity.magnitude > walkSpeed && Vector3.Dot(new Vector3(moveStrafe, 0, moveWalk), planarVelocity) > 0)
        {
            moveStrafe = 0;
            moveWalk = 0;
        }
        else
        {
            if (!Mathf.Approximately(0, planarVelocity.z)
                && Mathf.Approximately(moveWalk, Mathf.Sign(planarVelocity.z))
                && Mathf.Abs(planarVelocity.z) > walkSpeed)
                moveWalk = 0;
            if (!Mathf.Approximately(0, planarVelocity.x)
                && Mathf.Approximately(moveStrafe, Mathf.Sign(planarVelocity.x))
                && Mathf.Abs(planarVelocity.x) > walkSpeed)
                moveStrafe = 0;
        }

        // First we get the movement direction using the Input class
        if (moveStrafe == 0 && moveWalk == 0)
            return;

        var vecMove = new Vector3(moveStrafe, 0, moveWalk);
        vecMove /= vecMove.magnitude;

        var trueDirection = transform.forward * vecMove.z + transform.right * vecMove.x;
        _velocity += trueDirection * walkAcceleration * Time.deltaTime;
    }


    void Jump()
    {
        if (!_grounded || !_canJump)
            return;

    
        if (Input.GetButton("Jump"))
        {
            _velocity.y += jumpPower;
            StartCoroutine(ReJumpTimeout());
        }
    }

    IEnumerator ReJumpTimeout()
    {
        _canJump = false; // What do you think would be an appropriate value?
        yield return new WaitForSeconds(0.2f);
        _canJump = true;
    }

    /// Decelerates the player's movement on the xz plane when no movement keys are pressed.
    void PlanarDecelerate()
    {
        if (AnyMovementKeys())
            return;

        var deceleration = Vector3.zero;
        deceleration.x = -Mathf.Sign(_velocity.x);
        deceleration.z = -Mathf.Sign(_velocity.z);

        _velocity +=  walkAcceleration * 0.7f * Time.deltaTime * deceleration;
        if (Mathf.Abs(_velocity.x) < 3f)
            _velocity.x = 0;
        if (Mathf.Abs(_velocity.z) < 3f)
            _velocity.z = 0;
    }

    void CheckGrounded()
    {

        _grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void CollisionResolution(CollisionFlags flags)
    {
        // Simple collision reaction
        // If a below collision has been detected
        if ((flags & CollisionFlags.Below) == CollisionFlags.Below)
        {
             // And we are moving down // Set the Y velocity to zero
            if (_velocity.y < 0) _velocity.y = 0;
        }
    }

    bool AnyMovementKeys() => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);


    // OnDrawGizmos and OnDrawGizmosSelected allow you to draw
    // 'gizmos' in the scene. OnDrawGizmosSelected only draws when
    // the GameObject this script is on is selected.
    private void OnDrawGizmos()
    {
        if (groundCheck)
        {
            Gizmos.color = _grounded ? Color.green : Color.grey;
            Gizmos.DrawWireSphere(groundCheck.position,  groundDistance);
        }
    }
}
