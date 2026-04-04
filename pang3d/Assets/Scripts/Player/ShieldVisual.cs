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

	private GameObject _capsuleObject;
	private MeshFilter _meshFilter;
	private MeshRenderer _meshRenderer;
	private Material _plasmaMaterial;
	private bool _isActive;
	private float _activationTime;

	private void Awake()
	{
		_capsuleObject = new GameObject("ShieldCapsule");
		_capsuleObject.transform.SetParent(transform);
		_capsuleObject.transform.SetLocalPositionAndRotation(yOffset, Quaternion.identity);
		Mesh capsuleMesh = CreateCapsuleMesh(capsuleRadius, capsuleHeight);

		_meshFilter = _capsuleObject.AddComponent<MeshFilter>();
		_meshFilter.mesh = capsuleMesh;

		_meshRenderer = _capsuleObject.AddComponent<MeshRenderer>();
		_meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		_meshRenderer.receiveShadows = false;

		Shader shader = Shader.Find("Custom/PlasmaShield");
		if (shader == null)
		{
			shader = Shader.Find("Standard");
		}

		_plasmaMaterial = new Material(shader);
		_plasmaMaterial.SetColor("_Color", plasmaColor);
		_plasmaMaterial.SetColor("_GlowColor", glowColor);
		_plasmaMaterial.SetFloat("_Speed", animationSpeed);
		_plasmaMaterial.SetFloat("_Strength", distortionStrength);
		_plasmaMaterial.SetFloat("_FresnelPower", fresnelPower);
		_plasmaMaterial.SetFloat("_PulseSpeed", pulseSpeed);
		_plasmaMaterial.SetFloat("_Activation", 0f);
		_plasmaMaterial.SetFloat("_Mode", 3f);
		_plasmaMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		_plasmaMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		_plasmaMaterial.EnableKeyword("_ALPHABLEND_ON");
		_plasmaMaterial.renderQueue = 3000;

		_meshRenderer.material = _plasmaMaterial;
		_capsuleObject.SetActive(false);
	}

	private Mesh CreateCapsuleMesh(float radius, float height)
	{
		var mesh = new Mesh
		{
			name = "PlasmaCapsule"
		};

		int segments = 24;
		int heightSegments = 12;

		float halfHeight = height * 0.5f;
		float hemisphereHeight = radius;

		int vertCount = (segments + 1) * (heightSegments + 1) * 2 + segments * 2 + 2;
		Vector3[] vertices = new Vector3[vertCount];
		Vector3[] normals = new Vector3[vertCount];
		Vector2[] uvs = new Vector2[vertCount];
		int[] triangles = new int[(segments * (heightSegments * 6 + 6)) * 2 + segments * segments * 6];

		int vertIndex = 0;
		int triIndex = 0;

		float totalHeight = halfHeight + hemisphereHeight * 0.8f;

		for (int y = 0; y <= heightSegments; y++)
		{
			float v = (float)y / heightSegments;
			float posY = -totalHeight + v * totalHeight * 2;

			if (posY < -halfHeight + hemisphereHeight * 0.5f)
			{
				float t = Mathf.Clamp01((-halfHeight + hemisphereHeight * 0.5f - posY) / hemisphereHeight);
				posY = -halfHeight + hemisphereHeight * 0.5f - Mathf.Sqrt(1 - t * t) * hemisphereHeight * 0.5f;
			}
			else if (posY > halfHeight - hemisphereHeight * 0.5f)
			{
				float t = Mathf.Clamp01((posY - (halfHeight - hemisphereHeight * 0.5f)) / hemisphereHeight);
				posY = halfHeight - hemisphereHeight * 0.5f + Mathf.Sqrt(1 - t * t) * hemisphereHeight * 0.5f;
			}

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

				Vector3 outward = new Vector3(cos, 0, sin);
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
		if (_isActive) return;

		_isActive = true;
		_activationTime = Time.time;
		_capsuleObject.SetActive(true);
		_plasmaMaterial.SetFloat("_Activation", 0.01f);
	}

	public void Deactivate()
	{
		_isActive = false;
		_capsuleObject.SetActive(false);
		if (_plasmaMaterial != null)
		{
			_plasmaMaterial.SetFloat("_Activation", 0f);
		}
	}

	private void Update()
	{
		if (!_isActive || _plasmaMaterial == null) return;

		float elapsed = Time.time - _activationTime;
		float activation = Mathf.Clamp01(elapsed * 3f);

		_plasmaMaterial.SetFloat("_Activation", activation);

		float pulse = 1f + Mathf.Sin(elapsed * pulseSpeed) * 0.05f;
		_capsuleObject.transform.localScale = new Vector3(pulse, 1f, pulse);
	}

	public bool IsActive => _isActive;
}
