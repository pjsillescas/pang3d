using TMPro;
using UnityEngine;

public class ScoreWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI ScoreText;

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
		ScoreText.text = playerData.GetScore().ToString();
	}
}
