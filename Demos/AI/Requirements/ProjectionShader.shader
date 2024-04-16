Shader "Custom/TextureProjectionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // _ProjTex ("Projector Texture", 2D) = "black" {}
        // _ProjMatrix ("Projection Matrix", Matrix) = {}
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
                float4 vertex : SV_POSITION;
                float4 projCoord : TEXCOORD1;
            };

            sampler2D _MainTex;
            // sampler2D _ProjTex;
            float4x4 _ProjMatrix;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.projCoord = mul(_ProjMatrix, mul(unity_ObjectToWorld,v.vertex));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Normalize the projected coordinates
                float2 projUV = i.projCoord.xy / i.projCoord.w;
                projUV = 0.5 * projUV + 0.5; // Transform to 0-1 range

                // Sample the projected texture
                fixed4 projColor = tex2D(_MainTex, projUV);

                // Sample the main texture
                // fixed4 col = tex2D(_MainTex, i.uv);

                // Simple blend of textures
                return projColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
