using TMPro;
using UnityEngine;

public class PlayerWidget : MonoBehaviour
{
	[SerializeField]
	private int PlayerId;
	[SerializeField]
	private TextMeshProUGUI ScoreText;
	[SerializeField]
	private TextMeshProUGUI LivesText;

	private void OnEnable()
	{
		GameStats.OnPlayerDataChanged += OnPlayerDataChanged;
	}

	private void OnDisable()
	{
		GameStats.OnPlayerDataChanged += OnPlayerDataChanged;
	}

	private void OnPlayerDataChanged(object sender, PlayerDataDTO playerData)
	{
		if (playerData.GetPlayerId() == PlayerId)
		{
			ScoreText.text = playerData.GetScore().ToString();
			LivesText.text = playerData.GetLives().ToString();
		}
	}
}
