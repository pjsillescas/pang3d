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

	private void Awake()
	{
		balls = new();
		PangBall.OnBallSpawned += OnBallSpawned;
		PangBall.OnBallDestroyed += OnBallDestroyed;

		isGamePaused = false;
	}

	private void OnDestroy()
	{
		PangBall.OnBallSpawned += OnBallSpawned;
		PangBall.OnBallDestroyed += OnBallDestroyed;
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
			Debug.Log("you won!!");
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		OnGameStarted?.Invoke(this, EventArgs.Empty);
	}

	// Update is called once per frame
	void Update()
	{
		//Debug.Log(balls.Count);
	}
}
