Shader "Custom/Water"
{
    Properties
    {
        _WaterColor("Water Color", Color) = (0.0, 0.0, 1.0, 1.0)
        _ReflectionColor("Reflection Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _ReflectionThreshold("Reflection Threshold", Range(0, 1)) = 0.5
        _ReflectionRange("Reflection Range", Range(0, 0.1)) = 0.05
        _FlowSpeed1("Flow Speed 1", Range(0, 1)) = 0.1
        _FlowSpeed2("Flow Speed 2", Range(0, 1)) = 0.2
        _Size1("Size 1", Range(0, 5)) = 1
        _Size2("Size 2", Range(0, 5)) = 1
        _Noise("Noise", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _Noise;

        struct Input
        {
            float2 uv_Noise;
            float3 worldPos;
        };

        fixed4 _WaterColor;
        fixed4 _ReflectionColor;
        half _ReflectionThreshold;
        half _ReflectionRange;
        half _Size1;
        half _Size2;
        half _FlowSpeed1;
        half _FlowSpeed2;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed4 interpolation(float t, fixed4 a, fixed4 b)
        {
            // float x = exp(-pow(t - _ReflectionThreshold, 2) / (_ReflectionRange));
            
            // float x = abs(t - _ReflectionThreshold) < _ReflectionRange;
            float y = smoothstep(_ReflectionThreshold - _ReflectionRange * 2, _ReflectionThreshold + _ReflectionRange * 2, t);
            float x = 1 - abs(y - .5) * 2;

            return lerp(a, b, x);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float noise1 = tex2D(_Noise, (IN.worldPos.xz) / _Size1 + _Time * _FlowSpeed1).r;
            float noise2 = tex2D(_Noise, (IN.worldPos.xz) / _Size2 + _Time * _FlowSpeed2).r;
            float noise = noise1 + noise2;

            fixed4 color = interpolation(noise, _WaterColor, _ReflectionColor);

            o.Albedo = color.rgb;
            o.Metallic = 0;
            o.Smoothness = 0;
            o.Alpha = color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
