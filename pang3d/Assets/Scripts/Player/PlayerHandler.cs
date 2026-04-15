using UnityEngine;

public class PlayerHandler
{
	private PangThirdPersonController controller;
	private Transform spawnPoint;
	private GameObject playerPrefab;
	private int playerId;

	public PlayerHandler(Transform spawnPoint, GameObject playerPrefab, int playerId)
	{
		this.spawnPoint = spawnPoint;
		this.playerPrefab = playerPrefab;
		this.playerId = playerId;

		controller = null;
	}

	private PangThirdPersonController SpawnPlayer()
	{

		if (controller != null)
		{
			controller.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
			controller.StartInvulnerability();

		}
		else
		{
			controller = GameObject.Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<PangThirdPersonController>();
			controller.SetPlayerId(playerId);
		}

		return controller;
	}

	public PangThirdPersonController TrySpawnPlayer()
	{
		Debug.Log("spawn player 1");
		if (spawnPoint != null && playerPrefab != null)
		{
			controller = SpawnPlayer();
		}
		else
		{
			if (controller != null)
			{
				GameObject.Destroy(controller.gameObject, 0.1f);
			}
			controller = null;
		}

		return controller;
	}

	public void DestroyCharacter()
	{
		GameObject.Destroy(controller.gameObject, 0.1f);
		controller = null;
	}
}
