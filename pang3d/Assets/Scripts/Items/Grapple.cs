public class Grapple : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		controller.SetHookType(PangHook.HookType.GRAPPLE);
	}
}
