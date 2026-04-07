using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectScene : MonoBehaviour
{
	[Header("Character Prefabs")]
	[SerializeField]
	private List<PlayerCharacterSO> Characters = new();

	private CharacterSelectionWidget selectionWidget;

	void Start()
	{
		selectionWidget = FindAnyObjectByType<CharacterSelectionWidget>();

		if (selectionWidget != null)
		{
			//selectionWidget.AddCharacter(malePlayerPrefab, "Male Player");
			//selectionWidget.AddCharacter(femalePlayerPrefab, "Female Player");

			Characters.ForEach(data => selectionWidget.AddCharacter(data.CharacterPrefab, data.CharacterName));

			selectionWidget.OnCharacterSelected += OnCharacterSelected;
			selectionWidget.Activate();
		}
	}

	private void OnCharacterSelected(object sender, GameObject selectedPrefab)
	{
		Debug.Log($"Character selected: {selectedPrefab.name}");

		PlayerPrefs.SetString("SelectedCharacter", selectedPrefab.name);

		SceneManager.LoadScene("Level1");
	}

	void OnDestroy()
	{
		if (selectionWidget != null)
		{
			selectionWidget.OnCharacterSelected -= OnCharacterSelected;
		}
	}
}
