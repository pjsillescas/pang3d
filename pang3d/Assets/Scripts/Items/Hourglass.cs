using UnityEngine;

public class Hourglass : Item
{
	protected override void PerformItemAction(PangThirdPersonController controller)
	{
		FindAnyObjectByType<GameManager>().RunHourglassItemAction();
	}
}
