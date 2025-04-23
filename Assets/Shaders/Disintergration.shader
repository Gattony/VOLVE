Shader "Custom/URP_DisintegrateSprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Float) = 0
        _EdgeWidth ("Edge Width", Float) = 0.05
        _EdgeColor ("Edge Color", Color) = (1, 0.5, 0, 1)
        _Tiling ("Noise Tiling", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float _DissolveAmount;
            float _EdgeWidth;
            float4 _EdgeColor;
            float _Tiling;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 noiseUV = IN.uv * _Tiling;
                float noiseValue = tex2D(_NoiseTex, noiseUV).r;

                float dissolveThreshold = _DissolveAmount;

                float edge = smoothstep(dissolveThreshold - _EdgeWidth, dissolveThreshold, noiseValue);
                float alphaMask = step(noiseValue, dissolveThreshold);

                float4 texColor = tex2D(_MainTex, IN.uv);

                float edgeFactor = edge * (1 - alphaMask);

                float4 finalColor = lerp(_EdgeColor, texColor, alphaMask);
                finalColor.a *= alphaMask + edgeFactor;

                return finalColor;
            }
            ENDHLSL
        }
    }
}
