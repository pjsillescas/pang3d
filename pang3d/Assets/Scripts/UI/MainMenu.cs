using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputManager))]
public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private string FirstLevel = "Playground";

	private InputManager inputManager;
	private CharacterSelectionWidget characterSelectionWidget;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		inputManager = GetComponent<InputManager>();
		characterSelectionWidget = FindAnyObjectByType<CharacterSelectionWidget>();

		inputManager.OnHook += ChooseCharacter;
	}

	private void OnDestroy()
	{
		inputManager.OnHook -= ChooseCharacter;
	}
	
	private void StartGame(object sender, int playerId)
	{
		Debug.Log("Playground");
		FindAnyObjectByType<GameStats>().ResetStats();
		SceneManager.LoadScene(FirstLevel);
	}

	private void ChooseCharacter(object sender, int playerId)
	{
		var widget = characterSelectionWidget;
		//widget.AddCharacter(playerMalePrefab, "Male");
		//widget.AddCharacter(playerFemalePrefab, "Female");
		widget.OnCharacterSelected += (sender, prefab) => {
			Debug.Log($"Selected: {prefab.name}");
		};
		widget.Activate();
	}
}
