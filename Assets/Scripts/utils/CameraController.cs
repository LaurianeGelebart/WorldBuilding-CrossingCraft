using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private float _rotationX;
    private float _rotationY;

    private bool _cameraEnabled = true;
    public float sensitivity = 5f;
    public float moveSpeed = 12f;

    private Rigidbody rb;


    void Start(){
        rb = GetComponent<Rigidbody>();

        //Block the cursor in the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Lock/unlock camera movements on left click
        if(Input.GetMouseButtonDown(0)){
            _cameraEnabled = !_cameraEnabled;
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = _cameraEnabled ? CursorLockMode.Locked : CursorLockMode.None;
        }    

        if (_cameraEnabled) {
            HandleMovement();
        } else {
            rb.velocity= Vector3.zero;
        }
    }

    private void HandleMovement(){
        // //Make the camera follow the movement of my mouse
        _rotationX -=Input.GetAxis("Mouse Y")*sensitivity;
        _rotationY +=Input.GetAxis("Mouse X") *sensitivity;

        //Prevent over-rotation around X
        _rotationX = Mathf.Clamp(_rotationX,-90,90);

        transform.rotation = Quaternion.Euler(_rotationX,_rotationY,0);
        //}

        
        // //Camera movement
        Vector3 moveDirection = Vector3.zero;
        
        // Forward/Backward movement (Z/S keys)
        moveDirection += transform.forward * Input.GetAxis("Vertical") * moveSpeed;

        // Left/Right movement (Q/D keys)
        moveDirection += transform.right * Input.GetAxis("Horizontal") * moveSpeed;

        // Apply movement
        rb.velocity = moveDirection;
    }
}
