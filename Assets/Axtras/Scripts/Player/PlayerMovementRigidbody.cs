using UnityEngine;

public class PlayerMovementRigidbody : MonoBehaviour
{
    private PlayerConsumable playerConsumable;
    private Transform groundCheck;
    private CapsuleCollider playerCollider;
    private Rigidbody rb;
    private float originalHeight;
    private bool isGrounded;

    [Header("Move Settings")]
    public bool isWalking;
    public bool isRunning;
    public bool isCrouching;
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

    private void Start() {
        playerConsumable = GetComponent<PlayerConsumable>();

        playerCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        groundCheck = transform.Find("GroundCheck");
        if (groundCheck == null) {
            Debug.LogError("GroundCheck object not found. Please add a child object named 'GroundCheck' to the player.");
        }

        originalHeight = playerCollider.height;
    }

    private void Update() {
        isRunning = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            isCrouching = !isCrouching;
            playerCollider.height = isCrouching ? crouchHeight : originalHeight;
        }

        if (isGrounded && Input.GetButtonDown("Jump") && !isCrouching) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        isWalking = move.magnitude > 0 && !isRunning && !isCrouching;

        float currentSpeed = speed;
        if (isRunning && !isCrouching) {
            currentSpeed *= runSpeedMultiplier;
        }
        else if (isCrouching) {
            currentSpeed *= crouchSpeedMultiplier;
        }

        if (playerConsumable.isConsuming) {
            currentSpeed *= playerConsumable.speedReductionMultiplier;
        }

        Vector3 moveVelocity = move * currentSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }
}
