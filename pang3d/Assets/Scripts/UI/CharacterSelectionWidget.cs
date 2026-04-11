using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelectionWidget : MonoBehaviour
{
	public class SelectedCharacterData
	{
		public int playerId;
		public PlayerCharacterSO character;
	}

	public event EventHandler<SelectedCharacterData> OnCharacterSelected;

	[SerializeField]
	private int PlayerId = 1;

	[SerializeField]
	private List<PlayerCharacterSO> characters = new();

	[SerializeField]
	private Transform characterContainer;

	[SerializeField]
	private TextMeshProUGUI characterNameText;
	[SerializeField]
	private TextMeshProUGUI selectionTitleText;
	[SerializeField]
	private TextMeshProUGUI playerPressStartText;

	[SerializeField]
	private float rotationSpeed = 50f;

	[SerializeField]
	private bool autoRotate = true;

	private int currentIndex;
	private GameObject currentCharacterInstance;
	private bool isActive;
	private bool isPlayerSelecting;
	private bool isPlayerSelected;
	private InputManager inputManager;

	private void Awake()
	{
		currentIndex = 0;
		isActive = false;
		isPlayerSelecting = false;
		isPlayerSelected = false;
	}

	private void Start()
	{
		selectionTitleText.text = $"Select character Player {PlayerId}";
		playerPressStartText.text = $"Player {PlayerId} press start";
		inputManager = FindAnyObjectByType<InputManager>();
		inputManager.OnHook += OnSelectCharacter;
		inputManager.OnMove += OnCharacterCarousel;
	}

	void Update()
	{
		if (!isActive)
		{
			return;
		}

		HandleRotation();
	}

	public bool IsReady()
	{
		Debug.Log($"{PlayerId} active {isActive} selecting {isPlayerSelecting} selected {isPlayerSelected}");
		return !isActive || !isPlayerSelecting || isPlayerSelecting && isPlayerSelected;
	}

	private void OnSelectCharacter(object sender, int playerId)
	{
		if(playerId != PlayerId)
		{
			return;
		}

		if (isPlayerSelecting)
		{
			SelectCurrentCharacter();
		}
		else
		{
			Activate(playerId);
		}
	}

	private void OnCharacterCarousel(object sender, InputManager.MoveData data)
	{
		if(data.playerId != PlayerId)
		{
			return;
		}

		var threshold = 0.01f;
		var x = data.movement.x;

		if (x >= threshold)
		{
			NextCharacter();
		}
		else if (x <= -threshold)
		{
			PreviousCharacter();
		}
	}

	private void HandleRotation()
	{
		if (currentCharacterInstance != null && autoRotate)
		{
			currentCharacterInstance.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
		}
	}

	public void SetCharacters(List<PlayerCharacterSO> newCharacters)
	{
		characters = newCharacters;
		if (characters.Count > 0)
		{
			currentIndex = 0;
			ShowCurrentCharacter();
		}
	}

	public void AddCharacter(GameObject prefab, string name)
	{
		characters.Add(new() { CharacterPrefab = prefab, CharacterName = name });
		if (characters.Count == 1)
		{
			currentIndex = 0;
			ShowCurrentCharacter();
		}
	}

	public void ClearCharacters()
	{
		characters.Clear();
		DestroyCurrentCharacter();
		currentIndex = 0;
	}

	public void Activate(int playerId)
	{
		if(isPlayerSelecting)
		{
			return;
		}

		gameObject.SetActive(true);
		isActive = true;
		
		if (playerId == PlayerId)
		{
			isPlayerSelecting = true;
			if (characters.Count > 0)
			{
				ShowCurrentCharacter();
			}

			characterNameText.gameObject.SetActive(true);
			selectionTitleText.gameObject.SetActive(true);

			playerPressStartText.gameObject.SetActive(false);
		}
		else
		{
			playerPressStartText.gameObject.SetActive(true);
			characterNameText.gameObject.SetActive(false);
			selectionTitleText.gameObject.SetActive(false);
			isPlayerSelecting = false;
		}
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
		isActive = false;
	}

	public void NextCharacter()
	{
		if (characters.Count == 0)
		{
			return;
		}

		currentIndex = (currentIndex + 1) % characters.Count;
		ShowCurrentCharacter();
	}

	public void PreviousCharacter()
	{
		if (characters.Count == 0)
		{
			return;
		}

		currentIndex = (currentIndex - 1 + characters.Count) % characters.Count;
		ShowCurrentCharacter();
	}

	public void SelectCurrentCharacter()
	{
		if (characters.Count == 0)
		{
			return;
		}

		isPlayerSelected = true;
		Debug.Log($"selected character for player {PlayerId}");
		OnCharacterSelected?.Invoke(this, new() { playerId = PlayerId, character = characters[currentIndex] });
	}

	public GameObject GetSelectedCharacter()
	{
		if (characters.Count == 0)
		{
			return null;
		}

		return characters[currentIndex].CharacterPrefab;
	}

	public string GetSelectedCharacterName()
	{
		if (characters.Count == 0)
		{
			return "";
		}

		return characters[currentIndex].CharacterName;
	}

	public int GetCurrentIndex()
	{
		return currentIndex;
	}

	public int GetCharacterCount()
	{
		return characters.Count;
	}

	private void ShowCurrentCharacter()
	{
		if (characters.Count == 0)
		{
			return;
		}

		DestroyCurrentCharacter();

		var option = characters[currentIndex];
		if (option.CharacterPrefab != null)
		{
			currentCharacterInstance = Instantiate(option.CharacterPrefab, characterContainer);
			currentCharacterInstance.transform.SetLocalPositionAndRotation(Vector3.zero, new Quaternion(0, 180, 0, 0));
			currentCharacterInstance.GetComponent<PangThirdPersonController>().enabled = false;

			CenterCameraOnCharacter();
		}

		if (characterNameText != null)
		{
			characterNameText.text = option.CharacterName;
		}
	}

	private void CenterCameraOnCharacter()
	{
		Camera.main.transform.position = characterContainer.position + new Vector3(0f, 1f, 3f);
		Camera.main.transform.LookAt(characterContainer.position + Vector3.up);
	}

	private void DestroyCurrentCharacter()
	{
		if (currentCharacterInstance != null)
		{
			Destroy(currentCharacterInstance);
			currentCharacterInstance = null;
		}
	}

	private void OnDestroy()
	{
		DestroyCurrentCharacter();
		if (inputManager != null)
		{
			inputManager.OnHook -= OnSelectCharacter;
			inputManager.OnMove -= OnCharacterCarousel;
		}
	}
}
