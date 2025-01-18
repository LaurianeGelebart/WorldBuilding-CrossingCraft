Shader "Unlit/Water"
{
    Properties
    {
        _ColorSurfaceTop ("Foam", Color) = (1,1,1,1)
        _ColorSurfaceBottom ("Caustics", Color) = (1,1,1,1)
        _ColorDeep ("Depth", Color) = (1,1,1,1)
        _TopThreshold ("Foam Threshold", Range(0, 1)) = 0.2
        _BottomThreshold ("Caustics Threshold", Range(0, 1)) = 0.2
        _Scale ("Scale", Range(0, 3)) = 1
        _Speed ("Speed", Range(0, 2)) = 1
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            uint hash(uint x, uint seed) {
                const uint m = 0x5bd1e995;
                uint hash = seed;
                // process input
                uint k = x;
                k *= m;
                k ^= k >> 24;
                k *= m;
                hash *= m;
                hash ^= k;
                // some final mixing
                hash ^= hash >> 13;
                hash *= m;
                hash ^= hash >> 15;
                return hash;
            }
            
            uint hash(uint3 x, uint seed) {
                const uint m = 0x5bd1e995;
                uint hash = seed;
                // process first vector element
                uint k = x.x;
                k *= m;
                k ^= k >> 24;
                k *= m;
                hash *= m;
                hash ^= k;
                // process second vector element
                k = x.y;
                k *= m;
                k ^= k >> 24;
                k *= m;
                hash *= m;
                hash ^= k;
                // process third vector element
                k = x.z;
                k *= m;
                k ^= k >> 24;
                k *= m;
                hash *= m;
                hash ^= k;
                // some final mixing
                hash ^= hash >> 13;
                hash *= m;
                hash ^= hash >> 15;
                return hash;
            }
            
            float3 gradientDirection(uint hash) {
                switch (int(hash) & 15) {
                case 0:
                    return float3(1, 1, 0);
                case 1:
                    return float3(-1, 1, 0);
                case 2:
                    return float3(1, -1, 0);
                case 3:
                    return float3(-1, -1, 0);
                case 4:
                    return float3(1, 0, 1);
                case 5:
                    return float3(-1, 0, 1);
                case 6:
                    return float3(1, 0, -1);
                case 7:
                    return float3(-1, 0, -1);
                case 8:
                    return float3(0, 1, 1);
                case 9:
                    return float3(0, -1, 1);
                case 10:
                    return float3(0, 1, -1);
                case 11:
                    return float3(0, -1, -1);
                case 12:
                    return float3(1, 1, 0);
                case 13:
                    return float3(-1, 1, 0);
                case 14:
                    return float3(0, -1, 1);
                case 15:
                    return float3(0, -1, -1);
                }
                return float3(0, 0, 0); // fallback
            }
            
            float interpolate(float value1, float value2, float value3, float value4, float value5, float value6, float value7, float value8, float3 t) {
                return lerp(
                    lerp(lerp(value1, value2, t.x), lerp(value3, value4, t.x), t.y),
                    lerp(lerp(value5, value6, t.x), lerp(value7, value8, t.x), t.y),
                    t.z
                );
            }
            
            float3 fade(float3 t) {
                return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
            }
            
            float perlinNoise(float3 position, uint seed) {
                float3 floorPosition = floor(position);
                float3 fractPosition = position - floorPosition;
                uint3 cellCoordinates = uint3(floorPosition);
                float value1 = dot(gradientDirection(hash(cellCoordinates, seed)), fractPosition);
                float value2 = dot(gradientDirection(hash((cellCoordinates + uint3(1, 0, 0)), seed)), fractPosition - float3(1, 0, 0));
                float value3 = dot(gradientDirection(hash((cellCoordinates + uint3(0, 1, 0)), seed)), fractPosition - float3(0, 1, 0));
                float value4 = dot(gradientDirection(hash((cellCoordinates + uint3(1, 1, 0)), seed)), fractPosition - float3(1, 1, 0));
                float value5 = dot(gradientDirection(hash((cellCoordinates + uint3(0, 0, 1)), seed)), fractPosition - float3(0, 0, 1));
                float value6 = dot(gradientDirection(hash((cellCoordinates + uint3(1, 0, 1)), seed)), fractPosition - float3(1, 0, 1));
                float value7 = dot(gradientDirection(hash((cellCoordinates + uint3(0, 1, 1)), seed)), fractPosition - float3(0, 1, 1));
                float value8 = dot(gradientDirection(hash((cellCoordinates + uint3(1, 1, 1)), seed)), fractPosition - float3(1, 1, 1));
                return interpolate(value1, value2, value3, value4, value5, value6, value7, value8, fade(fractPosition));
            }
    
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
                float4 worldPos : TEXCOORD1;
            };

            fixed4 _ColorDeep;
            fixed4 _ColorSurfaceTop;
            fixed4 _ColorSurfaceBottom;
            float _TopThreshold;
            float _BottomThreshold;
            float _Scale;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.worldPos.xz * _Scale + 1000;
                uint seed = 0x578437adU;
                float2 t = float2(_Time.y, 0) * _Speed;

                float value1 = perlinNoise(1.5 * float3(uv + t * .25, 0), seed) * 2.0;
                value1 += perlinNoise(4.0 * float3(uv + t * 1., 0), seed) * .25;
                value1 = value1 * 0.5 + 0.5;
    
                float value2 = perlinNoise(2.0 * float3(uv + t * .5, 0), seed) * 2.0;
                value2 += perlinNoise(3.0 * float3(uv + t * 1., 0), seed) * 1.;
                value2 = value2 * 0.5 + 0.5;
    
                bool foam = abs(value1 - 0.5) < _TopThreshold;
                bool caustics = abs(value2 - 0.5) < _BottomThreshold;
    
                fixed4 c = _ColorDeep;
                c = lerp(c, _ColorSurfaceBottom, caustics ? _ColorSurfaceBottom.a : 0);
                c = lerp(c, _ColorSurfaceTop, foam ? _ColorSurfaceTop.a : 0);
                
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                fixed4 col = c;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
