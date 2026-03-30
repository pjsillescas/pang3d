using UnityEngine;
using static PangHook;

public class Missile : MonoBehaviour
{
	[SerializeField]
	private float Speed = 20.0f;

	[SerializeField]
	private float maxDistance = 10.0f;

	private Vector3 origin;
	private int playerId;

	public void Shoot(Vector3 position, float angle, int playerId)
	{
		this.playerId = playerId;
		transform.Rotate(0, 0, angle);
		transform.position = position;
		origin = position;
	}

	// Update is called once per frame
	void Update()
	{
		var newPosition = transform.position + transform.up * Speed * Time.deltaTime;
		transform.position = new Vector3(newPosition.x, newPosition.y, 0);

		if ((transform.position - origin).sqrMagnitude > maxDistance * maxDistance)
		{
			Destroy(gameObject, 0.01f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out DestructibleObject destructibleObject))
		{
			destructibleObject.DestroyObject(playerId);
			DestroyMissile();
		}
		else if (other.CompareTag("Surface")) // Hard surfaces stop the hook
		{
			Debug.Log($"missile triggered with {other.gameObject.name}");
			DestroyMissile();
		}
	}

	private void DestroyMissile()
	{
		gameObject.SetActive(false);

		Destroy(gameObject);
	}

}
