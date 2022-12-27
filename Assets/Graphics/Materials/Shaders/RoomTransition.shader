Shader "Unlit/RoomTransition"
{
    Properties
    {
        _MainTexture ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0,0,0)
        _GridS("Grid Size", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 worUv : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _GridS;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worUv = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = 1;//tex2D(_MainTex, i.uv);
                col.rg *= abs(i.uv - 0.5) * 2;
                i.uv *= 20;
                i.uv = floor(i.uv);
                i.uv /= 20;
                i.uv -= 0.5;
                i.uv = abs(i.uv);
                i.uv *= 1;
                float2 aUv = i.worUv;
                i.worUv += 9999;
                //i.worUv.y += sin((i.worUv + _Time.y));
                i.worUv *= 20;
                i.worUv = floor(i.worUv);
                i.worUv /= 20;
                i.worUv %= 0.5;
                i.worUv -= 0.25;
                i.worUv = abs(i.worUv);
                clip((min(i.worUv.x, i.worUv.y) > i.uv.x -.25) - 1);
                return tex2D(_MainTex, aUv);
            }
            ENDCG
        }
    }
}
