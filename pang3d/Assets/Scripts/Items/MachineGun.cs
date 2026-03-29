public class MachineGun : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		controller.SetHookType(PangHook.HookType.MACHINE_GUN);
	}
}
