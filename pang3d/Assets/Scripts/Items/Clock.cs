using UnityEngine;

public class Clock : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		controller.AddMaxHooks(1);
	}
}
