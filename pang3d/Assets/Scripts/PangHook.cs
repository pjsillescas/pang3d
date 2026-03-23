using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(CapsuleCollider))]
public class PangHook : MonoBehaviour
{
	public event EventHandler<PangBall> OnBallHit;

	[SerializeField]
	private float speed = 15f;
	[SerializeField]
	private float maxLength = 20f;

	[SerializeField]
	private float hookRadius = 0.1f;

	private Vector3 origin;
	private LineRenderer line;
	private float currentLength = 0f;
	private bool isShooting = false;

	private Material mat;

	private CapsuleCollider capsuleCollider;
	private Action onHookDestroyed;

	void Awake()
	{
		line = GetComponent<LineRenderer>();
		line.positionCount = 2;

		mat = line.material;
		line.enabled = false;

		capsuleCollider = GetComponent<CapsuleCollider>();
	}

	void Update()
	{
		if (!isShooting)
		{
			return;
		}

		// Extend upward
		currentLength += speed * Time.deltaTime;

		if (currentLength >= maxLength)
		{
			currentLength = maxLength;
			StopHook();
		}
		
		UpdateLine();
	}

	public void Shoot(Transform originTransform, Action onHookDestroyed)
	{
		if (isShooting)
		{
			return;
		}

		this.onHookDestroyed = onHookDestroyed;
		origin = originTransform.position;

		isShooting = true;
		currentLength = 0f;

		line.enabled = true;
	}

	void StopHook()
	{
		isShooting = false;
		line.enabled = false;

		DestroyHook();
	}

	void UpdateLine()
	{
		Vector3 start = origin;
		Vector3 end = start + Vector3.up * currentLength;

		line.SetPosition(0, start);
		line.SetPosition(1, end);

		// Tile texture so it looks like repeating chain links
		float tileAmount = currentLength;
		mat.mainTextureScale = new Vector2(1, tileAmount);

		UpdateCollider(start, end);
	}

	void UpdateCollider(Vector3 start, Vector3 end)
	{
		Vector3 direction = end - start;
		float length = direction.magnitude;

		// Position collider in the middle
		var newPosition = start + direction * 0.5f;

		// Rotate collider to match direction
		var newRotation = Quaternion.FromToRotation(Vector3.up, direction);
		capsuleCollider.transform.SetPositionAndRotation(newPosition, newRotation);

		// Capsule settings
		capsuleCollider.height = length;
		capsuleCollider.radius = hookRadius;
		capsuleCollider.direction = 1; // Y axis
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out PangBall ball))
		{
			Debug.Log($"ball hit {ball.name}");
			OnBallHit?.Invoke(this, ball);
			ball.DestroyBall();

			DestroyHook();
		}
	}

	private void DestroyHook()
	{
		onHookDestroyed?.Invoke();

		gameObject.SetActive(false);

		Destroy(gameObject);
	}
}
