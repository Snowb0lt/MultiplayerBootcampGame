using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float playerSpeed = 10f;
    [SerializeField] private float playerTurnSpeed = 10f;

    private Rigidbody playerRb;
    private float horizontal;
    private float vertical;

    [SerializeField] private string horizontalInput;
    [SerializeField] private string verticalInput;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerRb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (!IsOwner) return;

        horizontal = Input.GetAxis(horizontalInput);
        vertical = Input.GetAxis(verticalInput);
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;

        playerRb.velocity = playerRb.transform.forward * playerSpeed * vertical;
        playerRb.rotation = Quaternion.Euler(transform.eulerAngles + transform.up * horizontal * playerTurnSpeed);
    }
}
