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

	public void SetPlayerCharacter(int playerId, PlayerCharacterSO character)
	{
		if(playerId == 1)
		{
			Player1Prefab = character.CharacterPrefab;
		}
		else if (playerId == 2)
		{
			Player2Prefab = character.CharacterPrefab;
		}
		else
		{
			Debug.LogError($"Invalid player {playerId}");
		}
	}

	private PlayerInits GetPlayerInits()
	{
		if (playerInits == null)
		{
			playerInits = FindAnyObjectByType<PlayerInits>();
		}

		return playerInits;
	}

	public bool TrySpawnPlayer1()
	{
		var playerInits = GetPlayerInits();
		if (playerInits != null && Player1Prefab != null)
		{
			return playerInits.SpawnPlayer(Player1Prefab, 1) != null;
		}

		return false;
	}

	public bool TrySpawnPlayer2()
	{
		var playerInits = GetPlayerInits();
		if (playerInits != null && Player2Prefab != null)
		{
			return playerInits.SpawnPlayer(Player2Prefab, 2) != null;
		}

		return false;
	}
}
