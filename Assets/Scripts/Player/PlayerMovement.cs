
using System;
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask whatIsWall;
    public float wallrunForce,maxWallrunTime, maxWallSpeed;
    bool isWallRight, isWallLeft, isWallFront, isWallBack;
    bool isWallRunning;
    public float maxWallRunCameraTilt, wallRunCameraTilt;
    
    public Transform playerCam;
    public Transform orientation;
    public Transform robotMesh;
    public Animator animator;


    private Rigidbody rb;
    private CapsuleCollider capsule;

    private float xRotation;

    public float moveSpeed = 4500;
    public float mass = 10;
    public float maxSpeed = 20;
    private float startMaxSpeed;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;
    public bool readyToSlide = true;

    public bool readyToJump = true;
    public bool readyToWallJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    public float x, y;
    bool jumping, sliding;

    public float dashForce;
    public float dashTime;
    bool allowDashForceCounter;
    public bool readyToDash;
    Vector3 dashStartVector;
    private bool cancellingGrounded;


    private Vector3 normalVector = Vector3.up;
    private float desiredX;


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
        InputManager();
        WallRunCameraTilt();
        CheckForWall();
        WallRunInput();
    }

    private void InputManager()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        sliding = Input.GetKey(KeyCode.LeftShift);
        if (grounded && !sliding)
            rb.drag = 5;
        else if (rb.drag == 5) {
            rb.drag = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && readyToSlide && (x != 0 || y != 0))
            if (!grounded && readyToDash)
                Dash();
            else if (grounded) {
                StartSlide();
                rb.drag = 0;
            }
        if (Input.GetKeyUp(KeyCode.LeftShift) && grounded) {
            StopSlide();
        }
    }

   private void StartSlide() {
         if (animator.GetInteger("SlideState") != 1)
            animator.SetInteger("SlideState", 1);
        readyToSlide = false;
        GetComponent<CapsuleCollider>().height = 1.9f;
        Vector3 center = GetComponent<CapsuleCollider>().center;
        center.y = -1f;
        GetComponent<CapsuleCollider>().center = center;
        if (rb.velocity.magnitude > 0.5f) {
                rb.AddForce(orientation.transform.forward * slideForce);
        }
        Invoke(nameof(StopSlide), 0.8f);
    }

    private void StopSlide() {
        GetComponent<CapsuleCollider>().height = 3.5f;
        Vector3 center = GetComponent<CapsuleCollider>().center;
        center.y = -0.2f;
        GetComponent<CapsuleCollider>().center = center;
        readyToSlide = true;
        animator.SetInteger("SlideState", 2);
    }


    IEnumerator RotateMe(Vector3 byAngles, float inTime, float targetAngle) {
        Debug.Log(byAngles);
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for(var t = 0f; t < 1; t += Time.deltaTime/inTime) {
            transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    public void rotatePlayer(float angle) {
        if (angle != 180 && angle != 0 && transform.eulerAngles.y > 90 &&  transform.eulerAngles.y < 270)
            angle = -angle;
        StartCoroutine(RotateMe(Vector3.forward * (((((angle - transform.eulerAngles.z) % 360) + 540) % 360) - 180), 1, angle));
    }

    private void Movement()
    {
        rb.AddForce(-transform.up * Time.deltaTime * 25);
        
        Vector2 mag = FindVelRelativeToLook();

        float xMag = mag.x, yMag = mag.y;
        Vector3 v = rb.velocity;
        if (v.y > (Mathf.Round(transform.eulerAngles.z) == 180 ? (startMaxSpeed * 1.8f) : startMaxSpeed))
            v.y = startMaxSpeed;
        if (v.y < (Mathf.Round(transform.eulerAngles.z) == 0 ? -(startMaxSpeed * 2f) : -startMaxSpeed))
            v.y = -startMaxSpeed;
        if (v.x > (Mathf.Round(transform.eulerAngles.z) == 90 ? (startMaxSpeed * 2f) : startMaxSpeed))
            v.x = startMaxSpeed;
        if (v.x < (Mathf.Round(transform.eulerAngles.z) == -90 ? -(startMaxSpeed * 2f) : -startMaxSpeed))
            v.x = -startMaxSpeed;
        if (v.z > startMaxSpeed)
            v.z = startMaxSpeed;
        if (v.z < -startMaxSpeed)
            v.z = -startMaxSpeed;
        rb.velocity = v;
        if (y > 0 && grounded)
            animator.SetBool("IsRunning", true);
        else if (y <= 0 || animator.GetBool("IsRunning") || !grounded)
            animator.SetBool("IsRunning", false);

        CounterMovement(x, y, mag);
 
        if ((readyToJump || readyToWallJump) && jumping) Jump();

        float maxSpeed = this.maxSpeed;

        if ((x > 0 && xMag > maxSpeed) || (x < 0 && xMag < -maxSpeed)) x = 0;
        if ((y > 0 && yMag > maxSpeed ) || (y < 0 && yMag < -maxSpeed)) y = 0;

        float multiplier = 1f, multiplierV = 1f;
        
        if (!grounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }
 
        if (grounded && !readyToSlide) multiplierV = 0f;

        if (((!isWallFront && y > 0) || (!isWallBack && y < 0)))
            rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        if (!isWallRunning && ((!isWallRight && x > 0) || (!isWallLeft && x < 0))) {
            rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
        }
    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;
            rb.AddForce(transform.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
          
            rb.velocity = Vector3.zero;

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (isWallRunning && readyToWallJump)
        {
            readyToWallJump = false;

            if (isWallLeft && !Input.GetKey(KeyCode.D) || isWallRight && !Input.GetKey(KeyCode.Q))
            {
                rb.AddForce(transform.up * 650 * 1.4f);
                rb.AddForce(normalVector * 650 * 0.5f);
            } else {
                rb.AddForce(transform.up * 650 * 1.4f);
                rb.AddForce(normalVector * 650 * 0.5f);
            }

            if ((isWallRight || isWallLeft) && (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.D))) rb.AddForce(-orientation.up * 650 * 1f);
            if (isWallRight && Input.GetKey(KeyCode.Q)) {
                rb.AddForce(-orientation.right * 650 * 1.6f);
            }
            if (isWallLeft && Input.GetKey(KeyCode.D))  {
                rb.AddForce(orientation.right * 650 * 1.6f);
            }

            rb.AddForce(orientation.forward * 650 * 1f);

            allowDashForceCounter = false;
        }
    }

    private void WallRunInput()
    {
        if (Input.GetKey(KeyCode.Z) && ((isWallRight) || (isWallLeft))) 
            StartWallrun();
        if (isWallRunning && ((!isWallRight && !isWallLeft) || y == 0))
            StopWallRun();
    }

    private void StartWallrun()
    {
        Vector3 v = rb.velocity;
        if ((Mathf.Round(transform.eulerAngles.z) == 180 || (Mathf.Round(transform.eulerAngles.z) == 0)) && v.y < 0)
            v.y = 0;
        else if ((Mathf.Round(transform.eulerAngles.z) == -90 || (Mathf.Round(transform.eulerAngles.z) == 90)) && v.x < 0)
            v.x = 0;
        rb.velocity = v;
        rb.useGravity = false;
        isWallRunning = true;
        allowDashForceCounter = false;
        capsule.material.dynamicFriction = 0.6f;
        capsule.material.staticFriction = 0.6f;

        if (rb.velocity.magnitude <= maxWallSpeed)
        {
            rb.AddForce(orientation.forward * wallrunForce * Time.deltaTime);

            if (isWallRight && isWallRunning)
                rb.AddForce(orientation.right * wallrunForce / 3 * Time.deltaTime);
            else if (isWallLeft && isWallRunning)
                rb.AddForce(-orientation.right * wallrunForce / 3 * Time.deltaTime);
        }
    }
    private void StopWallRun()
    {
        capsule.material.dynamicFriction = 0;
        capsule.material.staticFriction = 0;
        isWallRunning = false;
        rb.useGravity = true;
        readyToDash = true;
        CancelInvoke(nameof(ResetWallJump));
        Invoke(nameof(ResetWallJump), jumpCooldown);
    }
    private void CheckForWall()
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, 1f, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 1f, whatIsWall);
        isWallFront = Physics.Raycast(transform.position, orientation.forward, 1f, whatIsWall);
        isWallBack = Physics.Raycast(transform.position, -orientation.forward, 1f, whatIsWall);


        if (!isWallLeft && !isWallRight && isWallRunning) 
            StopWallRun();
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
        dashStartVector = orientation.forward;

        allowDashForceCounter = true;

        readyToDash = false;
        rb.useGravity = false;

        rb.velocity = Vector3.zero;
        rb.AddForce(orientation.forward * dashForce);

        Invoke("ActivateGravity", dashTime);
    }



    private void ActivateGravity()
    {
        rb.useGravity = true;
    }

    private void WallRunCameraTilt()
    {
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, wallRunCameraTilt);

        if (Math.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallRight)
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;
        if (Math.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallLeft)
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;

        if (wallRunCameraTilt > 0 && !isWallRight && !isWallLeft)
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;
        if (wallRunCameraTilt < 0 && !isWallRight && !isWallLeft)
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;
    }
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = transform.eulerAngles.y;
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

    private void OnCollisionStay(Collision other)
    {
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            grounded = true;
            readyToDash = true;
            cancellingGrounded = false;
            normalVector = normal;
            CancelInvoke(nameof(StopGrounded));
        }

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
