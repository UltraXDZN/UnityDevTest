Shader "Custom/UDT_Water" {
    Properties{
        _SurfCol("Surface Color", Color) = (1,1,1,1)
        _SurfColTex("Surface Color Tex", 2D) = "white" {}
        _SpecLvl("Specular Level", float) = 0.
        _SpecSize("Specular Size", Range(0.,1.)) = 0.5
        _SpecSmooth("Specular Smoothness", Range(0.,1.)) = 0.5
        _VolDirtStart("Volume dirt min distance", float) = 1.
        _VolDirtEnd("Volume dirt max distance", float) = 5.
        _VolDirtCol("Volume dirt Color", Color) = (1,1,1,1)
        _Roughness("Reflection Roughness", Range(0., 1.)) = 0.5
        _Roughness2("Refraction Roughness", Range(0., 1.)) = 0.5
        _IOR("IOR", float) = 1.333
        _WaveTex("Wave Texture", 2D) = "white" {}
        _Flow("Flow", Vector) = (0.1,0,0,0)
    }
        SubShader{

            Tags {
                "Queue" = "Transparent"
            }

            GrabPass{"_OpaquePass"}

            Pass {
                Tags {
                    "LightMode" = "ForwardBase"
                }

                ZWrite Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fwdbase
                #include "UnityCG.cginc"
                #include "AutoLight.cginc"
                #include "Lighting.cginc"

                fixed4 _SurfCol;
                sampler2D _SurfColTex;
                fixed _Shade;
                fixed _SpecLvl;
                fixed _SpecSize;
                fixed _SpecSmooth;
                fixed _VolDirtStart;
                fixed _VolDirtEnd;
                fixed4 _VolDirtCol;
                samplerCUBE _Cube;
                fixed _Roughness;
                fixed _IOR;
                sampler2D _WaveTex;
                fixed2 _Flow;

                struct x2v {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float4 uv : TEXCOORD0;
                };

                struct v2f {
                    float4 pos : SV_POSITION;
                    float3 worldNormal : NORMAL;
                    float4 uv : TEXCOORD0;
                    float4 objPos : TEXCOORD3;
                    float3 viewDir : TEXCOORD1;
                    UNITY_SHADOW_COORDS(2)
                };

                float3 SurfaceFunction(sampler2D BumpRaw, float2 flow, float2 UV) {
                    float pi = 3.14159265f;
                    float steppedTime1 = 10 * _Time.x - floor(10 * _Time.x);
                    float steppedTime2 = 16 * _Time.x - floor(16 * _Time.x);
                    float steppedTime3 = steppedTime1 + 0.5 - floor(steppedTime1 + 0.5);
                    float4 comp1 = (tex2D(BumpRaw, UV + steppedTime1 * flow) * 2. - 1.) * pow(sin(steppedTime1 * pi), 2.);
                    float4 comp2 = (tex2D(BumpRaw, UV + float2(-0.3, 0.4) + (steppedTime2 - 0.3) * flow) * 2. - 1.) * pow(sin(steppedTime2 * pi), 2.);
                    float4 comp3 = (tex2D(BumpRaw, UV + float2(0.5, 0.8) + (steppedTime3 + 0.5) * flow) * 2. - 1.) * pow(sin(steppedTime3 * pi), 2.);
                    float height = (comp1.a + comp2.a + comp3.a * 2.) / 4.;
                    float normalsx = comp1.x + comp2.x + comp3.x * 2.;
                    float normalsy = comp1.y + comp2.y + comp3.y * 2.;
                    return float3(normalsx, normalsy, height);
                }

                v2f vert(x2v v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    o.uv = v.uv;
                    o.objPos = v.vertex;
                    o.viewDir = WorldSpaceViewDir(v.vertex);
                    TRANSFER_SHADOW(o);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target{
                    float3 normal = normalize(i.worldNormal);

                    float2 WaveFunction = float2(0.05 * sin(10. * i.objPos.x + sin(5 * i.objPos.z) + 3. * _Time.y), 0.);

                    WaveFunction = 10. * SurfaceFunction(_WaveTex, _Flow.xy, i.uv);

                    normal = normalize(normal + float3(WaveFunction.x, 0., WaveFunction.y));

                    float shadow = SHADOW_ATTENUATION(i);
                    float DotPr = clamp(dot(_WorldSpaceLightPos0, normal) - 1. + shadow, 0., 1.) * _Shade + 1. - _Shade;
                    float3 viewDir = normalize(i.viewDir);
                    float3 refl = dot(normal, normalize(_WorldSpaceLightPos0 + viewDir));
                    float4 c = _LightColor0 * _SurfCol * tex2D(_SurfColTex, i.uv);

                    float DiffuseFac = DotPr;
                    float3 DiffusePart = c.xyz * DiffuseFac;

                    float SpecSizeMod = 1. - pow(_SpecSize, 5);
                    float SpecularFac = smoothstep(SpecSizeMod - _SpecSmooth, SpecSizeMod + _SpecSmooth, refl) * _SpecLvl;
                    float3 SpecularPart = c.xyz * SpecularFac;

                    float3 reflectedDir = reflect(-viewDir, normal);

                    half4 skyData = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectedDir, _Roughness);
                    half3 skyColor = DecodeHDR(skyData, unity_SpecCube0_HDR);

                    //float3 ReflectionPart = texCUBE(_Cube, reflectedDir);
                    float3 ReflectionPart = skyColor;

                    return half4(DiffusePart + SpecularPart + (ReflectionPart * c.a), c.a);
                }
                ENDCG
            }

            Pass{
                Tags {
                    "LightMode" = "Always"
                }

                Blend OneMinusDstAlpha DstAlpha

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct x2v {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float4 uv : TEXCOORD0;
                };

                struct v2f {
                    float4 grabPos : TEXCOORD0;
                    float4 pos : SV_POSITION;
                    float4 objPos : TEXCOORD3;
                    float3 Normal : NORMAL;
                    float4 screenuv : TEXCOORD1;
                    float4 uv : TEXCOORD2;
                };

                v2f vert(x2v v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.objPos = v.vertex;
                    o.uv = v.uv;
                    o.grabPos = ComputeGrabScreenPos(o.pos);
                    o.Normal = UnityObjectToWorldNormal(v.normal);
                    o.screenuv = ComputeScreenPos(o.pos);
                    return o;
                }

                sampler2D _OpaquePass;
                sampler2D _CameraDepthTexture;
                fixed _VolDirtStart;
                fixed _VolDirtEnd;
                fixed4 _VolDirtCol;
                fixed _IOR;
                fixed _Roughness2;
                sampler2D _WaveTex;

                fixed2 _Flow;

                float3 SurfaceFunction(sampler2D BumpRaw, float2 flow, float2 UV) {
                    float pi = 3.14159265f;
                    float steppedTime1 = 10 * _Time.x - floor(10 * _Time.x);
                    float steppedTime2 = 16 * _Time.x - floor(16 * _Time.x);
                    float steppedTime3 = steppedTime1 + 0.5 - floor(steppedTime1 + 0.5);
                    float4 comp1 = (tex2D(BumpRaw, UV + steppedTime1 * flow) * 2. - 1.) * pow(sin(steppedTime1 * pi), 2.);
                    float4 comp2 = (tex2D(BumpRaw, UV + float2(-0.3, 0.4) + (steppedTime2 - 0.3) * flow) * 2. - 1.) * pow(sin(steppedTime2 * pi), 2.);
                    float4 comp3 = (tex2D(BumpRaw, UV + float2(0.5, 0.8) + (steppedTime3 + 0.5) * flow) * 2. - 1.) * pow(sin(steppedTime3 * pi), 2.);
                    float height = (comp1.a + comp2.a + comp3.a * 2.) / 4.;
                    float normalsx = comp1.x + comp2.x + comp3.x * 2.;
                    float normalsy = comp1.y + comp2.y + comp3.y * 2.;
                    return float3(normalsx, normalsy, height);
                }

                half4 frag(v2f i) : SV_Target {
                    float2 uv = i.screenuv.xy / i.screenuv.w;
                    float2 WaveFunction = float2(0.05 * sin(10. * i.objPos.x + sin(5 * i.objPos.z) + 3. * _Time.y), 0.);
                    WaveFunction = 10. * SurfaceFunction(_WaveTex, _Flow.xy, i.uv);
                    float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(i.screenuv.z);
                    float backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
                    float depthDifference = backgroundDepth - surfaceDepth - 0.25;
                    float2 uvOffset = WaveFunction * (_IOR - 1.) * min(depthDifference, 1.);
                    uv = uv + uvOffset;
                    backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
                    depthDifference = backgroundDepth - surfaceDepth - 0.25;
                    if (depthDifference <= 0.) {
                        uv = i.screenuv.xy / i.screenuv.w;
                    }
                    backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
                    depthDifference = backgroundDepth - surfaceDepth - 0.25;
                    float4 BackgroundColor = tex2D(_OpaquePass, uv);
                    float MainGradient = smoothstep(_VolDirtStart, _VolDirtEnd, depthDifference) * _VolDirtCol.a;
                    float3 OutputColor = _VolDirtCol.xyz * MainGradient + BackgroundColor.xyz * (1. - MainGradient);

                    return half4(OutputColor, BackgroundColor.a);
                }
                ENDCG
            }
        }
        Fallback Off
}