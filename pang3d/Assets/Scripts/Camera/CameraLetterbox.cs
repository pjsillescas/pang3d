using UnityEngine;

[ExecuteAlways]
public class CameraLetterbox : MonoBehaviour
{
	public float targetAspect = 16f / 9f;

	void Update()
	{
		Camera cam = GetComponent<Camera>();

		float windowAspect = (float)Screen.width / Screen.height;
		float scaleHeight = windowAspect / targetAspect;

		Rect rect = cam.rect;

		if (scaleHeight < 1.0f)
		{
			// top/bottom bars
			rect.width = 1.0f;
			rect.height = scaleHeight;
			rect.x = 0;
			rect.y = (1.0f - scaleHeight) / 2.0f;
		}
		else
		{
			// side bars (rare)
			float scaleWidth = 1.0f / scaleHeight;

			rect.width = scaleWidth;
			rect.height = 1.0f;
			rect.x = (1.0f - scaleWidth) / 2.0f;
			rect.y = 0;
		}

		cam.rect = rect;
	}
}