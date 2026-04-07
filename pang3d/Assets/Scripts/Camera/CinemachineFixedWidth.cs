using Unity.Cinemachine;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(CinemachineCamera))]
public class CinemachineFixedWidth : MonoBehaviour
{
	[SerializeField]
	private float targetWidth = 20f; // gameplay width in world units
	//[SerializeField]
	private Camera mainCamera;

	private CinemachineCamera vcam;

	void Awake()
	{
		vcam = GetComponent<CinemachineCamera>();
		mainCamera = Camera.main;
	}

	void EnsureInitialized()
	{
		if (vcam == null)
		{
			vcam = GetComponent<CinemachineCamera>();
		}
	}
	
	void LateUpdate()
	{
		EnsureInitialized();

		if (mainCamera == null || vcam == null)
		{
			return;
		}

		float vFOV = mainCamera.fieldOfView * Mathf.Deg2Rad;

		// Convert vertical FOV → horizontal FOV
		float hFOV = 2f * Mathf.Atan(Mathf.Tan(vFOV / 2f) * mainCamera.aspect);

		// Compute required distance
		float distance = targetWidth / (2f * Mathf.Tan(hFOV / 2f));

		// Move camera along its forward axis
		Transform camTransform = vcam.transform;

		Vector3 forward = camTransform.forward;
		Vector3 targetPos = vcam.Follow.position;

		camTransform.position = targetPos - forward * distance;
	}
}