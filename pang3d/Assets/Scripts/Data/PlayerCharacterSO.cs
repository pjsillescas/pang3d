using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacterSO", menuName = "Scriptable Objects/PlayerCharacterSO")]
public class PlayerCharacterSO : ScriptableObject
{
    public GameObject CharacterPrefab;
    public string CharacterName;
}
