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
		string timeStr;
		while (currentTime > 0)
		{
			timeStr = currentTime.ToString("D3");
			TimeText.text = $"Time: {timeStr}";

			if (isTimerPaused)
			{
				yield return null;
				continue;
			}

			yield return waitForOneSecond;
			currentTime--;
		}
		timeStr = currentTime.ToString("D3");
		TimeText.text = $"Time: {timeStr}";

		OnTimeout?.Invoke(this,EventArgs.Empty);
	}
}
