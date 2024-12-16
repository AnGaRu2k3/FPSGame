using UnityEngine;
using Unity.Netcode;

public class Vision : NetworkBehaviour
{
    [SerializeField] private float mouseSensitive = 100f;
    [SerializeField] private Transform UpperBody;
    [SerializeField] private float topClamp = -90f;
    [SerializeField] private float bottomClamp = 90f;

    [SerializeField] private NetworkVariable<float> xRota = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private NetworkVariable<float> yRota = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private bool isCursorLocked = true;

    private void Start()
    {
        if (IsOwner)
        {
            // Lock the mouse only for the owning player
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Register callbacks for changes to NetworkVariables
        xRota.OnValueChanged += OnXRotationChanged;
        yRota.OnValueChanged += OnYRotationChanged;
    }

    private void Update()
    {
        if (!IsOwner) return; // Only owner can control vision

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
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitive * Time.deltaTime; // Horizontal
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitive * Time.deltaTime; // Vertical

        // Update rotation values
        xRota.Value -= mouseY;
        xRota.Value = Mathf.Clamp(xRota.Value, topClamp, bottomClamp);
        yRota.Value += mouseX;

        // Apply rotation to the owning player
        UpperBody.localRotation = Quaternion.Euler(xRota.Value, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, yRota.Value, 0f);
    }

    private void ToggleCursorLock()
    {
        isCursorLocked = !isCursorLocked;
        Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isCursorLocked;
    }

    private void OnXRotationChanged(float oldValue, float newValue)
    {
        // Apply new rotation for non-owner clients
        UpperBody.localRotation = Quaternion.Euler(newValue, 0f, 0f);
    }

    private void OnYRotationChanged(float oldValue, float newValue)
    {
        // Apply new rotation for non-owner clients
        transform.rotation = Quaternion.Euler(0f, newValue, 0f);
    }
    public override void OnDestroy()
    {
        // Unregister callbacks to prevent memory leaks
        xRota.OnValueChanged -= OnXRotationChanged;
        yRota.OnValueChanged -= OnYRotationChanged;

        base.OnDestroy();
    }
}
