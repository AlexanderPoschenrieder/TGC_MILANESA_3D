/*
* Shader utilizado por el ejemplo "Lights/EjemploMultiDiffuseLights.cs"
* Permite aplicar iluminaci�n din�mica con PhongShading a nivel de pixel.
* Soporta hasta 4 luces por objeto en la misma pasada.
* Las luces tienen atenuaci�n por distancia.
* Solo se calcula el componente Diffuse para acelerar los c�lculos. Se ignora
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
//float lightIntensity; //Intensidad de las 4 luces
//float lightAttenuation; //Factor de atenuacion de las 4 luces



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
};

//Vertex Shader
VS_OUTPUT proyectar_al_piso_vs(VS_INPUT input)
{
	VS_OUTPUT output;

	float4 puntoPelota = mul(input.Position, matWorld);
	float4 puntoLuz = lightPosition;
	
	float4 v = normalize(puntoPelota-puntoLuz);
	
	float t = (1-puntoLuz.y)/v.y;
	
	float4 puntoProyectado = puntoLuz + v * t;
	//float4 puntoProyectado4 = (puntoProyectado.x, puntoProyectado.y, puntoProyectado.z, 1);
	
	//Proyectar posicion
	
	output.Position = mul(puntoProyectado, matViewProj);
	//output.Position = mul(input.Position, matWorldViewProj);

	return output;
}


//Input del Pixel Shader
struct PS_INPUT
{
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
};

float4 make_black_shadow_ps(PS_INPUT input) : COLOR0
{      

	return 1;
}



technique SombrasTechnique
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 proyectar_al_piso_vs();
	  PixelShader = compile ps_3_0 make_black_shadow_ps();
   }

}


