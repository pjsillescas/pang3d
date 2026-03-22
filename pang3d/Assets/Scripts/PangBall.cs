using UnityEngine;

public class PangBall : MonoBehaviour
{
	[SerializeField]
	private float horizontalSpeed = 3f;
	[SerializeField]
	private float gravity = 20f;
	[SerializeField]
	private float bounceForce = 10f;
	[SerializeField]
	private float speed = 8f;

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
		
		directionVector = new Vector3(1, 1, 0).normalized;
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
		/*
		Debug.Log($"colliding with {col.collider.name}");
		// Wall bounce
		if (col.gameObject.CompareTag("Wall"))
		{
			direction *= -1;
		}

		if (col.gameObject.CompareTag("Player"))
		{
			Debug.Log("colliding with player");
		}

		if (col.gameObject.CompareTag("Ground"))
		{
			verticalVelocity = bounceForce;
			transform.position = new Vector3(transform.position.x, radius, transform.position.z);
		}
		*/

		ContactPoint contact = collision.contacts[0];
		Vector3 normal = contact.normal;

		// Floor check
		if (Vector3.Dot(normal, Vector3.up) > 0.7f)
		{
			//directionVector.y = bounceForce / speed;
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
		;
	}
}
