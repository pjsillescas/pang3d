using System.Collections.Generic;
using System.Linq;
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
	}

	public int GetNumberOfPlayers()
	{
		/*
		var player1 = Player1Prefab != null ? 1 : 0;
		var player2 = Player2Prefab != null ? 1 : 0;

		return player1 + player2;
		*/
		return (new List<GameObject>() { Player1Prefab, Player2Prefab }).Count(prefab => prefab != null);
	}

	public void SetPlayerCharacter(int playerId, PlayerCharacterSO character)
	{
		if (playerId == 1)
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

	public GameObject GetPlayer1Prefab() => Player1Prefab;
	public GameObject GetPlayer2Prefab() => Player2Prefab;
}
