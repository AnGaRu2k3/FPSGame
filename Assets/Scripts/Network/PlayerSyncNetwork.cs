using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkSync : NetworkBehaviour
{
    // Singleton 
    public static PlayerNetworkSync Instance { get; private set; }

    public NetworkVariable<Vector3> NetworkPosition = new NetworkVariable<Vector3>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<Vector3> NetworkVelocity = new NetworkVariable<Vector3>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> NetworkXRotation = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> NetworkYRotation = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        if (IsOwner)
        {
            NetworkPosition.Value = newPosition;
        }
    }

    public void UpdateVelocity(Vector3 newVelocity)
    {
        if (IsOwner)
        {
            NetworkVelocity.Value = newVelocity;
        }
    }

    public void UpdateRotation(float xRotation, float yRotation)
    {
        if (IsOwner)
        {
            NetworkXRotation.Value = xRotation;
            NetworkYRotation.Value = yRotation;
        }
    }
}
