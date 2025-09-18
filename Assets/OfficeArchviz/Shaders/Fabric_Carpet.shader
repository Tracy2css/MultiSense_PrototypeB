// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WILL/Fabric_Carpet"
{
	Properties
	{
		_AlbedoColor("Albedo Color", Color) = (1,1,1,1)
		_Albedo("Albedo", 2D) = "white" {}
		_RimColor("RimColor", Color) = (0,0,0,0)
		_RimPower("RimPower", Range( 0 , 10)) = 0
		[NoScaleOffset][Normal]_Normals("Normals", 2D) = "bump" {}
		_NormalIntensity("Normal Intensity", Float) = 1
		[NoScaleOffset]_Metalic("Metalic", 2D) = "white" {}
		_MetalicIntensity("Metalic Intensity", Range( 0 , 1)) = 0
		_SmothnessIntensity("Smothness Intensity", Range( 0 , 1)) = 0.5
		[NoScaleOffset]_Occlusion("Occlusion", 2D) = "white" {}
		_OcclusionIntensity("Occlusion Intensity", Range( 0 , 1)) = 1
		[NoScaleOffset]_HeightMap("HeightMap", 2D) = "white" {}
		_Scale("Scale", Range( 0 , 1)) = 0
		_CurvatureU("Curvature U", Range( 0 , 100)) = 0
		_CurvatureV("Curvature V", Range( 0 , 30)) = 0
		_RefPlane("Ref Plane", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[Header(Parallax Occlusion Mapping)]
		_CurvFix("Curvature Bias", Range( 0 , 1)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			half2 uv_texcoord;
			float3 viewDir;
			INTERNAL_DATA
			float3 worldNormal;
			float3 worldPos;
		};

		uniform half _NormalIntensity;
		uniform sampler2D _Normals;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _HeightMap;
		uniform half _Scale;
		uniform half _RefPlane;
		uniform float _CurvFix;
		uniform half _CurvatureU;
		uniform half _CurvatureV;
		uniform float4 _HeightMap_ST;
		uniform half4 _AlbedoColor;
		uniform half _RimPower;
		uniform half4 _RimColor;
		uniform sampler2D _Metalic;
		uniform half _MetalicIntensity;
		uniform half _SmothnessIntensity;
		uniform sampler2D _Occlusion;
		uniform half _OcclusionIntensity;


		inline float2 POM( sampler2D heightMap, float2 uvs, float2 dx, float2 dy, float3 normalWorld, float3 viewWorld, float3 viewDirTan, int minSamples, int maxSamples, float parallax, float refPlane, float2 tilling, float2 curv, int index )
		{
			float3 result = 0;
			int stepIndex = 0;
			int numSteps = ( int )lerp( (float)maxSamples, (float)minSamples, (float)dot( normalWorld, viewWorld ) );
			float layerHeight = 1.0 / numSteps;
			float2 plane = parallax * ( viewDirTan.xy / viewDirTan.z );
			uvs += refPlane * plane;
			float2 deltaTex = -plane * layerHeight;
			float2 prevTexOffset = 0;
			float prevRayZ = 1.0f;
			float prevHeight = 0.0f;
			float2 currTexOffset = deltaTex;
			float currRayZ = 1.0f - layerHeight;
			float currHeight = 0.0f;
			float intersection = 0;
			float2 finalTexOffset = 0;
			while ( stepIndex < numSteps + 1 )
			{
				result.z = dot( curv, currTexOffset * currTexOffset );
				currHeight = tex2Dgrad( heightMap, uvs + currTexOffset, dx, dy ).r * ( 1 - result.z );
				if ( currHeight > currRayZ )
				{
					stepIndex = numSteps + 1;
				}
				else
				{
					stepIndex++;
					prevTexOffset = currTexOffset;
					prevRayZ = currRayZ;
					prevHeight = currHeight;
					currTexOffset += deltaTex;
					currRayZ -= layerHeight * ( 1 - result.z ) * (1+_CurvFix);
				}
			}
			int sectionSteps = 10;
			int sectionIndex = 0;
			float newZ = 0;
			float newHeight = 0;
			while ( sectionIndex < sectionSteps )
			{
				intersection = ( prevHeight - prevRayZ ) / ( prevHeight - currHeight + currRayZ - prevRayZ );
				finalTexOffset = prevTexOffset + intersection * deltaTex;
				newZ = prevRayZ - intersection * layerHeight;
				newHeight = tex2Dgrad( heightMap, uvs + finalTexOffset, dx, dy ).r;
				if ( newHeight > newZ )
				{
					currTexOffset = finalTexOffset;
					currHeight = newHeight;
					currRayZ = newZ;
					deltaTex = intersection * deltaTex;
					layerHeight = intersection * layerHeight;
				}
				else
				{
					prevTexOffset = finalTexOffset;
					prevHeight = newHeight;
					prevRayZ = newZ;
					deltaTex = ( 1 - intersection ) * deltaTex;
					layerHeight = ( 1 - intersection ) * layerHeight;
				}
				sectionIndex++;
			}
			#ifdef UNITY_PASS_SHADOWCASTER
			if ( unity_LightShadowBias.z == 0.0 )
			{
			#endif
				if ( result.z > 1 )
					clip( -1 );
			#ifdef UNITY_PASS_SHADOWCASTER
			}
			#endif
			return uvs + finalTexOffset;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			half3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float2 appendResult74 = (half2(_CurvatureU , _CurvatureV));
			float2 OffsetPOM80 = POM( _HeightMap, uv_Albedo, ddx(uv_Albedo), ddy(uv_Albedo), ase_worldNormal, ase_worldViewDir, i.viewDir, 128, 128, _Scale, _RefPlane, _HeightMap_ST.xy, appendResult74, 0 );
			float2 myVarName081 = OffsetPOM80;
			float2 temp_output_83_0 = ddx( uv_Albedo );
			float2 temp_output_82_0 = ddy( uv_Albedo );
			half3 tex2DNode3 = UnpackScaleNormal( tex2D( _Normals, myVarName081, temp_output_83_0, temp_output_82_0 ) ,_NormalIntensity );
			o.Normal = tex2DNode3;
			float3 normalizeResult23 = normalize( i.viewDir );
			float dotResult21 = dot( tex2DNode3 , normalizeResult23 );
			o.Albedo = ( ( tex2D( _Albedo, myVarName081, temp_output_83_0, temp_output_82_0 ) * _AlbedoColor ) + ( pow( ( 1.0 - saturate( dotResult21 ) ) , _RimPower ) * _RimColor ) ).rgb;
			half4 tex2DNode67 = tex2D( _Metalic, myVarName081, temp_output_83_0, temp_output_82_0 );
			o.Metallic = ( tex2DNode67 * _MetalicIntensity ).r;
			o.Smoothness = ( tex2DNode67.a * _SmothnessIntensity );
			half4 temp_cast_2 = (1.0).xxxx;
			float4 lerpResult56 = lerp( temp_cast_2 , tex2D( _Occlusion, myVarName081, temp_output_83_0, temp_output_82_0 ) , _OcclusionIntensity);
			o.Occlusion = lerpResult56.r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=15401
137;334;1611;969;3651.788;314.0845;2.182678;True;True
Node;AmplifyShaderEditor.RangedFloatNode;73;-2140.161,1390.179;Float;False;Property;_CurvatureU;Curvature U;13;0;Create;True;0;0;False;0;0;0;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-2140.161,1470.179;Float;False;Property;_CurvatureV;Curvature V;14;0;Create;True;0;0;False;0;0;0;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;75;-1932.161,1198.179;Float;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;74;-1804.162,1374.179;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;78;-2582.159,1004.84;Float;True;Property;_HeightMap;HeightMap;11;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;79;-2048.161,858.6792;Float;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;76;-2084.161,1084.679;Float;False;Property;_Scale;Scale;12;0;Create;True;0;0;False;0;0;0.01;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-2094.948,1176.824;Float;False;Property;_RefPlane;Ref Plane;15;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ParallaxOcclusionMappingNode;80;-1669.063,985.079;Float;False;0;128;128;10;0.02;0;False;1,1;True;10,0;Texture2D;7;0;FLOAT2;0,0;False;1;SAMPLER2D;;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT2;0,0;False;6;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;22;-1964.647,270.3855;Float;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DdyOpNode;82;-1301.762,902.1793;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DdxOpNode;83;-1298.562,830.1794;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-2201.283,2.562882;Float;False;Property;_NormalIntensity;Normal Intensity;5;0;Create;True;0;0;False;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;81;-1375.984,736.0905;Float;False;myVarName0;1;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalizeNode;23;-1674.703,272.1001;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;3;-1832.961,-31.71479;Float;True;Property;_Normals;Normals;4;2;[NoScaleOffset];[Normal];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;21;-1483.003,193.6997;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;20;-1307.804,169.1996;Float;False;1;0;FLOAT;1.23;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;5;-1138.801,211.4989;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1235.402,326.8994;Float;False;Property;_RimPower;RimPower;3;0;Create;True;0;0;False;0;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-641.8203,-220.1103;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;25;-952.2028,412.8693;Float;False;Property;_RimColor;RimColor;2;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;26;-946.0033,234.8994;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;41;-548.3755,-18.63113;Float;False;Property;_AlbedoColor;Albedo Color;0;0;Create;True;0;0;False;0;1,1,1,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;71;-520.37,1085.593;Float;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-638.006,1192.015;Float;False;Property;_OcclusionIntensity;Occlusion Intensity;10;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-703.603,250.0991;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-625.077,615.0067;Float;False;Property;_MetalicIntensity;Metalic Intensity;7;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-616.9632,830.5913;Float;True;Property;_Occlusion;Occlusion;9;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;67;-631.8114,391.4552;Float;True;Property;_Metalic;Metalic;6;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-309.1437,-80.19556;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-620.9904,696.0483;Float;False;Property;_SmothnessIntensity;Smothness Intensity;8;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-179.4864,152.7165;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;56;-185.294,980.7056;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-184.0533,547.3076;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-191.1833,397.6124;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;105.8787,300.8613;Half;False;True;2;Half;;0;0;Standard;WILL/Fabric_Carpet;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;74;0;73;0
WireConnection;74;1;72;0
WireConnection;80;0;79;0
WireConnection;80;1;78;0
WireConnection;80;2;76;0
WireConnection;80;3;75;0
WireConnection;80;4;77;0
WireConnection;80;5;74;0
WireConnection;82;0;79;0
WireConnection;83;0;79;0
WireConnection;81;0;80;0
WireConnection;23;0;22;0
WireConnection;3;1;81;0
WireConnection;3;3;83;0
WireConnection;3;4;82;0
WireConnection;3;5;51;0
WireConnection;21;0;3;0
WireConnection;21;1;23;0
WireConnection;20;0;21;0
WireConnection;5;0;20;0
WireConnection;1;1;81;0
WireConnection;1;3;83;0
WireConnection;1;4;82;0
WireConnection;26;0;5;0
WireConnection;26;1;28;0
WireConnection;27;0;26;0
WireConnection;27;1;25;0
WireConnection;4;1;81;0
WireConnection;4;3;83;0
WireConnection;4;4;82;0
WireConnection;67;1;81;0
WireConnection;67;3;83;0
WireConnection;67;4;82;0
WireConnection;40;0;1;0
WireConnection;40;1;41;0
WireConnection;39;0;40;0
WireConnection;39;1;27;0
WireConnection;56;0;71;0
WireConnection;56;1;4;0
WireConnection;56;2;54;0
WireConnection;68;0;67;4
WireConnection;68;1;70;0
WireConnection;69;0;67;0
WireConnection;69;1;66;0
WireConnection;0;0;39;0
WireConnection;0;1;3;0
WireConnection;0;3;69;0
WireConnection;0;4;68;0
WireConnection;0;5;56;0
ASEEND*/
//CHKSM=78646CE045829A6A8C459760B9B69B4720A1E208