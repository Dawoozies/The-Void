Shader "Unlit/NewUnlitShader"
{
    Properties
    {
		_AlphaCutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_LineThreshold ("Line Threshold", Range(0.0, 3.0)) = 0.0
		//_VertexStretchDirection ("Vertex Stretch Direction", Vector) = (0,0,0,0)
		//_StretchX ("Stretch X Value", Range(0.0, 10.0)) = 1.0
		//_StretchY ("Stretch Y Value", Range(0.0, 10.0)) = 1.0
		//_ShearX ("Shear X Value", Range(-10.0, 10.0)) = 0.0
		//_ShearY ("Shear Y Value", Range(-10.0, 10.0)) = 0.0
		//_Color ("Tint", Color) = (0, 0, 0, 1)
		_UseOriginalColor ("Use Original Color (0 = No || 1 = Yes)", integer) = 1
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader{
		Tags{ 
			"RenderType"="Transparent" 
			"Queue"="Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite off
		Cull off

		Pass{

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;

			//fixed4 _Color;
			float _AlphaCutoff;
			float _LineThreshold;
			//float4 _VertexStretchDirection;
			//float _StretchX;
			//float _StretchY;
			//float _ShearX;
			//float _ShearY;
			int _UseOriginalColor;

			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert(appdata v){
				v2f o;
				//Fuck with this
				//This is what you want to mess with
				//float4 stretch = float4(_StretchX, _StretchY, 1, 1);
				//float4 shear = float4(_ShearX, _ShearY, 0,0);
				//float4 function = float4(v.vertex[0]*_ShearX + v.vertex[1]*_ShearY, v.vertex[0]*_ShearX + v.vertex[1]*_ShearY,1,1);
				//float4 vertex = v.vertex;
				//if(dot(v.vertex, _VertexStretchDirection) > 0)
				//{
				//	//then vertex is at least somewhat in the same direction
				//	vertex *= float4(_StretchX,_StretchY,1,1);
				//}
				//else
				//{
				//	if(dot(v.vertex, float4(1,1,1,1)) > 0)
				//	{
				//		 
				//	}
				//}
				//o.position = UnityObjectToClipPos(v.vertex)*stretch;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 output = fixed4(1,1,1,1);
				float magnitudeRGB = pow(col[0], 2)+pow(col[1], 2)+pow(col[2], 2);
				//output[3] *= col[3];
				//For just regular display of sprite
				//output[0] = col[0];
				//output[1] = col[1];
				//output[2] = col[2];
				//col is true color of Texture
				//i.color is the recolor via sprite renderer
				//at least at a basic level rn

				//This is for color override
				if(magnitudeRGB <= pow(_LineThreshold, 2))
				{
					//Sorry we want to know when the combinedRGB is near zero then we can say
					//this color must be black or close to it
					output[0] = 0;
					output[1] = 0;
					output[2] = 0;
					output[3] = step(_AlphaCutoff, col[3]);	
				}
				else
				{
					output[0] = i.color[0];
					output[1] = i.color[1];
					output[2] = i.color[2];
					output[3] = step(_AlphaCutoff, col[3])*i.color[3];
				}

				//If _UseOriginalColor == 1 then overwrite 0,1,2 entries with original color
				if(_UseOriginalColor == 1)
				{
					output[0] = col[0];
					output[1] = col[1];
					output[2] = col[2];
				}
				return output;
			}

			ENDCG
		}
	}
}
// 0 = r ... 3 = a