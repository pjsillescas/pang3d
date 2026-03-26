public class PlayerDataDTO
{
	public const int LIVES_DEFAULT = 3;

	private readonly int playerId;
	
	private int lives;
	private int score;

	public PlayerDataDTO(int playerId)
	{
		this.playerId = playerId;
		
		Reset();
	}

	public void Reset()
	{
		lives = LIVES_DEFAULT;
		score = 0;
	}

	public int GetPlayerId() => playerId;

	public PlayerDataDTO SetLives(int lives)
	{
		this.lives = lives;
		return this;
	}

	public PlayerDataDTO AddLives(int lives)
	{
		this.lives += lives;
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
		return this;
	}

	public int GetScore() => score;
	public int GetLives() => lives;
}