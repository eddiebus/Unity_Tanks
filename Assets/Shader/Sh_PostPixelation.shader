Shader "Custom/Sh_PostPixelation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Pixels ("Pixels", int) = 1000
        _OutlineLength ("OutLine", float) = 2.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            int _Pixels;
            float _OutlineLength;
            sampler2D _MainTex;
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            float2 GetPixelSize(){
                return float2 (
                20.0 * (1.0 / _Pixels),
                20.0 * (1.0 / _Pixels)
                );
            }
            float2 GetPixelatedPoint(float2 uv){
                float dx = 20.0 * (1.0 / _Pixels);
                float dy = 20.0 * (1.0 / _Pixels);

                float sampleX = dx * ceil(uv.x / dx);
                float sampleY = dy * ceil(uv.y / dy);

                return float2(sampleX,sampleY);
            }

            float GetDepthEdgeValue(float2 uv,float radius){
                uv = GetPixelatedPoint(uv);
                float2 uv0 = uv + float2(0 ,-radius );
                float2 uv1 = uv + float2(0 ,radius );
                float2 uv2 = uv + float2(radius ,0 );
                float2 uv3 = uv + float2(-radius ,0);

                float cen = tex2D(_CameraDepthTexture, uv);

                float d0 = tex2D(_CameraDepthTexture, uv0);
                float d1 = tex2D(_CameraDepthTexture, uv1);
                float d2 = tex2D(_CameraDepthTexture, uv2);
                float d3 = tex2D(_CameraDepthTexture, uv3);

                float depth = length(float2(d1 - d0,d3 - d2));
                float res1 = abs( round ( depth * 80) );
                float result = abs ( (d0 + d1 + d2 + d3) - cen * 4 ) * 900;
                return  floor( result ) ;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dx = 15.0 * (1.0 / _Pixels);
                float dy = 15.0 * (1.0 / _Pixels);

                float sampleX = dx * floor(i.uv.x / dx);
                float sampleY = dy * floor(i.uv.y / dy);
                float2 sameplePoint = float2( sampleX , sampleY );

                fixed4 col = tex2D(_MainTex,  GetPixelatedPoint( i.uv )  );
                float edge = GetDepthEdgeValue( 
                    i.uv ,0.001 * _OutlineLength );

                if (edge > 0.1){
                    float4 solidblack = float4(0,0,0,0.9);
                    float4 newcol = lerp(col,solidblack,0.8);
                    col = newcol;
                    //col = float4(1,0,0,1);
                }

                //col = float4(edge,0,0,1);
                return col;
            }
            ENDCG
        }


    }
}
