// ---------------------------------------------------------
// Enviroment Map
// ---------------------------------------------------------



/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float3 fvLightPosition = float3( 0.00, 0.00, 1000.00 );
float3 fvEyePosition = float3( 2600.00, 4000.00, 100.00 );
// Fresnel
float FBias = 0.4;
int FPower = 1;
float FEscala = 0.5;

float k_la = 0.7;							// luz ambiente global
float k_ld = 0.8;							// luz difusa
float k_ls = 1.0;							// luz specular
float fSpecularPower = 16.84;

float kx = 1.0;							// coef. de reflexion
float kc = 0.0;							// coef. de refraccion
bool usar_fresnel = 0;


// Textura auxiliar:
texture aux_Tex;
sampler2D auxMap =
sampler_state
{
   Texture = (aux_Tex);
   ADDRESSU = MIRROR;
   ADDRESSV = MIRROR;
   MINFILTER = LINEAR;
   MAGFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

// enviroment map
texture  g_txCubeMap;
samplerCUBE g_samCubeMap = 
sampler_state
{
    Texture = <g_txCubeMap>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
};




/**************************************************************************************/
/* RenderCubeMap */
/**************************************************************************************/
void VSCubeMap( float4 Pos : POSITION,
                float3 Normal : NORMAL,
                float2 Texcoord : TEXCOORD0,
                out float4 oPos : POSITION,
                out float3 EnvTex : TEXCOORD0,
                out float2 Tex : TEXCOORD2,
                out float3 N : TEXCOORD1 ,
                out float3 EnvTex1 : TEXCOORD3,
                out float3 EnvTex2 : TEXCOORD4,
                out float3 EnvTex3 : TEXCOORD5,
                out float3 wPos :	TEXCOORD6,
                out float  Fresnel : COLOR
                )
{

	wPos = mul( Pos, matWorld);
	float3 vEyeR = normalize(wPos-fvEyePosition);

	// corrijo la normal (depende de la malla)
	// ej. el tanque esta ok, la esfera esta invertida.
	//Normal*= -1;
    float3 vN = mul( Normal, (float3x3)matWorld);
    vN = normalize( vN );
    EnvTex = reflect(vEyeR,vN);
    
	// Refraccion de la luz
    EnvTex1 = refract(vEyeR,vN,1.001);
    EnvTex2 = refract(vEyeR,vN,1.009);
    EnvTex3 = refract(vEyeR,vN,1.02);
    Fresnel = FBias + FEscala*pow(1 + dot(vEyeR,vN),FPower);    
    
    // proyecto
    oPos = mul( Pos, matWorldViewProj );
    
    //Propago la textura
    Tex = Texcoord;
    
    //Propago la normal
    N = vN;
    
 }


float4 PSCubeMap(	float3 EnvTex: TEXCOORD0, 
					float3 N:TEXCOORD1,
					float3 Texcoord: TEXCOORD2,
					float3 Tex1 : TEXCOORD3,
					float3 Tex2 : TEXCOORD4,
					float3 Tex3 : TEXCOORD5,
					float Fresnel : COLOR,
					float3 wPos: TEXCOORD6
				) : COLOR0
{      
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular
	
	N = normalize(N);

	// 1- calculo la luz diffusa
	float3 LD = normalize(fvLightPosition-wPos);
	ld += saturate(dot(N, LD))*k_ld;
	
	// 2- calcula la reflexion specular
	float3 D = normalize(wPos-fvEyePosition);
	float ks = saturate(dot(reflect(LD,N), D));
	ks = pow(ks,fSpecularPower);
	le += ks*k_ls;

	//Obtener el texel de textura
	float k = 1;
	float4 fvBaseColor = k*texCUBE( g_samCubeMap, EnvTex ) +
						(1-k)*tex2D( diffuseMap, Texcoord );
	
	// suma luz diffusa, ambiente y especular
	fvBaseColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);
	fvBaseColor.a = 1;
	float4 color_reflejado = fvBaseColor;

	float4 color_refractado = float4(
		texCUBE( g_samCubeMap, Tex1).x, 
		texCUBE( g_samCubeMap, Tex2).y,
		texCUBE( g_samCubeMap, Tex3).z,
		1);
	//float4 color_refractado = texCUBE( g_samCubeMap, Tex1);
	
	if(usar_fresnel)
		return color_reflejado*Fresnel + color_refractado*(1-Fresnel);
	else
		return color_reflejado*kx + color_refractado*kc;
	
	//return color_refractado;
	//return color_reflejado;
}


technique RenderCubeMap
{
    pass p0
    {
        VertexShader = compile vs_3_0 VSCubeMap();
        PixelShader = compile ps_3_0 PSCubeMap();
    }
}

