using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private string FirstLevel = "Playground";
	[SerializeField]
	private Button StartButton;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Debug.Log("adding start button listener");
		StartButton.onClick.RemoveAllListeners();
		StartButton.onClick.AddListener(StartButtonClick);
	}

	private void StartButtonClick()
	{
		Debug.Log("Playground");
		SceneManager.LoadScene(FirstLevel);
	}
}
