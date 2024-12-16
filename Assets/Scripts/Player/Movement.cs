using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.8f * 2;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private bool isGrounded;

    [SerializeField] private CharacterController controller;

    // NetworkVariables
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector3> networkVelocity = new NetworkVariable<Vector3>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (IsServer)
        {
            networkPosition.Value = transform.position;
        }

        // Add callback for sync
        networkPosition.OnValueChanged += OnNetworkPositionChanged;
        networkVelocity.OnValueChanged += OnNetworkVelocityChanged;
    }

    private void Update()
    {
        if (!IsOwner) return;

        isGrounded = controller.isGrounded;

        // Get input 
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 0.01f)
        {
            controller.Move(move * speed * Time.deltaTime);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Each 10 frame update the network variables
        if (Time.frameCount % 5 == 0)
        {
            networkPosition.Value = transform.position;
            networkVelocity.Value = velocity;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        // If the player on the ground
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1f; // Keep the character grounded
        }

        // Apply gravity
        velocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    private void OnNetworkPositionChanged(Vector3 oldValue, Vector3 newValue)
    {
        if (!IsOwner)
        {
            transform.position = newValue;
        }
    }

    private void OnNetworkVelocityChanged(Vector3 oldValue, Vector3 newValue)
    {
        if (!IsOwner)
        {
            velocity = newValue;
            controller.Move(velocity * Time.deltaTime);
        }
    }
    public override void OnDestroy()
    {
        // Unsubscribe from network variable events
        networkPosition.OnValueChanged -= OnNetworkPositionChanged;
        networkVelocity.OnValueChanged -= OnNetworkVelocityChanged;

        base.OnDestroy();  // Make sure to call the base class OnDestroy
    }
}
