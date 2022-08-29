using UnityEngine;
using UnityEngine.InputSystem;

namespace MyGame
{

    //[RequireComponent(typeof(CharacterController))]
    //[RequireComponent(typeof(PlayerInput))]

    public class PlayerMovement : MonoBehaviour
    {

        [Header("Player")]
        [Tooltip("Walking speed")]
        public float moveSpeed = 2.0f;

        [Tooltip("Sprinting speed")]
        public float sprintSpeed = 6f;

        [Tooltip("Smoothen rotation times this ")]
        [Range(0.0f, 0.3f)]
        public float rotationSmoothTime = 0.12f;

        [Tooltip("Acceleration & deceleration")]
        public float speedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("Jumping height")]
        public float jumpHeight = 1.2f;

        [Tooltip("PLAYER'S gravity. Unity handles -9.81f")]
        public float gravity = -15.0f;

        [Space(10)]
        [Tooltip("Jumping cooldown")]
        public float jumpCooldown = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float fallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool isGrounded = true;

        [Tooltip("Useful for rough ground")]
        public float groundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundedRadius = 0.21f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask ground;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float topClamp = 70.0f;

        [Tooltip("How low in degrees can you move the camera down")]
        public float bottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float cameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool lockCameraPosition = false;
        public Vector2 inputDirection = new Vector2();
        //[HideInInspector] public float horizontalInput;
        //[HideInInspector] public float verticalInput;

        // cinemachine
        private float _cinemachineTargetHorizontal;
        private float _cinemachineTargetVertical;

        // player
        public float _speed;
        private float targetSpeed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float targetAngle = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _velocityLimit = 53.0f;
        private bool isReadyToJump = true;
        private bool isJumping = false;
        public bool isSprinting = false;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        //private ThirdPersonActionAsset playerActionAsset;
        //private InputAction move;
        //private InputActions inputSystem;
        private InputActions input;
        private InputAction look;
        private Animator _animator;
        private CharacterController _controller;
        private Transform _mainCamera;
        public Transform camera;
        public float turnSmoothTime = 0.1f;
        float turnSmoothVelocity;

        [SerializeField]
        private HealthManager healthManager;

        private Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        private Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        private const float _threshold = 0.01f;

        private const float speedOffset = 0.1f;

        private bool _hasAnimator;


        //private Vector2 lookAt;
        //private Vector3 mousePosition;
        //private Vector3 mouseDelta;

        [Header("Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode sprintKey = KeyCode.LeftShift;

        private void Awake()
        {
            _mainCamera = Camera.main.transform;
            //playerActionAsset = new ThirdPersonActionAsset();
            input = new InputActions();
        }
        private void OnEnable() {
            //    playerActionAsset.Player.Jump.started += DoJump;
            //    playerActionAsset.Player.Sprint.started += DoSprint;
            //    playerActionAsset.Player.Sprint.canceled += DoSprint;
            //    move = playerActionAsset.Player.Move;
            input.Player.Enable();
            look = input.Player.Look;
        }
        private void OnDisable() {
            //    playerActionAsset.Player.Jump.started -= DoJump;
            //    playerActionAsset.Player.Sprint.started -= DoSprint;
            //    playerActionAsset.Player.Sprint.canceled -= DoSprint;
            input.Player.Disable();
        }

        private void Start()
        {
            //mousePosition = Input.mousePosition;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _cinemachineTargetHorizontal = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            //inputSystem = GetComponent<InputActions>();
            //input = GetComponent<Input>();

            AssignAnimationIDs();

            // reset timeouts on start
            _fallTimeoutDelta = fallTimeout;
        }

        private void Update()
        {
            Vector2 look = input.Player.Look.ReadValue<Vector2>();
            if (PauseManager.isPaused) return;
            GroundedCheck();
            MyInput();
            Move();
            Jump();
        }

        private void LateUpdate() {
            CameraRotation();
        }
        private void FixedUpdate()
        {
            //Move();
            //aca me quiero encargar de setear los estados segun los valores de las variables y mis booleanos
        }

        private void MyInput()
        {
            if (Input.GetKey(jumpKey) && isGrounded)
            {
                isJumping = true;
            }
            isSprinting = Input.GetKey(sprintKey);
        }

        private void Move()
        {
            float horizontalAxis = Input.GetAxisRaw("Horizontal");
            float verticalAxis = Input.GetAxisRaw("Vertical");
            // dont lose speed inair if stop sprint or if wind runs out
            if (isGrounded)
            {
                targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
            }
            Vector2 inputDirection = new Vector2(horizontalAxis, verticalAxis);
            if (inputDirection == Vector2.zero) targetSpeed = 0.0f;
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float inputMagnitude = inputDirection.magnitude;
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                //creates curved result giving more organic speed change
                //note T in Lerp is clamped, so we don't need to clamp speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f)
            {
                _animationBlend = 0f;
            }
            //if (verticalAxis == 0f)
            //{
            //    Debug.Log("vertical cero");
            //    targetAngle = Mathf.Atan2(inputDirection.x, 0) * Mathf.Rad2Deg;
            //}
            //else 
            if (inputDirection != Vector2.zero) {
                targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + camera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
            //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            //if (inputDirection != Vector2.zero) {
            //    targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + camera.eulerAngles.y;
            //    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //    transform.rotation = Quaternion.Euler(0f, angle, 0f);
            // }
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 motion = moveDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;
            _controller.Move(motion);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void CameraRotation() {
            //    //float horizontalInput = Input.GetAxisRaw("Horizontal");
            //    //float verticalInput = Input.GetAxisRaw("Vertical");
            //    //Vector2 lookAt = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 lookAt = look.ReadValue<Vector2>();
            //    // if there is an input and camera position is not fixed
            if (lookAt.sqrMagnitude >= _threshold && !lockCameraPosition) {
            //        // REMINDER: Don't multiply mouse input by Time.deltaTime. Units stuff;
            //        Debug.Log("ESTA LLEGANDO ALGO");
                float deltaTimeMultiplier = 1.0f;

                _cinemachineTargetHorizontal += lookAt.x * deltaTimeMultiplier;
                _cinemachineTargetVertical += lookAt.y * deltaTimeMultiplier;
            }
            //    // clamp  rotation so values are limited 360 degrees
            _cinemachineTargetHorizontal = ClampAngle(_cinemachineTargetHorizontal, float.MinValue, float.MaxValue);
            _cinemachineTargetVertical = ClampAngle(_cinemachineTargetVertical, bottomClamp, topClamp);
            //    // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetVertical + cameraAngleOverride,
            _cinemachineTargetHorizontal, 0.0f);
        }


            public void Jump()
        {
            if (isGrounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = fallTimeout;
                // update animator to base state
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }
                // stop velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (isJumping && isReadyToJump)
                {
                    isJumping = false;
                    isReadyToJump = false;
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    Invoke(nameof(ResetJump), jumpCooldown);
                    // update animator to jump
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                    healthManager.Health(-15);
                }
            }
            else
            {
                // update animator to freefall anim
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }
            // apply gravity over time
            //(multiply by delta time twice to linearly speed up over time, but...
            //... would have to set certain limit so it doesnt fly away uncontrolled)
            if (_verticalVelocity < _velocityLimit)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        private void ResetJump()
        {
            isJumping = false;
            isReadyToJump = true;
        }
        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
                transform.position.z);
            isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, ground,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, isGrounded);
            }
        }
        //private void DoSprint(InputAction.CallbackContext value) {
        //    isSprinting = value.started;
        //}
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        private void OnDrawGizmosSelected()
        {
            if (isGrounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;
            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
                groundedRadius);
        }
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }
        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}





//private void Move() {
//    // set target speed based on move and sprint speed
//    float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
//    Vector2 movement = move.ReadValue<Vector2>();
//    // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

//    // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
//    // if there is no input, set the target speed to 0
//    if (movement == Vector2.zero) targetSpeed = 0.0f;

//    // a reference to the players current horizontal velocity
//    float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
//    float inputMagnitude = movement.magnitude;

//    // accelerate or decelerate to target speed
//    if (currentHorizontalSpeed < targetSpeed - speedOffset ||
//        currentHorizontalSpeed > targetSpeed + speedOffset) {
//        // creates curved result rather than a linear one giving a more organic speed change
//        // note T in Lerp is clamped, so we don't need to clamp our speed
//        _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
//            Time.deltaTime * speedChangeRate);

//        // round speed to 3 decimal places
//        _speed = Mathf.Round(_speed * 1000f) / 1000f;
//    } else {
//        _speed = targetSpeed;
//    }

//    _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);

//    if (_animationBlend < 0.01f) {
//        _animationBlend = 0f;
//    }
//    // normalise input direction
//    Vector3 inputDirection = new Vector3(movement.x, 0.0f, movement.y).normalized;
//    // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
//    // if there is a move input rotate player when the player is moving
//    if (movement != Vector2.zero) {
//        _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
//        _mainCamera.eulerAngles.y;
//        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
//            rotationSmoothTime);

//        // rotate to face input direction relative to camera position
//        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
//    }

//    Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
//    Vector3 motion = targetDirection.normalized * (_speed * Time.deltaTime) +
//                     new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;
//    // move the player
//    //motion = AdjustVelocityToSlope(motion);
//    _controller.Move(motion);
//    // update animator if using character
//    if (_hasAnimator) {
//        _animator.SetFloat(_animIDSpeed, _animationBlend);
//        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
//    }
//}

//private Vector3 AdjustVelocityToSlope(Vector3 motion) {
//    var ray = new Ray(transform.position, Vector3.down);
//    Vector3 response;
//    if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.25f)) {
//        var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
//        var adjustedVelocity = slopeRotation * motion;
//        if (adjustedVelocity.y < 0) {
//            return adjustedVelocity;
//        } 
//    }
//    return motion;
//}



//private void CameraRotation() {
//    //float horizontalInput = Input.GetAxisRaw("Horizontal");
//    //float verticalInput = Input.GetAxisRaw("Vertical");
//    //Vector2 lookAt = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
//    //Vector2 lookAt = look.ReadValue<Vector2>();
//    // if there is an input and camera position is not fixed
//    if (mouseDelta.sqrMagnitude >= _threshold && !lockCameraPosition) {
//        // REMINDER: Don't multiply mouse input by Time.deltaTime. Units stuff;
//        Debug.Log("ESTA LLEGANDO ALGO");
//        float deltaTimeMultiplier = 1.0f;

//        _cinemachineTargetHorizontal += mouseDelta.x * deltaTimeMultiplier;
//        _cinemachineTargetVertical += mouseDelta.z * deltaTimeMultiplier;
//    }
//    // clamp  rotation so values are limited 360 degrees
//    _cinemachineTargetHorizontal = ClampAngle(_cinemachineTargetHorizontal, float.MinValue, float.MaxValue);
//    _cinemachineTargetVertical = ClampAngle(_cinemachineTargetVertical, bottomClamp, topClamp);
//    // Cinemachine will follow this target
//    CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetVertical + cameraAngleOverride,
//        _cinemachineTargetHorizontal, 0.0f);
//}