// using System.ComponentModel;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerMovement : MonoBehaviour {

// // ------------------  VARIABLES  --------------------
//     public float playerHeight = 2f;
//     public Transform orientation;
//     public Transform playerObject;
//     [Header("References")]
//     // private PlayerCamera camera;

//     [Header("Movement")]
//     public float movementSpeed; // 12
//     public float crouchMaxSpeed; // 2
//     public float walkMaxSpeed; // 4
//     public float sprintMaxSpeed; // 7
//     public float slideMaxSpeed; //
//     public float airMaxSpeed; // 7
//     public float maxSpeed; // will change by state
//     public float airMultiplier; // 0.4
//     public float speedIncreaseMultiplier;
//     public float slopeIncreaseMultiplier;
//     public float groundDrag; // 5
//     //private float desiredMovementSpeed;
//     //private float lastDesiredMovementSpeed;
//     [HideInInspector] public float maxYSpeed;

//     [Header("Jumping")]
//     public float jumpForce; //13
//     public float jumpCooldown; //0.25
//     private bool isReadyToJump;
//     public int doubleJumps = 1;
//     private int doubleJumpsLeft;

//     [Header("Crounching")]
//     public float crouchYScale;
//     private float startYScale;

//     [Header("Keybinds")]
//     public KeyCode jumpKey = KeyCode.Space;
//     public KeyCode sprintKey = KeyCode.LeftShift;
//     public KeyCode crouchKey = KeyCode.LeftAlt;

//     [Header("Ground Check")]
//     public LayerMask whatIsGround;
//     public Transform groundCheck;
//     public float groundCheckRadius = 0.2f;
//     [HideInInspector] public bool isGrounded;

//     [Header("Slope Handler")]
//     public float maxSlopeAngle = 44f;
//     private RaycastHit slopeHit;
//     //private bool isOnNonSprintableSlope; 
//     //private bool isExitingSlope;

//     [HideInInspector] public float horizontalInput;
//     [HideInInspector] public float verticalInput;
//     private Vector3 moveDirection;
//     private Vector3 spawnSpot;
//     private Rigidbody body;

//     // state variable will store the current state.
//     // will use these in StateHandler();
//     [HideInInspector] public MovementState state;
//     public enum MovementState {
//         unlimited, // when with no speed restrictions
//         restricted, // no xz movement allowed
//         freeze,
//         walking,
//         sprinting,
//         air,
//         crouching,
//         sliding
//     }
//     [HideInInspector] public bool isFreezed;
//     [HideInInspector] public bool isUnlimited;
//     [HideInInspector] public bool isRestricted;
//     [HideInInspector] public bool isWalking;
//     [HideInInspector] public bool isSprinting;
//     [HideInInspector] public bool isCrouching;
//     [HideInInspector] public bool isSliding;

//     private void Start() {
//         body = GetComponent<Rigidbody>();
//         camera = GetComponent<PlayerCamera>();
//         body.freezeRotation = true;
//         isReadyToJump = true;
//         maxYSpeed = -1; //only for climbing and wallruns. will add it
//         startYScale = transform.localScale.y;
//         spawnSpot = transform.position;
//         state = MovementState.walking;
        
//         if (whatIsGround.value == 0) {
//             whatIsGround = LayerMask.GetMask("Default");
//         }
//     }

//     private void Update() {
//         // keys input
//         MyInput();
//         // speed limit
//         // movementSpeed handler
//         StateHandler();
//         // 
//         LimitVelocity();
//         // ground checker
//         isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight*0.5f + 0.2f, whatIsGround);
//         // Drag handler
//         DragHandle();
//         // spawning
//         if (transform.position.y < -45f) {
//             Respawn();
//         }
//         if (isGrounded && doubleJumpsLeft != doubleJumps) {
//             ResetDoubleJumps();
//         }
//     }
//     private void FixedUpdate() {
//         if ((state==MovementState.walking || state==MovementState.sprinting || state==MovementState.crouching || state==MovementState.air || state==MovementState.sliding) &&
//         state != MovementState.restricted) {
//             MovePlayer();
//         } else {
//             LimitVelocity();
//         }
//     }

//     private void Respawn() {
//         transform.position = spawnSpot;
//     }

// // ------------------  INPUTS  --------------------

//     private void MyInput() {
//         // verify there is floor in order to enable movement
//         if (isGrounded || isSecondJumping) {
//             horizontalInput = Input.GetAxisRaw("Horizontal");
//             verticalInput = Input.GetAxisRaw("Vertical");
//         }

//         // verify jump conditions + jump
//         if (Input.GetKey(jumpKey) && isReadyToJump && isGrounded) {
//             isReadyToJump = false;
//             Jump();
//             Invoke(nameof(ResetJump), jumpCooldown);
//         } else if (Input.GetKey(jumpKey) && isReadyToJump && state == MovementState.air) {
//             DoubleJump();
//         }

//         // start crouch
//         if (Input.GetKeyDown(crouchKey) && (state==MovementState.walking || state==MovementState.air)) {
//             StartCrouch();
//         }
//         // stop crouch
//         if (Input.GetKeyUp(crouchKey) && isCrouching) {
//             StopCrouch();
//         }
//         isSprinting = Input.GetKey(sprintKey);
//     }
//     // called when walking, sprinting, crouching and onAir
//     private void MovePlayer() {
//         // calculation of movement direction
//         if (isGrounded || isOnSlope() || isSecondJumping) {
//             //necesito isOnSlope el if? lo siento raro, es un booleano nomas, probar slopes sin el if
//             moveDirection = orientation.forward*verticalInput + orientation.right*horizontalInput;   
//         }

//         // on ground
//         if(isGrounded) {
//             body.AddForce(moveDirection.normalized*movementSpeed*10f, ForceMode.Force);
//         }
//         // on slope
//         if(isOnSlope()) {
//             body.AddForce(GetSlopeMovementDirection()*movementSpeed*10f, ForceMode.Force);
//             if (!isJumping && body.velocity.y > 0) {
//                 body.AddForce(Vector3.down*80f, ForceMode.Force);
//             }
//         }
//         // on air
//         else if (!isGrounded) {
//             body.AddForce(moveDirection.normalized*movementSpeed*10f*airMultiplier, ForceMode.Force);
//         }
        
//         if (isSliding) {
//             body.AddForce(moveDirection.normalized*12f, ForceMode.Force);
//         }
//     }

//     private void LimitVelocity() {
//         //get xz speed
//         Vector3 bodyFlatSpeed = new Vector3(body.velocity.x, 0f, body.velocity.z);
//         // get y speed
//         float currentYSpeed = body.velocity.y;
//         // limit if excess flat speed
//         if (bodyFlatSpeed.magnitude > maxSpeed) {
//             // get excessed speed vector
//             Vector3 limitedFlatSpeed = bodyFlatSpeed.normalized*maxSpeed;
//             // apply max speed optimal vector
//             body.velocity = new Vector3(limitedFlatSpeed.x, body.velocity.y, limitedFlatSpeed.z);
//         }
//         // limit vertical speed excess
//         if (maxYSpeed != -1 && currentYSpeed > maxYSpeed) {
//             body.velocity = new Vector3(body.velocity.x, maxYSpeed, body.velocity.z);
//         }
//     }

//     // will validate movement state and assign its corresponding speed
//     private void StateHandler() {
//         // freezed
//         if (isFreezed) {
//             state = MovementState.freeze;
//             maxSpeed = 0f;
//             body.velocity = Vector3.zero;
//         }
//         // unlimited speed
//         else if (isUnlimited) {
//             state = MovementState.unlimited;
//             maxSpeed = Mathf.Infinity;
//         }
//         // restricted
//         else if (isRestricted) {
//             state = MovementState.restricted;
//             maxSpeed = sprintMaxSpeed;
//         }
//         // sliding
//         else if (isSliding) {
//             state = MovementState.sliding;
//             maxSpeed = slideMaxSpeed;
//         }
//         // crouch mode
//         else if (isCrouching && isGrounded) {
//             state = MovementState.crouching;
//             maxSpeed = crouchMaxSpeed;
//         } 
//         // sprinting mode
//         if (isGrounded && isSprinting) {
//             state = MovementState.sprinting;
//             maxSpeed = sprintMaxSpeed;
//         } 
//         // walking mode
//         else if (isGrounded) {
//             state = MovementState.walking;
//             maxSpeed = walkMaxSpeed;
//         }
//         // air mode
//         else {
//             state = MovementState.air;
//             if (isWalking) {
//                 maxSpeed = walkMaxSpeed;
//             } else if (isSprinting) {
//                 maxSpeed = sprintMaxSpeed;
//             } else if (isSliding) {
//                 maxSpeed = slideMaxSpeed;
//             }
//         }
//     }

//     private void DragHandle() {
//         if(state == MovementState.walking || state == MovementState.sprinting) {
//             body.drag = groundDrag;
//         } else {
//             body.drag = 0;
//         }
//     }
//     // jumping code
//     private void ResetJump() {
//         isReadyToJump = true;
//     }
//     public void ResetDoubleJumps() {
//         doubleJumpsLeft = doubleJumps;
//         isSecondJumping = false;
//     }
//     public void Jump() {
//         isJumping = true;
//         // y-velocity set to 0 to maintain same jump height always
//         body.velocity = new Vector3(body.velocity.x, 0f, body.velocity.z);
//         body.AddForce(orientation.up*jumpForce, ForceMode.Impulse);
//     }
//     public void DoubleJump() {
//         isJumping = false;
//         isSecondJumping = true;
//         if (doubleJumpsLeft <= 0) {
//             return;
//         }
//         Vector3 flatSpeed = new Vector3(body.velocity.x, 0f, body.velocity.z);
//         body.velocity = orientation.forward*flatSpeed.magnitude;
//         body.AddForce(orientation.up*jumpForce, ForceMode.Impulse);
//         doubleJumpsLeft--;
//     }
//     private void StartCrouch() {
//         state = MovementState.crouching;
//         playerObject.localScale = new Vector3(playerObject.localScale.x, crouchYScale, playerObject.localScale.z);
//         body.AddForce(Vector3.down*10f, ForceMode.Impulse);
//         isCrouching = true;
//     }
//     private void StopCrouch() {
//         playerObject.localScale = new Vector3(playerObject.localScale.x, startYScale, playerObject.localScale.z);
//         isCrouching = false;
//     }

//     // slope verifications
//     public bool isOnSlope() {
//         if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight*0.5f + 0.5f,whatIsGround)) {
//             float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
//             return angle < maxSlopeAngle;
//         } else {
//             return false;
//         }
//     }

//     public Vector3 GetSlopeMovementDirection() {
//         return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
//     }

// }
