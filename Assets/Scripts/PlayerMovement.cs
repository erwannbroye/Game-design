
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Wallrunning
    public LayerMask whatIsWall;
    public float wallrunForce,maxWallrunTime, maxWallSpeed;
    bool isWallRight, isWallLeft;
    bool isWallRunning;
    public float maxWallRunCameraTilt, wallRunCameraTilt;

    
    //Assingables
    public Transform playerCam;
    public Transform orientation;

    //Other
    private Rigidbody rb;
    private CapsuleCollider capsule;

    //Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    private float startMaxSpeed;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    //Slide & Slide
    private Vector3 SlideScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;
    public bool readyToSlide = true;

    //Jumping
    public bool readyToJump = true;
    public bool readyToWallJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    //Input
    public float x, y;
    bool jumping, sprinting, sliding;

    //AirDash
    public float dashForce;
    public float dashTime;
    bool allowDashForceCounter;
    bool readyToDash;
    Vector3 dashStartVector;


    //sliding
    private Vector3 normalVector = Vector3.up;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        startMaxSpeed = maxSpeed;
    }

    void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        MyInput();
        Look();
        CheckForWall();
        WallRunInput();
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        sliding = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.LeftShift) && readyToSlide && (x != 0 || y != 0))
            if (!grounded && readyToDash)
                Dash();
            else if (grounded)
                StartSlide();
        if (Input.GetKeyUp(KeyCode.LeftShift) && grounded)
            StopSlide();
    }

   private void StartSlide() {
        readyToSlide = false;
        transform.localScale = SlideScale;
        if (rb.velocity.magnitude > 0.5f) {
                rb.AddForce(orientation.transform.forward * slideForce);
        }
        Invoke(nameof(StopSlide), 0.8f);
    }

    private void StopSlide() {
        transform.localScale = playerScale;
        readyToSlide = true;
    }

    private void Movement()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 10);
        
        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;
        Vector3 v = rb.velocity;
        if (v.y > 20)
        v.y = 20.0f;
        rb.velocity = v;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);
        
        //If holding jump && ready to jump, then jump
        if ((readyToJump || readyToWallJump) && jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;
        
        //If sliding down a ramp, add force down so player stays grounded and also builds speed

        
        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if ((x > 0 && xMag > maxSpeed) || (x < 0 && xMag < -maxSpeed)) x = 0;
        if ((y > 0 && yMag > maxSpeed ) || (y < 0 && yMag < -maxSpeed)) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;
        
        // Movement in air
        if (!grounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }
        
        // Movement while sliding
        if (grounded && !readyToSlide) multiplierV = 0f;

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        if (!isWallRunning)
            rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
          
            rb.velocity = Vector3.zero;

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //Walljump
        if (isWallRunning && readyToWallJump)
        {
            Debug.Log("walljump");
            readyToWallJump = false;

            //normal jump
            if (isWallLeft && !Input.GetKey(KeyCode.D) || isWallRight && !Input.GetKey(KeyCode.Q))
            {
                rb.AddForce(Vector2.up * jumpForce * 1.5f);
                rb.AddForce(normalVector * jumpForce * 0.5f);
            } else {
                rb.AddForce(Vector2.up * jumpForce * 1.2f);
                rb.AddForce(normalVector * jumpForce * 0.5f);
            }

            //sidwards wallhop
            if (isWallRight || isWallLeft && Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) rb.AddForce(-orientation.up * jumpForce * 1f);
            if (isWallRight && Input.GetKey(KeyCode.Q)) {
                rb.AddForce(-orientation.right * jumpForce * 1.5f);
                Debug.Log("jump left");
            }
            if (isWallLeft && Input.GetKey(KeyCode.D))  {
                Debug.Log("jump right");
                rb.AddForce(orientation.right * jumpForce * 1.5f);
            }

            //Always add forward force
            rb.AddForce(orientation.forward * jumpForce * 1f);

            //Disable dashForceCounter if doublejumping while dashing
            allowDashForceCounter = false;

        }
    }

    private void WallRunInput() //make sure to call in void Update
    {
        if ((Input.GetKey(KeyCode.D) && isWallRight) || (Input.GetKey(KeyCode.Q) && isWallLeft)) 
            StartWallrun();
        if (y == 0 && isWallRunning)
            StopWallRun();
    }
    private void StartWallrun()
    {
        Vector3 v = rb.velocity;
        if (v.y < 0) {
            v.y = 0;
            rb.velocity = v;
        }
        rb.useGravity = false;
        isWallRunning = true;
        allowDashForceCounter = false;
        capsule.material.dynamicFriction = 0.6f;
        capsule.material.staticFriction = 0.6f;

        if (rb.velocity.magnitude <= maxWallSpeed)
        {
            rb.AddForce(orientation.forward * wallrunForce * Time.deltaTime);

            //Make sure char sticks to wall
            if (isWallRight)
                rb.AddForce(orientation.right * wallrunForce / 5 * Time.deltaTime);
            else
                rb.AddForce(-orientation.right * wallrunForce / 5 * Time.deltaTime);
        }
    }
    private void StopWallRun()
    {
        if (isWallRunning) {
            capsule.material.dynamicFriction = 0;
            capsule.material.staticFriction = 0;
            isWallRunning = false;
            rb.useGravity = true;
            readyToDash = true;
            CancelInvoke(nameof(ResetWallJump));
            Invoke(nameof(ResetWallJump), jumpCooldown);
        }
    }
    private void CheckForWall() //make sure to call in void Update
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, 1f, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 1f, whatIsWall);
        //leave wall run
        if (!isWallLeft && !isWallRight) StopWallRun();
        //reset double jump (if you have one :D)
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void ResetWallJump()
    {
        readyToWallJump = true;
    }

    private void Dash()
    {
        //saves current velocity
        dashStartVector = orientation.forward;

        allowDashForceCounter = true;

        readyToDash = false;
        rb.useGravity = false;

        //Add force
        rb.velocity = Vector3.zero;
        rb.AddForce(orientation.forward * dashForce);

        Invoke("ActivateGravity", dashTime);
    }
    private void ActivateGravity()
    {
        rb.useGravity = true;
    }

   
    private float desiredX;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, wallRunCameraTilt);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);

        //While Wallrunning
        //Tilts camera in .5 second
        if (Math.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallRight)
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;
        if (Math.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallLeft)
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;

        //Tilts camera back again
        if (wallRunCameraTilt > 0 && !isWallRight && !isWallLeft)
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;
        if (wallRunCameraTilt < 0 && !isWallRight && !isWallLeft)
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;
    }
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
        
        //Limit diagonal running. This will also cause a full stop if sliding fast and un-Sliding, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                readyToDash = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }
}
