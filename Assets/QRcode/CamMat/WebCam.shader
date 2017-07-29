// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/WebCam" {
    Properties {
        _MainTex("Base (RGB)", 2D) = "white" {}
        [MaterialToggle] isFlip("is Flip", float) = 0
    }
 
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            sampler2D _MainTex;
            fixed4 _MainTex_ST;
 			float isFlip;
            struct appdata {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };
 
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
  
            v2f vert (appdata v) {
				
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); 
                float x = v.texcoord.x ;
                float y = v.texcoord.y;
             	if(isFlip)
             	{
             		x = 1-x;
             	}
                o.uv =  float2(x, y);

                return o;
            }
   			
            fixed4 frag (v2f i) : COLOR {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}