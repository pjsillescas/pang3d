using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputManager))]
public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private string FirstLevel = "Playground";

	private InputManager inputManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		inputManager = GetComponent<InputManager>();

		inputManager.OnHook += StartGame;
	}

	private void OnDestroy()
	{
		inputManager.OnHook -= StartGame;
	}
	
	private void StartGame(object sender, EventArgs args)
	{
		Debug.Log("Playground");
		FindAnyObjectByType<GameStats>().ResetStats();
		SceneManager.LoadScene(FirstLevel);
	}
}
