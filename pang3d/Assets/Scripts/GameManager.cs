using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public enum GameResult { WON, LOST };

	private static event EventHandler OnGameStarted;
	private static event EventHandler<GameResult> OnGameEnded;
	
	private List<PangBall> balls;

	private void Awake()
	{
		balls = new();
		PangBall.OnBallSpawned += OnBallSpawned;
		PangBall.OnBallDestroyed += OnBallDestroyed;
	}

	private void OnBallSpawned(object sender, PangBall ball)
	{
		balls.Add(ball);
	}

	private void OnBallDestroyed(object sender, PangBall ball)
	{
		balls.Remove(ball);

		if(balls.Count <= 0)
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
