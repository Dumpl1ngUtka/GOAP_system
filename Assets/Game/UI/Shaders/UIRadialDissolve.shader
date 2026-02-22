Shader "UI/RadialDissolve"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        
        _Center ("Local Center", Vector) = (0,0,0,0)
        _Radius ("Local Radius", Float) = 100
        _Softness ("Softness", Float) = 20
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 localPosition : TEXCOORD1;
                float4 worldPosition : TEXCOORD2; // Needed for clipping
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            
            float4 _Center;
            float _Radius;
            float _Softness;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.worldPosition = v.vertex; // Unity UI passes world pos in v.vertex usually? No, it passes local usually but Canvas handles it.
                // Actually for UI, v.vertex is usually in Canvas space (which might be world space for World Space Canvas).
                // But we want Local Space of the RectTransform.
                // Wait, standard UI shader uses v.vertex as worldPosition for clipping.
                // If we want local position relative to the RectTransform pivot, we need to know that.
                // But v.vertex IS the local position relative to the pivot for a standard Image/Quad!
                
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.localPosition = v.vertex;
                OUT.worldPosition = v.vertex; // For clipping, usually world pos is needed. Let's check UnityUI.cginc
                
                // UnityGet2DClipping expects world position.
                // If v.vertex is local, we need to transform it to world?
                // In standard UI shader:
                // OUT.worldPosition = v.vertex;
                // OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                // This implies v.vertex IS world position (or whatever space the Canvas is in).
                
                // If the Canvas is Overlay, World Position = Screen Position (pixels).
                // If the Canvas is World Space, World Position = 3D World Position.
                
                // BUT, we want the effect to be local to the object.
                // If v.vertex is World Position, we can't easily get Local Position without passing a matrix or center.
                // However, we are passing _Center from script.
                // If we pass _Center as World Position from script, then we can use distance(worldPos, center).
                // But if the object moves, we have to update _Center every frame.
                
                // The user wanted "funnel to center".
                // If we use UVs, it's always local (0-1).
                // But UVs distort if the image is not square or if sliced.
                // Sliced images have distorted UVs.
                
                // Let's stick to World Space logic but update Center in Update() as I did in the script.
                // Wait, I changed the script to use local coordinates logic for radius calculation, 
                // BUT I am passing `_rectTransform.rect.center` as `_Center`.
                // `rect.center` is usually (0,0) if pivot is center.
                // If I pass (0,0) as center, and the shader uses World Position, it will be at world (0,0).
                
                // So, if I want to use Local Position in shader:
                // I need v.vertex to be local.
                // Is v.vertex local in UI?
                // "In the vertex shader, the input vertex position is in local space." - usually true for MeshRenderers.
                // For UI (CanvasRenderer), the mesh is generated.
                // If I use `OUT.worldPosition = v.vertex` and `UnityObjectToClipPos(v.vertex)`, 
                // `UnityObjectToClipPos` expects Object Space (Local).
                // So v.vertex IS Local Space.
                
                // So `OUT.localPosition = v.vertex` should give me local coordinates relative to Pivot.
                // And `_Center` passed as (0,0) (if pivot is center) should work.
                
                // However, for Clipping to work, we need World Position.
                // `UnityGet2DClipping` needs World Position.
                // `UnityObjectToClipPos` takes Local and makes Clip.
                // How do we get World for Clipping?
                // `mul(unity_ObjectToWorld, v.vertex)`
                
                OUT.worldPosition = mul(unity_ObjectToWorld, v.vertex);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                // Local Space Radial Dissolve
                // IN.localPosition is interpolated local position.
                // _Center is passed as Local Position (e.g. 0,0).
                
                float dist = distance(IN.localPosition.xy, _Center.xy);

                float alphaFactor = 1.0 - smoothstep(_Radius, _Radius + _Softness, dist);
                
                color.a *= alphaFactor;

                return color;
            }
        ENDCG
        }
    }
}