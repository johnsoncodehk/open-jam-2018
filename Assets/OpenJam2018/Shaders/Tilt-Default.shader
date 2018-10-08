Shader "Tilt/Default" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _Angle ("View Angle", Range(0,90)) = 0
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert vertex:vert
      struct Input {
          float2 uv_MainTex;
      };
	  float _Angle;
	  void vert (inout appdata_full v) {
		  float4 vertex = v.vertex;
		  float rad = radians(_Angle);
		  float cosAngle = cos(rad);
		  float sinAngle = sin(rad);
		  float4 worldVertex = mul(unity_ObjectToWorld, vertex);
		  float4 targetWorldVertex = worldVertex;
		  targetWorldVertex.y += (cosAngle - 1) * worldVertex.y;
		  targetWorldVertex.z += sinAngle * worldVertex.y;
		  vertex = mul(unity_WorldToObject, targetWorldVertex);
		  v.vertex = vertex;
      }
      sampler2D _MainTex;
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }