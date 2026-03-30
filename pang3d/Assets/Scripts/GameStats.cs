using System;
using UnityEngine;
using static PangThirdPersonController;

public class GameStats : MonoBehaviour
{
	public static event EventHandler<PlayerDataDTO> OnPlayerDataChanged;

	private PlayerDataDTO player1Data;
	private PlayerDataDTO player2Data;

	private void Awake()
	{
		player1Data = new PlayerDataDTO(1);
		player2Data = new PlayerDataDTO(2);

		DontDestroyOnLoad(this);
	}

	public void ResetStats()
	{
		player1Data.Reset();
		player2Data.Reset();
	}

	public PlayerDataDTO GetPlayerData(int playerId)
	{
		return playerId == 1 ? player1Data : player2Data;
	}

	private void Start()
	{
		OnPlayerDataChanged?.Invoke(this, player1Data);
		OnPlayerDataChanged?.Invoke(this, player2Data);
	}

	private void OnEnable()
	{
		PangBall.OnBallDestroyed += OnBallDestroyed;
		PangThirdPersonController.OnPlayerKilled += OnPlayerKilled;
		PangThirdPersonController.OnPlayerNewLife += OnPlayerNewLife;
		PangThirdPersonController.OnScoreAdded += OnScoreAdded;

	}

	private void OnDisable()
	{
		PangBall.OnBallDestroyed -= OnBallDestroyed;
		PangThirdPersonController.OnPlayerKilled -= OnPlayerKilled;
		PangThirdPersonController.OnPlayerNewLife -= OnPlayerNewLife;
		PangThirdPersonController.OnScoreAdded -= OnScoreAdded;
	}

	private void OnScoreAdded(object sender, ScoreStruct scoreStruct)
	{
		var playerData = scoreStruct.playerId == 1 ? player1Data : player2Data;
		playerData.AddScore(scoreStruct.score);

		OnPlayerDataChanged?.Invoke(this, playerData);
	}

	private void OnPlayerKilled(object sender, int playerId)
	{
		AddLives(playerId, -1);
	}

	private void OnPlayerNewLife(object sender, int playerId)
	{
		AddLives(playerId, 1);
	}

	private void AddLives(int playerId, int numLives)
	{
		PlayerDataDTO playerData = (playerId == 1) ? player1Data : player2Data;
		playerData.AddLives(numLives);
		OnPlayerDataChanged?.Invoke(this, playerData);
	}

	private void OnBallDestroyed(object sender, PangBall ball)
	{
		var score = ball.GetBallType() switch
		{
			PangBall.BallType.BALL1 => 400,
			PangBall.BallType.BALL2 => 800,
			PangBall.BallType.BALL3 => 1200,
			_ => 1600,
		};

		var playerId = ball.GetDestroyedBy();
		PlayerDataDTO playerData = (playerId == 1) ? player1Data : player2Data;
		playerData.AddScore(score);

		OnPlayerDataChanged?.Invoke(this, playerData);
	}
}
