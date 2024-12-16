using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = -9.8f * 2;
    [SerializeField] private CharacterController controller;

    [SerializeField] private bool isGrounded;
    private Vector3 velocity;

    // NetworkVariables
    [SerializeField]
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(
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
    }

    private void Update()
    {
        if (!IsOwner) return;

        isGrounded = controller.isGrounded;

        // Get input for movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        // Apply movement
        if (move.magnitude > 0.01f)
        {
            controller.Move(move * speed * Time.deltaTime);  // Move character using CharacterController
        }

        // Jumping logic
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Apply jump force
        }

        // Apply gravity
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;  // Apply gravity if not grounded
        }
        else
        {
            velocity.y = -2f;  // Small value to keep player grounded
        }

        // Apply movement with gravity and jump
        controller.Move(velocity * Time.deltaTime);

        // Update position on network
        networkPosition.Value = transform.position;
    }

    private void OnNetworkPositionChanged(Vector3 oldValue, Vector3 newValue)
    {
        if (!IsOwner)
        {
            // Update position of this player based on the networked value
            transform.position = newValue;
        }
    }

    public override void OnDestroy()
    {
        // Unsubscribe from network variable events
        networkPosition.OnValueChanged -= OnNetworkPositionChanged;

        base.OnDestroy();  // Make sure to call the base class OnDestroy
    }
}
