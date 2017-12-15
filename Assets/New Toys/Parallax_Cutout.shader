Shader "CookbookShaders/ParallaxCutout" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
		_NormalTex ("Normal Map", 2D) = "bump" {}
		_NormalMapIntensity("Normal Map Intensity", Range(-1,1)) = 1
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_DispTex ("Displacement Map", 2D) = "white" {}
		_Height ("Displacement Height", Range(0,1)) = 1
		_StepCount ("Step Count", Range(0,300)) = 100
	}
	SubShader {
		Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout"}
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard vertex:vert fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalTex;
		sampler2D _DispTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_DispTex;
			float3 tangentViewDir;
		};

		fixed _Cutoff;
		half _NormalMapIntensity;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _Height;
		fixed _StepCount;

		UNITY_INSTANCING_CBUFFER_START(Props)
		UNITY_INSTANCING_CBUFFER_END


		void vert(inout appdata_full i, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			//Transform the view direction from world space to tangent space			
			float3 worldVertexPos = mul(unity_ObjectToWorld, i.vertex).xyz;
			float3 worldViewDir = worldVertexPos - _WorldSpaceCameraPos;

			//To convert from world space to tangent space we need the following
			//https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
			float3 worldNormal = UnityObjectToWorldNormal(i.normal);
			float3 worldTangent = UnityObjectToWorldDir(i.tangent.xyz);
			float3 worldBitangent = cross(worldNormal, worldTangent) * i.tangent.w * unity_WorldTransformParams.w;

			//Use dot products instead of building the matrix
			o.tangentViewDir = float3(
				dot(worldViewDir, worldTangent),
				dot(worldViewDir, worldNormal),
				dot(worldViewDir, worldBitangent)
				);
		}


		//Get the height from a uv position
		float getHeight(float2 texturePos)
		{
			float4 colorNoise = tex2Dlod(_DispTex, float4(texturePos, 0, 0));
			float height = (1 - colorNoise.r) * -1 * _Height;
			return height;
		}


		//Get the texture position by interpolation between the position where we hit terrain and the position before
		float2 getWeightedTexPos(float3 rayPos, float3 rayDir, float stepDistance)
		{
			float3 oldPos = rayPos - stepDistance * rayDir;
			float oldHeight = getHeight(oldPos.xz);
			float oldDistToTerrain = abs(oldHeight - oldPos.y);
			float currentHeight = getHeight(rayPos.xz);
			float currentDistToTerrain = rayPos.y - currentHeight;
			float weight = currentDistToTerrain / (currentDistToTerrain - oldDistToTerrain);
			float2 weightedTexPos = oldPos.xz * weight + rayPos.xz * (1 - weight);
			return weightedTexPos;
		}


		void surf (Input IN, inout SurfaceOutputStandard o) {

			float3 rayPos = float3(IN.uv_DispTex.x, 0, IN.uv_DispTex.y);
			float3 rayDir = normalize(IN.tangentViewDir);
			int STEPS = (int) _StepCount;
			float stepDistance = 0.01;
			float4 finalColor = 0;
			float4 finalNorm = 1;

			for (int i = 0; i < STEPS; i++)
			{
				float height = getHeight(rayPos.xz);
				if (rayPos.y < height)
				{
					float2 weightedTex = getWeightedTexPos(rayPos, rayDir, stepDistance);
					float height = getHeight(weightedTex);
					finalColor = tex2Dlod(_MainTex, float4(weightedTex, 0, 0));
					finalNorm = tex2Dlod(_NormalTex, float4(weightedTex, 0, 0));
					break;
				}
				rayPos += stepDistance * rayDir;
			}

			clip(finalColor.a - _Cutoff);

			o.Albedo = finalColor.rgb;
			o.Alpha = finalColor.a;

			//fixed3 n = UnpackNormal(tex2D(_NormalTex, IN.uv_DispTex));
			fixed3 n = finalNorm;
			n.x *= _NormalMapIntensity;
			n.y *= _NormalMapIntensity;
			o.Normal = normalize(n);

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
