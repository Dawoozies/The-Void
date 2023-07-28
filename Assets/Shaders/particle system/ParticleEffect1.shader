Shader "Unlit/ParticleEffect1" {
Properties {
    _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    _MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend SrcAlpha OneMinusSrcAlpha
    //Blend DstColor SrcColor
    ColorMask RGB
    Cull Off Lighting Off ZWrite Off
    SubShader {
        Pass {
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles
            
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;
            
            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float4 texcoords : TEXCOORD0;
                float4 texcoords1 : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                //float2 texcoord2 : TEXCOORD1;
                //fixed blend : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            float4 _MainTex_ST;
            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                //v.vertex += float4(-10,0,0,0);
                
                float3 center = v.texcoords1.yzw;
                float3 velocity = float3(v.texcoords.zw,v.texcoords1.x);
                float speed = v.texcoords1.y;
                //v.vertex.xyz += velocity;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.vertex.xyz *= velocity.xyz;
                //v.vertex.xyz -= center;
                
                
                //at first pass o.vertex is our pivot point
                //float4 originalPosition = vertexBuffer.Load(0);

                
                //o.vertex.xyz += velocity;
                //o.vertex = mul(unity_ObjectToWorld, v.vertex);
                //o.color = v.color * _TintColor;
                
                
                //o.color = v.color*(smoothstep(0,0.001,abs(velocity.x)));
                //o.color = v.color*center.x;
                
                //o.texcoord = TRANSFORM_TEX(v.texcoords.xy,_MainTex);
                //o.texcoord2 = TRANSFORM_TEX(v.texcoords.zw,_MainTex);
                //o.blend = v.texcoordBlend;
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 colA = tex2D(_MainTex, i.texcoord);
                //fixed4 spriteRendererColor = tex2D(_MainTex, i.texcoord2);
                //fixed4 col = 2.0 * i.color * lerp(colA, colB, i.blend);
                return i.color;
                //return colA;
            }
            ENDCG 
        }
    }   
}
}