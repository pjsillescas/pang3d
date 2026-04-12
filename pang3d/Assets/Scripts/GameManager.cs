using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private const float HOURGLASS_TIMEOUT_SECONDS = 10;

	public enum GameResult { WON, LOST_LIFE, LOST_GAME };
	public class GameResultData
	{
		public GameResult gameResult;
		public int playerId;
	}

	public static event EventHandler OnGameStarted;
	public static event EventHandler<GameResultData> OnGameEnded;
	public static event EventHandler OnPause;
	public static event EventHandler OnUnpause;
	public static event EventHandler<int> OnPlayerStart;

	private List<PangBall> balls;
	private bool isGamePaused;
	private TimeWidget timeWidget;
	private LevelInfoWidget levelInfoWidget;
	private PlayerChoices playerChoices;
	private PlayerHandler playerHandler1;
	private PlayerHandler playerHandler2;

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

		Time.timeScale = 1;
	}

	public bool AreThereTwoPlayers()
	{
		var playersData = FindAnyObjectByType<PlayerChoices>();

		return playersData.GetNumberOfPlayers() == 2;
	}

	public bool IsGameOver()
	{
		var playersData = FindAnyObjectByType<PlayerChoices>();
		var stats = FindAnyObjectByType<GameStats>();
		var data1 = stats.GetPlayerData(1);
		var data2 = stats.GetPlayerData(2);
		return (playersData.GetPlayer1Prefab() == null || data1.GetLives() == 0) && (playersData.GetPlayer2Prefab() == null || data2.GetLives() == 0);
	}

	private void OnGameEndedMethod(object sender, GameResultData resultData)
	{
		var resultString = resultData.gameResult == GameResult.WON ? "you won!!" : "you lost";
		Debug.Log($"Game finished: {resultString}");

		if (IsGameOver())
		{
			Time.timeScale = 0;
		}
		else
		{
			if (resultData.playerId == 1)
			{
				SpawnPlayer1();
			}
			else
			{
				SpawnPlayer2();
			}
		}
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

	private void TrySetColor(PangBall ball)
	{
		if (levelInfoWidget != null)
		{
			ball.SetColor(levelInfoWidget.GetLevelData().LevelColor);
		}
	}

	private void OnBallSpawned(object sender, PangBall ball)
	{
		TrySetColor(ball);
		balls.Add(ball);
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
			OnGameEnded?.Invoke(this, new() { gameResult = GameResult.WON, playerId = 0 });
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		OnGameStarted?.Invoke(this, EventArgs.Empty);
		timeWidget = FindAnyObjectByType<TimeWidget>();
		playerChoices = FindAnyObjectByType<PlayerChoices>();

		levelInfoWidget = FindAnyObjectByType<LevelInfoWidget>();
		timeWidget.StartTimer(levelInfoWidget.GetLevelData().Time);
		timeWidget.OnTimeout += OnTimeout;
		GameStats.OnPlayerDataChanged += OnPlayerDataChanged;
		OnGameEnded += OnGameEndedMethod;

		balls.ForEach(ball => TrySetColor(ball));

		StartCoroutine(StartGame());
	}

	private PlayerHandler GetPlayerHandler1()
	{
		if (playerHandler1 == null)
		{
			var spawnPoint = FindAnyObjectByType<PlayerInits>().GetPlayerInitPoint(1);
			var playerPrefab = playerChoices.GetPlayer1Prefab();
			playerHandler1 = new PlayerHandler(spawnPoint, playerPrefab, 1);
		}

		return playerHandler1;
	}

	private PlayerHandler GetPlayerHandler2()
	{
		if (playerHandler2 == null)
		{
			var spawnPoint = FindAnyObjectByType<PlayerInits>().GetPlayerInitPoint(2);
			var playerPrefab = playerChoices.GetPlayer2Prefab();
			playerHandler2 = new PlayerHandler(spawnPoint, playerPrefab, 2);
		}

		return playerHandler2;
	}
	
	public PangThirdPersonController SpawnPlayer1()
	{
		//var controller = playerChoices.TrySpawnPlayer1();
		var controller = GetPlayerHandler1().TrySpawnPlayer();
		if (controller != null)
		{
			OnPlayerStart?.Invoke(this, 1);
		}

		return controller;
	}

	public PangThirdPersonController SpawnPlayer2()
	{
		//var controller = playerChoices.TrySpawnPlayer2();
		var controller = GetPlayerHandler2().TrySpawnPlayer();
		if (controller != null)
		{
			OnPlayerStart?.Invoke(this, 2);
		}

		return controller;
	}

	private IEnumerator StartGame()
	{
		playerChoices = FindAnyObjectByType<PlayerChoices>();

		SpawnPlayer1();
		SpawnPlayer2();
		
		yield return new WaitForSeconds(0.1f);

		var initGameWidget = FindAnyObjectByType<InitGameWidget>();
		initGameWidget.gameObject.SetActive(true);

		TogglePause();
		yield return new WaitForSeconds(2f);

		initGameWidget.gameObject.SetActive(false);
		
		TogglePause();
	}


	private void OnPlayerDataChanged(object sender, PlayerDataDTO playerData)
	{
		var lastEvent = playerData.GetLastEvent();
		if (lastEvent == PlayerDataDTO.PlayerDataEvent.LOST_LIFE)
		{
			if (playerData.GetLives() < 1)
			{
				Debug.LogWarning("lives out!!");
				OnGameEnded?.Invoke(this, new() { gameResult = GameResult.LOST_GAME, playerId = playerData.GetPlayerId() });
			}
			else
			{
				OnGameEnded?.Invoke(this, new() { gameResult = GameResult.LOST_LIFE, playerId = playerData.GetPlayerId() });
			}
		}
	}

	private void OnTimeout(object sender, EventArgs args)
	{
		Debug.LogWarning("time out");
		OnGameEnded?.Invoke(this, new() { gameResult = GameResult.LOST_LIFE, playerId = 0 });
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
