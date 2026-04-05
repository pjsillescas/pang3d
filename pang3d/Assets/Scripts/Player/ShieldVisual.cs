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
		Mesh capsuleMesh = CreateCapsuleMesh(capsuleRadius, capsuleHeight);

		meshFilter = capsuleObject.AddComponent<MeshFilter>();
		meshFilter.mesh = capsuleMesh;

		meshRenderer = capsuleObject.AddComponent<MeshRenderer>();
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		meshRenderer.receiveShadows = false;

		Shader shader = null;// Shader.Find("Custom/PlasmaShield");
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
		int heightSegments = 12;

		float halfHeight = height * 0.5f;
		float hemisphereHeight = radius;

		int vertCount = (segments + 1) * (heightSegments + 1) * 2 + segments * 2 + 2;
		var vertices = new Vector3[vertCount];
		var normals = new Vector3[vertCount];
		var uvs = new Vector2[vertCount];
		int[] triangles = new int[(segments * (heightSegments * 6 + 6)) * 2 + segments * segments * 6];

		int vertIndex = 0;
		int triIndex = 0;

		float totalHeight = halfHeight + hemisphereHeight * 0.8f;

		for (int y = 0; y <= heightSegments; y++)
		{
			float v = (float)y / heightSegments;
			float posY = -totalHeight + v * totalHeight * 2;

			for (int x = 0; x <= segments; x++)
			{
				float u = (float)x / segments;
				float angle = u * Mathf.PI * 2;

				float adjustedY = posY;
				float radiusMod = radius;

				if (posY < -halfHeight + hemisphereHeight * 0.6f)
				{
					float sphereY = posY + halfHeight;
					float sphereR = hemisphereHeight * 0.6f;
					float distFromCenter = Mathf.Abs(sphereY) / sphereR;
					if (distFromCenter < 1f)
					{
						float remaining = Mathf.Sqrt(1 - distFromCenter * distFromCenter);
						radiusMod = radius * remaining;
					}
				}
				else if (posY > halfHeight - hemisphereHeight * 0.6f)
				{
					float sphereY = posY - halfHeight;
					float sphereR = hemisphereHeight * 0.6f;
					float distFromCenter = Mathf.Abs(sphereY) / sphereR;
					if (distFromCenter < 1f)
					{
						float remaining = Mathf.Sqrt(1 - distFromCenter * distFromCenter);
						radiusMod = radius * remaining;
					}
				}

				float cos = Mathf.Cos(angle);
				float sin = Mathf.Sin(angle);
				vertices[vertIndex] = new Vector3(cos * radiusMod, adjustedY, sin * radiusMod);

				var outward = new Vector3(cos, 0, sin);
				if (posY < -halfHeight + hemisphereHeight * 0.6f || posY > halfHeight - hemisphereHeight * 0.6f)
				{
					float sphereY = posY < 0 ? posY + halfHeight : posY - halfHeight;
					outward = (vertices[vertIndex] - new Vector3(0, sphereY, 0)).normalized;
				}
				normals[vertIndex] = outward;
				uvs[vertIndex] = new Vector2(u, v);
				vertIndex++;
			}
		}

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
