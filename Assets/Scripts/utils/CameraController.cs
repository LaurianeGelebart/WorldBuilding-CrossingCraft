using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private float _rotationX;
    private float _rotationY;
    public float sensitivity = 5f;
    public float moveSpeed = 12f;

    // Update is called once per frame
    void Update()
    {
        // //Make the camera follow the movement of my mouse
        //if(!IsMouseOffScreen()){
        _rotationX -=Input.GetAxis("Mouse Y")*sensitivity;
        _rotationY +=Input.GetAxis("Mouse X") *sensitivity;

        //Prevent over-rotation around X
        _rotationX = Mathf.Clamp(_rotationX,-90,90);

        transform.rotation = Quaternion.Euler(_rotationX,_rotationY,0);
        //}


        // //Camera movement
        Vector3 moveDirection = Vector3.zero;
        
        // Forward/Backward movement (Z/S keys)
        moveDirection += transform.forward * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // Left/Right movement (Q/D keys)
        moveDirection += transform.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        // Apply movement
        transform.position += moveDirection;
    }

    // //Uncomment if you want the camera to stop following the movement of your mouse once the cursor is out the game screen
    // private bool IsMouseOffScreen()
    // {
    //     if(Input.mousePosition.x <= 2 || 
    //     Input.mousePosition.y <= 2 || 
    //     Input.mousePosition.x >= Screen.width-2 ||
    //     Input.mousePosition.y >= Screen.height -2){
    //         return true;
    //     }else{
    //         return false;
    //     }
    // }
}
