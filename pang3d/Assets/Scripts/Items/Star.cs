public class Star : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		controller.AddMaxHooks(1);
	}
}
