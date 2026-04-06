using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public event EventHandler<int> OnHook;
	public event EventHandler<int> OnShoot;
	public event EventHandler<int> OnSprintBegin;
	public event EventHandler<int> OnSprintEnd;

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

	public Vector2 GetMoveVector(int playerId)
	{
		if (playerId == 1)
		{
			return input.Player.Move.ReadValue<Vector2>();
		}
		
		// Player 2 move when ready
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
			OnShoot?.Invoke(this, 1);
		}

		if (input.Player.Hook.WasPressedThisFrame())
		{
			OnHook?.Invoke(this, 1);
		}

		if (input.Player.Sprint.WasPressedThisFrame())
		{
			OnSprintBegin?.Invoke(this, 1);
		}

		if (input.Player.Sprint.WasReleasedThisFrame())
		{
			OnSprintEnd?.Invoke(this, 1);
		}

		if (input.Player.Pause.WasReleasedThisFrame())
		{
			gameManager.TogglePause();
		}

	}
}
