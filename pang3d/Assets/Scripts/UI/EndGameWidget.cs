using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI GameOverText;

	[SerializeField]
	private TextMeshProUGUI LevelCompletedText;
	[SerializeField]
	private TextMeshProUGUI LifeLostText;

	private InputManager inputManager;
	private LevelInfoWidget levelInfoWidget;
	private GameManager.GameResult gameResult;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		GameOverText.enabled = false;
		LevelCompletedText.enabled = false;
		LifeLostText.enabled = false;
		levelInfoWidget = FindAnyObjectByType<LevelInfoWidget>();
		inputManager = FindAnyObjectByType<InputManager>();

		inputManager.OnHook -= OnGoToNextLevel;
	}

	private void OnEnable()
	{
		GameManager.OnGameEnded += OnGameEnded;
	}

	private void OnDisable()
	{
		GameManager.OnGameEnded -= OnGameEnded;
	}

	private void OnGameEnded(object sender, GameManager.GameResult gameResult)
	{
		Debug.Log($"endgamewidget {gameResult}");
		this.gameResult = gameResult;
		GameOverText.enabled = gameResult == GameManager.GameResult.LOST_GAME;
		LifeLostText.enabled = gameResult == GameManager.GameResult.LOST_LIFE;
		LevelCompletedText.enabled = gameResult == GameManager.GameResult.WON;

		inputManager.OnHook += OnGoToNextLevel;
	}

	private void OnGoToNextLevel(object sender, EventArgs args)
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
}
