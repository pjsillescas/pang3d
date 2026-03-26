using TMPro;
using UnityEngine;

public class LevelInfoWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI LevelNameText;

	[SerializeField]
	private LevelDataSO LevelInfo;

    public LevelDataSO GetLevelData() => LevelInfo;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        LevelNameText.text = LevelInfo.LevelName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
