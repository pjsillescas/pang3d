using UnityEngine;

public class GameOverWatcher : MonoBehaviour
{
	private InputManager inputManager;
	private GameManager gameManager;
	private GameStats gameStats;
	private PlayerChoices playerChoices;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		gameManager = FindAnyObjectByType<GameManager>();
		gameStats = FindAnyObjectByType<GameStats>();
		playerChoices = FindAnyObjectByType<PlayerChoices>();

		inputManager = FindAnyObjectByType<InputManager>();
		inputManager.OnHook += OnStartNewContinue;
	}

	private void OnDestroy()
	{
		if(inputManager != null)
		{
			inputManager.OnHook -= OnStartNewContinue;
		}
	}

	private bool PlayerCanStartNewGame(int playerId)
	{
		var stats1 = gameStats.GetPlayerData(1);
		var stats2 = gameStats.GetPlayerData(2);

		if (playerId == 1)
		{
			return playerChoices.GetPlayer2Prefab() != null && stats2.GetLives() > 0 && stats1.GetLives() == 0;
		}
		else
		{
			return playerChoices.GetPlayer1Prefab() != null && stats1.GetLives() > 0 && stats2.GetLives() == 0;
		}
	}

	private void OnStartNewContinue(object sender, int playerId)
	{
		if(PlayerCanStartNewGame(playerId))
		{
			gameStats.GetPlayerData(playerId).Reset();
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
}
