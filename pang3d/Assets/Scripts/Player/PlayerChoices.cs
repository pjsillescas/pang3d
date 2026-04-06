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
		playerInits = FindAnyObjectByType<PlayerInits>();

		//TrySpawnPlayer1();
		//TrySpawnPlayer2();
	}

	public void TrySpawnPlayer1()
	{
		if (playerInits != null && Player1Prefab != null)
		{
			playerInits.SpawnPlayer(Player1Prefab, 1);
		}
	}

	public void TrySpawnPlayer2()
	{
		if (playerInits != null && Player2Prefab != null)
		{
			playerInits.SpawnPlayer(Player2Prefab, 2);
		}
	}
}
