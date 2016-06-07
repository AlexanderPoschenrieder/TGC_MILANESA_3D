/*
* Shader utilizado por el ejemplo "Lights/EjemploMultiDiffuseLights.cs"
* Permite aplicar iluminación dinámica con PhongShading a nivel de pixel.
* Soporta hasta 4 luces por objeto en la misma pasada.
* Las luces tienen atenuación por distancia.
* Solo se calcula el componente Diffuse para acelerar los cálculos. Se ignora
* el Specular.
*/

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

//Textura para Lightmap
texture texLightMap;
sampler2D lightMap = sampler_state
{
   Texture = (texLightMap);
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



//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialDiffuseColor; //Color RGB
float3 materialSpecularColor = float3 ( 1.00, 1.00, 1.00);

//Variables de las 4 luces
float3 fvEyePosition;
float shininess = 0.2;

float kx = 0.9;
float kc = 0.1;

float3 lightColor[4]; //Color RGB de las 4 luces
float4 lightPosition[4]; //Posicion de las 4 luces
float lightIntensity[4]; //Intensidad de las 4 luces
float lightAttenuation[4]; //Factor de atenuacion de las 4 luces



/**************************************************************************************/
/* MultiDiffuseLightsTechnique */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT
{
   float4 Position : POSITION0;
   float3 Normal : NORMAL0;
   float4 Color : COLOR;
   float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float2 Tex : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float3 EnvTex : TEXCOORD2;
	float3 EnvTex1 : TEXCOORD3;
	float3 EnvTex2 : TEXCOORD4;
	float3 EnvTex3 : TEXCOORD5;
	float3 WorldPosition : TEXCOORD6;
	float3 N : TEXCOORD7;
};

//Vertex Shader
VS_OUTPUT vs_general(VS_INPUT input)
{
	VS_OUTPUT output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
	output.Tex = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space 
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
	
	
	float3 vEyeR = normalize(output.Position-fvEyePosition);

	// corrijo la normal (depende de la malla)
	// ej. el tanque esta ok, la esfera esta invertida.
	//input.Normal*= -1;
    float3 vN = mul( input.Normal, (float3x3)matWorld);
    vN = normalize( vN );
    output.EnvTex = reflect(vEyeR,vN);
    
	// Refraccion de la luz
    output.EnvTex1 = refract(vEyeR,vN,1.001);
    output.EnvTex2 = refract(vEyeR,vN,1.009);
    output.EnvTex3 = refract(vEyeR,vN,1.02); 
    
    
    
    //Propago la normal
    output.N = vN;
	
	
	

	return output;
}


//Input del Pixel Shader
struct PS_INPUT
{
	float4 Position : POSITION0;
	float2 Tex : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float3 EnvTex : TEXCOORD2;
	float3 EnvTex1 : TEXCOORD3;
	float3 EnvTex2 : TEXCOORD4;
	float3 EnvTex3 : TEXCOORD5;
	float3 WorldPosition : TEXCOORD6;
	float3 N : TEXCOORD7;
};

//Funcion para calcular color RGB de Diffuse
float3 computeDiffuseAndSpecularComponent(float3 surfacePosition, float3 N, int i)
{
	//Calcular intensidad de luz, con atenuacion por distancia
	float3 L = lightPosition[i].xyz - surfacePosition;
	float distAtten = length(L);
	
	float3 Ln = L / distAtten;
	distAtten = distAtten * lightAttenuation[i];
	float intensity = lightIntensity[i] / distAtten; //Dividimos intensidad sobre distancia

	//Calcular Diffuse (N dot L)
	float3 diffuseComponent = intensity * lightColor[i].rgb * materialDiffuseColor * max(0.0, abs(dot(normalize(N), normalize(Ln))));
	
	float D = normalize(lightPosition[i].xyz-fvEyePosition);
	//float ks = saturate(absdot(reflect(Ln,N), D)); //o usar Ln en vez de L? mmm
	float ks = saturate(abs(dot(2 * N * dot(Ln, N) - Ln, D)));
	ks = pow(ks,25);
	
	float3 specularComponent = ks;
	
	return diffuseComponent + specularComponent;// + max(0.0, specularComponent));
}



//Pixel Shader para Point Light
float4 point_light_ps(PS_INPUT input) : COLOR0
{      
	float3 Nn = normalize(input.WorldNormal);

	//Emissive + Diffuse de 4 luces PointLight
	float3 diffuseLighting = materialEmissiveColor;

	//Diffuse 0
	diffuseLighting += computeDiffuseAndSpecularComponent(input.WorldPosition, Nn, 0);

	//Diffuse 1
	diffuseLighting += computeDiffuseAndSpecularComponent(input.WorldPosition, Nn, 1);
	
	//Diffuse 2
	diffuseLighting += computeDiffuseAndSpecularComponent(input.WorldPosition, Nn, 2);
	
	//Diffuse 3
	diffuseLighting += computeDiffuseAndSpecularComponent(input.WorldPosition, Nn, 3);
	
	
	float k = 0.30;
	float4 fvBaseColor = k*texCUBE( g_samCubeMap, input.EnvTex ) +
						(1-k)*tex2D( diffuseMap, input.Tex );
	
	float4 color_reflejado = fvBaseColor * float4(diffuseLighting.x, diffuseLighting.y, diffuseLighting.z, 1);

	float4 color_refractado = float4(
		texCUBE( g_samCubeMap, input.EnvTex1).x, 
		texCUBE( g_samCubeMap, input.EnvTex2).y,
		texCUBE( g_samCubeMap, input.EnvTex3).z,
		1);
	
	return color_reflejado*kx + color_refractado*kc;

}



/*
* Technique con iluminacion
*/
technique DifAndSpecTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 vs_general();
	  PixelShader = compile ps_3_0 point_light_ps();
   }

}


