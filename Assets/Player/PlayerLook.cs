using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSpeed = 1.25f;
    public Transform playerBody;
    float xRotation = 0f;

    float mouseX; 
    float mouseY;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //hides the cursour when Playing the Game 

    }
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSpeed; //Inputs which Unity handles from the mouses movement
        mouseY = Input.GetAxis("Mouse Y") * mouseSpeed;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);//stops camera going full 360 acting like a neck now


        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); //rotates the Camera 
        playerBody.Rotate(Vector3.up * mouseX); //rotates the player body based on the rotation of the camera only on the horizontal Axis

    }
}
