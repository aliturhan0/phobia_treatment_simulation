Shader "Custom/WaterShader"
{
    Properties
    {
        // --- ORTAK DEĞİŞKENLER ---
        _BaseColor   ("Base Color", Color) = (0.20,0.70,0.95,1)
        _PatternTex  ("Water Pattern (RGB/A)", 2D) = "white" {}

        _ContactFoamDist ("Contact Foam Distance", Range(0.01,5.0)) = 0.5  
        _ContactFoamBoost ("Contact Foam Boost", Range(0,10)) = 5.0  
        _ContactSoftness ("Contact Softness", Range(0.5,5)) = 2.0
        _ContactEdgeScale ("Contact Edge Scale", Range(0,500)) = 150.0 
        _Alpha      ("Alpha", Range(0,1)) = 1

        [HideInInspector]_RippleColor  ("Ripple Color", Color) = (0.05,0.45,0.95,1)
        [HideInInspector]_RippleDensity ("Ripple Density", Float) = 2.0
        [HideInInspector]_RippleSpeed  ("Ripple Speed (XY)", Vector) = (0.6, 0.35, 0, 0)
        [HideInInspector]_RippleSine  ("Ripple Sine (freq,amp)", Vector) = (8.0, 0.22, 0, 0)
        [HideInInspector]_RippleSharp  ("Ripple Sharpness", Range(0.5,6)) = 2.8

        [HideInInspector]_EdgeWidth   ("Edge Width", Range(0,1)) = 0.28
        [HideInInspector]_EdgeGateR   ("Edge Gate", Range(0.0,1.0)) = 0.85
        [HideInInspector]_CenterLS   ("Center (Local)", Vector) = (0,0,0,0)
        [HideInInspector]_Radius    ("Radius (m)", Float) = 5.0
        [HideInInspector][NoScaleOffset]_MaskTex ("Mask", 2D) = "white" {}
        
        [HideInInspector]_PatternTiling ("Pattern Tiling", Float) = 0.6
        [HideInInspector]_PatternSpeed ("Pattern Speed XY", Vector) = (0.05, 0.03, 0, 0)
        _PatternContrast("Pattern Contrast", Range(0.5,6)) = 2.0
        _PatternAmount ("Pattern Amount", Range(0,1.5)) = 0.35
        
        [HideInInspector][NoScaleOffset]_FoamTex ("Foam Texture", 2D) = "white" {}
    }

    // ----------------------------------------------------------------
    // 1. SUB-SHADER: URP (Universal Render Pipeline)
    // ----------------------------------------------------------------
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            TEXTURE2D(_MaskTex); SAMPLER(sampler_MaskTex);
            TEXTURE2D(_FoamTex); SAMPLER(sampler_FoamTex);
            TEXTURE2D(_PatternTex); SAMPLER(sampler_PatternTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor, _RippleColor;
                float _RippleDensity; float4 _RippleSpeed; float4 _RippleSine; float _RippleSharp;
                float4 _CenterLS; float _Radius; float _EdgeWidth; float _EdgeGateR;
                float _PatternTiling; float4 _PatternSpeed;
                float _PatternContrast; float _PatternAmount;
                float _ContactFoamDist, _ContactFoamBoost, _ContactSoftness, _ContactEdgeScale;
                float _Alpha;
            CBUFFER_END

            struct Attributes { float4 posOS:POSITION; float2 uv:TEXCOORD0; };
            struct Varyings {
                float4 posHCS:SV_POSITION;
                float3 posWS :TEXCOORD0;
                float3 posLS :TEXCOORD1;
                float2 uv  :TEXCOORD2;
                float4 scr  :TEXCOORD3;
                float fog  :TEXCOORD4;
            };

            Varyings vert(Attributes i){
                Varyings o;
                VertexPositionInputs vp=GetVertexPositionInputs(i.posOS.xyz);
                o.posHCS=vp.positionCS; o.posWS=vp.positionWS; o.posLS=i.posOS.xyz;
                o.uv=i.uv; o.scr=ComputeScreenPos(vp.positionCS); o.fog=ComputeFogFactor(vp.positionCS.z);
                return o;
            }

            float radial01_local(float2 xz){ float2 c=_CenterLS.xz; float d=length(xz-c); return saturate(d/max(_Radius,1e-3)); }

            float shorelineMask(float2 uv, float2 xzLS) {
                float a = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, uv).a;
                float hasMask = step(0.001, a) * step(a, 0.999);
                float w = max(fwidth(a)*2.0, 0.001);
                float ring = 1.0 - smoothstep(0.5 - _EdgeWidth - w, 0.5 + _EdgeWidth + w, a);
                float r = radial01_local(xzLS);
                float radial = 1.0 - smoothstep(1.0 - _EdgeWidth, 1.0, r);
                float gate = smoothstep(_EdgeGateR, 1.0, r);
                ring *= gate; radial *= gate;
                return lerp(radial, ring, hasMask);
            }

            float contactFoam(float4 scrPos) {
                float2 uv = scrPos.xy / scrPos.w;
                float scene01 = SampleSceneDepth(uv); 
                float sceneEye = LinearEyeDepth(scene01, _ZBufferParams);
                float surfEye  = scrPos.w;
                float depthDiff = max(0.0, sceneEye - surfEye);
                float d = saturate(depthDiff / max(_ContactFoamDist, 1e-4));
                return saturate(pow(1.0 - d, _ContactSoftness) * _ContactFoamBoost); 
            }

            float patternLuma(float3 posWS) {
                float2 p = posWS.xz * _PatternTiling + _Time.y * _PatternSpeed.xy;
                float3 s1 = SAMPLE_TEXTURE2D(_PatternTex, sampler_PatternTex, p).rgb;
                float3 s2 = SAMPLE_TEXTURE2D(_PatternTex, sampler_PatternTex, p*1.313 + 5.21).rgb;
                float3 s = s1*0.6 + s2*0.4;
                float l   = dot(s, float3(0.299,0.587,0.114));
                float aa = fwidth(l) * 1.5; float t = 0.5;
                return smoothstep(t - aa, t + aa, pow(saturate(l), _PatternContrast));
            }

            half4 frag(Varyings i):SV_Target {
                float r = radial01_local(i.posLS.xz);
                float3 baseCol = lerp(_RippleColor.rgb, _BaseColor.rgb, pow(r, 1.6));
                float pat = patternLuma(i.posWS);
                float3 water = lerp(baseCol, baseCol + _PatternAmount, pat * saturate(r * 1.6));
                float edgeM = shorelineMask(i.uv, i.posLS.xz);
                float foam  = edgeM + contactFoam(i.scr);
                float3 col = lerp(water, 1.0.xxx, saturate(foam));
                col = MixFog(col, i.fog);
                return half4(col, _Alpha);
            }
            ENDHLSL
        }
    }

    // ----------------------------------------------------------------
    // 2. SUB-SHADER: BUILT-IN - SERT (SHARP) VERSİYON
    // ----------------------------------------------------------------
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            sampler2D _MaskTex; float4 _MaskTex_ST;
            sampler2D _FoamTex; sampler2D _PatternTex;
            sampler2D _CameraDepthTexture; 

            float4 _BaseColor, _RippleColor;
            float _RippleDensity; float4 _RippleSpeed; float4 _RippleSine; float _RippleSharp;
            float4 _CenterLS; float _Radius; float _EdgeWidth; float _EdgeGateR;
            float _PatternTiling; float4 _PatternSpeed;
            float _PatternContrast; float _PatternAmount;
            float _ContactFoamDist, _ContactFoamBoost, _ContactSoftness, _ContactEdgeScale;
            float _Alpha;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f {
                float4 pos : SV_POSITION; float3 posWS : TEXCOORD0; float3 posLS : TEXCOORD1;
                float2 uv : TEXCOORD2; float4 screenPos : TEXCOORD3; UNITY_FOG_COORDS(4)
            };

            v2f vert (appdata v) {
                v2f o; o.pos = UnityObjectToClipPos(v.vertex);
                o.posWS = mul(unity_ObjectToWorld, v.vertex).xyz; o.posLS = v.vertex.xyz;
                o.uv = v.uv; o.screenPos = ComputeScreenPos(o.pos);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            float radial01_local(float2 xz){ float2 c=_CenterLS.xz; float d=length(xz-c); return saturate(d/max(_Radius,1e-3)); }

            float shorelineMask(float2 uv, float2 xzLS) {
                float a = tex2D(_MaskTex, uv).a;
                float hasMask = step(0.001, a) * step(a, 0.999);
                float w = max(fwidth(a)*2.0, 0.001);
                float ring = 1.0 - smoothstep(0.5 - _EdgeWidth - w, 0.5 + _EdgeWidth + w, a);
                float r = radial01_local(xzLS);
                float radial = 1.0 - smoothstep(1.0 - _EdgeWidth, 1.0, r);
                float gate = smoothstep(_EdgeGateR, 1.0, r);
                ring *= gate; radial *= gate;
                return lerp(radial, ring, hasMask);
            }

            // BUILT-IN İÇİN ÖZEL SHARP FOAM
            float contactFoam(float4 screenPos) {
                float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(screenPos)));
                float surfZ = screenPos.w;
                float depthDiff = max(0.0, sceneZ - surfZ);

                // FIX: "Smoothluk" problemini çözmek için
                // Derinlik farkını çok daha hızlı "doyuma" ulaştırıyoruz.
                // Normalde _ContactFoamDist'e bölerken, burada 3 kat daha hızlı bitiriyoruz.
                float val = depthDiff / max(_ContactFoamDist, 0.001);
                val = saturate(val);

                // FIX: Klasik pow yerine 'smoothstep' hilesi.
                // Bu işlem gradient'i sıkıştırır ve bulanık kenarları yok eder.
                // _ContactSoftness ayarı artık köpüğün "keskinliğini" daha agresif kontrol eder.
                float sharpFactor = _ContactSoftness * 3.0; // Keskinliği zorla artır
                float foam = pow(1.0 - val, sharpFactor);
                
                // Ekstra keskinlik için thresholding:
                // Köpüğü en uçta 1.0, biraz içeride hemen 0.0 yap
                foam = smoothstep(0.2, 0.9, foam); 

                return foam * _ContactFoamBoost;
            }

            float patternLuma(float3 posWS) {
                float2 p = posWS.xz * _PatternTiling + _Time.y * _PatternSpeed.xy;
                float3 s1 = tex2D(_PatternTex, p).rgb; float3 s2 = tex2D(_PatternTex, p*1.313 + 5.21).rgb;
                float3 s = s1*0.6 + s2*0.4;
                float l = dot(s, float3(0.299, 0.587, 0.114));
                float aa = fwidth(l) * 1.5; float t = 0.5;
                // Pattern kontrastını built-in için artır
                return smoothstep(t - aa, t + aa, pow(saturate(l), _PatternContrast * 1.5));
            }

            fixed4 frag (v2f i) : SV_Target {
                float r = radial01_local(i.posLS.xz);
                float3 baseCol = lerp(_RippleColor.rgb, _BaseColor.rgb, pow(r, 1.6));
                
                float pat = patternLuma(i.posWS);
                
                // Pattern amount boost
                float3 water = lerp(baseCol, baseCol + (_PatternAmount * 2.5), pat * saturate(r * 1.6));
                
                float edgeM = shorelineMask(i.uv, i.posLS.xz);
                float foam = edgeM + contactFoam(i.screenPos);
                
                // Köpük rengini kesin beyaz yapmak için saturate
                float3 col = lerp(water, float3(1,1,1), saturate(foam));
                UNITY_APPLY_FOG(i.fogCoord, col);
                return fixed4(col, _Alpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}