// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Spine/Skeleton Tint
// - Two color tint
// - unlit
// - Premultiplied alpha blending
// - No depth, no backface culling, no fog.

Shader "Spine/Skeleton Tint" {
	Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
		_Black ("Black Point", Color) = (0,0,0,0)
		[NoScaleOffset] _MainTex ("MainTex", 2D) = "black" {}
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		_angle ("Angle" , Range(0,360))=60
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			uniform float4 _Color;
			uniform float4 _Black;
			uniform float _angle;

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				 //在MVP变换之后再进行旋转操作,并修改顶点的Z值(深度)
                //弧度
                fixed radian = _angle / 180.0 * 3.14159;
                fixed cosTheta = cos(radian);
                fixed sinTheta = sin(radian);

                //旋转中心点(测试用的四边形, 正常的spine做的模型脚下旋转的点就是(0,0), 可以省去下面这一步已经旋转完成后的 +center操作)
                //half2 center = half2(0, -0);
                //v.vertex.zy -= center;

                half z = v.vertex.z * cosTheta - v.vertex.y * sinTheta;
                half y = v.vertex.z * sinTheta + v.vertex.y * cosTheta;
                v.vertex = half4(v.vertex.x, y, z, v.vertex.w);

                //v.vertex.zy += center;

                float4 verticalClipPos = UnityObjectToClipPos(v.vertex);
                o.pos.z = verticalClipPos.z / verticalClipPos.w * o.pos.w ;

				o.vertexColor = v.vertexColor * float4(_Color.rgb * _Color.a, _Color.a); // Combine a PMA version of _Color with vertexColor.
				return o;
			}

			float4 frag (VertexOutput i) : COLOR {
				float4 texColor = tex2D(_MainTex, i.uv);
				return (texColor * i.vertexColor) + float4(((1-texColor.rgb) * texColor.a * _Black.rgb), 0);
			}
			ENDCG
		}

		Pass {
			Name "Caster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1

			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			struct v2f { 
				V2F_SHADOW_CASTER;
				float2 uv : TEXCOORD1;
			};

			uniform float4 _MainTex_ST;

			v2f vert (appdata_base v) {
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			uniform sampler2D _MainTex;
			uniform fixed _Cutoff;

			float4 frag (v2f i) : COLOR {
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a - _Cutoff);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}
