using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Scriptable Objects/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
	public string LevelName = "Level";
	public string NextLevelName = "NextLevel";
	public int Time = 200;
}
