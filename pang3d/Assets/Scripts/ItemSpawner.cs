using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
	[SerializeField]
	private List<Item> ItemPrefabs;

	public void TrySpawnRandomItem(float probability, Vector3 position)
	{
		float rand = Random.Range(0,1f);

		if (rand <= probability)
		{
			var index = Random.Range(0, ItemPrefabs.Count);
			var itemPrefab = ItemPrefabs[index];

			Instantiate(itemPrefab, position, Quaternion.identity);
		}
	}
}
