

using UnityEngine;

public class DoubleHarpoon : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		Debug.Log("perform double harpoon");
		controller.AddMaxHooks(1);
	}
}
