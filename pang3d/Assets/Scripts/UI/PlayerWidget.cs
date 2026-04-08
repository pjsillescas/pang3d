using System;
using TMPro;
using UnityEngine;

public class PlayerWidget : MonoBehaviour
{
	[SerializeField]
	private int PlayerId;
	[SerializeField]
	private TextMeshProUGUI ScoreText;
	[SerializeField]
	private TextMeshProUGUI LivesText;

	private void OnEnable()
	{
		GameStats.OnPlayerDataChanged += OnPlayerDataChanged;
		GameManager.OnPlayerStart += OnPlayerStart;
		GameManager.OnGameEnded += OnGameEnded;
	}

	private void OnDisable()
	{
		GameStats.OnPlayerDataChanged -= OnPlayerDataChanged;
		GameManager.OnPlayerStart -= OnPlayerStart;
		GameManager.OnGameEnded -= OnGameEnded;
	}

	private void OnGameEnded(object sender, GameManager.GameResultData gameResultData)
	{
		if ((gameResultData.playerId == 0 || gameResultData.playerId == PlayerId) && gameResultData.gameResult == GameManager.GameResult.LOST_GAME)
		{
			SetGameOver();
		}
	}

	void Start()
	{
		SetGameOver();
	}

	private void SetGameOver()
	{
		ScoreText.text = "GAME OVER";
		LivesText.text = "";
	}

	private void OnPlayerStart(object sender, int playerId)
	{
		var gameStats = FindAnyObjectByType<GameStats>();
		OnPlayerDataChanged(null, gameStats.GetPlayerData(playerId));
	}
	/*
	public void InitializePlayer(int playerId)
	{
		;
	}
	*/
	private void OnPlayerDataChanged(object sender, PlayerDataDTO playerData)
	{
		if (playerData.GetPlayerId() == PlayerId)
		{
			ScoreText.text = playerData.GetScore().ToString();
			LivesText.text = playerData.GetLives().ToString();
		}
	}
}
