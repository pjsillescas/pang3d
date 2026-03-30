using UnityEngine;

public class DestructibleBall : DestructibleObject
{
	public override void DestroyObject(int playerId)
	{
		if (TryGetComponent(out PangBall ball))
		{
			ball.DestroyBall(playerId);
		}
	}
}
