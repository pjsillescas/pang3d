using UnityEngine;

public class ShieldVisual : MonoBehaviour
{
    [SerializeField] private float ringRadius = 0.8f;
    [SerializeField] private float ringThickness = 0.08f;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseAmount = 0.1f;
    [SerializeField] private float yOffset = 0.05f;
    [SerializeField] private Color ringColor = new Color(0.3f, 0.8f, 1f, 0.8f);
    [SerializeField] private Color glowColor = new Color(0.5f, 0.9f, 1f, 1f);
    [SerializeField] private Material ringMaterial;
    [SerializeField] private Material glowMaterial;

    private LineRenderer _ringRenderer;
    private LineRenderer _glowRenderer;
    private GameObject _ringObject;
    private GameObject _glowObject;
    private bool _isActive;
    private float _baseScale;
    private float _activationTime;

    private void Awake()
    {
        _ringObject = new GameObject("ShieldRing");
        _ringObject.transform.SetParent(transform);
        _ringObject.transform.localPosition = Vector3.zero;
        _ringObject.transform.localRotation = Quaternion.identity;

        _glowObject = new GameObject("ShieldGlow");
        _glowObject.transform.SetParent(transform);
        _glowObject.transform.localPosition = Vector3.zero;
        _glowObject.transform.localRotation = Quaternion.identity;

        _ringRenderer = _ringObject.AddComponent<LineRenderer>();
        _glowRenderer = _glowObject.AddComponent<LineRenderer>();

        SetupRingRenderer(_ringRenderer, ringColor, ringThickness);
        SetupRingRenderer(_glowRenderer, glowColor, ringThickness * 2.5f);

        DrawCircle(_ringRenderer, ringRadius);
        DrawCircle(_glowRenderer, ringRadius + ringThickness);

        _ringObject.SetActive(false);
        _glowObject.SetActive(false);
    }

    private void SetupRingRenderer(LineRenderer renderer, Color color, float width)
    {
        renderer.startWidth = width;
        renderer.endWidth = width;
        renderer.loop = true;
        renderer.positionCount = 65;
        renderer.useWorldSpace = false;

        if (ringMaterial != null)
        {
            renderer.material = ringMaterial;
        }
        else
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = color;
            renderer.material = mat;
        }

        renderer.startColor = color;
        renderer.endColor = color;
    }

    private void DrawCircle(LineRenderer renderer, float radius)
    {
        int segments = renderer.positionCount - 1;
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * 360f;
            float radian = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * radius;
            float z = Mathf.Sin(radian) * radius;
            renderer.SetPosition(i, new Vector3(x, yOffset, z));
        }
    }

    public void Activate()
    {
        if (_isActive) return;

        _isActive = true;
        _activationTime = Time.time;
        _ringObject.SetActive(true);
        _glowObject.SetActive(true);
    }

    public void Deactivate()
    {
        _isActive = false;
        _ringObject.SetActive(false);
        _glowObject.SetActive(false);
    }

    private void Update()
    {
        if (!_isActive) return;

        float elapsed = Time.time - _activationTime;
        float pulse = 1f + Mathf.Sin(elapsed * pulseSpeed) * pulseAmount;
        float breathe = 1f + Mathf.Sin(elapsed * pulseSpeed * 0.5f) * 0.05f;

        float activationFade = Mathf.Clamp01(elapsed * 3f);
        float scale = pulse * breathe * activationFade;

        _ringObject.transform.localScale = new Vector3(scale, scale, scale);
        _glowObject.transform.localScale = new Vector3(scale * 1.05f, scale * 1.05f, scale * 1.05f);

        Color ringCol = ringColor;
        ringCol.a *= activationFade;
        _ringRenderer.startColor = ringCol;
        _ringRenderer.endColor = ringCol;

        Color glowCol = glowColor;
        glowCol.a *= activationFade * 0.5f;
        _glowRenderer.startColor = glowCol;
        _glowRenderer.endColor = glowCol;
    }

    public bool IsActive => _isActive;
}
