#pragma target 3.0

fixed4 _Color;
sampler2D _MainTex;
half _Glossiness;
half _Metallic;

uniform float _FurLength;
uniform float _Cutoff;
uniform float _CutoffEnd;
uniform float _EdgeFade;

uniform fixed3 _Gravity;
uniform fixed _GravityStrength;

sampler2D _StrandTex;
uniform float _StrandColorStrength;

void vert (inout appdata_full v)
{
	fixed3 direction = lerp(v.normal, _Gravity * _GravityStrength + v.normal * (1-_GravityStrength), FUR_MULTIPLIER);
	v.vertex.xyz += direction * _FurLength * FUR_MULTIPLIER * v.color.a;
}

struct Input {
	float2 uv_MainTex;
	float3 viewDir;
};

void surf (Input IN, inout SurfaceOutputStandard o)
{
	fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

	fixed4 mult = tex2D (_StrandTex, fixed2(FUR_MULTIPLIER, 0.5f));
	//fixed3 strand = tex2D (_StrandTex, fixed2(FUR_MULTIPLIER, 0.5f));
	//strand = strand - 0.5f;
	//fixed4 mult = saturate(strand) * c.rgb;
	//fixed4 screen = saturate(-strand) + 0.5f;
	//screen = 1 - 2(1 - c.rgb)(1 - screen);
	//strand = strand * 2.0f - 1.0f;
	//fixed3 m = saturate(-strand);
	//fixed3 s = saturate(strand);
	//m = m * c.rgb;
	//s = 1 - 2*(1 - c.rgb)*(1 - s);
	//fixed3 overlay = m + s;

	o.Albedo = c.rgb * lerp(1, mult, _StrandColorStrength);
	//o.Albedo = c.rgb * lerp(1, overlay, _StrandColorStrength);
	//o.Albedo = lerp(c.rgb, overlay, _StrandColorStrength);
	// 1 - 2(1-a)(1-b)
	// 1 - 2(1 - c.rgb)(1 - screen)

	o.Metallic = _Metallic * FUR_MULTIPLIER * FUR_MULTIPLIER;
	o.Smoothness = _Glossiness * FUR_MULTIPLIER * FUR_MULTIPLIER;

	o.Alpha = step(lerp(_Cutoff, _CutoffEnd, FUR_MULTIPLIER), c.a);

	float alpha = dot(IN.viewDir, o.Normal);
	alpha *= 1 - (FUR_MULTIPLIER * FUR_MULTIPLIER);
	alpha = 1 - 2 * _EdgeFade * (1 - alpha);

	o.Alpha *= alpha;
}