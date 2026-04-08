using System;
using TMPro;
using UnityEngine;
using static PangThirdPersonController;

public class PlayerWidget : MonoBehaviour
{
	[SerializeField]
	private int PlayerId;
	[SerializeField]
	private TextMeshProUGUI ScoreText;
	[SerializeField]
	private TextMeshProUGUI LivesText;
	[SerializeField]
	private TextMeshProUGUI WeaponText;

	private void OnEnable()
	{
		GameStats.OnPlayerDataChanged += OnPlayerDataChanged;
		GameManager.OnPlayerStart += OnPlayerStart;
		GameManager.OnGameEnded += OnGameEnded;
		PangThirdPersonController.OnHookChanged += OnHookChanged;
	}

	private void OnDisable()
	{
		GameStats.OnPlayerDataChanged -= OnPlayerDataChanged;
		GameManager.OnPlayerStart -= OnPlayerStart;
		GameManager.OnGameEnded -= OnGameEnded;
		PangThirdPersonController.OnHookChanged -= OnHookChanged;
	}

	private void OnHookChanged(object sender, HookData data)
	{
		if (data.playerId == PlayerId)
		{
			WeaponText.text = data.hookType switch
			{
				PangHook.HookType.MACHINE_GUN => "Machine Gun",
				PangHook.HookType.HOOK => "Hook",
				PangHook.HookType.GRAPPLE => "Grapple",
				_ => ""
			}
			;
		}
	}

	private void OnGameEnded(object sender, GameManager.GameResultData gameResultData)
	{
		if ((gameResultData.playerId == 0 || gameResultData.playerId == PlayerId) && gameResultData.gameResult == GameManager.GameResult.LOST_GAME)
		{
			SetGameOver();
		}
	}

	void Start()
	{
		SetGameOver();
	}

	private void SetGameOver()
	{
		ScoreText.text = "GAME OVER";
		LivesText.text = "";
		WeaponText.text = "";
	}

	private void OnPlayerStart(object sender, int playerId)
	{
		var gameStats = FindAnyObjectByType<GameStats>();
		OnPlayerDataChanged(null, gameStats.GetPlayerData(playerId));
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
