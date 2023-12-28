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
                _Pixels = abs(_Pixels);
                return float2 (
                20.0 * (1.0 / _Pixels),
                20.0 * (1.0 / _Pixels)
                );
            }
            float2 GetPixelatedPoint(float2 uv){
                
                float dy  =GetPixelSize().y;
                float dx = GetPixelSize().x;

                if (_Pixels < 500){
                    return uv;
                }

                float sampleX = dx * ceil(uv.x / dx);
                float sampleY = dy * ceil(uv.y / dy);

                return float2(sampleX,sampleY);
            }

            float GetDepthEdgeValue(float2 uv,float radius){

                // pixelated center
                uv = GetPixelatedPoint(uv);
                // int outline only. 
                radius = floor (radius);

                // Base radius to pixels
                float2 pixelSize = GetPixelSize();
                float2 pixOffset = float2(pixelSize.x * radius, pixelSize.y * radius); 

                // Get pixelated points to sample
                float2 uv0 = uv + float2( -pixOffset.x,-pixOffset.y);
                float2 uv1 = uv + float2( pixOffset.x,-pixOffset.y);
                float2 uv2 = uv + float2(-pixOffset.x,pixOffset.y);
                float2 uv3 = uv + float2(pixOffset.x,pixOffset.y);

                float cen = tex2D(_CameraDepthTexture, uv);

                // multipler to make difference around 1.0 value
                float depthToOne = 1 / cen;

                // Sample Points
                float d0 = tex2D(_CameraDepthTexture, uv0);
                float d1 = tex2D(_CameraDepthTexture, uv1);
                float d2 = tex2D(_CameraDepthTexture, uv2);
                float d3 = tex2D(_CameraDepthTexture, uv3);

                float result =  ( (d0 + d1 + d2 + d3) - cen * 4 ) * (depthToOne * 10);
                return  floor ( result )  ;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                float dx = 15.0 * (1.0 / _Pixels);
                float dy = 15.0 * (1.0 / _Pixels);

                float sampleX = dx * floor(i.uv.x / dx);
                float sampleY = dy * floor(i.uv.y / dy);
                float2 sameplePoint = float2( sampleX , sampleY );

                fixed4 col = tex2D(_MainTex,  GetPixelatedPoint( i.uv )  );
                float depth = tex2D (_CameraDepthTexture, GetPixelatedPoint(i.uv) ).r; 
                float edge = GetDepthEdgeValue( 
                    i.uv ,_OutlineLength );

                if (edge > 0.0){
                    float greyfloat = 0.1;
                    float4 solidblack = float4(greyfloat,greyfloat,greyfloat,1.0);
                    float4 newcol = lerp(col,solidblack,1.0);
                    col = newcol;
                    //col = float4(1,0,0,1);
                }

                return col;
            }
            ENDCG
        }


    }
}
