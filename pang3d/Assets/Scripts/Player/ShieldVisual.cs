using UnityEngine;

public class ShieldVisual : MonoBehaviour
{
	[SerializeField]
	private float capsuleRadius = 0.5f;
	[SerializeField]
	private float capsuleHeight = 1.8f;
	[SerializeField]
	private Color plasmaColor = new(0.3f, 0.8f, 1f, 0.5f);
	[SerializeField]
	private Color glowColor = new(0.5f, 0.9f, 1f, 1f);
	[SerializeField]
	private float animationSpeed = 2f;
	[SerializeField]
	private float distortionStrength = 0.15f;
	[SerializeField]
	private float fresnelPower = 2f;
	[SerializeField]
	private float pulseSpeed = 3f;
	[SerializeField]
	private Vector3 yOffset = new(0, 0.5f, 0);

	private GameObject capsuleObject;
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private Material plasmaMaterial;
	private bool isActive;
	private float activationTime;

	private void Awake()
	{
		capsuleObject = new GameObject("ShieldCapsule");
		capsuleObject.transform.SetParent(transform);
		capsuleObject.transform.SetLocalPositionAndRotation(yOffset, Quaternion.identity);

		capsuleRadius = 0.5f;
		capsuleHeight = 2f;
		
		Mesh capsuleMesh = CreateCapsuleMesh(capsuleRadius, capsuleHeight);

		meshFilter = capsuleObject.AddComponent<MeshFilter>();
		meshFilter.mesh = capsuleMesh;

		meshRenderer = capsuleObject.AddComponent<MeshRenderer>();
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;

		Shader shader = Shader.Find("Custom/PlasmaShield");
		if (shader == null)
		{
			shader = Shader.Find("Standard");
		}

		plasmaMaterial = new Material(shader);
		plasmaMaterial.SetColor("_Color", plasmaColor);
		plasmaMaterial.SetColor("_GlowColor", glowColor);
		plasmaMaterial.SetFloat("_Speed", animationSpeed);
		plasmaMaterial.SetFloat("_Strength", distortionStrength);
		plasmaMaterial.SetFloat("_FresnelPower", fresnelPower);
		plasmaMaterial.SetFloat("_PulseSpeed", pulseSpeed);
		plasmaMaterial.SetFloat("_Activation", 0f);
		plasmaMaterial.SetFloat("_Mode", 3f);
		plasmaMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		plasmaMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		plasmaMaterial.EnableKeyword("_ALPHABLEND_ON");
		plasmaMaterial.renderQueue = 3000;

		meshRenderer.material = plasmaMaterial;
		capsuleObject.SetActive(false);
	}

	private Mesh CreateCapsuleMesh(float radius, float height)
	{
		var mesh = new Mesh()
		{
			name = "PlasmaCapsule"
		};

		int segments = 24;
		int heightSegments = 8;

		float halfHeight = height * 0.5f;
		float cylinderHeight = height - radius * 2f;
		float hemisphereY = halfHeight - radius;

		int vertCount = (segments + 1) * (heightSegments + 3) + 2;
		var vertices = new Vector3[vertCount];
		var normals = new Vector3[vertCount];
		var uvs = new Vector2[vertCount];

		int[] triangles = new int[segments * (heightSegments * 6 + 12)];

		int vertIndex = 0;
		int triIndex = 0;

		for (int y = 0; y <= heightSegments; y++)
		{
			float v = (float)y / heightSegments;
			float posY;

			if (y == 0)
			{
				posY = -halfHeight + radius;
			}
			else if (y == heightSegments)
			{
				posY = halfHeight - radius;
			}
			else
			{
				float t = (float)y / heightSegments;
				posY = Mathf.Lerp(-hemisphereY, hemisphereY, t);
			}

			posY += 1.5f;

			for (int x = 0; x <= segments; x++)
			{
				float u = (float)x / segments;
				float angle = u * Mathf.PI * 2;
				float cos = Mathf.Cos(angle);
				float sin = Mathf.Sin(angle);

				vertices[vertIndex] = new Vector3(cos * radius, posY, sin * radius);
				normals[vertIndex] = new Vector3(cos, 0, sin);
				uvs[vertIndex] = new Vector2(u, v);
				vertIndex++;
			}
		}

		vertices[vertIndex] = new Vector3(0, -halfHeight, 0);
		normals[vertIndex] = new Vector3(0, -1, 0);
		uvs[vertIndex] = new Vector2(0.5f, 0);
		int bottomCenter = vertIndex;
		vertIndex++;

		vertices[vertIndex] = new Vector3(0, halfHeight, 0);
		normals[vertIndex] = new Vector3(0, 1, 0);
		uvs[vertIndex] = new Vector2(0.5f, 1);
		int topCenter = vertIndex;
		vertIndex++;

		for (int y = 0; y < heightSegments; y++)
		{
			for (int x = 0; x < segments; x++)
			{
				int current = y * (segments + 1) + x;
				int next = current + segments + 1;

				triangles[triIndex++] = current;
				triangles[triIndex++] = next;
				triangles[triIndex++] = current + 1;

				triangles[triIndex++] = current + 1;
				triangles[triIndex++] = next;
				triangles[triIndex++] = next + 1;
			}
		}

		int bottomRingStart = 0;
		for (int x = 0; x < segments; x++)
		{
			triangles[triIndex++] = bottomCenter;
			triangles[triIndex++] = bottomRingStart + x + 1;
			triangles[triIndex++] = bottomRingStart + x;
		}

		int topRingStart = heightSegments * (segments + 1);
		for (int x = 0; x < segments; x++)
		{
			triangles[triIndex++] = topRingStart + x;
			triangles[triIndex++] = topRingStart + x + 1;
			triangles[triIndex++] = topCenter;
		}

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();

		return mesh;
	}

	public void Activate()
	{
		if (isActive)
		{
			return;
		}

		isActive = true;
		activationTime = Time.time;
		capsuleObject.SetActive(true);
		plasmaMaterial.SetFloat("_Activation", 0.01f);
	}

	public void Deactivate()
	{
		isActive = false;
		capsuleObject.SetActive(false);
		if (plasmaMaterial != null)
		{
			plasmaMaterial.SetFloat("_Activation", 0f);
		}
	}

	private void Update()
	{
		if (!isActive || plasmaMaterial == null) return;

		float elapsed = Time.time - activationTime;
		float activation = Mathf.Clamp01(elapsed * 3f);

		plasmaMaterial.SetFloat("_Activation", activation);

		float pulse = 1f + Mathf.Sin(elapsed * pulseSpeed) * 0.05f;
		capsuleObject.transform.localScale = new Vector3(pulse, 1f, pulse);
	}

	public bool IsActive => isActive;
}
