using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(CapsuleCollider))]
public class PangHook : MonoBehaviour
{
	private const float GRAPPLE_TIMEOUT = 10f;

	public enum HookType { HOOK, GRAPPLE, MACHINE_GUN }

	[SerializeField]
	private float speed = 15f;
	[SerializeField]
	private float maxLength = 20f;

	[SerializeField]
	private float hookRadius = 0.1f;

	private int playerId;
	private Vector3 origin;
	private LineRenderer line;
	private float currentLength = 0f;
	private bool isShooting = false;

	private Material mat;
	private HookType hookType;

	private CapsuleCollider capsuleCollider;
	private Action onHookDestroyed;
	private bool isGamePaused;
	private bool canGrow;

	void Awake()
	{
		line = GetComponent<LineRenderer>();
		line.positionCount = 2;

		mat = line.material;
		line.enabled = false;

		capsuleCollider = GetComponent<CapsuleCollider>();
		isGamePaused = false;

		canGrow = true;
	}

	private void Start()
	{
		GameManager.OnPause += OnPause;
		GameManager.OnUnpause += OnUnpause;

	}

	private void OnDestroy()
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


	void Update()
	{
		if (!isShooting || isGamePaused || !canGrow)
		{
			return;
		}

		// Extend upward
		currentLength += speed * Time.deltaTime;

		if (currentLength >= maxLength)
		{
			currentLength = maxLength;
			if (hookType == HookType.HOOK)
			{
				StopHook();
			}
			else
			{
				StartCoroutine(StopHookGrapple());
			}
		}

		UpdateLine();
	}

	public void Shoot(Transform originTransform, HookType hookType, Action onHookDestroyed, int playerId)
	{
		if (isShooting || isGamePaused)
		{
			return;
		}

		this.playerId = playerId;
		this.hookType = hookType;
		this.onHookDestroyed = onHookDestroyed;
		origin = originTransform.position;

		isShooting = true;
		currentLength = 0f;

		line.enabled = true;
	}

	private void StopHook()
	{
		isShooting = false;
		line.enabled = false;

		DestroyHook();
	}

	private void UpdateLine()
	{
		if(!canGrow)
		{
			return;
		}

		Vector3 start = origin;
		Vector3 end = start + Vector3.up * currentLength;

		line.SetPosition(0, start);
		line.SetPosition(1, end);

		// Tile texture so it looks like repeating chain links
		float tileAmount = currentLength;
		mat.mainTextureScale = new Vector2(1, tileAmount);

		UpdateCollider(start, end);
	}

	private void UpdateCollider(Vector3 start, Vector3 end)
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

		//tipCollider.transform.SetPositionAndRotation(end, newRotation);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.TryGetComponent(out DestructibleObject destructibleObject))
		{
			destructibleObject.DestroyObject(playerId);
			DestroyHook();
		}
		else if (other.CompareTag("Surface")) // Hard surfaces stop the hook
		{
			Debug.Log($"hook triggered with {other.gameObject.name}");
			//DestroyHook();
			if (hookType == HookType.HOOK)
			{
				StopHook();
			}
			else
			{
				StartCoroutine(StopHookGrapple());
			}

		}
	}

	private IEnumerator StopHookGrapple()
	{
		canGrow = false;
		yield return new WaitForSeconds(GRAPPLE_TIMEOUT);

		DestroyHook();
	}

	private void DestroyHook()
	{
		onHookDestroyed?.Invoke();

		gameObject.SetActive(false);

		Destroy(gameObject);
	}
}
