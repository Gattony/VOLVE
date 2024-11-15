using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;

    private new AudioSource audio;

    //Getting audio
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }
    public void Fire()
    {
        // Instantiating a bullet prefab to mouse position
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);

        //Playing audio whenever it fires
        if (audio != null)
        {
            audio.Play();
        }
    }

}