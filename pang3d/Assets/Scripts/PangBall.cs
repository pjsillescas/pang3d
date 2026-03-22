using System;
using UnityEngine;

public class PangBall : MonoBehaviour
{
	public enum BallDirection { LEFT, RIGHT }
	public static event EventHandler<PangBall> OnBallSpawned;
	public static event EventHandler<PangBall> OnBallDestroyed;

	[SerializeField]
	private BallDirection InitialDirection = BallDirection.RIGHT;
	[SerializeField]
	private float horizontalSpeed = 3f;
	[SerializeField]
	private float gravity = 20f;
	[SerializeField]
	private float bounceForce = 10f;

	[SerializeField]
	private LayerMask HookLayer;

	private float verticalVelocity;
	private int direction = 1; // 1 = right, -1 = left
	private float radius;

	private Vector3 directionVector;
	private void UpdateRadius()
	{
		var collider = GetComponent<SphereCollider>();
		radius = collider.radius * transform.localScale.x;

		Debug.Log($"ball {name}: {radius}");
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		UpdateRadius();
		verticalVelocity = 0;

		direction = InitialDirection == BallDirection.RIGHT ? 1 : -1;

		directionVector = new Vector3(1, 1, 0).normalized;

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
		Vector3 pos = transform.position;

		// Horizontal movement
		pos.x += direction * horizontalSpeed * Time.deltaTime;

		// Vertical movement
		verticalVelocity -= gravity * Time.deltaTime;
		pos.y += verticalVelocity * Time.deltaTime;

		transform.position = pos;

		//var displacement = speed * Time.deltaTime * directionVector;
		//transform.position += displacement;
	}

	void OnCollisionEnter(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		Vector3 normal = contact.normal;

		// Floor check
		if (Vector3.Dot(normal, Vector3.up) > 0.7f)
		{
			verticalVelocity = bounceForce;
			transform.position = new Vector3(transform.position.x, radius, transform.position.z);
			directionVector = Vector3.up;
		}
		else
		{
			direction *= -1;
			directionVector = Vector3.Reflect(directionVector, normal);
		}

		directionVector = directionVector.normalized;

	}

	public void DestroyBall()
	{
		Debug.Log("ball destroyed");
		OnBallDestroyed?.Invoke(this, this);

		Destroy(gameObject);
		;
	}
}
