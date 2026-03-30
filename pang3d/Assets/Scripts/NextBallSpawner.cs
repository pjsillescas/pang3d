using System;
using UnityEngine;
using UnityEngine.UIElements;

public class NextBallSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject Ball2Prefab;
	[SerializeField]
	private GameObject Ball3Prefab;
	[SerializeField]
	private GameObject Ball4Prefab;

	public void SpawnNextBalls(PangBall ball)
	{
		Debug.Log(ball);

		switch(ball.GetBallType())
		{
			case PangBall.BallType.BALL1:
				SpawnBalls(ball, Ball2Prefab);
				break;
			case PangBall.BallType.BALL2:
				SpawnBalls(ball, Ball3Prefab);
				break;
			case PangBall.BallType.BALL3:
				SpawnBalls(ball, Ball4Prefab);
				break;
			case PangBall.BallType.BALL4:
			default:
				break;
		}
	}

	private void SpawnBalls(PangBall parentBall, GameObject ballPrefab)
	{
		var position = parentBall.transform.position;
		var ballLeft = Instantiate(ballPrefab, position, Quaternion.identity).GetComponent<PangBall>();
		ballLeft.SetInitialDirection(PangBall.BallDirection.LEFT);
		var ballRight = Instantiate(ballPrefab, position, Quaternion.identity).GetComponent<PangBall>();
		ballRight.SetInitialDirection(PangBall.BallDirection.RIGHT);

		if(parentBall.IsPaused())
		{
			ballLeft.Pause();
			ballRight.Pause();
		}
	}
}
