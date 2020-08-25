Shader "BUSSID/GlassDecalDoubleSideMobile" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_ColorSticker("ColorSticker", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.985
		_Metallic ("Metallic", Range(0,1)) = 1.0
		_Fresnel ("Fresnel", Range(0,4)) = 2.5
		_DecalTex ("Decal (RGBA)", 2D) = "black" {}
		[Toggle(USE_REFLECTIONS)] _UseReflections ("Use reflections", Float) = 0
		_ReflStrength ("Refl strength", Range(0,1)) = 0.0
		_ReflSmoothness ("Refl smoothness", Range(0,1)) = 1.0
		_ReflFresnel ("Refl fresnel", Range(0,4)) = 1.0
	}
	
	SubShader
	{
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
		LOD 1000
		
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		Pass // Interior pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase"}
			
			Cull Front
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma shader_feature USE_GLOW
			//#pragma shader_feature USE_REFLECTIONS // no reflections on interior side
			
			#include "UnityCG.cginc"
			
			#define GLASS
			#define GLASS_INTERIOR
			
			#include "diffuse.cginc"
			
			ENDCG
		}
		
		Pass // exterior
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase"}
			
			Cull Back
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma shader_feature USE_GLOW
			#pragma shader_feature USE_REFLECTIONS
			
			#include "UnityCG.cginc"
			
			#define GLASS
			
			#include "mobile_diffuse.cginc"
			
			ENDCG
		}
	
		Pass // exterior add pass (spot light)
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardAdd"}
			
			ZWrite Off Blend One One
			Cull Back
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile POINT SPOT
			#include "UnityCG.cginc"
			
			#define GLASS
			#define FORWARD_ADD
			
			#include "mobile_diffuse.cginc"
			
			ENDCG
		}
		
	}
	
	SubShader
	{
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
		LOD 200
		
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		Pass // Interior pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase"}
			
			Cull Front
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			
			#include "UnityCG.cginc"
			
			#define LOW_QUALITY
			#define GLASS
			#define GLASS_INTERIOR
			
			#include "mobile_diffuse.cginc"
			
			ENDCG
		}
		
		Pass // exterior
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase"}
			
			Cull Back
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			#define LOW_QUALITY
			#define GLASS
			#pragma shader_feature USE_REFLECTIONS
			
			#include "mobile_diffuse.cginc"
			
			ENDCG
		}
		
	}	
	
	FallBack "Diffuse"
}