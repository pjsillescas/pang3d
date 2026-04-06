using UnityEngine;

public class PlayerChoices : MonoBehaviour
{
	[SerializeField]
	private GameObject Player1Prefab;
	[SerializeField]
	private GameObject Player2Prefab;

	private PlayerInits playerInits;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		DontDestroyOnLoad(this);

		playerInits = null;
		//TrySpawnPlayer1();
		//TrySpawnPlayer2();
	}

	private PlayerInits GetPlayerInits()
	{
		if (playerInits == null)
		{
			playerInits = FindAnyObjectByType<PlayerInits>();
		}

		return playerInits;
	}

	public void TrySpawnPlayer1()
	{
		var playerInits = GetPlayerInits();
		if (playerInits != null && Player1Prefab != null)
		{
			playerInits.SpawnPlayer(Player1Prefab, 1);
		}
	}

	public void TrySpawnPlayer2()
	{
		var playerInits = GetPlayerInits();
		if (playerInits != null && Player2Prefab != null)
		{
			playerInits.SpawnPlayer(Player2Prefab, 2);
		}
	}
}
