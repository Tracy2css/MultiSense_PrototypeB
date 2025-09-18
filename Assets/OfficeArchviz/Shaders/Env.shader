// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WILL/Env"
{
	Properties
	{
		[Gamma][Header(Cubemap)]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,1)
		_env("env", 2D) = "white" {}
		_Exposure("Exposure", Range( 0 , 8)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Background"  "Queue" = "Background+0" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _env;
		uniform float4 _env_ST;
		uniform half4 _TintColor;
		uniform half _Exposure;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_env = i.uv_texcoord * _env_ST.xy + _env_ST.zw;
			o.Emission = ( tex2D( _env, uv_env ) * unity_ColorSpaceDouble * _TintColor * _Exposure ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=15401
7;423;1611;970;868.167;440.9538;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;3;-474.167,449.0461;Half;False;Property;_Exposure;Exposure;3;0;Create;True;0;0;False;0;1;1;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorSpaceDouble;5;-441.167,61.04617;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-436.167,242.0461;Half;False;Property;_TintColor;Tint Color;1;1;[Gamma];Create;True;0;0;False;1;Header(Cubemap);0.5,0.5,0.5,1;0.5,0.5,0.5,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-475.167,-156.9538;Float;True;Property;_env;env;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-51.16699,5.046173;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;243.2001,-200;Float;False;True;2;Float;;0;0;Unlit;WILL/Env;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;True;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0;True;False;0;True;Background;;Background;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;1;0
WireConnection;4;1;5;0
WireConnection;4;2;2;0
WireConnection;4;3;3;0
WireConnection;0;2;4;0
ASEEND*/
//CHKSM=9D1DBB8CB6FCFA2081B35239689ADB0AA1375FC7