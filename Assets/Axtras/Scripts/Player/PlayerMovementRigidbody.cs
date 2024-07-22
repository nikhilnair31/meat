using UnityEngine;

public class PlayerMovementRigidbody : MonoBehaviour
{
    private Transform groundCheck;
    private CapsuleCollider playerCollider;
    private Animator playerAnimator;
    private float originalHeight;
    private bool isGrounded;
    private bool isRunning;
    private bool isCrouching;

    [Header("Move Settings")]
    public Rigidbody rb;
    [SerializeField] private float speed = 12f;

    [Header("Run Settings")]
    [SerializeField] private float runSpeedMultiplier = 1.5f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private float crouchHeight = 1f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Settings")]
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Consuming Settings")]
    public bool isConsuming = false;
    public float speedReductionMultiplier = 1f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponentInChildren<Animator>();

        if (TryGetComponent<CapsuleCollider>(out playerCollider)) {
            originalHeight = playerCollider.height;
        }

        groundCheck = transform.Find("GroundCheck");
        if (groundCheck == null) {
            Debug.LogError("GroundCheck object not found. Please add a child object named 'GroundCheck' to the player.");
        }
    }

    private void Update() {
        isRunning = Input.GetKey(KeyCode.LeftShift);
        playerAnimator.SetBool("isRunning", isRunning);

        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            isCrouching = !isCrouching;
            playerAnimator.SetBool("isCrouching", isCrouching);
            playerCollider.height = isCrouching ? crouchHeight : originalHeight;
        }

        if (isGrounded && Input.GetButtonDown("Jump") && !isCrouching) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        playerAnimator.SetBool("isGrounded", isGrounded);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float currentSpeed = speed;
        if (isRunning && !isCrouching) {
            currentSpeed *= runSpeedMultiplier;
        }
        else if (isCrouching) {
            currentSpeed *= crouchSpeedMultiplier;
        }

        if (isConsuming) {
            currentSpeed *= speedReductionMultiplier;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        Vector3 moveVelocity = move * currentSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        playerAnimator.SetFloat("moveVelocity", moveVelocity.magnitude);
    }
}
