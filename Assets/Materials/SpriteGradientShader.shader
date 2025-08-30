Shader "Custom/SpriteGradientAlpha"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1,1,1,1)
        _GradientDirection ("Gradient Direction", Vector) = (1,0,0,0) // X=left→right, Y=bottom→top
        _FadeStart ("Fade Start", Float) = 0
        _FadeEnd ("Fade End", Float) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
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

            float4 _Color;
            float4 _GradientDirection;
            float _FadeStart;
            float _FadeEnd;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Compute fade based on UV along the gradient direction
                float t = dot(i.uv, _GradientDirection.xy); 
                float alphaFade = saturate((t - _FadeStart)/(_FadeEnd - _FadeStart));
                return float4(_Color.rgb, _Color.a * alphaFade);
            }
            ENDHLSL
        }
    }
}
