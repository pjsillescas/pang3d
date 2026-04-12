using UnityEngine;

public class PlayerInits : MonoBehaviour
{
    [SerializeField]
    private Transform playerEntryPoint1;
	[SerializeField]
	private Transform playerEntryPoint2;

	public Transform GetPlayerInitPoint(int playerId)
	{
		return playerId == 1 ? playerEntryPoint1 : playerEntryPoint2;
	}
}
