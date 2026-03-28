public class ExtraLife : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		controller.AddLife();
	}
}
