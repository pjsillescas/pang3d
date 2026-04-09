using System;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
	public static event EventHandler<Item> OnItemPickedUp;

	protected abstract void PerformItemAction(PangThirdPersonController controller);

	private void FixedUpdate()
	{
		transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out PangThirdPersonController controller))
		{
			OnItemPickedUp?.Invoke(this, this);
			PerformItemAction(controller);

			Destroy(gameObject, 0.1f);
		}
	}
}
