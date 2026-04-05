using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimeWidget : MonoBehaviour
{
	public event EventHandler OnTimeout;

	[SerializeField]
	private TextMeshProUGUI TimeText;

	private int currentTime;
	private Coroutine timer;
	private bool isTimerPaused;

	private void Awake()
	{
		currentTime = 0;
		timer = null;
		isTimerPaused = false;
	}

	private void OnEnable()
	{
		GameManager.OnPause += OnPause;
		GameManager.OnUnpause += OnUnpause;
	}

	private void OnDisable()
	{
		GameManager.OnPause -= OnPause;
		GameManager.OnUnpause -= OnUnpause;
	}

	private void OnPause(object sender, EventArgs args)
	{
		PauseTimer();
	}
	private void OnUnpause(object sender, EventArgs args)
	{
		UnpauseTimer();
	}

	public void StartTimer(int maxTime)
	{
		currentTime = maxTime;
		isTimerPaused = false;
		if (timer != null)
		{
			StopCoroutine(timer);
		}

		timer = StartCoroutine(DoTimer());
	}

	public void PauseTimer()
	{
		isTimerPaused = true;
	}

	public void PauseTimer(int time)
	{
		StartCoroutine(WaitToUnpause(time));
	}

	private IEnumerator WaitToUnpause(int time)
	{
		isTimerPaused = true;

		yield return new WaitForSeconds(time);

		isTimerPaused = false;

	}

	public void UnpauseTimer()
	{
		isTimerPaused = false;
	}

	private IEnumerator DoTimer()
	{
		var waitForOneSecond = new WaitForSeconds(1);
		while (currentTime > 0)
		{
			DisplayTime();

			if (isTimerPaused)
			{
				yield return null;
				continue;
			}

			yield return waitForOneSecond;
			if(!isTimerPaused)
			{
				currentTime--;
			}
		}

		DisplayTime();

		OnTimeout?.Invoke(this,EventArgs.Empty);
	}

	private void DisplayTime()
	{
		var timeStr = currentTime.ToString("D3");
		TimeText.text = $"Time: {timeStr}";
	}
}
