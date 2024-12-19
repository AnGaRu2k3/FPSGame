using UnityEngine;
using Photon.Pun;

public class Vision : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform upperBody;
    [SerializeField] private float topClamp = -90f;
    [SerializeField] private float bottomClamp = 90f;

    private bool isCursorLocked = true;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {   
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;   
    }

    void Update()
    {

        // Toggle cursor lock state
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }

        if (isCursorLocked)
        {
            HandleMouseInput();
        }
    }

    private void HandleMouseInput()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; // Horizontal
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; // Vertical

        // Update rotation values
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);
        yRotation += mouseX;

        // Apply rotation 
        upperBody.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void ToggleCursorLock()
    {
        isCursorLocked = !isCursorLocked;
        Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isCursorLocked;
    }
}
