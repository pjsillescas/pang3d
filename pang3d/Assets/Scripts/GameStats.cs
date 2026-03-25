using System;
using UnityEngine;

public class GameStats : MonoBehaviour
{
	public static event EventHandler<PlayerDataDTO> OnPlayerDataChanged;

	private PlayerDataDTO player1Data;
	private PlayerDataDTO player2Data;


	private void Awake()
	{
		player1Data = new PlayerDataDTO();
		player2Data = new PlayerDataDTO();
		
		DontDestroyOnLoad(this);
	}

	private void OnEnable()
	{
		PangBall.OnBallDestroyed += OnBallDestroyed;
		PangThirdPersonController.OnPlayerKilled += OnPlayerKilled;

	}

	private void OnDisable()
	{
		PangBall.OnBallDestroyed -= OnBallDestroyed;
		PangThirdPersonController.OnPlayerKilled -= OnPlayerKilled;
	}

	private void OnPlayerKilled(object sender, int playerId)
	{
		PlayerDataDTO playerData = (playerId == 1) ? player1Data : player2Data;
		playerData.AddLives(-1);
		OnPlayerDataChanged?.Invoke(this, player1Data);
	}

	private void OnBallDestroyed(object sender, PangBall ball)
	{
		var score = ball.GetBallType() switch
		{
			PangBall.BallType.BALL1 => 100,
			PangBall.BallType.BALL2 => 200,
			PangBall.BallType.BALL3 => 300,
			_ => 400,
		};

		player1Data.AddScore(score);

		OnPlayerDataChanged?.Invoke(this, player1Data);
	}
}
