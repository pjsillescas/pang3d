using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public enum GameResult { WON, LOST_LIFE, LOST_GAME };

	public static event EventHandler OnGameStarted;
	public static event EventHandler<GameResult> OnGameEnded;
	public static event EventHandler OnPause;
	public static event EventHandler OnUnpause;

	private List<PangBall> balls;
	private bool isGamePaused;
	private TimeWidget timeWidget;
	private LevelInfoWidget levelInfoWidget;

	public List<PangBall> GetBalls() => balls;

	private void Awake()
	{
		balls = new();
		PangBall.OnBallSpawned += OnBallSpawned;
		PangBall.OnBallDestroyed += OnBallDestroyed;

		isGamePaused = false;
		Time.timeScale = 1;

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
		//PangThirdPersonController.OnPlayerKilled -= OnPlayerKilled;

	}

	private void OnGameEndedMethod(object sender, GameResult result)
	{
		var resultString = result == GameResult.WON ? "you won!!" : "you lost";
		Debug.Log($"Game finished: {resultString}");
		Time.timeScale = 0;
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
		Debug.Log("ball spawned");
	}

	private void OnBallDestroyed(object sender, PangBall ball)
	{
		balls.Remove(ball);

		if (balls.Count <= 0)
		{
			StartCoroutine(CheckForGameEnded());
		}
	}

	private IEnumerator CheckForGameEnded()
	{
		yield return new WaitForSeconds(1f);

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
		//PangThirdPersonController.OnPlayerKilled += OnPlayerKilled;
	}
	
	private void OnPlayerDataChanged(object sender, PlayerDataDTO playerData)
	{
		var lastEvent = playerData.GetLastEvent();
		if (lastEvent == PlayerDataDTO.PlayerDataEvent.LOST_LIFE)
		{
			if (playerData.GetLives() < 1)
			{
				Debug.LogWarning("lives out!!");
				OnGameEnded?.Invoke(this, GameResult.LOST_GAME);
			}
			else
			{
				OnGameEnded?.Invoke(this, GameResult.LOST_LIFE);
			}
		}
	}

	private void OnTimeout(object sender, EventArgs args)
	{
		Debug.LogWarning("time out");
		OnGameEnded?.Invoke(this, GameResult.LOST_LIFE);
	}

	private const float CLOCK_TIMEOUT_SECONDS = 10;

	public void RunClockItemAction()
	{
		balls.ForEach(ball => ball.Pause());

		StartCoroutine(WaitClockTimeout());
	}

	private IEnumerator WaitClockTimeout()
	{
		yield return new WaitForSeconds(CLOCK_TIMEOUT_SECONDS);
		
		balls.ForEach(ball => ball.UnPause());
	}

	public void RunDynamiteItemAction(int playerId)
	{
		StartCoroutine(DynamiteItemAction(playerId));
		//controller.AddMaxHooks(1);
	}

	private IEnumerator DynamiteItemAction(int playerId)
	{
		var waitForSeconds = new WaitForSeconds(0.1f);
		bool allSmallBalls;
		int maxIterations = 5;
		do
		{
			var balls = this.balls.Where(ball => ball.GetBallType() != PangBall.BallType.BALL4).ToList();
			balls.ForEach(ball => ball.DestroyBall(playerId));

			allSmallBalls = balls.Count == 0;
			Debug.Log($"numballs {balls.Count}");
			maxIterations--;

			yield return waitForSeconds;
		}
		while (!allSmallBalls && maxIterations > 0);
		Debug.Log("finish dynamite");

	}

	private const float HOURGLASS_TIMEOUT_SECONDS = 10;

	public void RunHourglassItemAction()
	{
		balls.ForEach(ball => ball.SetSlowMode());

		StartCoroutine(WaitHourglassTimeout());
	}

	private IEnumerator WaitHourglassTimeout()
	{
		yield return new WaitForSeconds(HOURGLASS_TIMEOUT_SECONDS);

		balls.ForEach(ball => ball.SetFastMode());
	}

	public void RunStarItemAction(int playerId)
	{
		new List<PangBall>(balls).ForEach(ball => ball.DestroyBallCompletely(playerId));
	}
}
