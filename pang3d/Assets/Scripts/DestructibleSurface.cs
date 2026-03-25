using UnityEngine;

public class DestructibleSurface : DestructibleObject
{
	public override void DestroyObject()
	{
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
