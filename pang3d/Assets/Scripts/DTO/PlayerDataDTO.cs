using UnityEngine;

public class PlayerDataDTO
{
	public const int LIVES_DEFAULT = 3;

	private readonly int playerId;

	public enum PlayerDataEvent { NONE, ADD_LIFE, LOST_LIFE, ADD_SCORE }

	private int lives;
	private int score;
	private PlayerDataEvent lastEvent;

	public PlayerDataDTO(int playerId)
	{
		this.playerId = playerId;
		
		Reset();
	}

	public void Reset()
	{
		lives = LIVES_DEFAULT;
		score = 0;
		lastEvent = PlayerDataEvent.NONE;
	}

	public int GetPlayerId() => playerId;

	public PlayerDataDTO SetLives(int lives)
	{
		Debug.Log($"setting lives {lives}");
		this.lives = lives;
		return this;
	}

	public PlayerDataDTO AddLives(int lives)
	{
		this.lives += lives;

		lastEvent = (lives > 0) ? PlayerDataEvent.ADD_LIFE : PlayerDataEvent.LOST_LIFE;
		return this;
	}

	public PlayerDataDTO SetScore(int score)
	{
		this.score = score;
		return this;
	}

	public PlayerDataDTO AddScore(int score)
	{
		this.score += score;

		lastEvent = PlayerDataEvent.ADD_SCORE;
		return this;
	}

	public int GetScore() => score;
	public int GetLives() => lives;
	public PlayerDataEvent GetLastEvent() => lastEvent;
}