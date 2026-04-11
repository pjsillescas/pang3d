using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CharacterSelectionWidget;

[RequireComponent(typeof(InputManager))]
public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private string FirstLevel = "Playground";
	[SerializeField]
	private CharacterSelectionWidget characterSelectionWidgetPlayer1;
	[SerializeField]
	private CharacterSelectionWidget characterSelectionWidgetPlayer2;
	
	private InputManager inputManager;
	private TitleWidget titleWidget;
	private PlayerChoices playerChoices;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		inputManager = GetComponent<InputManager>();
		//characterSelectionWidgetPlayer1 = FindAnyObjectByType<CharacterSelectionWidget>();
		titleWidget = FindAnyObjectByType<TitleWidget>();
		playerChoices = FindAnyObjectByType<PlayerChoices>();

		titleWidget.Activate();
		characterSelectionWidgetPlayer1.Deactivate();
		characterSelectionWidgetPlayer2.Deactivate();
		
		characterSelectionWidgetPlayer1.OnCharacterSelected += OnCharacterSelected;
		characterSelectionWidgetPlayer2.OnCharacterSelected += OnCharacterSelected;

		inputManager.OnHook += SelectCharacter;
	}

	private void OnDestroy()
	{
		if(inputManager != null)
		{
			inputManager.OnHook -= SelectCharacter;
		}
	}
	
	private void StartGame()
	{
		FindAnyObjectByType<GameStats>().ResetStats();
		SceneManager.LoadScene(FirstLevel);
	}

	private void SelectCharacter(object sender, int playerId)
	{
		titleWidget.Deactivate();

		characterSelectionWidgetPlayer1.Activate(playerId);
		characterSelectionWidgetPlayer2.Activate(playerId);
	}

	private void OnCharacterSelected(object sender, SelectedCharacterData characterData) {
		Debug.Log($"Selected: {characterData.character.CharacterName}");
		playerChoices.SetPlayerCharacter(characterData.playerId, characterData.character);

		CheckForStartGame();
	}

	private void CheckForStartGame()
	{
		if (characterSelectionWidgetPlayer1.IsReady() && characterSelectionWidgetPlayer2.IsReady())
		{
			StartGame();
		}
	}
}
