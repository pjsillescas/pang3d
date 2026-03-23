using UnityEngine;

public class Stairs : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && other.TryGetComponent(out PangThirdPersonController controller))
		{
			controller.ActivateStairs();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && other.TryGetComponent(out PangThirdPersonController controller))
		{
			controller.DeactivateStairs();
		}
	}
}
