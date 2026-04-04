Shader "Custom/PlasmaShield"
{
    Properties
    {
        _Color ("Color", Color) = (0.3, 0.8, 1, 0.5)
        _GlowColor ("Glow Color", Color) = (0.5, 0.9, 1, 1)
        _Speed ("Animation Speed", Float) = 2.0
        _Strength ("Distortion Strength", Float) = 0.15
        _FresnelPower ("Fresnel Power", Float) = 2.0
        _PulseSpeed ("Pulse Speed", Float) = 3.0
        _Activation ("Activation", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+10" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float4 color : COLOR;
            };

            float4 _Color;
            float4 _GlowColor;
            float _Speed;
            float _Strength;
            float _FresnelPower;
            float _PulseSpeed;
            float _Activation;

            float noise(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float plasmaNoise(float3 p, float time)
            {
                float v1 = sin(p.x * 3.0 + time);
                float v2 = sin(p.y * 2.5 + time * 0.8);
                float v3 = sin(p.z * 3.5 + time * 1.2);
                float v4 = sin(length(p.xy) * 4.0 - time);
                float v5 = sin(length(p.xz) * 3.0 + time * 0.9);
                float v6 = sin(p.y * p.z * 2.0 + time * 1.1);
                
                float n1 = noise(p.xy * 2.0 + time * 0.3);
                float n2 = noise(p.yz * 2.5 + time * 0.4);
                float n3 = noise(p.xz * 1.8 + time * 0.5);
                
                return (v1 + v2 + v3 + v4 + v5 + v6) / 6.0 + 0.5 + (n1 + n2 + n3) / 9.0;
            }

            v2f vert (appdata v)
            {
                v2f o;
                
                float time = _Time.y * _Speed;
                
                float3 pos = v.vertex.xyz;
                float3 norm = v.normal;
                
                float3 noisePos = pos * 2.0 + time;
                float3 noisePos2 = pos * 3.0 - time * 0.7;
                
                float displacement = plasmaNoise(noisePos, time) * _Strength;
                float displacement2 = plasmaNoise(noisePos2, time * 0.8) * _Strength * 0.5;
                
                float3 displacedPos = pos + norm * (displacement + displacement2);
                
                float pulse = sin(_Time.y * _PulseSpeed) * 0.03;
                displacedPos += norm * pulse;
                
                o.vertex = UnityObjectToClipPos(displacedPos);
                o.worldPos = mul(unity_ObjectToWorld, displacedPos).xyz;
                o.worldNormal = UnityObjectToWorldNormal(norm);
                o.uv = v.uv;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                o.color = v.color;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);
                
                float fresnel = pow(1.0 - max(dot(normal, viewDir), 0.0), _FresnelPower);
                
                float time = _Time.y * _Speed;
                float plasma = plasmaNoise(i.worldPos * 1.5, time);
                float plasma2 = plasmaNoise(i.worldPos * 2.5 - time * 0.3, time * 1.2);
                
                float plasmaVariation = (plasma + plasma2) * 0.5;
                
                float4 baseColor = lerp(_Color, _GlowColor, plasmaVariation);
                
                float alpha = _Activation * (_Color.a + fresnel * 0.4 + plasmaVariation * 0.2);
                alpha = clamp(alpha, 0.0, 1.0);
                
                float glow = fresnel * 0.6 + plasmaVariation * 0.4;
                float4 finalColor = lerp(baseColor, _GlowColor, glow);
                finalColor.a = alpha;
                
                float edgeGlow = pow(1.0 - abs(dot(normal, viewDir)), 3.0);
                finalColor.rgb += _GlowColor.rgb * edgeGlow * 0.5 * _Activation;
                
                return finalColor;
            }
            ENDCG
        }
    }
}
