using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioUnit : MonoBehaviour
{
	[SerializeField]
	private AudioType Type;

	[SerializeField]
	private List<AudioClip> Clips;

	private AudioSource source;

	public AudioType GetAudioType() => Type;

	public void PlayRandom()
	{
		PlayRandom(1f);
	}

	public void PlayRandom(float volume)
	{
		if (source == null)
		{
			return;
		}

		int k = Random.Range(0, Clips.Count);
		source.PlayOneShot(Clips[k], volume);
	}

	void Start()
	{
		source = GetComponent<AudioSource>();
	}
}
