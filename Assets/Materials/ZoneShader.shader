Shader "Unlit/ZoneShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1.000000,1.000000,1.000000,1.000000)
        [ShowAsVector2] _LineSpeed ("Line Speed", Vector) = (2.0, 2.0, 0.0, 0.0)
        _LineBrightness ("Line Brightness", Float) = 10.0
        _Brightness ("Brightness", Float) = 1.0
    }
    SubShader
    {
        LOD 100
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha One
        cull off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float2 _LineSpeed;
            float _LineBrightness;
            float _Brightness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 speed = _LineSpeed * _Time * _MainTex_ST.xy;
                float mask = (tex2D(_MainTex, i.uv + speed).w + tex2D(_MainTex, i.uv + speed * float2(0.5f, -0.5f)).w) * _LineBrightness;
                float inv_uv_y = (1.0f - i.uv.y);
                float opacity = _Color.w * mask * inv_uv_y;
                float flicker = sin(_Time.x * 100.0f) * 0.5f + 1.0f;
                fixed4 color = fixed4(_Color.xyz * inv_uv_y, opacity * flicker);
                color.xyz += _Color.xyz * inv_uv_y * _Brightness;
                color.w += _Color.w * flicker * inv_uv_y * _Brightness;

                return color;
            }
            ENDCG
        }
    }
}
