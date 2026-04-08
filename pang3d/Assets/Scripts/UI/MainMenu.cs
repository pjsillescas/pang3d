using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CharacterSelectionWidget;

[RequireComponent(typeof(InputManager))]
public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private string FirstLevel = "Playground";

	private InputManager inputManager;
	private CharacterSelectionWidget characterSelectionWidget;
	private TitleWidget titleWidget;
	private PlayerChoices playerChoices;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		inputManager = GetComponent<InputManager>();
		characterSelectionWidget = FindAnyObjectByType<CharacterSelectionWidget>();
		titleWidget = FindAnyObjectByType<TitleWidget>();
		playerChoices = FindAnyObjectByType<PlayerChoices>();

		titleWidget.Activate();
		characterSelectionWidget.Deactivate();

		inputManager.OnHook += ChooseCharacter;
	}

	private void OnDestroy()
	{
		inputManager.OnHook -= ChooseCharacter;
	}
	
	private void StartGame()
	{
		FindAnyObjectByType<GameStats>().ResetStats();
		SceneManager.LoadScene(FirstLevel);
	}

	private void ChooseCharacter(object sender, int playerId)
	{
		titleWidget.Deactivate();
		//characterSelectionWidget.AddCharacter(playerMalePrefab, "Male");
		//characterSelectionWidget.AddCharacter(playerFemalePrefab, "Female");
		characterSelectionWidget.OnCharacterSelected += OnCharacterSelected;
		characterSelectionWidget.Activate();
	}

	private void OnCharacterSelected(object sender, SelectedCharacterData characterData) {
		Debug.Log($"Selected: {characterData.character.CharacterName}");
		playerChoices.SetPlayerCharacter(characterData.playerId, characterData.character);

		StartGame();
	}
}
