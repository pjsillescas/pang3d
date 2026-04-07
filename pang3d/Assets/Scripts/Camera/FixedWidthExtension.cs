using Unity.Cinemachine;
using UnityEngine;

[ExecuteAlways]
public class FixedWidthExtension : CinemachineExtension
{
	public float targetWidth = 20f;

	protected override void PostPipelineStageCallback(
		CinemachineVirtualCameraBase vcam,
		CinemachineCore.Stage stage,
		ref CameraState state,
		float deltaTime)
	{
		if (stage != CinemachineCore.Stage.Body)
			return;

		if (!vcam.Follow)
			return;

		float vFOV = state.Lens.FieldOfView * Mathf.Deg2Rad;
		float aspect = state.Lens.Aspect;

		float hFOV = 2f * Mathf.Atan(Mathf.Tan(vFOV / 2f) * aspect);

		float distance = targetWidth / (2f * Mathf.Tan(hFOV / 2f));

		Vector3 forward = state.RawOrientation * Vector3.forward;

		state.RawPosition = state.ReferenceLookAt - forward * distance;
	}
}