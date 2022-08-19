using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObject;
    public Rigidbody body;
    public Transform combatLookAt;
    public GameObject thirdPersonCamera;
    public GameObject combatCamera;
    
    public float rotationSpeed;

    public CameraStyle currentStyle;
    public enum CameraStyle {
        Basic,
        Combat
    }


    private void Start() {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }
    private void Update() {

        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            SwitchCameraStyle(CameraStyle.Basic);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            SwitchCameraStyle(CameraStyle.Combat);
        }


        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        //rotate orientation
        Vector3 viewDirection = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDirection.normalized;
        
        //rotate player 
        if(currentStyle == CameraStyle.Basic) {
            Vector3 inputDirection = orientation.forward*verticalInput + orientation.right*horizontalInput;
            if(inputDirection != Vector3.zero) {
                playerObject.forward = Vector3.Slerp(playerObject.forward, inputDirection.normalized, Time.deltaTime*rotationSpeed);
            }
        } else if (currentStyle == CameraStyle.Combat) {
            Vector3 combatLookDirection = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = combatLookDirection.normalized;
            playerObject.forward = combatLookDirection.normalized;
        }
    }
    private void SwitchCameraStyle(CameraStyle newStyle) {
        combatCamera.SetActive(false);
        thirdPersonCamera.SetActive(false);
        if(newStyle==CameraStyle.Basic) {
            thirdPersonCamera.SetActive(true);
        } else if(newStyle==CameraStyle.Combat) {
            combatCamera.SetActive(true);
        }
        currentStyle = newStyle;
    }
}
