// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/LandingLightShader"
{
    Properties
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
        _Color ("Main Color", Color) = (1.000000,1.000000,1.000000,1.000000)
    }
    SubShader
    {
        LOD 100
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float speed = _Time * 20.0f;
                float4 wPos = mul(unity_ObjectToWorld, float4(0,0,0,-1)); // getting Object-Worldposition
                float sharpen = 0.5f;
                float intensity = (frac(wPos.x * 0.05f + speed) - sharpen) / (1.0f - sharpen);
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return fixed4(col.xyz, col.w * intensity * intensity);
            }
            ENDCG
        }
    }
}
