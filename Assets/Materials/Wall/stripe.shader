Shader "Unlit/stripe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tiling ("Tiling", Range(1, 500)) = 10
        _Direction ("Direction", Range(0, 1)) = 0

        _Brightness ("Brightness", Range(0, 2)) = 1 //*The intensity of the brightness*//
        _BlendFactor ("Blend Factor", Range(0, 1)) = 0.5  //**//

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            int _Tiling;

            float _Brightness; //* Added brightness variable *//
            float _BlendFactor; //* Added blend factor variable*//


            float _Direction;
            fixed4 frag (v2f i) : SV_Target
            {
                float pos = lerp(i.uv.x, i.uv.y, _Direction) * _Tiling;
                fixed value = floor(frac(pos) + 0.5);

                //* Get the texture color*//
                fixed4 texColor = tex2D(_MainTex, i.uv);

                //*Blend the texture color and the stripe color*//
                fixed4 stripeColor = value;
                fixed4 blendedColor = lerp(texColor, stripeColor, _BlendFactor);

                // Apply brightness
                blendedColor *= _Brightness;


                return blendedColor;
            }
            ENDCG
        }
    }
}
