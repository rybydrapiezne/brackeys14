Shader "Custom/FogPixelatedWithLeftFade"
{
    Properties
    {
        _Color ("Fog Color", Color) = (1,1,1,0.5)
        _Octaves ("Octaves", Int) = 4
        _EnablePixelation ("Enable Pixelation", Float) = 1
        _PixelationAmount ("Pixelation Amount", Float) = 150
        _FogMask ("Fog Mask", 2D) = "white" {}
        _FogDirection ("Fog Direction", Vector) = (1,1,0,0)
        _ScrollNoiseTex ("Scroll Noise", Float) = 0
        _NoiseScrollDirection ("Noise Scroll Direction", Vector) = (1,0,0,0)
        _Alpha ("Alpha", Float) = 1
        _SpriteWidth("Sprite Width (px)", Float) = 256
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

            sampler2D _FogMask;
            float4 _Color;
            int _Octaves;
            float _EnablePixelation;
            float _PixelationAmount;
            float4 _FogDirection;
            float _ScrollNoiseTex;
            float4 _NoiseScrollDirection;
            float _Alpha;
            float _SpriteWidth;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // ---- Noise functions ----
            float rand(float2 coord)
            {
                return frac(sin(dot(coord, float2(56,78))) * 1000.0);
            }

            float noise(float2 coord)
            {
                float2 i = floor(coord);
                float2 f = frac(coord);

                float a = rand(i);
                float b = rand(i + float2(1.0,0.0));
                float c = rand(i + float2(0.0,1.0));
                float d = rand(i + float2(1.0,1.0));

                float2 cubic = f*f*(3.0-2.0*f);

                return lerp(a,b,cubic.x) + (c-a)*cubic.y*(1.0-cubic.x) + (d-b)*cubic.x*cubic.y;
            }

            float fbm(float2 coord, int octaves)
            {
                float value = 0.0;
                float scale = 0.5;
                for(int i=0;i<8;i++) // max 8 for safety
                {
                    if(i >= octaves) break;
                    value += noise(coord) * scale;
                    coord *= 2.0;
                    scale *= 0.5;
                }
                return value;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Pixelation
                if(_EnablePixelation > 0.5)
                {
                    uv = round(uv * _PixelationAmount) / _PixelationAmount;
                }

                float2 coord = uv * 20.0;

                // Scroll noise texture
                if(_ScrollNoiseTex > 0.5)
                {
                    coord += _NoiseScrollDirection.xy * _Time.y;
                }

                float2 motion = float2(fbm(coord + _FogDirection.xy * _Time.y, _Octaves),0.0);
                float finalFog = fbm(coord + motion, _Octaves);

                // Sample fog mask
                float maskValue = tex2D(_FogMask, i.uv).r;
                finalFog *= maskValue;

                // Left-side fade over 64 pixels
                float fadeWidth = 64.0 / _SpriteWidth; 
                float edgeFade = saturate(uv.x / fadeWidth);
                finalFog *= edgeFade;

                return float4(_Color.rgb, finalFog * _Color.a * _Alpha);
            }
            ENDHLSL
        }
    }
}
