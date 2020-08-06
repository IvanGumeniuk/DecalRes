Shader "BUSSID/MobileDiffuse"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		[Toggle(USE_GLOW)] _UseGlow ("Use glow", Float) = 0
		[Toggle(MULTIPLY_GLOW)] _MultiplyGlow ("Multiply glow", Float) = 0
		[Toggle(GLOW_ALWAYS)] _GlowAlways ("Always glow", Float) = 0
		[NoScaleOffset] _GlowTex ("Glow map", 2D) = "black" {}
		[Toggle(USE_REFLECTIONS)] _UseReflections ("Use reflections", Float) = 0
		_ReflStrength ("Refl strength", Range(0,1)) = 0.0
		_ReflSmoothness ("Refl smoothness", Range(0,1)) = 1.0
		[Toggle(USE_REFLECTION_MASK)] _UseReflectionMask ("Use reflection mask", Float) = 0
		[NoScaleOffset] _ReflMask ("Reflection mask", 2D) = "white" {}
		[Toggle(USE_EMISSION_COLOR)] _UseEmissionColor ("Use emission", Float) = 0
		_EmissionColor("color", Color) = (1,1,1,1)
		_EmissionMultColor("Emission multiply color", Color) = (1,1,1,1)
		[Toggle(SKIP_NIGHT_LIGHTS)] _SkipNightLights ("Skip night lights", Float) = 0
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 1000

		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase"}
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			
			#pragma shader_feature USE_GLOW
			#pragma shader_feature MULTIPLY_GLOW
			#pragma shader_feature GLOW_ALWAYS
			#pragma shader_feature USE_REFLECTIONS
			#pragma shader_feature USE_REFLECTION_MASK
			#pragma shader_feature USE_EMISSION_COLOR
			#pragma shader_feature SKIP_NIGHT_LIGHTS
			
			#include "mobile_diffuse.cginc"
			
			ENDCG
		}
		
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardAdd"}
			
			ZWrite Off Blend One One
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile POINT SPOT
			
			#include "UnityCG.cginc"

			#define FORWARD_ADD
			#include "mobile_diffuse.cginc"
			
			ENDCG
		}
		
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase"}
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			#pragma shader_feature USE_GLOW
			#pragma shader_feature MULTIPLY_GLOW
			#pragma shader_feature USE_EMISSION_COLOR
			#pragma shader_feature USE_REFLECTIONS
			#pragma shader_feature USE_REFLECTION_MASK
			#pragma shader_feature SKIP_NIGHT_LIGHTS
			
			#define LOW_QUALITY
			
			#include "mobile_diffuse.cginc"
			
			ENDCG
		}
		
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardAdd"}
			
			ZWrite Off Blend One One
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile POINT SPOT
			
			#include "UnityCG.cginc"
			
			#define LOW_QUALITY
			#define FORWARD_ADD
			#include "mobile_diffuse.cginc"
			
			ENDCG
		}
		
	}

	Fallback "Mobile/VertexLit"

}
