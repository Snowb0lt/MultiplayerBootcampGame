using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 10f;
    [SerializeField] private float playerTurnSpeed = 10f;

    private Rigidbody playerRb;
    private float horizontal;
    private float vertical;

    [SerializeField] private string horizontalInput;
    [SerializeField] private string verticalInput;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        horizontal = Input.GetAxis(horizontalInput);
        vertical = Input.GetAxis(verticalInput);
    }

    private void FixedUpdate()
    {
        playerRb.velocity = playerRb.transform.forward * playerSpeed * vertical;
        playerRb.rotation = Quaternion.Euler(transform.eulerAngles + transform.up * horizontal * playerTurnSpeed);
    }
}
