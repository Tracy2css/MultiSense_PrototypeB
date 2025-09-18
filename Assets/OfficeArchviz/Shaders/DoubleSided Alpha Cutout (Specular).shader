// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WILL/DoubleSided Alpha Cutout (Specular)"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Color("Color", Color) = (1,1,1,0)
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Normal_Power("Normal_Power", Float) = 1
		_SpecularColor("Specular Color", Color) = (0.5,0.5,0.5,0)
		_Specular("Specular", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_AmbientOcclusion("AmbientOcclusion", 2D) = "white" {}
		_AmbientOcclusion_Power("AmbientOcclusion_Power", Range( 0 , 1)) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Normal_Power;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _SpecularColor;
		uniform sampler2D _Specular;
		uniform float4 _Specular_ST;
		uniform float _Smoothness;
		uniform sampler2D _AmbientOcclusion;
		uniform float4 _AmbientOcclusion_ST;
		uniform float _AmbientOcclusion_Power;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ) ,_Normal_Power );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ( _Color * tex2DNode1 ).rgb;
			float2 uv_Specular = i.uv_texcoord * _Specular_ST.xy + _Specular_ST.zw;
			float4 tex2DNode3 = tex2D( _Specular, uv_Specular );
			o.Specular = ( _SpecularColor * tex2DNode3 ).rgb;
			o.Smoothness = ( tex2DNode3.a * _Smoothness );
			float2 uv_AmbientOcclusion = i.uv_texcoord * _AmbientOcclusion_ST.xy + _AmbientOcclusion_ST.zw;
			float lerpResult8 = lerp( 1.0 , tex2D( _AmbientOcclusion, uv_AmbientOcclusion ).g , _AmbientOcclusion_Power);
			o.Occlusion = lerpResult8;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=15401
7;264;1611;970;1609.037;345.3901;1.60079;True;True
Node;AmplifyShaderEditor.RangedFloatNode;20;-439.544,1193.934;Float;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-425,-127;Float;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-337.1136,-308.8366;Float;False;Property;_Color;Color;1;0;Create;True;0;0;False;0;1,1,1,0;0.8235294,0.8235294,0.8235294,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-496.842,944.9443;Float;True;Property;_AmbientOcclusion;AmbientOcclusion;8;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-510.0368,787.7769;Float;False;Property;_Smoothness;Smoothness;7;0;Create;True;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-490.1819,1307.161;Float;False;Property;_AmbientOcclusion_Power;AmbientOcclusion_Power;9;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-534.2626,533.9252;Float;True;Property;_Specular;Specular;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-766.5145,177.3633;Float;False;Property;_Normal_Power;Normal_Power;4;0;Create;True;0;0;False;0;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;21;-473.5328,349.2622;Float;False;Property;_SpecularColor;Specular Color;5;0;Create;True;0;0;False;0;0.5,0.5,0.5,0;0.5,0.5,0.5,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-425,130;Float;True;Property;_Normal;Normal;3;0;Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;8;-133.082,1107.462;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-107.5164,659.1887;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-90.11359,-186.6367;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-117.6481,326.5962;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;374.2907,-36.68009;Float;False;True;2;Float;;0;0;StandardSpecular;WILL/DoubleSided Alpha Cutout (Specular);False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;5;10;0
WireConnection;8;0;20;0
WireConnection;8;1;4;2
WireConnection;8;2;9;0
WireConnection;14;0;3;4
WireConnection;14;1;11;0
WireConnection;6;0;5;0
WireConnection;6;1;1;0
WireConnection;19;0;21;0
WireConnection;19;1;3;0
WireConnection;0;0;6;0
WireConnection;0;1;2;0
WireConnection;0;3;19;0
WireConnection;0;4;14;0
WireConnection;0;5;8;0
WireConnection;0;10;1;4
ASEEND*/
//CHKSM=1ACC5CBB65EC31BBBF676EFD98DA47D959C279E7