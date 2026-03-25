using UnityEngine;

public class DestructibleBall : DestructibleObject
{
	public override void DestroyObject()
	{
		if (TryGetComponent(out PangBall ball))
		{
			ball.DestroyBall();
		}
	}
}
