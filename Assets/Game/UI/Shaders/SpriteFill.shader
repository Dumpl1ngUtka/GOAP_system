Shader "Unlit/SpriteFill"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FillAmount ("Fill Amount", Range(0,1)) = 0
        _Color ("Tint + Alpha", Color) = (1, 1, 1, 0.3)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            float _FillAmount;
            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                clip(i.uv.x - (1 - _FillAmount));
                return col;
                return col;
            }
            ENDCG
        }
    }
}
