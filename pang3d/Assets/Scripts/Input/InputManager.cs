using System;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public event EventHandler OnJump;
	public event EventHandler OnSprintBegin;
	public event EventHandler OnSprintEnd;

	private InputActions input;
	private bool isEnabled;

	void Awake()
	{
		input = new InputActions();
		isEnabled = false;
	}

	private void OnEnable()
	{
		input.Enable();
		isEnabled = true;
	}

	private void OnDisable()
	{
		input.Disable();
		isEnabled = false;
	}

	public Vector2 GetMoveVector()
	{
		return input.Player.Move.ReadValue<Vector2>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!isEnabled)
		{
			return;
		}

		if (input.Player.Jump.WasPressedThisFrame())
		{
			OnJump?.Invoke(this, EventArgs.Empty);
		}

		if (input.Player.Sprint.WasPressedThisFrame())
		{
			OnSprintBegin?.Invoke(this, EventArgs.Empty);
		}

		if (input.Player.Sprint.WasReleasedThisFrame())
		{
			OnSprintEnd?.Invoke(this, EventArgs.Empty);
		}

	}
}
