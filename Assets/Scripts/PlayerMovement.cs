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

        if (IsServer && IsLocalPlayer)
        {
            if (GameManager.Instance._state.Value == 1)
            {
                this.horizontal = Input.GetAxis(horizontalInput);
                this.vertical = Input.GetAxis(verticalInput);
            }
            else
            {
                this.horizontal = 0;
                this.vertical = 0;
            }
        }
        else if(IsClient && IsLocalPlayer)
        {
            Debug.Log("Calling RPC");
            MovementServerRPC(Input.GetAxis(horizontalInput), Input.GetAxis(verticalInput));
        }

    }

    [ServerRpc]
    public void MovementServerRPC(float horizontal, float vertical)
    {
        if (GameManager.Instance._state.Value == 1)
        {
            this.horizontal = horizontal;
            this.vertical = vertical;
        }
        else
        {
            this.horizontal = 0;
            this.vertical = 0;
        }

    }

    private void FixedUpdate()
    {
        playerRb.velocity = playerRb.transform.forward * playerSpeed * vertical;
        playerRb.rotation = Quaternion.Euler(transform.eulerAngles + transform.up * horizontal * playerTurnSpeed);
    }
}
