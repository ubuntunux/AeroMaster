// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/cliff_shader"
{
    Properties
    {
        _BaseColor ("BaseColor", Color) = (1,1,1,1)
        _TopSurfaceColor ("TopSurfaceColor", Color) = (1,1,1,1)
        _TopSurfaceFactor ("TopSurfaceFactor", Range(0,1)) = 0.75
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float worldNormalY;
        };

        half _Glossiness;
        half _Metallic;
        half _TopSurfaceFactor;
        fixed4 _BaseColor;
        fixed4 _TopSurfaceColor;

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            o.worldNormalY = mul(v.normal, unity_ObjectToWorld).y;
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float d = (IN.worldNormalY * 0.5f + 0.5f);
            float t = _TopSurfaceFactor < d ? 1.0f : 0.0f;
            fixed4 color = tex2D (_MainTex, IN.uv_MainTex) * lerp(_BaseColor, _TopSurfaceColor, t);
            
            o.Albedo = color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
