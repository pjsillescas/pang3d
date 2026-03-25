using TMPro;
using UnityEngine;

public class LivesWidget : MonoBehaviour
{
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
		LivesText.text = playerData.GetLives().ToString();
	}
}
