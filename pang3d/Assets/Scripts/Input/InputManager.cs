using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public event EventHandler OnHook;
	public event EventHandler OnShoot;
	public event EventHandler OnSprintBegin;
	public event EventHandler OnSprintEnd;

	private InputActions input;
	private bool isEnabled;
	private GameManager gameManager;

	void Awake()
	{
		input = new InputActions();
		isEnabled = false;
	}

	private void Start()
	{
		gameManager = FindAnyObjectByType<GameManager>();
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

		if (input.Player.Shoot.WasPressedThisFrame())
		{
			OnShoot?.Invoke(this, EventArgs.Empty);
		}

		if (input.Player.Hook.WasPressedThisFrame())
		{
			OnHook?.Invoke(this, EventArgs.Empty);
		}

		if (input.Player.Sprint.WasPressedThisFrame())
		{
			OnSprintBegin?.Invoke(this, EventArgs.Empty);
		}

		if (input.Player.Sprint.WasReleasedThisFrame())
		{
			OnSprintEnd?.Invoke(this, EventArgs.Empty);
		}

		if (input.Player.Pause.WasReleasedThisFrame())
		{
			gameManager.TogglePause();
		}

	}
}
