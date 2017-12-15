﻿Shader "Fur/Fur" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_FurLength ("Fur Length", Range (.0002, 1)) = 0.1
		_Cutoff ("Alpha cutoff", Range(0,1)) = 00
		_CutoffEnd ("Alpha cutoff end", Range(0,1)) = 0.5
		_EdgeFade ("Edge Fade", Range (0,1)) = 0.4

		_Gravity ("Gravity direction", Vector) = (0,0,1,0)
		_GravityStrength ("Gravity strength", Range(0,1)) = 0.25

		_StrandTex ("Strand Colors", 2D) = "white" {}
		_StrandColorStrength("Strand Color Multiply Strength", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 200
		Cull Back
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		UNITY_INSTANCING_CBUFFER_START(Props)

		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.05
		#include "FurPass.cginc"
		ENDCG
//		CGPROGRAM
//		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
//		#define FUR_MULTIPLIER 0.10
//		#include "FurPass.cginc"
//		ENDCG
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.15
		#include "FurPass.cginc"
		ENDCG
//		CGPROGRAM
//		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
//		#define FUR_MULTIPLIER 0.20
//		#include "FurPass.cginc"
//		ENDCG
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.25
		#include "FurPass.cginc"
		ENDCG
//		CGPROGRAM
//		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
//		#define FUR_MULTIPLIER 0.30
//		#include "FurPass.cginc"
//		ENDCG
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.35
		#include "FurPass.cginc"
		ENDCG
//		CGPROGRAM
//		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
//		#define FUR_MULTIPLIER 0.40
//		#include "FurPass.cginc"
//		ENDCG
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.45
		#include "FurPass.cginc"
		ENDCG
//		CGPROGRAM
//		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
//		#define FUR_MULTIPLIER 0.50
//		#include "FurPass.cginc"
//		ENDCG
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.55
		#include "FurPass.cginc"
		ENDCG
//		CGPROGRAM
//		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
//		#define FUR_MULTIPLIER 0.60
//		#include "FurPass.cginc"
//		ENDCG
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.65
		#include "FurPass.cginc"
		ENDCG
//		CGPROGRAM
//		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
//		#define FUR_MULTIPLIER 0.70
//		#include "FurPass.cginc"
//		ENDCG
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.75
		#include "FurPass.cginc"
		ENDCG
//		CGPROGRAM
//		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
//		#define FUR_MULTIPLIER 0.80
//		#include "FurPass.cginc"
//		ENDCG
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.85
		#include "FurPass.cginc"
		ENDCG
//		CGPROGRAM
//		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
//		#define FUR_MULTIPLIER 0.90
//		#include "FurPass.cginc"
//		ENDCG
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend vertex:vert
		#define FUR_MULTIPLIER 0.95
		#include "FurPass.cginc"
		ENDCG


	}
	FallBack "Diffuse"
}
