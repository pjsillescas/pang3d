using UnityEngine;

public abstract class Item : MonoBehaviour
{
	protected abstract void PerformItemAction(PangThirdPersonController controller);
	
	private void FixedUpdate()
	{
		transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out PangThirdPersonController controller))
		{
			PerformItemAction(controller);

			Destroy(gameObject, 0.1f);
		}
	}
}
