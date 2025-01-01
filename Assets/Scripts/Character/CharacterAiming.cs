using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CharacterAiming : MonoBehaviourPun
{
    [SerializeField] float turnSpeed;
    [SerializeField] Camera mainCamera;
    void Start()
    {
        if (photonView.IsMine)
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yawCamera, 0);
    }
}
