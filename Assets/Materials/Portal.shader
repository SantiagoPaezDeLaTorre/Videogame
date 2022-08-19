// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Portal"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "bump" {}
		_NormalMapScale("NormalMapScale", Range( 0 , 1)) = 0
		_TimeScale("TimeScale", Float) = 0
		_PortalColor("PortalColor", Color) = (0.4764151,1,0.5113207,0)
		_Float0("Float 0", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform sampler2D _TextureSample0;
		uniform float _TimeScale;
		uniform float _NormalMapScale;
		uniform float4 _PortalColor;
		uniform float _Float0;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float mulTime59 = _Time.y * _TimeScale;
			float cos63 = cos( mulTime59 );
			float sin63 = sin( mulTime59 );
			float2 rotator63 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos63 , -sin63 , sin63 , cos63 )) + float2( 0.5,0.5 );
			float4 screenColor64 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + float4( UnpackScaleNormal( tex2D( _TextureSample0, rotator63 ), _NormalMapScale ) , 0.0 ) ).xy);
			float4 lerpResult77 = lerp( screenColor64 , ( _PortalColor * screenColor64 ) , _Float0);
			o.Emission = lerpResult77.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
0;0;1920;1019;359.192;750.4408;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;70;-1855.285,-310.3952;Inherit;False;768.9714;569.5783;GenerateRotation;5;60;59;61;62;63;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-1805.285,143.183;Inherit;False;Property;_TimeScale;TimeScale;2;0;Create;True;0;0;0;False;0;False;0;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;71;-1089.205,-102.7048;Inherit;False;604.8459;680.3381;Normal Map;2;53;54;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;62;-1682.653,-260.3952;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;61;-1653.782,-77.03406;Inherit;False;Constant;_Vector1;Vector 1;3;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;59;-1639.161,81.54546;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-1039.205,318.6333;Inherit;True;Property;_NormalMapScale;NormalMapScale;1;0;Create;True;0;0;0;False;0;False;0;0.028;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;72;-635.0701,-549.1687;Inherit;False;488.3236;398.3086;Get Screen current texture;2;65;66;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RotatorNode;63;-1296.314,-30.7287;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GrabScreenPosition;65;-610.6694,-489.5682;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;53;-804.3586,-52.70483;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;04fcaf755a84b9e40b595f9598869355;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;73;20.60526,-514.7747;Inherit;False;562.8595;491.45;Recolorize;3;64;68;67;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-349.9031,-373.8138;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenColorNode;64;107.8051,-212.2245;Inherit;False;Global;_GrabScreen0;Grab Screen 0;3;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;67;121.4832,-464.7746;Inherit;False;Property;_PortalColor;PortalColor;3;0;Create;True;0;0;0;False;0;False;0.4764151,1,0.5113207,0;1,0.8250326,0.7783019,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;76;11.13598,-894.4898;Inherit;False;594.1555;359.0346;Depth Fade mask;2;75;74;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;79;716.7966,-200.423;Inherit;False;Property;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;1;0.3949253;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;462.1644,-217.5254;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;75;61.136,-794.4554;Inherit;False;Property;_DepthFadeDistance;DepthFadeDistance;4;0;Create;True;0;0;0;False;0;False;0;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;78;656.5953,-628.4421;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;77;834.6949,-508.6588;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;74;337.2933,-844.4898;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1090.667,-276.8247;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Portal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Front;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;59;0;60;0
WireConnection;63;0;62;0
WireConnection;63;1;61;0
WireConnection;63;2;59;0
WireConnection;53;1;63;0
WireConnection;53;5;54;0
WireConnection;66;0;65;0
WireConnection;66;1;53;0
WireConnection;64;0;66;0
WireConnection;68;0;67;0
WireConnection;68;1;64;0
WireConnection;78;0;74;0
WireConnection;77;0;64;0
WireConnection;77;1;68;0
WireConnection;77;2;79;0
WireConnection;74;0;75;0
WireConnection;0;2;77;0
ASEEND*/
//CHKSM=A5501F5186D956BEC82F32351B42C4204FDF0365