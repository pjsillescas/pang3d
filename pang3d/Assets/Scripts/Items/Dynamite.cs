public class Dynamite : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		controller.AddMaxHooks(1);
	}
}
