public class Star : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		FindAnyObjectByType<GameManager>().RunStarItemAction();
		controller.AddScore(10000);
	}
}
