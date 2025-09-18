// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WILL/Standard"
{
	Properties
	{
		_AlbedoColor("Albedo Color", Color) = (1,1,1,1)
		_Albedo("Albedo", 2D) = "white" {}
		[Normal]_Normals("Normals", 2D) = "bump" {}
		_NormalIntensity("Normal Intensity", Float) = 1
		_Metalic("Metalic", 2D) = "white" {}
		_MetalicIntensity("Metalic Intensity", Range( 0 , 1)) = 0
		_SmothnessIntensity("Smothness Intensity", Range( 0 , 1)) = 0.5
		_Occlusion("Occlusion", 2D) = "white" {}
		_OcclusionIntensity("Occlusion Intensity", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			half2 uv_texcoord;
		};

		uniform half _NormalIntensity;
		uniform sampler2D _Normals;
		uniform float4 _Normals_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform half4 _AlbedoColor;
		uniform sampler2D _Metalic;
		uniform float4 _Metalic_ST;
		uniform half _MetalicIntensity;
		uniform half _SmothnessIntensity;
		uniform sampler2D _Occlusion;
		uniform float4 _Occlusion_ST;
		uniform half _OcclusionIntensity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normals = i.uv_texcoord * _Normals_ST.xy + _Normals_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normals, uv_Normals ) ,_NormalIntensity );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = ( tex2D( _Albedo, uv_Albedo ) * _AlbedoColor ).rgb;
			float2 uv_Metalic = i.uv_texcoord * _Metalic_ST.xy + _Metalic_ST.zw;
			half4 tex2DNode67 = tex2D( _Metalic, uv_Metalic );
			o.Metallic = ( tex2DNode67 * _MetalicIntensity ).r;
			o.Smoothness = ( tex2DNode67.a * _SmothnessIntensity );
			half4 temp_cast_2 = (1.0).xxxx;
			float2 uv_Occlusion = i.uv_texcoord * _Occlusion_ST.xy + _Occlusion_ST.zw;
			float4 lerpResult56 = lerp( temp_cast_2 , tex2D( _Occlusion, uv_Occlusion ) , _OcclusionIntensity);
			o.Occlusion = lerpResult56.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=15401
7;29;2546;1364;1871.03;209.9749;1.3;True;True
Node;AmplifyShaderEditor.SamplerNode;4;-616.9632,830.5913;Float;True;Property;_Occlusion;Occlusion;7;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;54;-638.006,1192.015;Float;False;Property;_OcclusionIntensity;Occlusion Intensity;8;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-997.4802,213.1631;Float;False;Property;_NormalIntensity;Normal Intensity;3;0;Create;True;0;0;False;0;1;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;67;-631.8114,391.4552;Float;True;Property;_Metalic;Metalic;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;71;-520.37,1085.593;Float;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-641.8203,-220.1103;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;41;-548.3755,-18.63113;Float;False;Property;_AlbedoColor;Albedo Color;0;0;Create;True;0;0;False;0;1,1,1,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;66;-625.077,615.0067;Float;False;Property;_MetalicIntensity;Metalic Intensity;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-620.9904,696.0483;Float;False;Property;_SmothnessIntensity;Smothness Intensity;6;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;56;-185.294,980.7056;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;-629.1628,178.8854;Float;True;Property;_Normals;Normals;2;1;[Normal];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-184.0533,547.3076;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-309.1437,-80.19556;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-191.1833,397.6124;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;105.8787,300.8613;Half;False;True;2;Half;;0;0;Standard;WILL/Standard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;56;0;71;0
WireConnection;56;1;4;0
WireConnection;56;2;54;0
WireConnection;3;5;51;0
WireConnection;68;0;67;4
WireConnection;68;1;70;0
WireConnection;40;0;1;0
WireConnection;40;1;41;0
WireConnection;69;0;67;0
WireConnection;69;1;66;0
WireConnection;0;0;40;0
WireConnection;0;1;3;0
WireConnection;0;3;69;0
WireConnection;0;4;68;0
WireConnection;0;5;56;0
ASEEND*/
//CHKSM=551B01527514F485907DBE2C2420F1814ABDDA26