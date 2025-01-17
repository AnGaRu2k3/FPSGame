using UnityEngine;
using Photon.Pun;

public class CharacterMovement : MonoBehaviourPun
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = -9.8f * 2;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool canMove = true;

    private Vector3 velocity;
    private bool isGrounded;

    private float movementX;
    private float movementZ;
    private bool isJumping;

    private bool startWalking = false;

    private PlayerSFXManager playerSFXManager;
    private GameObject sfxSourceHolder;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        controller.enabled = photonView.IsMine;
        animator.enabled = true;

        if (photonView.IsMine)
        {
            if (!mainCamera) mainCamera = Camera.main;
        }

        sfxSourceHolder = GameObject.Find("SFXSourceHolder");
        playerSFXManager = GetComponent<PlayerSFXManager>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();
            SyncAnimationState();
        }
        else
        {
            UpdateAnimation();
        }
    }

    private void HandleMovement()
    {

        // apply gravity
        velocity.y += gravity * Time.deltaTime;

        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // keep character on ground
        }
        controller.Move(velocity * Time.deltaTime);
        if (!canMove) return; // if player cant move like death or something

        // get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // convert to camera transform
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = mainCamera.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 move = forward * vertical + right * horizontal;

        if (move.magnitude > 0.1f)
        {
            controller.Move(move * speed * Time.deltaTime);
            sfxSourceHolder.transform.position = controller.transform.position;
            // AudioManager.instance.PlayWalk();
            playerSFXManager.PlayWalk();
        }

        // sync animation
        movementX = horizontal;
        movementZ = vertical;


        // jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
        }


        UpdateAnimation();

    }
    public void CanMove(bool status)
    {
        canMove = status;
    }
    private void SyncAnimationState()
    {
        photonView.RPC("UpdateAnimationState", RpcTarget.Others, movementX, movementZ, isJumping);
        isJumping = false;
    }

    [PunRPC]
    private void UpdateAnimationState(float x, float z, bool jumping)
    {
        movementX = x;
        movementZ = z;
        isJumping = jumping;
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("MovementX", movementX, 0.1f, Time.deltaTime);
        animator.SetFloat("MovementZ", movementZ, 0.1f, Time.deltaTime);

        animator.SetBool("Jump", isJumping);
        if (!isJumping && isGrounded)
        {
            animator.SetBool("Jump", false);
        }
    }
}
