using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public enum GameResult { WON, LOST };

	public static event EventHandler OnGameStarted;
	public static event EventHandler<GameResult> OnGameEnded;
	public static event EventHandler OnPause;
	public static event EventHandler OnUnpause;

	private List<PangBall> balls;
	private bool isGamePaused;
	private TimeWidget timeWidget;
	private LevelInfoWidget levelInfoWidget;

	private void Awake()
	{
		balls = new();
		PangBall.OnBallSpawned += OnBallSpawned;
		PangBall.OnBallDestroyed += OnBallDestroyed;

		isGamePaused = false;
	}

	private void OnDestroy()
	{
		PangBall.OnBallSpawned -= OnBallSpawned;
		PangBall.OnBallDestroyed -= OnBallDestroyed;
		
		if (timeWidget != null)
		{
			timeWidget.OnTimeout -= OnTimeout;
		}

		GameStats.OnPlayerDataChanged -= OnPlayerDataChanged;
		OnGameEnded -= OnGameEndedMethod;
	}

	private void OnGameEndedMethod(object sender, GameResult result)
	{
		var resultString = result == GameResult.WON ? "you won!!" : "you lost";
		Debug.Log($"Game finished: {resultString}");
	}

	public void TogglePause()
	{
		isGamePaused = !isGamePaused;

		if (isGamePaused)
		{
			OnPause?.Invoke(this, EventArgs.Empty);
		}
		else
		{
			OnUnpause?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool IsGamePaused() => isGamePaused;

	private void OnBallSpawned(object sender, PangBall ball)
	{
		balls.Add(ball);
	}

	private void OnBallDestroyed(object sender, PangBall ball)
	{
		balls.Remove(ball);

		if (balls.Count <= 0)
		{
			OnGameEnded?.Invoke(this, GameResult.WON);
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		OnGameStarted?.Invoke(this, EventArgs.Empty);
		timeWidget = FindAnyObjectByType<TimeWidget>();

		levelInfoWidget = FindAnyObjectByType<LevelInfoWidget>();
		timeWidget.StartTimer(levelInfoWidget.GetLevelData().Time);
		timeWidget.OnTimeout += OnTimeout;
		GameStats.OnPlayerDataChanged += OnPlayerDataChanged;
		OnGameEnded += OnGameEndedMethod;
	}

	private void OnPlayerDataChanged(object sender, PlayerDataDTO playerData)
	{
		if(playerData.GetLives() <= 0)
		{
			OnGameEnded?.Invoke(this, GameResult.LOST);
		}
	}

	private void OnTimeout(object sender, EventArgs args)
	{
		Debug.Log("time out");
		OnGameEnded?.Invoke(this, GameResult.LOST);
	}

	// Update is called once per frame
	void Update()
	{
		//Debug.Log(balls.Count);
	}
}
