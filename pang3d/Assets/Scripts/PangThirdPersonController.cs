using System;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class PangThirdPersonController : MonoBehaviour
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	public float MoveSpeed = 2.0f;

	[Tooltip("Sprint speed of the character in m/s")]
	public float SprintSpeed = 5.335f;

	[Tooltip("How fast the character turns to face movement direction")]
	[Range(0.0f, 0.3f)]
	public float RotationSmoothTime = 0.12f;

	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate = 10.0f;

	public AudioSource AudioFootsteps;
	public AudioSource LandingAudio;
	public AudioSource AudioFoley;
	public AudioClip LandingAudioClip;
	public AudioClip[] FootstepAudioClips;
	[Range(0, 1)] public float FootstepAudioVolume = 0.5f;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;

	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity = -15.0f;

	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.50f;

	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.15f;

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool Grounded = true;

	[Tooltip("Useful for rough ground")]
	public float GroundedOffset = -0.14f;

	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float GroundedRadius = 0.28f;

	[Tooltip("What layers the character uses as ground")]
	public LayerMask GroundLayers;

	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 70.0f;

	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = -30.0f;

	[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
	public float CameraAngleOverride = 0.0f;

	[Tooltip("For locking the camera position on all axis")]
	public bool LockCameraPosition = false;

	// player
	private float _speed;
	private float _animationBlend;
	private float _targetRotation = 0.0f;
	private float _rotationVelocity;
	private float _verticalVelocity;

	// animation IDs
	private int _animIDSpeed;
	private int _animIDGrounded;
	private int _animIDFreeFall;
	private int _animIDMotionSpeed;

	private Animator _animator;
	private InputManager inputManager;
	private GameObject _mainCamera;
	private bool isSprinting;
	private Vector3 velocity;

	private bool _hasAnimator;

	private void Awake()
	{
		_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

		velocity = Vector3.zero;
	}

	private void Start()
	{
		_hasAnimator = TryGetComponent(out _animator);
		inputManager = GetComponent<InputManager>();

		isSprinting = false;
		inputManager.OnSprintBegin += OnSprintBegin;
		inputManager.OnSprintEnd += OnSprintEnd;

		AssignAnimationIDs();
	}

	private void OnDestroy()
	{
		inputManager.OnSprintBegin -= OnSprintBegin;
		inputManager.OnSprintEnd -= OnSprintEnd;

	}

	private void OnSprintBegin(object sender, EventArgs args)
	{
		isSprinting = true;
	}
	private void OnSprintEnd(object sender, EventArgs args)
	{
		isSprinting = false;
	}

	private void Update()
	{
		_hasAnimator = TryGetComponent(out _animator);

		OnMove(inputManager.GetMoveVector());
		JumpAndGravity();
		GroundedCheck();
	}

	private void AssignAnimationIDs()
	{
		_animIDSpeed = Animator.StringToHash("Speed");
		_animIDGrounded = Animator.StringToHash("Grounded");
		_animIDFreeFall = Animator.StringToHash("FreeFall");
		_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
	}

	private void GroundedCheck()
	{
		// set sphere position, with offset
		var spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
			transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
			QueryTriggerInteraction.Ignore);

		// update animator if using character
		if (_hasAnimator)
		{
			_animator.SetBool(_animIDGrounded, Grounded);
		}
	}

	private void OnMove(Vector2 moveVector)
	{
		var inputX = moveVector.x;
		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = isSprinting ? SprintSpeed : MoveSpeed;

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		if (inputX == 0)
		{
			targetSpeed = 0.0f;
		}

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset ||
			currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
				Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
		if (_animationBlend < 0.01f)
		{
			_animationBlend = 0f;
		}

		// if there is a move input rotate player when the player is moving
		if (inputX != 0)
		{
			_targetRotation = Mathf.Atan2(inputX, 0) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
			float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
				RotationSmoothTime);

			// rotate to face input direction relative to camera position
			transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
		}

		var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

		// move the player
		velocity = targetDirection.normalized * _speed + new Vector3(0.0f, _verticalVelocity, 0.0f);
		transform.position = transform.position + velocity * Time.deltaTime;

		// update animator if using character
		if (_hasAnimator)
		{
			_animator.SetFloat(_animIDSpeed, _animationBlend);
			_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
		}
	}

	private void JumpAndGravity()
	{
		// update animator if using character
		if (_hasAnimator)
		{
			_animator.SetBool(_animIDFreeFall, !Grounded);
		}

		if (Grounded)
		{
			_verticalVelocity = 0;
		}
		else
		{
			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}

			_verticalVelocity += Gravity * Time.deltaTime;
		}
	}
	
	private void OnDrawGizmosSelected()
	{
		var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		Gizmos.color = Grounded ? transparentGreen : transparentRed;

		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Gizmos.DrawSphere(
			new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
			GroundedRadius);
	}
	
	// Animation event
	public void OnFootstep(AnimationEvent animationEvent)
	{
		if (animationEvent.animatorClipInfo.weight > 0.5f)
		{
			if (AudioFootsteps != null)
			{
				AudioFootsteps.Play();
			}
			
			if (AudioFoley != null)
			{
				AudioFoley.Play();
			}
		}
	}

	// Animation event
	public void OnLand(AnimationEvent animationEvent)
	{
		if (LandingAudio != null && animationEvent.animatorClipInfo.weight > 0.5f)
		{
			LandingAudio.Play();
		}
	}
}
