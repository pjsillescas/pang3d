using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PangBall : MonoBehaviour
{
	public enum BallType { BALL1, BALL2, BALL3, BALL4 }
	public enum BallDirection { LEFT, RIGHT }

	public static event EventHandler<PangBall> OnBallSpawned;
	public static event EventHandler<PangBall> OnBallDestroyed;

	[SerializeField]
	private BallType ballType;
	[SerializeField]
	private BallDirection InitialDirection = BallDirection.RIGHT;
	[SerializeField]
	private float horizontalSpeed = 3f;
	[SerializeField]
	private float gravity = 20f;
	[SerializeField]
	private float bounceForce = 10f;

	private float verticalVelocity;
	private int direction = 1; // 1 = right, -1 = left
	private float radius;
	private bool isGamePaused;

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

	private void OnPause(object sender, EventArgs args)
	{
		isGamePaused = true;
	}

	private void OnUnpause(object sender, EventArgs args)
	{
		isGamePaused = false;
	}

	//private Vector3 directionVector;
	private void UpdateRadius()
	{
		var collider = GetComponent<SphereCollider>();
		radius = collider.radius * transform.localScale.x;

		Debug.Log($"ball {name}: {radius}");
	}

	public BallType GetBallType() => ballType;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		isGamePaused = false;
		UpdateRadius();
		verticalVelocity = 0;

		direction = InitialDirection == BallDirection.RIGHT ? 1 : -1;

		//directionVector = new Vector3(1, 1, 0).normalized;

		OnBallSpawned?.Invoke(this, this);
	}

	public void SetInitialDirection(BallDirection ballDirection)
	{
		InitialDirection = ballDirection;
		direction = InitialDirection == BallDirection.RIGHT ? 1 : -1;
	}

	// Update is called once per frame
	void Update()
	{
		if (isGamePaused)
		{
			return;
		}

		Vector3 pos = transform.position;

		// Horizontal movement
		pos.x += direction * horizontalSpeed * Time.deltaTime;

		// Vertical movement
		verticalVelocity -= gravity * Time.deltaTime;
		pos.y += verticalVelocity * Time.deltaTime;
		pos.z = 0f;
		transform.position = pos;

		//var displacement = speed * Time.deltaTime * directionVector;
		//transform.position += displacement;
	}

	void OnCollisionEnter(Collision collision)
	{
		if (isGamePaused)
		{
			return;
		}

		if(collision.gameObject.TryGetComponent(out PangThirdPersonController controller))
		{
			controller.KillCharacter();
			return;
		}

		ContactPoint contact = collision.contacts[0];
		Vector3 normal = contact.normal;

		// Floor check
		if (Vector3.Dot(normal, Vector3.up) > 0.7f || collision.gameObject.CompareTag("Ground"))
		{
			verticalVelocity = bounceForce;
			transform.position = new Vector3(transform.position.x, transform.position.y + radius, transform.position.z);
			//directionVector = Vector3.up;
		}
		else
		{
			direction *= -1;
			//directionVector = Vector3.Reflect(directionVector, normal);
		}
	}

	public void DestroyBall()
	{
		OnBallDestroyed?.Invoke(this, this);

		if (TryGetComponent(out NextBallSpawner ballSpawner))
		{
			ballSpawner.SpawnNextBalls(this);
		}

		Destroy(gameObject, 0.01f);
	}
}
