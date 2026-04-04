Shader "Custom/DynamicReflectionSphere"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _ReflectionColor ("Reflection Color", Color) = (1, 1, 1, 1)
        _PatternScale ("Pattern Scale", Float) = 4.0
        _PatternSpeed ("Pattern Speed", Float) = 1.0
        _FresnelPower ("Fresnel Power", Float) = 2.0
        _ReflectionIntensity ("Reflection Intensity", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            float4 _BaseColor;
            float4 _ReflectionColor;
            float _PatternScale;
            float _PatternSpeed;
            float _FresnelPower;
            float _ReflectionIntensity;

            float3 hash3(float3 p)
            {
                p = float3(dot(p, float3(127.1, 311.7, 74.7)),
                           dot(p, float3(269.5, 183.3, 246.1)),
                           dot(p, float3(113.5, 271.9, 124.6)));
                return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
            }

            float voronoi(float2 uv, float time)
            {
                float2 p = floor(uv);
                float2 f = frac(uv);
                float res = 8.0;
                
                for (int j = -1; j <= 1; j++)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        float2 b = float2(i, j);
                        float2 r = b - f + 0.5 + 0.5 * sin(time + 6.2831 * hash3(float3(p + b, time * 0.1)).xy);
                        float d = dot(r, r);
                        res = min(res, d);
                    }
                }
                return sqrt(res);
            }

            float hexagonalPattern(float2 uv, float time)
            {
                float2 p = uv * _PatternScale;
                float t = time * _PatternSpeed;
                
                float2 q = float2(p.x * 2.0 * 0.5773503, p.y + p.x * 0.5773503);
                float2 pi = floor(q);
                float2 pf = frac(q);
                
                float v = fmod(pi.x + pi.y, 3.0);
                float ca = step(1.0, v);
                float cb = step(2.0, v);
                float ma = step(pf.x, pf.y);
                
                float2 e = float2(ca - ma * cb, (1.0 - ca) * (1.0 - ma));
                float2 o = (1.0 - e - e.yx) * (0.5 + 0.5 * sin(t + 6.2831 * hash3(float3(pi, t * 0.5)).x));
                
                return smoothstep(0.0, 0.5, 1.0 - length(pf - 0.5 - e + o));
            }

            float wavePattern(float2 uv, float time)
            {
                float t = time * _PatternSpeed;
                float2 p = uv * _PatternScale;
                
                float wave1 = sin(p.x * 3.0 + t * 2.0) * 0.5 + 0.5;
                float wave2 = sin(p.y * 4.0 - t * 1.5 + p.x) * 0.5 + 0.5;
                float wave3 = sin(length(p) * 2.0 - t * 3.0) * 0.5 + 0.5;
                
                return wave1 * wave2 * wave3;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float3 normal = normalize(i.worldNormal);
                
                float2 sphericalUV;
                //sphericalUV.x = atan2(normal.z, normal.x) / (2.0 * 3.14159) + 0.5;
                //sphericalUV.y = asin(normal.y) / 3.14159 + 0.5;
                
                sphericalUV.x = sqrt(normal.z*normal.z + normal.x*normal.x);
                sphericalUV.y = normal.y;
                
                float time = _Time.y;
                
                float vor = voronoi(sphericalUV * 6.0, time * 0.5);
                float hex = hexagonalPattern(sphericalUV, time);
                float wave = wavePattern(sphericalUV, time);
                
                //float pattern = vor * 0.4 + hex * 0.3 + wave * 0.3;
                float pattern = vor * 0.01 + hex*0 + wave * 0.7;
                pattern = pow(pattern, 1.2);
                
                float fresnel = pow(1.0 - max(0.0, dot(viewDir, normal)), _FresnelPower);
                
                float3 reflectionDir = reflect(-viewDir, normal);
                float reflectionAngle = dot(reflectionDir, float3(0.0, 1.0, 0.0)) * 0.5 + 0.5;
                
                float3 reflectColor = _ReflectionColor.rgb * pattern;
                reflectColor += _ReflectionColor.rgb * fresnel * 0.5;
                reflectColor *= _ReflectionIntensity;
                
                float3 finalColor = lerp(_BaseColor.rgb, reflectColor, fresnel * 0.8 + 0.2);
                finalColor += pattern * _ReflectionColor.rgb * _ReflectionIntensity * 0.3;
                
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}