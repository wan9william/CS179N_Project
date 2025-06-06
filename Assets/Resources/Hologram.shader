Shader "Custom/SpriteBars"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Frequency ("Bar Frequency", Float) = 5
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Frequency;


            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float frequency = _Frequency + _SinTime.z;
                fixed4 c = tex2D(_MainTex, i.uv) * _Color;
                float s = 0.5 * sin(6.28 * i.uv.y * frequency) + 0.5;
                //float bars = step(0.5, 0.5 * s + 0.5);
                c.a *= 0.5 * (s < 0.2 ? 0 : 1);
                return c;
            }
            ENDCG
        }
    }
}
