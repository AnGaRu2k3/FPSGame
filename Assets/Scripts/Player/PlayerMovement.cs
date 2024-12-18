using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = -9.8f * 2;
    [SerializeField] private CharacterController controller;

    private bool isGrounded;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {

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

        // Apply gravity 
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;  // Apply gravity if not grounded
        }
        else
        {
            velocity.y = -2f;  // Small value to keep player grounded
        }

        // Apply jump 
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Apply jump force
        }

        // Apply movement with gravity and jump
        controller.Move(velocity * Time.deltaTime);
    }
}
