using UnityEngine;

public class DestructibleSurface : DestructibleObject
{
	[SerializeField]
	private float ItemProbability = 0.7f;

	public override void DestroyObject()
	{
		var itemSpawner = FindAnyObjectByType<ItemSpawner>();

		if(itemSpawner != null)
		{
			itemSpawner.TrySpawnRandomItem(ItemProbability, transform.position);
		}

		Destroy(gameObject, 0.1f);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
