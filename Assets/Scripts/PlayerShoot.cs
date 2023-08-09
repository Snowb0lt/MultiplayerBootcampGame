using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootSpeed;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private KeyCode shootInput;
    private Rigidbody playerRb;
    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(shootInput))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject tempBullet = Instantiate(bullet, shootPoint.position, shootPoint.rotation);

        tempBullet.GetComponent<Rigidbody>().AddForce(playerRb.velocity + tempBullet.transform.forward * shootSpeed, ForceMode.VelocityChange);
        Destroy(tempBullet, 5f);
    }
}
