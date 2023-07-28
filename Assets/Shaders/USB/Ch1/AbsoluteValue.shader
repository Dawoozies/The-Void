Shader "Unlit/AbsoluteValue"
{
    Properties
    {
        _Rotation("Rotation", Range(0,360))=0
        _Zoom("Zoom", Range(0,1))=0
        _Speed("Rotation Speed", Range(0,3))=1
        _TanExampleColor("TanExampleColor", Color) = (1,1,1,1)
        _TanExampleSections("TanExampleSections", Range(2,10)) = 10

        [IntRange]_FloorExampleSections("FloorExampleSections", Range(2,10))=5
        _FloorExampleGamma("FloorExampleGamma", Range(0,1))=0

        _StepExampleEdge("StepExampleEdge", Range(0,1)) = 0.5
        
        _SmoothStepExampleEdge("SmoothStepExampleEdge", Range(0,1))=0.5
        _SmoothStepExampleSmoothingFactor("SmoothStepExampleSmoothingFactor", Range(0,1))=0.1

        _LengthExampleRadius("LengthExampleRadius", Range(0.0,0.5))=0.3
        _LengthExampleCenter("LengthExampleCenter", Range(0,1))=0.5
        _LengthExampleSmooth("LengthExampleSmooth", Range(0.0,0.5))=0.01

        _FracExampleSize("FracExampleSize", Range(0.0,0.5))=0.3

        _LerpExampleSkin01("LerpExampleSkin01", 2D) = "white" {}
        _LerpExampleSkin02("LerpExampleSkin02", 2D) = "black" {}
        _LerpExampleLerp("LerpExampleLerp", Range(0,1))=0.5

        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull off
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
                float2 uv_s01: TEXCOORD1;
                float2 uv_s02 : TEXCOORD2;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float2 uv_s01 : TEXCOORD1;
                float2 uv_s02 : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Rotation;
            float _Zoom;
            float _Speed;

            float4 _TanExampleColor;
            float _TanExampleSections;

            float _FloorExampleSections;
            float _FloorExampleGamma;

            float _StepExampleEdge;
            float _SmoothStepExampleEdge;
            float _SmoothStepExampleSmoothingFactor;

            float _LengthExampleRadius;
            float _LengthExampleCenter;
            float _LengthExampleSmooth;

            float _FracExampleSize;

            sampler2D _LerpExampleSkin01;
            float4 _LerpExampleSkin01_ST;
            sampler2D _LerpExampleSkin02;
            float4 _LerpExampleSkin02_ST;
            float _LerpExampleLerp;
            
            float3 rotation(float3 vertex)
            {
                float s = 0;
                float c = 0;
                sincos(_Time.y*_Speed, s, c);
                float3x3 rotY = float3x3
                (
                    c, 0, s,
                    0, 1, 0,
                    -s, 0, c
                );
                return mul(rotY, vertex);
            }
            v2f SinAndCosExample(appdata v)
            {
                v2f o;
                float3 rotVertex = rotation(v.vertex);
                o.vertex = UnityObjectToClipPos(rotVertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_s01 = TRANSFORM_TEX(v.uv_s01, _LerpExampleSkin01);
                o.uv_s02 = TRANSFORM_TEX(v.uv_s02, _LerpExampleSkin02);
                return o;
            }
            v2f vert (appdata v)
            {
                v2f oFinal;
                oFinal = SinAndCosExample(v);
                //o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                UNITY_TRANSFER_FOG(oFinal,oFinal.vertex);
                return oFinal;
            }

            void Unity_Rotate_Degrees_float
            (
                float2 UV,
                float2 Center,
                float Rotation,
                out float2 OUT
            )
            {
                Rotation = Rotation * (UNITY_PI/180.0);
                UV -= Center;
                float s = sin(Rotation);
                float c = sin(Rotation);
                float2x2 rMatrix = float2x2
                (
                    c, -s,
                    s, c
                );
                rMatrix *= 0.5;
                rMatrix += 0.5;
                rMatrix = 2*rMatrix-1;
                UV.xy = mul(UV.yx, rMatrix);
                UV += Center;
                OUT = UV;
            }
            float circle(float2 p, float center, float radius, float smooth)
            {
                float c = length(p-center);
                return smoothstep(c - smooth, c + smooth, radius);
            }
            fixed4 AbsValueExample(v2f i)
            {
                float2 absUV = abs(i.uv-float2(0.5,0.5));
                float rotation = _Rotation;
                float center = 0.5;
                float2 finalUV = 0;
                Unity_Rotate_Degrees_float(absUV, center, rotation, finalUV);
                fixed4 col = tex2D(_MainTex, finalUV);
                return col;
            }
            fixed4 CeilExample(v2f i)
            {
                float2 ceilUV = ceil(i.uv)*0.5;
                float2 finalUV = lerp(i.uv, ceilUV, _Zoom);
                fixed4 col = tex2D(_MainTex, finalUV);
                return col;
            }
            fixed4 TanExample(v2f i)
            {
                float4 tanCol = saturate(abs(tan((i.uv.y - _Time.x)*_TanExampleSections)));
                tanCol *= _TanExampleColor;
                fixed4 col = tex2D(_MainTex, i.uv)*tanCol;
                return col;
            }
            fixed4 FloorExample(v2f i)
            {
                float fv = floor(i.uv.y*_FloorExampleSections)*(_FloorExampleSections/100);
                //fixed4 col = tex2D(_MainTex, i.uv);
                return float4(fv.xxx,1)+_FloorExampleGamma;
            }
            fixed4 StepExample(v2f i)
            {
                fixed3 Step = 0;
                Step = step(i.uv.y, _StepExampleEdge);
                return fixed4(Step, 1);
            }
            fixed4 SmoothStepExample(v2f i)
            {
                fixed3 SmoothStep = 0;
                SmoothStep = smoothstep((i.uv.y-_SmoothStepExampleSmoothingFactor),(i.uv.y+_SmoothStepExampleSmoothingFactor),_SmoothStepExampleEdge);
                return fixed4(SmoothStep, 1);
            }
            fixed4 LengthExample(v2f i)
            {
                float c = circle(i.uv, _LengthExampleCenter, _LengthExampleRadius, _LengthExampleSmooth);
                return fixed4(c.xxx, 1);
            }
            fixed4 FracExample(v2f i)
            {
                i.uv *= 3;
                float2 fuv = frac(i.uv);
                float circle = length(fuv - 0.5);
                float wCircle = floor(_FracExampleSize/circle);
                return fixed4(wCircle.xxx,1);
            }
            fixed4 LerpExample(v2f i)
            {
                fixed4 skin01 = tex2D(_LerpExampleSkin01, i.uv_s01);
                fixed4 skin02 = tex2D(_LerpExampleSkin02, i.uv_s02);
                fixed4 render = lerp(skin01, skin02, _LerpExampleLerp);
                return render;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 output = fixed4(0,0,0,0);
                //output = AbsValueExample(i);
                //output = CeilExample(i);
                //output = TanExample(i);
                //output = FloorExample(i);
                //output = StepExample(i);
                //output = SmoothStepExample(i);
                //output = LengthExample(i);
                //output = FracExample(i);
                output = LerpExample(i);
                UNITY_APPLY_FOG(i.fogCoord, output);
                return output;
            }
            ENDCG
        }
    }
}
