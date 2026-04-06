using UnityEngine;

public class PlayerInits : MonoBehaviour
{
    [SerializeField]
    private Transform playerEntryPoint1;
	[SerializeField]
	private Transform playerEntryPoint2;

	public PangThirdPersonController SpawnPlayer(GameObject playerPrefab, int playerId)
	{
		return SpawnPlayer(playerPrefab, playerId == 1 ? playerEntryPoint1 : playerEntryPoint2);
	}

	private PangThirdPersonController SpawnPlayer(GameObject playerPrefab, Transform spawnPoint)
	{
		return Instantiate(playerPrefab, spawnPoint).GetComponent<PangThirdPersonController>();
	}
}
