Shader "Custom/NPC_InteractShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _InteractStrength ("Interact Strength", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            fixed4 _BaseColor;
            float _InteractStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = _BaseColor;

                // efek interaksi: kuning + terang
                color.rgb += _InteractStrength * float3(0.6, 0.6, 0.1);

                return color;
            }
            ENDCG
        }
    }
}
