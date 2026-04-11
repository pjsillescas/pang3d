using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public class MoveData
	{
		public int playerId;
		public Vector2 movement;
	}

	public event EventHandler<int> OnHook;
	public event EventHandler<int> OnShoot;
	public event EventHandler<int> OnSprintBegin;
	public event EventHandler<int> OnSprintEnd;
	public event EventHandler<MoveData> OnMove;

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
		return input.Player2.Move.ReadValue<Vector2>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!isEnabled)
		{
			return;
		}

		if (input.Player.Move.WasPressedThisFrame())
		{
			OnMove?.Invoke(this, new() { playerId = 1, movement = GetMoveVector(1) });
		}

		if (input.Player2.Move.WasPressedThisFrame())
		{
			OnMove?.Invoke(this, new() { playerId = 2, movement = GetMoveVector(2) });
		}
		
		if (input.Player.Shoot.WasPressedThisFrame())
		{
			OnShoot?.Invoke(this, 1);
		}

		if (input.Player2.Shoot.WasPressedThisFrame())
		{
			OnShoot?.Invoke(this, 2);
		}
		
		if (input.Player.Hook.WasPressedThisFrame())
		{
			OnHook?.Invoke(this, 1);
		}

		if (input.Player2.Hook.WasPressedThisFrame())
		{
			OnHook?.Invoke(this, 2);
		}
		
		if (input.Player.Sprint.WasPressedThisFrame())
		{
			OnSprintBegin?.Invoke(this, 1);
		}

		if (input.Player2.Sprint.WasPressedThisFrame())
		{
			OnSprintBegin?.Invoke(this, 2);
		}

		if (input.Player.Sprint.WasReleasedThisFrame())
		{
			OnSprintEnd?.Invoke(this, 1);
		}

		if (input.Player2.Sprint.WasReleasedThisFrame())
		{
			OnSprintEnd?.Invoke(this, 2);
		}
		
		if (input.Player.Pause.WasReleasedThisFrame() || input.Player2.Pause.WasReleasedThisFrame())
		{
			gameManager.TogglePause();
		}

	}
}
