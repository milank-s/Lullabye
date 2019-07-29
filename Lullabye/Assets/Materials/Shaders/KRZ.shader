Shader "KRZ" {
	Properties {
		_Color ("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex ("Color (RGBA)", 2D) = "white" {}
		_LightCutoff ("Light Cutoff", Range(0.0, 1.0)) = 0.2
		_Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.0
		_AlphaMap ("Alpha Mask", 2D) = "white" {}
	}
	
	SubShader {
   	 	Tags {"IgnoreProjector"="True" "RenderType"="Transparent"}

   	 	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
		}
		
    	LOD 300
 
		Cull Off
    
        
		CGPROGRAM
		#pragma surface surf KRZ fullforwardshadows alphatest:_Cutoff 
		sampler2D _MainTex;
		sampler2D _AlphaMap;
		float _LightCutoff;
		fixed4 _Color;
		
		struct SurfaceOutputKRZ {
			fixed3 Albedo;
			fixed3 Emission;
			fixed3 Normal;
			fixed Specular;
			fixed3 Alpha;
		};
        
		struct Input {
			float2 uv_MainTex;
			float2 uv_AlphaMap;
			float4 color : COLOR;
		};


		void surf (Input IN, inout SurfaceOutputKRZ o) {
	
			half3 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color * IN.color;
			o.Albedo = tex.rgb;
			o.Alpha = tex2D(_AlphaMap, IN.uv_AlphaMap);
		}

		inline fixed4 LightingKRZ (SurfaceOutputKRZ s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			atten = step(_LightCutoff, atten) * atten;

			float4 c;
			c.rgb = (_LightColor0.rgb * s.Albedo) * (atten);
			c.a = s.Alpha;

			return c;
		}
	
		ENDCG
	}
	FallBack "VertexLit"
}