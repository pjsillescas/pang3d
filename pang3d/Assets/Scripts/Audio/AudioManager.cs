using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PangHook;

public class AudioManager : MonoBehaviour
{
	const float FX_VOLUME = 0.02f;

	private List<AudioUnit> audioUnits;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		audioUnits = new(GetComponentsInChildren<AudioUnit>());
	}
	private void OnEnable()
	{
		PangBall.OnBallDestroyed += OnBallDestroyed;
		PangBall.OnBallBounce += OnBallBounce;
		Item.OnItemPickedUp += OnItemPickedUp;
		PangThirdPersonController.OnHookLaunched += OnHookLaunched;
		DestructibleSurface.OnSurfaceDestroyed += OnSurfaceDestroyed;
		GameManager.OnGameEnded += OnGameEnded;
	}

	private void OnDisable()
	{
		PangBall.OnBallDestroyed -= OnBallDestroyed;
		PangBall.OnBallBounce += OnBallBounce;
		Item.OnItemPickedUp -= OnItemPickedUp;
		PangThirdPersonController.OnHookLaunched -= OnHookLaunched;
		DestructibleSurface.OnSurfaceDestroyed += OnSurfaceDestroyed;
		GameManager.OnGameEnded += OnGameEnded;
	}

	private void Play(AudioType type)
	{
		Play(type, 1f);
	}

	private void Play(AudioType type, float volume)
	{
		if (audioUnits == null || audioUnits.Count == 0)
		{
			return;
		}

		var audioUnit = audioUnits.First(unit => unit.GetAudioType() == type);

		if (audioUnit != null)
		{
			audioUnit.PlayRandom(volume);
		}
	}

	private void OnBallDestroyed(object sender, PangBall ball)
	{
		Play(AudioType.POP, 0.5f);
	}

	private void OnBallBounce(object sender, PangBall ball)
	{
		Play(AudioType.BOUNCE, FX_VOLUME);
	}

	private void OnItemPickedUp(object sender, Item item)
	{
		Play(AudioType.ITEM_PICKUP, 0.5f);
	}

	private void OnHookLaunched(object sender, PangThirdPersonController controller)
	{
		if (controller.GetHookType() == HookType.MACHINE_GUN)
		{
			Play(AudioType.GUNSHOT, 0.5f);
		}
	}

	private void OnSurfaceDestroyed(object sender, DestructibleSurface surface)
	{
		Play(AudioType.SURFACE_DESTROY, 0.5f);
	}

	private void OnGameEnded(object sender, GameManager.GameResultData resultData)
	{
		var audioType = resultData.gameResult switch
		{
			GameManager.GameResult.WON => AudioType.END_SUCCESS,
			GameManager.GameResult.LOST_LIFE => AudioType.END_LIFE_LOST,
			GameManager.GameResult.LOST_GAME => AudioType.END_GAME_OVER,
			_ => AudioType.END_SUCCESS
		};

		Play(audioType);
	}


}
