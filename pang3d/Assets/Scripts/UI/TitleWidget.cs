using TMPro;
using UnityEngine;

public class TitleWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI TitleText;

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}
	public void Activate()
	{
		gameObject.SetActive(true);
	}
}
