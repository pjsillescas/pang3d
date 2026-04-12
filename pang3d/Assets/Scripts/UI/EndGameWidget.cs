using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameWidget : MonoBehaviour
{
	private enum HookType {  }
	[SerializeField]
	private TextMeshProUGUI GameOverText;

	[SerializeField]
	private TextMeshProUGUI LevelCompletedText;
	[SerializeField]
	private TextMeshProUGUI LifeLostText;

	private InputManager inputManager;
	private LevelInfoWidget levelInfoWidget;
	private GameManager.GameResult gameResult;
	private GameManager gameManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		GameOverText.enabled = false;
		LevelCompletedText.enabled = false;
		LifeLostText.enabled = false;
		levelInfoWidget = FindAnyObjectByType<LevelInfoWidget>();
		inputManager = FindAnyObjectByType<InputManager>();
		gameManager = FindAnyObjectByType<GameManager>();

		inputManager.OnHook -= OnGoToNextLevel;
	}

	private void OnEnable()
	{
		GameManager.OnGameEnded += OnGameEnded;
	}

	private void OnDisable()
	{
		GameManager.OnGameEnded -= OnGameEnded;
		
		inputManager.OnHook -= OnGoToNextLevel;
	}

	private void OnGameEnded(object sender, GameManager.GameResultData gameResultData)
	{
		//var gameManager = FindAnyObjectByType<GameManager>();
		if (gameManager.IsGameOver())
		{
			Debug.Log($"endgamewidget {gameResult}");
			this.gameResult = gameResultData.gameResult;
			GameOverText.enabled = gameResultData.gameResult == GameManager.GameResult.LOST_GAME;
			LifeLostText.enabled = gameResultData.gameResult == GameManager.GameResult.LOST_LIFE;
			LevelCompletedText.enabled = gameResultData.gameResult == GameManager.GameResult.WON;
			
			inputManager.OnHook += OnGoToNextLevel;
		}
		else if (gameResultData.gameResult == GameManager.GameResult.WON)
		{
			LevelCompletedText.enabled = true;
			inputManager.OnHook += OnGoToNextLevel;
			//inputManager.OnHook += Revive;
		}

	}

	private void OnGoToNextLevel(object sender, int playerId)
	{
		var levelData = levelInfoWidget.GetLevelData();
		string nextLevel = gameResult switch
		{
			GameManager.GameResult.WON => levelData.NextLevelName ?? "MainMenu",
			GameManager.GameResult.LOST_LIFE => levelData.LevelName,
			GameManager.GameResult.LOST_GAME => "MainMenu",
			_ => "MainMenu",
		};

		/*
		if (gameResult == GameManager.GameResult.LOST_LIFE)
		{
			nextLevel = levelData.LevelName;
		}
		else if (gameResult == GameManager.GameResult.LOST_GAME)
		{
			nextLevel = "MainMenu";
		}
		else
		{
			nextLevel = levelData.NextLevelName ?? "MainMenu";
		}
		*/

		SceneManager.LoadScene(nextLevel);
	}

	private void Revive(object sender, int playerId)
	{
		Debug.Log($"revive {playerId}");
		//var gameManager = FindAnyObjectByType<GameManager>();
		if (playerId == 1)
		{
			gameManager.SpawnPlayer1();
		}
		else
		{
			gameManager.SpawnPlayer2();
		}
	}

}
