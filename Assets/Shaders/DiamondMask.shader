Shader "Custom/DiamondMask" {
    Properties{
        _DiamondPixelSize("Diamond Pixel Size", Range(0, 1)) = 0.1
        _Progress("Progress", Range(0, 1)) = 0
        _StartColor("Start Color", Color) = (.25, .5, .5, 1)
        _EndColor("End Color", Color) = (.25, .5, .5, 1)
    }

        SubShader{
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                float _DiamondPixelSize;
                float _Progress;
                float4 _StartColor;
                float4 _EndColor;

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target{
                    float2 FRAGCOORD = i.vertex.xy / i.vertex.w;
                    float2 UV = i.uv;

                    float xFraction = frac(FRAGCOORD.x / _DiamondPixelSize);
                    float yFraction = frac(FRAGCOORD.y / _DiamondPixelSize);

                    float xDistance = abs(xFraction - 0.5);
                    float yDistance = abs(yFraction - 0.5);

                    float4 colorOverTime = lerp(_StartColor, _EndColor, _Progress);

                    if (xDistance + yDistance + UV.x + UV.y > _Progress * 4) {
                        discard;
                    }

                    return fixed4(1,1,1,1) * colorOverTime;
                }
                ENDCG
            }
    }
}