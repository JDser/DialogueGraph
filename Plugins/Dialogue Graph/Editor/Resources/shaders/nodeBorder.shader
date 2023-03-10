Shader "Hidden/DialogueGraph/NodeBorder"
{
    Properties
    {
        _Border("Border",float) = 1
        _Radius("Radius",float) = 1
        _PixelScale("PixelScale",float) = 1
        _Size("Size",Vector) = (100,100,0,0)
        _ColorStart("ColorStart",Color) = (1,1,0,1)
        _ColorEnd("ColorEnd", Color) = (0,1,1,1)
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
        LOD 100
        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
                float4 vertex : SV_POSITION;
                float4 pos : TEXCOORD2;
                float2 clipUV : TEXCOORD1;
                float height : TEXCOORD3;
            };

            float _Border;
            float _Radius;
            float _PixelScale;
            float2 _Size;
            fixed4 _ColorStart;
            fixed4 _ColorEnd;

            uniform float4x4 unity_GUIClipTextureMatrix;
            sampler2D _GUIClipTexture;

            v2f vert(appdata v)
            {
                v2f o;

                float2 size = _Size - float2(_Radius,_Radius);

                float margingScale = 2 + (_Border / _Radius / _PixelScale);

                o.pos = float4(v.vertex.xy * size + v.uv * margingScale * v.vertex.xy * _Radius, 0, 0);
                o.height = (v.vertex.y + 1) * 0.5;
                o.vertex = UnityObjectToClipPos(o.pos);
                o.uv = v.uv * margingScale;
                float3 eyePos = UnityObjectToViewPos(o.pos);
                o.clipUV = mul(unity_GUIClipTextureMatrix, float4(eyePos.xy, 0, 1.0));

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float pixelScale = 1.0f / abs(ddx(i.pos.x));

                float realRadius = (_Radius - _Border * 0.5 - 0.5); 
                float2 uvCenter = i.uv * _Radius / realRadius; 
                float uvDist = length(uvCenter); 
                float uvBorder = _Border * 0.5f / realRadius; 
                float borderDist = abs((uvDist - 1) / uvBorder); 
                
                float clipA = tex2D(_GUIClipTexture, i.clipUV).a;
                float pixelBorderSize = _Border * 0.5 * pixelScale;
                borderDist = pixelBorderSize * (1 - borderDist) + 0.5;

                fixed4 color = lerp(_ColorStart, _ColorEnd,i.height);
                return float4(color.rgb,color.a * saturate(borderDist) * clipA);
            }
            ENDCG
        }
    }
}