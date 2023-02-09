// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Mobile/ColorShifter" 
{

    Properties 
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Brightness ("(D:1.0f) Brightness", FLOAT) = 1.0
		_Contrast ("(D:0.0f) Contrast", FLOAT) = 0.0 
		_Hue ("(D:0.0f) Hue", FLOAT) = 0.0 
		_Saturation ("(D:1.0f) Saturation",FLOAT) = 1.0
		_Alpha ("(D:1.0f) Alpha",FLOAT) = 1.0

        // required for UI.Mask
        _StencilComp ("Stencil Comparison", Float) = 8
         _Stencil ("Stencil ID", Float) = 0
         _StencilOp ("Stencil Operation", Float) = 0
         _StencilWriteMask ("Stencil Write Mask", Float) = 255
         _StencilReadMask ("Stencil Read Mask", Float) = 255
         _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" }
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        // required for UI.Mask
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"
            float3 shift_col(float3 RGB, float3 shift)
            {
            	float3 RESULT = float3(RGB);
            	float VSU = shift.z*shift.y*cos(shift.x*3.14159265/180);
                float VSW = shift.z*shift.y*sin(shift.x*3.14159265/180);
                RESULT.x = (.299*shift.z+.701*VSU+.168*VSW)*RGB.x
                        + (.587*shift.z-.587*VSU+.330*VSW)*RGB.y
                        + (.114*shift.z-.114*VSU-.497*VSW)*RGB.z;                        
                RESULT.y = (.299*shift.z-.299*VSU-.328*VSW)*RGB.x
                        + (.587*shift.z+.413*VSU+.035*VSW)*RGB.y
                        + (.114*shift.z-.114*VSU+.292*VSW)*RGB.z;              
                RESULT.z = (.299*shift.z-.3*VSU+1.25*VSW)*RGB.x
                        + (.587*shift.z-.588*VSU-1.05*VSW)*RGB.y
                        + (.114*shift.z+.886*VSU-.203*VSW)*RGB.z;
            	return (RESULT);

            }
            struct v2f
            {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;

            };

            float4 _MainTex_ST;
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
            
            sampler2D _MainTex;
            float _Brightness;
            float _Contrast;
            float _Hue;
			float _Saturation;
			float _Alpha;
            half4 frag(v2f i) : COLOR
            {
                half4 col = tex2D(_MainTex, i.uv); 
                col.rgb = ((col.rgb - 0.5f) * max(_Contrast + 1.0f, -1.0f)) + 0.5f;
                float3 shift = float3(_Hue, _Saturation, _Brightness);
                return half4( half3(shift_col(col, shift)), col.a * _Alpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/Diffuse" //  it tells which shader should be used if no SubShaders from the current shader can run
}