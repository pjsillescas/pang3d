
using UnityEngine;

public class Barrier : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		Debug.Log("getting shield");
		controller.ActivateShield();
	}
}
