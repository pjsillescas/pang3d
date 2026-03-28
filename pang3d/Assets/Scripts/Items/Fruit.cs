using UnityEngine;

public class Fruit : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		controller.AddScore(500);
	}
}
