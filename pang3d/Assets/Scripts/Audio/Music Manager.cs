using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
	private AudioSource source;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		source = GetComponent<AudioSource>();

		var levelInfoWidget = FindAnyObjectByType<LevelInfoWidget>();
		if (levelInfoWidget != null)
		{
			var levelData = levelInfoWidget.GetLevelData();
			var musicClip = (levelData != null) ? levelData.MusicClip : null;

			if (musicClip != null)
			{
				source.clip = musicClip;
				source.loop = true;
				source.Play();
			}

		}
	}

	private void OnDestroy()
	{
		if(source != null)
		{
			source.Stop();
		}
	}

}
