// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FireFX"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Fire_Speed("Fire_Speed", Vector) = (0,1,0,0)
		_Tiling("Tiling", Vector) = (1,1,0,0)
		[HDR]_Fire_Color("Fire_Color", Color) = (0.9150943,0.1424862,0,0)
		[HDR]_Fire_Top_Gradient("Fire_Top_Gradient", Color) = (1.830189,0.4919612,0.149638,0)
		[HDR]_Multiply("Multiply", Color) = (8.251307,0.6048079,0,0)
		_Fire_Power_Opacity("Fire_Power_Opacity", Float) = 2.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Multiply;
		uniform float4 _Fire_Top_Gradient;
		uniform float2 _Tiling;
		uniform float _Fire_Power_Opacity;
		uniform float4 _Fire_Color;
		uniform sampler2D _TextureSample0;
		uniform float2 _Fire_Speed;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TexCoord9 = i.uv_texcoord * _Tiling;
			float temp_output_12_0 = pow( uv_TexCoord9.y , _Fire_Power_Opacity );
			float4 temp_output_15_0 = ( _Fire_Top_Gradient * temp_output_12_0 );
			float4 tex2DNode1 = tex2D( _TextureSample0, ( uv_TexCoord9 + ( _Time.y * _Fire_Speed ) ) );
			float4 temp_output_13_0 = ( _Fire_Color * tex2DNode1 );
			float4 blendOpSrc35 = temp_output_15_0;
			float4 blendOpDest35 = temp_output_13_0;
			o.Emission = ( _Multiply * ( saturate( (( blendOpDest35 > 0.5 ) ? ( 1.0 - 2.0 * ( 1.0 - blendOpDest35 ) * ( 1.0 - blendOpSrc35 ) ) : ( 2.0 * blendOpDest35 * blendOpSrc35 ) ) )) ).rgb;
			o.Alpha = ( tex2DNode1.a * temp_output_12_0 );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17800
782.6667;300.6667;1133.333;651.6667;2028.098;449.818;2.66135;True;True
Node;AmplifyShaderEditor.SimpleTimeNode;5;-1609.662,-5.186795;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;6;-1595.705,78.37255;Inherit;False;Property;_Fire_Speed;Fire_Speed;1;0;Create;True;0;0;False;0;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;10;-1927.283,-174.9692;Inherit;False;Property;_Tiling;Tiling;2;0;Create;True;0;0;False;0;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1397.957,34.76206;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1671.608,-192.9457;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;11;-1347.086,600.4969;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;39;-1072.86,726.9948;Inherit;False;Property;_Fire_Power_Opacity;Fire_Power_Opacity;6;0;Create;True;0;0;False;0;2.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-1300.083,-158.9892;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-956.5882,-162.7198;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;3f6bc15b91f423f44a133386ff1a2f46;3f6bc15b91f423f44a133386ff1a2f46;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;-906.538,-401.1919;Inherit;False;Property;_Fire_Color;Fire_Color;3;1;[HDR];Create;True;0;0;False;0;0.9150943,0.1424862,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;16;-939.0759,120.3973;Inherit;False;Property;_Fire_Top_Gradient;Fire_Top_Gradient;4;1;[HDR];Create;True;0;0;False;0;1.830189,0.4919612,0.149638,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;12;-773.418,600.3702;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;2.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-545.0572,-179.718;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-620.0786,-433.785;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;35;-64.41495,-327.1609;Inherit;True;Overlay;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;37;-18.34798,-565.9315;Inherit;False;Property;_Multiply;Multiply;5;1;[HDR];Create;True;0;0;False;0;8.251307,0.6048079,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;343.6753,-427.8412;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-301.9074,-612.0039;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-106.9363,-68.59529;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;23;-1040.702,334.472;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;7;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;754.8694,-151.0305;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;FireFX;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;5;0
WireConnection;7;1;6;0
WireConnection;9;0;10;0
WireConnection;11;0;9;0
WireConnection;8;0;9;0
WireConnection;8;1;7;0
WireConnection;1;1;8;0
WireConnection;12;0;11;1
WireConnection;12;1;39;0
WireConnection;15;0;16;0
WireConnection;15;1;12;0
WireConnection;13;0;14;0
WireConnection;13;1;1;0
WireConnection;35;0;15;0
WireConnection;35;1;13;0
WireConnection;38;0;37;0
WireConnection;38;1;35;0
WireConnection;24;0;13;0
WireConnection;24;1;15;0
WireConnection;2;0;1;4
WireConnection;2;1;12;0
WireConnection;23;0;11;1
WireConnection;0;2;38;0
WireConnection;0;9;2;0
ASEEND*/
//CHKSM=A084680BF71CEB996F4CA6C6A8D603E0DA189AE1