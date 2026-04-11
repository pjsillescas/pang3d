using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;

public class DestroyedSurface : MonoBehaviour
{
	private List<Rigidbody> rigidBodies;

	private float radius = 3f;
	private float force = 300f;
	private float destroyTimeoutSeconds = 1f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		rigidBodies = new (GetComponentsInChildren<Rigidbody>());

		Explode(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Explode(Vector3 position)
	{
		ApplyExplosion(position, radius, force);
	}

	private void ApplyExplosion(Vector3 position, float radius, float force)
	{
		rigidBodies.ForEach(rb =>
		{
			rb.AddExplosionForce(force, position, radius);
			Destroy(rb.gameObject, 1f);
		});
	}
}
