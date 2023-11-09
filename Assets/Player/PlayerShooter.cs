using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles aiming up and down shooting
/// </summary>
public class PlayerShooter : MonoBehaviour
{
    [SerializeField] Transform weaponHolster;
    [SerializeField] Transform bulletFirePoint;
    [SerializeField] Bullet bullet;

    void Start()
    {
        // This makes it so the game captures the mouse
        // Without it the mouse scrolls out the game window constantly
        // Try commenting out this line
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        Fire();
    }

    void Fire()
    {
         // Upon pressing the fire button

        // We want to Instantiate a bulletObj at the position and rotation
        // of bulletFirePoint
        // Instantiate creates a copy of a prefab and places it in the game world

        // Finally we call the Fire method on the bullet, passing in the direction
        // we want to fire it at
        if (!bullet)
            return;

        if (!Input.GetButtonDown("Fire1"))
            return;

        var bulletObj = Instantiate<Bullet>(bullet, bulletFirePoint.position, bulletFirePoint.rotation);
        bulletObj.Fire(bulletFirePoint.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        var pickup = other.GetComponent<Pickup>();
        if (pickup)
        {
            bullet = pickup.Weapon;
            Destroy(pickup.gameObject);
        }
    }
}

