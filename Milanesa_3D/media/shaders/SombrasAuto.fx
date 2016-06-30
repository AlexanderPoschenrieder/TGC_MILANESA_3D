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
float4x4 matViewProj;

	
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



//float3 lightColor; //Color RGB de las 4 luces
float4 lightPosition; //Posicion de las 4 luces
float4 centroPelota;
//float lightIntensity; //Intensidad de las 4 luces
//float lightAttenuation; //Factor de atenuacion de las 4 luces
float lightOrder;



/**************************************************************************************/
/* MultiDiffuseLightsTechnique */
/**************************************************************************************/


//Input del Vertex Shader
struct VS_INPUT
{
   float4 Position : POSITION0;
   float4 Normal : NORMAL0;
   float4 Color : COLOR;
   float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float Alpha : TEXCOORD1;
};

//Vertex Shader
VS_OUTPUT proyectar_al_piso_vs(VS_INPUT input)
{
	VS_OUTPUT output;
	
	

	float4 puntoPelota = mul(input.Position, matWorld);
	float4 puntoLuz = lightPosition;
	
	float4 v = normalize(puntoPelota-puntoLuz);
	float4 vc = normalize(centroPelota-puntoLuz);
	
	float t = (1-puntoLuz.y)/v.y;
	
	float4 puntoProyectado = puntoLuz + v * t;
	puntoProyectado.y = 0.03 * (5+lightOrder);

	float4 N = normalize(puntoPelota - centroPelota);
	
	
	
	output.Position = mul(puntoProyectado, matViewProj);
	output.Alpha = max(0, sign(dot(N, vc))) * (1- t/9593) * max(0, ((300-centroPelota.y)/300));

	return output;
}


//Input del Pixel Shader
struct PS_INPUT
{
	float Alpha : TEXCOORD1;
};

float4 make_black_shadow_ps(PS_INPUT input) : COLOR0
{      
	float4 pixel = float4(0, 0, 0, 1);
	//pixel = float4(0, 0, 0, 0.5f);
	return pixel;
}



technique SombrasTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 proyectar_al_piso_vs();
	  PixelShader = compile ps_3_0 make_black_shadow_ps();
   }
}