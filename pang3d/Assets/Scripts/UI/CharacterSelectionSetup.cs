using UnityEngine;
using TMPro;

public class CharacterSelectionSetup : MonoBehaviour
{
    [Header("Character Container (3D Scene Position)")]
    [SerializeField] private Transform characterContainer;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI characterNameText;

    [Header("Character Options")]
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private string[] characterNames;

    private CharacterSelectionWidget widget;

    void Start()
    {
        SetupWidget();
    }

    public void SetupWidget()
    {
        widget = GetComponent<CharacterSelectionWidget>();
        if (widget == null)
        {
            widget = gameObject.AddComponent<CharacterSelectionWidget>();
        }

        if (characterPrefabs != null && characterNames != null && characterPrefabs.Length == characterNames.Length)
        {
            for (int i = 0; i < characterPrefabs.Length; i++)
            {
                if (characterPrefabs[i] != null)
                {
                    widget.AddCharacter(characterPrefabs[i], characterNames[i]);
                }
            }
        }

        widget.Activate();
    }
}
