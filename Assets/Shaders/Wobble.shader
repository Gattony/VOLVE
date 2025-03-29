Shader "Custom/WobbleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WobbleStrength ("Wobble Strength", Float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _WobbleStrength;
            float2 _Velocity;

            v2f vert (appdata_t v)
            {
                v2f o;
                float wobble = sin(_Time.y * 10 + v.vertex.y * 5) * _WobbleStrength;
                v.vertex.x += _Velocity.x * wobble;
                v.vertex.y += _Velocity.y * wobble;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
