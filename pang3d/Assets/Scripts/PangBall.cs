using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class PangBall : MonoBehaviour
{
	public enum SpeedMode { SLOW, FAST }
	public enum BallType { BALL1, BALL2, BALL3, BALL4 }
	public enum BallDirection { LEFT, RIGHT }

	public static event EventHandler<PangBall> OnBallSpawned;
	public static event EventHandler<PangBall> OnBallDestroyed;

	[SerializeField]
	private float ItemProbability = 0.5f;
	[SerializeField]
	private BallType ballType;
	[SerializeField]
	private BallDirection InitialDirection = BallDirection.RIGHT;
	[SerializeField]
	private float horizontalSpeed = 5f;
	[SerializeField]
	private float gravity = 20f;
	[SerializeField]
	private float bounceForce = 15f;
	[SerializeField]
	private float DeltatimeFactorSlow = 0.5f;
	

	private float verticalVelocity;
	private int direction = 1; // 1 = right, -1 = left
	private float radius;
	private bool isPaused;
	private int destroyedBy;
	private bool useGravity;
	private bool isBouncing;
	private float deltaTimeFactor;
	private SpeedMode speedMode;
	private bool isHarmless;

	public SpeedMode GetSpeedMode() => speedMode;
	public void SetSpeedMode(SpeedMode speedMode)
	{
		if(speedMode == SpeedMode.FAST)
		{
			SetFastMode();
		}
		else
		{
			SetSlowMode();
		}
	}

	public void AddVerticalSpeed(float speed)
	{
		verticalVelocity += speed;
	}

	private void Awake()
	{
		// Flags
		isPaused = false;
		useGravity = true;
		isBouncing = false;
		isHarmless = false;

		// Velocities
		verticalVelocity = 0;
		SetFastMode();
		direction = InitialDirection == BallDirection.RIGHT ? 1 : -1;

	}

	private void OnEnable()
	{
		GameManager.OnPause += OnPause;
		GameManager.OnUnpause += OnUnpause;
	}

	private void OnDisable()
	{
		GameManager.OnPause -= OnPause;
		GameManager.OnUnpause -= OnUnpause;
	}

	public int GetDestroyedBy() => destroyedBy;

	private void OnPause(object sender, EventArgs args)
	{
		Pause();
	}


	public void Pause()
	{
		isPaused = true;
	}

	private void OnUnpause(object sender, EventArgs args)
	{
		UnPause();
	}


	public void UnPause()
	{
		isPaused = false;
	}

	public bool IsPaused() => isPaused;

	public void SetColor(Color color)
	{
		var meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material.SetColor("_BaseColor", color);
	}

	private void UpdateRadius()
	{
		var collider = GetComponent<SphereCollider>();
		radius = collider.radius * transform.localScale.x;
	}

	public BallType GetBallType() => ballType;

	public void SetFastMode()
	{
		Debug.Log("fast mode");
		speedMode = SpeedMode.FAST;
		deltaTimeFactor = 1.0f;
	}

	public void SetSlowMode()
	{
		Debug.Log("slow mode");
		speedMode = SpeedMode.SLOW;
		deltaTimeFactor = DeltatimeFactorSlow;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		UpdateRadius();

		//directionVector = new Vector3(1, 1, 0).normalized;

		OnBallSpawned?.Invoke(this, this);
	}

	public void SetInitialDirection(BallDirection ballDirection)
	{
		InitialDirection = ballDirection;
		direction = InitialDirection == BallDirection.RIGHT ? 1 : -1;
	}

	public void GoHarmless()
	{
		StartCoroutine(GoHarmlessCoroutine());
	}

	private IEnumerator GoHarmlessCoroutine()
	{
		isHarmless = true;

		yield return new WaitForSeconds(1f);

		isHarmless = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (isPaused)
		{
			return;
		}

		Vector3 pos = transform.position;

		var time = Time.deltaTime * deltaTimeFactor;

		// Horizontal movement
		pos.x += direction * horizontalSpeed * time;

		// Vertical movement
		if (useGravity)
		{
			verticalVelocity -= gravity * time;
		}
		pos.y += verticalVelocity * time;
		pos.z = 0f;
		transform.position = pos;

		//var displacement = speed * Time.deltaTime * directionVector;
		//transform.position += displacement;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (isPaused || isBouncing)
		{
			return;
		}

		if (collision.gameObject.TryGetComponent(out PangThirdPersonController controller) && !isHarmless)
		{
			var hasShield = controller.HasShield();
			controller.KillCharacter();

			if(hasShield)
			{
				DestroyBall(controller.GetPlayerId());
			}
			return;
		}

		if (collision.gameObject.CompareTag("Shield"))
		{
			Debug.Log($"ball {name} colliding with shield => destroyed");
			Destroy(gameObject, 0.01f);
		}

		ContactPoint contact = collision.contacts[0];
		Vector3 normal = contact.normal;

		// Floor check
		if (Vector3.Dot(normal, Vector3.up) > 0.7f || collision.gameObject.CompareTag("Ground"))
		{
			if (isBouncing)
			{
				return;
			}
			isBouncing = true;

			// Debug.Log("boing");
			//verticalVelocity = bounceForce;
			//isBouncing = false;
			StartCoroutine(SetBounceForce());
			transform.position = new Vector3(transform.position.x, transform.position.y + radius, transform.position.z);
			//directionVector = Vector3.up;
		}
		else
		{
			direction *= -1;
			//directionVector = Vector3.Reflect(directionVector, normal);
		}
	}

	private IEnumerator SetBounceForce()
	{
		const int FRAMES_PER_BOUNCE = 4;
		//float deltaVelocity = bounceForce / FRAMES_PER_BOUNCE;
		float deltaVelocity = bounceForce * 8f / 15f;
		//Debug.Log($"force {bounceForce} ({deltaVelocity})");

		useGravity = false;
		verticalVelocity = 0f;

		for (int i = 0; i < FRAMES_PER_BOUNCE; i++)
		{
			//verticalVelocity += deltaVelocity;
			float factor = i switch
			{
				0 => 8f,
				1 => 4f,
				2 => 2f,
				3 => 1f,
				_ => 1f,
			};

			verticalVelocity += deltaVelocity / factor;
			yield return null;
		}

		//Debug.Log($"vertical {verticalVelocity} ({bounceForce})");
		useGravity = true;
		
		isBouncing = false;

	}

	private void SpawnItem()
	{
		var itemSpawner = FindAnyObjectByType<ItemSpawner>();

		if (itemSpawner != null)
		{
			itemSpawner.TrySpawnRandomItem(ItemProbability, transform.position);
		}
	}

	private void DestroyBall(bool useSpawner, int playerId)
	{
		destroyedBy = playerId;

		SpawnItem();

		if (useSpawner && TryGetComponent(out NextBallSpawner ballSpawner))
		{
			ballSpawner.SpawnNextBalls(this);
		}

		OnBallDestroyed?.Invoke(this, this);

		Destroy(gameObject, 0.01f);
	}

	public void DestroyBall(int playerId)
	{
		DestroyBall(true, playerId);
	}

	public void DestroyBallCompletely(int playerId)
	{
		DestroyBall(false, playerId);
	}
}
