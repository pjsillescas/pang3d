using UnityEngine;

public class PlayerInits : MonoBehaviour
{
    [SerializeField]
    private Transform playerEntryPoint1;
	[SerializeField]
	private Transform playerEntryPoint2;

	public PangThirdPersonController SpawnPlayer(GameObject playerPrefab, int playerId)
	{
		var controller = SpawnPlayer(playerPrefab, playerId == 1 ? playerEntryPoint1 : playerEntryPoint2);
		controller.SetPlayerId(playerId);
		return controller;
	}

	private PangThirdPersonController SpawnPlayer(GameObject playerPrefab, Transform spawnPoint)
	{
		return Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<PangThirdPersonController>();
	}
}
