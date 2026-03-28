public class Dynamite : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		FindAnyObjectByType<GameManager>().RunDynamiteItemAction();
	}
}
