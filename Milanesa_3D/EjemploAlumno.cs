using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using System.Windows.Forms;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.Sound;

namespace AlumnoEjemplos.Milanesa_3D
{
    public enum EstadoJuego
    {
        MenuPrincipal,
        MenuControles,
        Juego,
        GameOver
    }

    public class EjemploAlumno : TgcExample
    {
        #region MENU

        Menu menuPrincipal;
        MenuControles menuControles;
        GameOver gameOver;
        public EstadoJuego estadoJuego;

        #endregion

        #region DECLARACIONES
        TgcSkyBox skyBox;
        TgcScene scene;
        TgcMesh mainCarMesh, secondCarMesh, iaCarMesh;
        Microsoft.DirectX.Direct3D.Effect mainEffect, carEffect;
        public Auto mainCar;
        public Auto2 secondCar;
        IMilanesaCamera camaraActiva1, camaraActiva2;
        float time;
        float gameDuration = 120f;
        CubeTexture cubeTexture;
        //List<TgcMesh> people = new List<TgcMesh>();

        Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

        TwoTargetsCamera camaraPelota1;
        MilanesaThirdPersonCamera camaraAuto1;

        TwoTargetsCamera camaraPelota2;
        MilanesaThirdPersonCamera camaraAuto2;

        Vector3 currentCameraPos = new Vector3();

        public Pelota pelota;
        public float lastElapsedTime = 0f;
        public TgcText2d txtScoreLocal = new TgcText2d();
        public TgcText2d txtScoreVisitante = new TgcText2d();
        TgcText2d txtTimer = new TgcText2d();
        public TgcText2d txtNitroVisitante = new TgcText2d();
        public TgcText2d txtNitroLocal = new TgcText2d();
        public TgcText2d txtGol = new TgcText2d();
        public float golTimer = 0f;
       
        public TgcText2d txtDebug = new TgcText2d();
        Viewport View1, View2, ViewF;
        public bool splitScreen = false;

        public int scoreLocal = 0;
        public int scoreVisitante = 0;

        public List<Auto> autitus;
        public List<TgcBoundingBox> limitesArcos;
        public List<TgcBoundingBox> laterales;

        public List<TgcBox> cajasVisiblesEscenario;
        public List<TgcMesh> meshesCajasEscenario;
        public List<TgcMesh> todosLosMeshes;


        public TgcBox piso;
        public TgcBox pisoArcoPositivo;
        public TgcBox pisoArcoNegativo;

        public TgcBoundingBox arcoPositivo;
        public TgcBoundingBox arcoNegativo;

        public TgcBoundingBox limiteArcoNegativo1;
        public TgcBoundingBox limiteArcoNegativo2;
        public TgcBoundingBox limiteArcoNegativo3;

        public TgcBox paredArcoNegativo1;
        public TgcBox paredArcoNegativo2;


        public TgcBoundingBox limiteArcoPositivo1;
        public TgcBoundingBox limiteArcoPositivo2;
        public TgcBoundingBox limiteArcoPositivo3;

        public TgcBox paredArcoPositivo1;
        public TgcBox paredArcoPositivo2;

        public TgcBoundingBox limiteLateralPositivo;
        public TgcBoundingBox limiteLateralNegativo;

        public TgcBox paredLateralPositiva;
        public TgcBox paredLateralNegativa;

        public TgcBox rejaLateralPositiva;
        public TgcBox rejaLateralNegativa;

        public TgcBox rejaFondoArcoPositivo, rejaInsidePositivoArcoPositivo, rejaInsideNegativoArcoPositivo, rejaTechoArcoPositivo, rejaFondoArcoNegativo, rejaInsidePositivoArcoNegativo, rejaInsideNegativoArcoNegativo, rejaTechoArcoNegativo;

        public TgcMesh meshPisoAuxiliar;

        static public string rootDir = GuiController.Instance.AlumnoEjemplosDir;
        static public string mediaFolder = rootDir + "Milanesa_3D\\media\\";
        //static string mediaFolder = rootDir + "MiGrupo\\media\\";
        static public string sceneFolder = mediaFolder + "meshes\\scenes\\";
        private TgcBox rejaArcoPositivo1;
        private TgcBox rejaArcoPositivo2;
        private TgcBox rejaSuperiorArcoPositivo;
        private TgcBox rejaArcoNegativo1;
        private TgcBox rejaArcoNegativo2;
        private TgcBox rejaSuperiorArcoNegativo;

        public List<TgcMesh> people;

        TgcBox[] lightMeshes;
        Microsoft.DirectX.Direct3D.Effect lightEffect;
        Microsoft.DirectX.Direct3D.Effect sombrasEffect, sombrasAutoEffect;
        Microsoft.DirectX.Direct3D.Effect pisoEffect;
        private string pathSoundtrack = mediaFolder + "music\\t.mp3";

        #endregion DECLARACIONES

        #region BUROCRACIA

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Milanesa_3D";
        } 

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string getDescription()
        {
            return "Rocket League - Futbol de autos. El juego dura dos minutos. Con V se habilita o deshabilita el split screen.";
        }

        #endregion

        #region ESCENARIO

        public void crearEscenario()
        {
            piso = TgcBox.fromExtremes(new Vector3(-2600, -500, -4000), new Vector3(2600, 0, 4000));

            pisoArcoPositivo = TgcBox.fromExtremes(new Vector3(-500, -500, 4000), new Vector3(500, 0, 4500));
            pisoArcoNegativo = TgcBox.fromExtremes(new Vector3(-500, -500, -4000), new Vector3(500, 0, -4500));

            arcoPositivo = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-500, 0, 4000), new Vector3(500, 400, 4500) });
            arcoPositivo.setRenderColor(Color.Red);
            arcoNegativo = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-500, 0, -4000), new Vector3(500, 400, -4500) });
            arcoNegativo.setRenderColor(Color.Blue);

            limiteArcoNegativo1 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-2800, -500, -4250), new Vector3(-500, 1000, -4000) });
            limiteArcoNegativo2 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(500, -500, -4250), new Vector3(2800, 1000, -4000) });
            limiteArcoNegativo3 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-500, 400, -4250), new Vector3(500, 1000, -4000) });

            paredArcoNegativo1 = TgcBox.fromExtremes(new Vector3(-2600, 0, -4050), new Vector3(-500, 100, -4000));
            paredArcoNegativo2 = TgcBox.fromExtremes(new Vector3(500, 0, -4050), new Vector3(2600, 100, -4000));

            rejaArcoNegativo1 = TgcBox.fromExtremes(new Vector3(-2600, 100, -4000), new Vector3(-500, 400, -4001));
            rejaArcoNegativo2 = TgcBox.fromExtremes(new Vector3(500, 100, -4000), new Vector3(2600, 400, -4001));
            rejaSuperiorArcoNegativo = TgcBox.fromExtremes(new Vector3(-2600, 400, -4001), new Vector3(2600, 1000, -4001));

            rejaFondoArcoPositivo = TgcBox.fromExtremes(new Vector3(-500, 0, 4500), new Vector3(500, 400, 4501));

            rejaInsidePositivoArcoPositivo = TgcBox.fromExtremes(new Vector3(499, 0, 4000), new Vector3(500, 400, 4500));
            rejaInsideNegativoArcoPositivo = TgcBox.fromExtremes(new Vector3(-499, 0, 4000), new Vector3(-500, 400, 4500));
            rejaTechoArcoPositivo = TgcBox.fromExtremes(new Vector3(-500, 400, 4000), new Vector3(500, 401, 4500));

            rejaFondoArcoNegativo = TgcBox.fromExtremes(new Vector3(-500, 0, -4500), new Vector3(500, 400, -4501));

            rejaInsidePositivoArcoNegativo = TgcBox.fromExtremes(new Vector3(499, 0, -4000), new Vector3(500, 400, -4500));
            rejaInsideNegativoArcoNegativo = TgcBox.fromExtremes(new Vector3(-499, 0, -4000), new Vector3(-500, 400, -4500));
            rejaTechoArcoNegativo = TgcBox.fromExtremes(new Vector3(-500, 400, -4000), new Vector3(500, 401, -4500));


            limiteArcoPositivo1 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-2800, -500, 4000), new Vector3(-500, 1000, 4250) });
            limiteArcoPositivo2 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(500, -500, 4000), new Vector3(2800, 1000, 4250) });
            limiteArcoPositivo3 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-500, 400, 4000), new Vector3(500, 1000, 4250) });

            paredArcoPositivo1 = TgcBox.fromExtremes(new Vector3(-2600, 0, 4000), new Vector3(-500, 100, 4050));
            paredArcoPositivo2 = TgcBox.fromExtremes(new Vector3(500, 0, 4000), new Vector3(2600, 100, 4050));

            rejaArcoPositivo1 = TgcBox.fromExtremes(new Vector3(-2600, 100, 4000), new Vector3(-500, 400, 4001));
            rejaArcoPositivo2 = TgcBox.fromExtremes(new Vector3(500, 100, 4000), new Vector3(2600, 400, 4001));
            rejaSuperiorArcoPositivo = TgcBox.fromExtremes(new Vector3(-2600, 400, 4000), new Vector3(2600, 1000, 4001));

            limiteLateralPositivo = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-2850, -500, -4000), new Vector3(-2600, 1000, 4000) });
            limiteLateralNegativo = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(2600, -500, -4000), new Vector3(2850, 1000, 4000) });

            paredLateralNegativa = TgcBox.fromExtremes(new Vector3(-2650, 0, -4000), new Vector3(-2600, 100, 4000));
            paredLateralPositiva = TgcBox.fromExtremes(new Vector3(2600, 0, -4000), new Vector3(2650, 100, 4000));

            rejaLateralNegativa = TgcBox.fromExtremes(new Vector3(-2601, 100, -4000), new Vector3(-2600, 1000, 4000));
            rejaLateralPositiva = TgcBox.fromExtremes(new Vector3(2600, 100, -4000), new Vector3(2601, 1000, 4000));


            TgcTexture pasto = TgcTexture.createTexture(d3dDevice, mediaFolder + "textures\\pasto.jpg");
            piso.setTexture(pasto);
            piso.UVTiling = new Vector2(150, 150);

            TgcTexture blue = TgcTexture.createTexture(d3dDevice, mediaFolder + "textures\\blue.jpg");
            TgcTexture red = TgcTexture.createTexture(d3dDevice, mediaFolder + "textures\\red.jpg");

            pisoArcoNegativo.setTexture(blue);
            pisoArcoPositivo.setTexture(red);
            pisoArcoPositivo.UVTiling = new Vector2(2, 2);
            pisoArcoNegativo.UVTiling = new Vector2(2, 2);

            cajasVisiblesEscenario.Add(piso);
            cajasVisiblesEscenario.Add(pisoArcoNegativo);
            cajasVisiblesEscenario.Add(pisoArcoPositivo);


            TgcTexture cemento = TgcTexture.createTexture(d3dDevice, mediaFolder + "textures\\paredRugosa.jpg");
            paredLateralNegativa.setTexture(cemento);
            paredLateralPositiva.setTexture(cemento);
            paredLateralNegativa.UVTiling = new Vector2(150, 1);
            paredLateralPositiva.UVTiling = new Vector2(150, 1);

            cajasVisiblesEscenario.Add(paredLateralNegativa);
            cajasVisiblesEscenario.Add(paredLateralPositiva);

            paredArcoNegativo1.setTexture(cemento);
            paredArcoNegativo2.setTexture(cemento);
            paredArcoNegativo1.UVTiling = new Vector2(20, 1);
            paredArcoNegativo2.UVTiling = new Vector2(20, 1);

            cajasVisiblesEscenario.Add(paredArcoNegativo1);
            cajasVisiblesEscenario.Add(paredArcoNegativo2);

            paredArcoPositivo1.setTexture(cemento);
            paredArcoPositivo2.setTexture(cemento);
            paredArcoPositivo1.UVTiling = new Vector2(20, 1);
            paredArcoPositivo2.UVTiling = new Vector2(20, 1);

            cajasVisiblesEscenario.Add(paredArcoPositivo1);
            cajasVisiblesEscenario.Add(paredArcoPositivo2);

            List<TgcBox> rejas = new List<TgcBox>();

            TgcTexture textura_reja = TgcTexture.createTexture(d3dDevice, mediaFolder + "textures\\fence.png");
            rejas.Add(rejaLateralNegativa);
            rejas.Add(rejaLateralPositiva);
            rejas.Add(rejaArcoPositivo1);
            rejas.Add(rejaArcoPositivo2);
            rejas.Add(rejaSuperiorArcoPositivo);
            rejas.Add(rejaArcoNegativo1);
            rejas.Add(rejaArcoNegativo2);
            rejas.Add(rejaSuperiorArcoNegativo);
            rejas.Add(rejaFondoArcoPositivo);
            rejas.Add(rejaInsidePositivoArcoPositivo);
            rejas.Add(rejaInsideNegativoArcoPositivo);
            rejas.Add(rejaTechoArcoPositivo);
            rejas.Add(rejaFondoArcoNegativo);
            rejas.Add(rejaInsidePositivoArcoNegativo);
            rejas.Add(rejaInsideNegativoArcoNegativo);
            rejas.Add(rejaTechoArcoNegativo);


            foreach (TgcBox r in rejas)
            {
                r.AlphaBlendEnable = true;
                r.setTexture(textura_reja);
                cajasVisiblesEscenario.Add(r);
            }

            rejaLateralNegativa.UVTiling = new Vector2(150, 9);
            rejaLateralPositiva.UVTiling = new Vector2(150, 9);

            rejaArcoPositivo1.UVTiling = new Vector2(20, 3);
            rejaArcoPositivo2.UVTiling = new Vector2(20, 3);
            rejaSuperiorArcoPositivo.UVTiling = new Vector2(50, 6);

            rejaArcoNegativo1.UVTiling = new Vector2(20, 3);
            rejaArcoNegativo2.UVTiling = new Vector2(20, 3);
            rejaSuperiorArcoNegativo.UVTiling = new Vector2(50, 6);

            rejaFondoArcoNegativo.UVTiling = new Vector2(10, 4);
            rejaFondoArcoPositivo.UVTiling = new Vector2(10, 4);
            rejaInsideNegativoArcoNegativo.UVTiling = new Vector2(5, 4);
            rejaInsideNegativoArcoPositivo.UVTiling = new Vector2(5, 4);
            rejaInsidePositivoArcoNegativo.UVTiling = new Vector2(5, 4);
            rejaInsidePositivoArcoPositivo.UVTiling = new Vector2(5, 4);

            rejaTechoArcoNegativo.UVTiling = new Vector2(10, 5);
            rejaTechoArcoPositivo.UVTiling = new Vector2(10, 5);


            foreach (TgcBox box in cajasVisiblesEscenario)
            {
                box.updateValues();
                TgcMesh mesh;
                mesh = box.toMesh("abc");
                meshesCajasEscenario.Add(mesh);

                //mñeh
                //esto es un paso extra porque a los tgcBox no les gusta iluminarse

                todosLosMeshes.Add(mesh);

            }

            meshPisoAuxiliar = piso.toMesh("abc"); 

            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, -500, 0);
            skyBox.Size = new Vector3(16000, 16000, 16000);
            string texturesPath = mediaFolder + "textures\\SkyBox3\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "Up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "Down.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "Left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "Right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "Back.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "Front.jpg");
            skyBox.updateValues();

            limitesArcos = new List<TgcBoundingBox>();
            laterales = new List<TgcBoundingBox>();

            limitesArcos.Add(limiteArcoNegativo3);
            limitesArcos.Add(limiteArcoNegativo2);
            limitesArcos.Add(limiteArcoNegativo1);
            limitesArcos.Add(limiteArcoPositivo3);
            limitesArcos.Add(limiteArcoPositivo2);
            limitesArcos.Add(limiteArcoPositivo1);
            laterales.Add(limiteLateralNegativo);
            laterales.Add(limiteLateralPositivo);



        }

        #endregion

        #region LUCES

        public void initLights()
        {
            int i = 0;
            lightMeshes = new TgcBox[4];

            while (i < 4)
            {
                lightMeshes[i] = TgcBox.fromSize(new Vector3(10, 10, 10), Color.White);

                i++;
            }
            lightMeshes[0].Position = new Vector3(2550, 2000, 3950);
            lightMeshes[1].Position = new Vector3(-2550, 2000, 3950);
            lightMeshes[2].Position = new Vector3(2550, 2000, -3950);
            lightMeshes[3].Position = new Vector3(-2550, 2000, -3950);
            //Meshes

            initReflectors();
        }

        private void initReflectors()
        {
            var loader = new TgcSceneLoader();
            var enterrar = Matrix.Translation(new Vector3(0, -2500, 0)) * Matrix.Translation(0, 0, 0);
            var rot = -3 * FastMath.QUARTER_PI;

            var reflector1 = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Reflector\\New+Light+Poles-TgcScene.xml").Meshes[0];
            var matrizPos = Matrix.Translation(-2500, 0, -4400);
            reflector1.AutoTransformEnable = false;
            reflector1.Transform = enterrar * Matrix.RotationY(rot) * matrizPos;
            rot -= FastMath.PI_HALF;

            var reflector2 = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Reflector\\New+Light+Poles-TgcScene.xml").Meshes[0];
            matrizPos = Matrix.Translation(2900, 0, -3950);
            reflector2.AutoTransformEnable = false;
            reflector2.Transform = enterrar * Matrix.RotationY(rot) * matrizPos;
            rot -= FastMath.PI_HALF;

            var reflector4 = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Reflector\\New+Light+Poles-TgcScene.xml").Meshes[0];
            matrizPos = Matrix.Translation(2500, 0, 4400);
            reflector4.AutoTransformEnable = false;
            reflector4.Transform = enterrar * Matrix.RotationY(rot) * matrizPos;
            rot -= FastMath.PI_HALF;

            var reflector3 = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Reflector\\New+Light+Poles-TgcScene.xml").Meshes[0];
            matrizPos = Matrix.Translation(-2900, 0, 3950);
            reflector3.AutoTransformEnable = false;
            reflector3.Transform = enterrar * Matrix.RotationY(rot) * matrizPos;

            todosLosMeshes.Add(reflector1);
            todosLosMeshes.Add(reflector2);
            todosLosMeshes.Add(reflector3);
            todosLosMeshes.Add(reflector4);
        }

        #endregion

        #region GUI
        public void createHud()
        {
            txtDebug.Text = "";
            txtDebug.Position = new Point(50, 10);
            txtDebug.changeFont(new System.Drawing.Font("Calibri", 18, FontStyle.Bold | FontStyle.Italic));
            txtDebug.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");

            txtScoreLocal.Text = "Equipo Rojo: " + scoreLocal.ToString();
            txtScoreLocal.Position = new Point(0, 10);
            txtScoreLocal.Size = new Size(300, 100);
            txtScoreLocal.changeFont(new System.Drawing.Font("Calibri", 18, FontStyle.Bold | FontStyle.Italic));
            txtScoreLocal.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");


            txtScoreVisitante.Text = "Equipo Azul: " + scoreVisitante.ToString();
            txtScoreVisitante.Position = new Point(0, 30);
            txtScoreVisitante.Size = new Size(300, 100);
            txtScoreVisitante.changeFont(new System.Drawing.Font("Calibri", 18, FontStyle.Bold | FontStyle.Italic));
            txtScoreVisitante.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");

            txtTimer.Text = "00:00";
            txtTimer.Position = new Point(0, 50);
            txtTimer.Size = new Size(300, 100);
            txtTimer.changeFont(new System.Drawing.Font("Calibri", 18, FontStyle.Bold | FontStyle.Italic));
            txtTimer.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");

            txtNitroLocal.Text = "Nitro ready!";
            txtNitroLocal.Position = new Point(200, 10);
            txtNitroLocal.Size = new Size(300, 100);
            txtNitroLocal.changeFont(new System.Drawing.Font("Calibri", 18, FontStyle.Bold | FontStyle.Italic));
            txtNitroLocal.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");

            txtNitroVisitante.Text = "Nitro ready!";
            txtNitroVisitante.Position = new Point(200, 30);
            txtNitroVisitante.Size = new Size(300, 100);
            txtNitroVisitante.changeFont(new System.Drawing.Font("Calibri", 18, FontStyle.Bold | FontStyle.Italic));
            txtNitroVisitante.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");

            txtGol.Text = "Guuulllll!!!";
            txtGol.Position = new Point(40, 50);
            txtGol.changeFont(new System.Drawing.Font("Calibri", 30, FontStyle.Bold | FontStyle.Italic));
            txtGol.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");

        }

        public void createVars()
        {
            

        }

        public void createModifiers()
        {
            GuiController.Instance.Modifiers.addFloat("PesoPelota", 25, 125, 75);
            GuiController.Instance.Modifiers.addFloat("Gravedad", -14, -7, -9.81f);
            GuiController.Instance.Modifiers.addColor("ColorHUD", Color.Gold);

            GuiController.Instance.Modifiers.addBoolean("lightEnable", "lightEnable", true);
            GuiController.Instance.Modifiers.addFloat("lightIntensity", 0, 1000, 300);
            GuiController.Instance.Modifiers.addFloat("lightAttenuation", 0.1f, 2, 0.15f);
            

        }
        #endregion

        #region INICIALIZACION
        public override void init()
        {

            time = 0f;
            cajasVisiblesEscenario = new List<TgcBox>();
            meshesCajasEscenario = new List<TgcMesh>();
            todosLosMeshes = new List<TgcMesh>();

            scoreLocal = 0;
            scoreVisitante = 0;

            TgcSceneLoader loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(sceneFolder + "predio\\predio-TgcScene.xml");
            cubeTexture = TextureLoader.FromCubeFile(d3dDevice, mediaFolder + "textures\\cubemap.dds");

            foreach (TgcMesh m in scene.Meshes)
            {
                todosLosMeshes.Add(m);
            }

            //la escena se tiene que renderizar ANTES del escenario para asegurar que funcione bien el alpha blending

            initMenues();
            initPeople();
            crearEscenario();
            initLights();
            createVars();
            createModifiers();
            createHud();


            d3dDevice.RenderState.ReferenceAlpha = 10;


            mainCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Auto\\Auto-TgcScene.xml").Meshes[0];
            // mainCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Sphere\\Sphere-TgcScene.xml").Meshes[0];
            secondCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Auto\\Auto-TgcScene.xml").Meshes[0];
            secondCarMesh.setColor(Color.Red);
            iaCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Auto\\Auto-TgcScene.xml").Meshes[0];
            iaCarMesh.setColor(Color.Green);


            todosLosMeshes.Add(mainCarMesh);
            todosLosMeshes.Add(secondCarMesh);

            mainEffect = TgcShaders.loadEffect(mediaFolder + "\\shaders\\EnvMap.fx");
            carEffect = TgcShaders.loadEffect(mediaFolder + "shaders\\DifAndSpec.fx");

            lightEffect = TgcShaders.loadEffect(mediaFolder + "shaders\\DiffuseLight.fx");
            sombrasEffect = TgcShaders.loadEffect(mediaFolder + "shaders\\Sombras.fx");
            sombrasAutoEffect = TgcShaders.loadEffect(mediaFolder + "shaders\\SombrasAuto.fx");

            meshPisoAuxiliar.Effect = lightEffect;
            meshPisoAuxiliar.Technique = "PisoLocoTechnique";

            pelota = new Pelota(this);

            autitus = new List<Auto>();

            CreateCars();
            SetCarPositions();
            CreateViewports();
            initCarCameras();
            initMusic();
            ResizeFrustum();

        }

        private void initPeople()
        {
            var rgen = new Random();
            var loader = new TgcSceneLoader();
            people = new List<TgcMesh>();
            int xpos = 3000;
            int ypos = 62;
            int i = 1;

            //tribuna positiva
            while (i <= 10)
            {
                int zpos = -2300;
                while (zpos < 2500)
                {
                    var person = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\People\\dwight\\2D+People+-+Dwight+2-TgcScene.xml").Meshes[0];
                    //var person = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\People\\3persons\\3-Spectators-TgcScene.xml").Meshes[0];

                    zpos = zpos + rgen.Next(40, 700);
                    person.rotateY(FastMath.PI_HALF);
                    person.Scale = new Vector3(0.045f, 0.045f, 0.045f);
                    person.Position = new Vector3(xpos, ypos, zpos);
                    people.Add(person);
                    todosLosMeshes.Add(person);
                    zpos = zpos + rgen.Next(40, 700);
                }

                xpos = xpos + 180;
                ypos = ypos + 62;

                i++;
            }


            //Tribuna negativa
            xpos = -2800;
            ypos = 62;
            i = 1;

            while (i <= 10)
            {
                int zpos = -2300;
                while (zpos < 2500)
                {
                    var person = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\People\\dwight\\2D+People+-+Dwight+2-TgcScene.xml").Meshes[0];
                    //var person = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\People\\3persons\\3-Spectators-TgcScene.xml").Meshes[0];

                    zpos = zpos + rgen.Next(40, 700);
                    person.rotateY(FastMath.PI_HALF);
                    person.Scale = new Vector3(0.045f, 0.045f, 0.045f);
                    person.Position = new Vector3(xpos, ypos, zpos);
                    people.Add(person);
                    todosLosMeshes.Add(person);
                    zpos = zpos + rgen.Next(40, 700);
                }

                xpos = xpos - 180;
                ypos = ypos + 62;

                i++;
            }
        }

        private void initMusic()
        {
            GuiController.Instance.Mp3Player.FileName = pathSoundtrack;
            var player = GuiController.Instance.Mp3Player;
            player.play(true);
        }

        private void initMenues()
        {
            menuPrincipal = new Menu(this);
            menuControles = new MenuControles(this);
            gameOver = new GameOver(this);
            estadoJuego = EstadoJuego.MenuPrincipal;
        }

        private void CreateCars()
        {
            mainCar = new Auto(mainCarMesh, this);
            autitus.Add(mainCar);
            secondCar = new Auto2(secondCarMesh, this);
            autitus.Add(secondCar);
        }

        private void SetCarPositions()
        {
            var direccion = arcoNegativo.Position - arcoPositivo.Position;
            var pos1 = Vector3.Add(arcoPositivo.Position, Vector3.Multiply(direccion, 0.1f));
            var pos2 = Vector3.Add(arcoNegativo.Position, Vector3.Multiply(direccion, -0.1f));
            var pos3 = Vector3.Add(pos1, new Vector3(300, 0, 0));
            

            mainCar.setPosition(pos1.X, 0, pos1.Z);
            secondCar.setPosition(pos2.X, 0, pos2.Z);
        }

#endregion

        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>

        #region RENDERING

        public override void render(float elapsedTime)
        {
            if (time >= gameDuration)
            {
                estadoJuego = EstadoJuego.GameOver;
            }
            switch (estadoJuego)
            {
                case EstadoJuego.MenuPrincipal:
                    menuPrincipal.render();
                    break;
                case EstadoJuego.MenuControles:
                    menuControles.render();
                    break;
                case EstadoJuego.Juego:
                    renderGame(elapsedTime);
                    break;
                case EstadoJuego.GameOver:
                    splitScreen = false;
                    d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                    d3dDevice.Viewport = ViewF;
                    gameOver.render();
                    break;
            }

           // people.ForEach(p => p.render());
        }

        private void renderGame(float elapsedTime)
        {

            lastElapsedTime = elapsedTime;

            foreach (var auto in autitus)
            {
                auto.elapsedTime = elapsedTime;
                auto.Mover(elapsedTime);

            }

            pelota.mover(elapsedTime);
            pelota.updateValues();

            golTimer = golTimer - elapsedTime;
            time = time + elapsedTime;

            SetCarCamera();
            SetViewport();
            DoRenderAll();
            //RenderAll(); //para crear las texturas de envirnoment map
        }

        private void DoRenderAll()
        {


            if (splitScreen)
            {
                d3dDevice.Viewport = View1;
                d3dDevice.Transform.View = camaraActiva1.GetUpdatedViewMatrix();
                d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                currentCameraPos = camaraActiva1.getPosition();
                RenderAllObjects(false);


                d3dDevice.Viewport = View2;
                d3dDevice.Transform.View = camaraActiva2.GetUpdatedViewMatrix();
                d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                currentCameraPos = camaraActiva2.getPosition();
                RenderAllObjects(false);

            }
            else
            {
                d3dDevice.Transform.View = camaraActiva1.GetUpdatedViewMatrix();
                d3dDevice.Viewport = ViewF;
                currentCameraPos = camaraActiva1.getPosition();
                //txtDebug.Text = currentCameraPos.ToString();
                RenderAllObjects(false);
            }
        }


        private void RenderAll() 
        {
            Control panel3d = GuiController.Instance.Panel3d;
            float aspectRatio = (float)panel3d.Width / (float)panel3d.Height;

            d3dDevice.EndScene();
            CubeTexture g_pCubeMap = new CubeTexture(d3dDevice, 256, 1, Usage.RenderTarget,
                Format.A16B16G16R16F, Pool.Default);
            
            Surface pOldRT = d3dDevice.GetRenderTarget(0);
            // ojo: es fundamental que el fov sea de 90 grados.
            // asi que re-genero la matriz de proyeccion
            d3dDevice.Transform.Projection =
                Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(90.0f),
                    1f, 1f, 10000f);

            // Genero las caras del enviroment map
            for (CubeMapFace nFace = CubeMapFace.PositiveX; nFace <= CubeMapFace.NegativeZ; ++nFace)
            {
                Surface pFace = g_pCubeMap.GetCubeMapSurface(nFace, 0);
                d3dDevice.SetRenderTarget(0, pFace);
                Vector3 Dir, VUP;
                Color color;
                switch (nFace)
                {
                    default:
                    case CubeMapFace.PositiveX:
                        // Left
                        Dir = new Vector3(1, 0, 0);
                        VUP = new Vector3(0, 1, 0);
                        color = Color.Black;
                        break;
                    case CubeMapFace.NegativeX:
                        // Right
                        Dir = new Vector3(-1, 0, 0);
                        VUP = new Vector3(0, 1, 0);
                        color = Color.Red;
                        break;
                    case CubeMapFace.PositiveY:
                        // Up
                        Dir = new Vector3(0, 1, 0);
                        VUP = new Vector3(0, 0, -1);
                        color = Color.Gray;
                        break;
                    case CubeMapFace.NegativeY:
                        // Down
                        Dir = new Vector3(0, -1, 0);
                        VUP = new Vector3(0, 0, 1);
                        color = Color.Yellow;
                        break;
                    case CubeMapFace.PositiveZ:
                        // Front
                        Dir = new Vector3(0, 0, 1);
                        VUP = new Vector3(0, 1, 0);
                        color = Color.Green;
                        break;
                    case CubeMapFace.NegativeZ:
                        // Back
                        Dir = new Vector3(0, 0, -1);
                        VUP = new Vector3(0, 1, 0);
                        color = Color.Blue;
                        break;
                }

                //Obtener ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
                Vector3 Pos = mainCarMesh.Position + new Vector3(0, 50, 0); //levanto un poquito el centro del cubo porque sino me queda al ras del piso
                //Vector3 Pos = mainCarMesh.Position;
                d3dDevice.Transform.View = Matrix.LookAtLH(Pos, Pos + Dir, VUP);

                d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, color, 1.0f, 0);



                d3dDevice.BeginScene();

                //Renderizar 
                RenderAllObjects(true);

                d3dDevice.EndScene();

                string fname = string.Format("D:\\UTN\\face{0:D}.bmp", nFace);
                SurfaceLoader.Save(fname, ImageFileFormat.Bmp, pFace);


            }
            // restuaro el render target
            d3dDevice.SetRenderTarget(0, pOldRT);
            //TextureLoader.Save("test.bmp", ImageFileFormat.Bmp, g_pCubeMap);

            // Restauro el estado de las transformaciones
            GuiController.Instance.CurrentCamera.updateViewMatrix(d3dDevice);
            d3dDevice.Transform.Projection =
                Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f),
                    aspectRatio, 1f, 10000f);

            // dibujo pp dicho
            d3dDevice.BeginScene();
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            mainEffect.SetValue("g_txCubeMap", g_pCubeMap);
            DoRenderAll();
            g_pCubeMap.Dispose();


        } //ESTE MÉTODO SÓLO PARA ENVIRONMENT MAP!

        private void RenderAllObjects(Boolean cubemap)
        {
            Microsoft.DirectX.Direct3D.Effect currentShader;
            String currentTechnique;
            bool lightEnable = (bool)GuiController.Instance.Modifiers["lightEnable"];
            if (lightEnable)
            {
                //Shader personalizado de iluminacion
                currentShader = this.lightEffect;
                currentTechnique = "MultiDiffuseLightsTechnique";

            }
            else
            {
                //Sin luz: Restaurar shader default
                currentShader = GuiController.Instance.Shaders.TgcMeshShader;
                currentTechnique = GuiController.Instance.Shaders.getTgcMeshTechnique(TgcMesh.MeshRenderType.DIFFUSE_MAP);
            }

            
            foreach (TgcMesh mesh in todosLosMeshes)
            {
                mesh.Effect = currentShader;
                mesh.Technique = currentTechnique;

                if (lightEnable)
                {
                    mainCarMesh.Effect = carEffect;
                    secondCarMesh.Effect = carEffect;
                    mainCarMesh.Effect.SetValue("g_txCubeMap", cubeTexture);
                    secondCarMesh.Effect.SetValue("g_txCubeMap", cubeTexture);
                    mainCarMesh.Technique = "DifAndSpecTechnique";

                    secondCarMesh.Technique = "DifAndSpecTechnique";
                }
            }

            ColorValue[] lightColors = new ColorValue[lightMeshes.Length];
            Vector4[] pointLightPositions = new Vector4[lightMeshes.Length];
            float[] pointLightIntensity = new float[lightMeshes.Length];
            float[] pointLightAttenuation = new float[lightMeshes.Length];
            for (int i = 0; i < lightMeshes.Length; i++)
            {
                TgcBox lightMesh = lightMeshes[i];

                lightColors[i] = ColorValue.FromColor(lightMesh.Color);
                pointLightPositions[i] = TgcParserUtils.vector3ToVector4(lightMesh.Position);
                pointLightIntensity[i] = (float)GuiController.Instance.Modifiers["lightIntensity"];
                pointLightAttenuation[i] = (float)GuiController.Instance.Modifiers["lightAttenuation"];

                
            }
            

            pelota.ownSphere.Effect = currentShader;
            pelota.ownSphere.Technique = currentTechnique;
            skyBox.render();

            foreach (TgcMesh mesh in todosLosMeshes)
            {
                if (lightEnable && mesh != mainCarMesh)
                {

                    //Cargar variables de shader
                    mesh.Effect.SetValue("lightColor", lightColors);
                    mesh.Effect.SetValue("lightPosition", pointLightPositions);
                    mesh.Effect.SetValue("lightIntensity", pointLightIntensity);
                    mesh.Effect.SetValue("lightAttenuation", pointLightAttenuation);
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));

                    mesh.Effect.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat4Array(currentCameraPos));
                    mesh.Effect.SetValue("shininess", 1);
                }

                //Renderizar modelo
                mesh.render();
            }

            if (lightEnable)
            {
                pelota.ownSphere.Effect.SetValue("lightColor", lightColors);
                pelota.ownSphere.Effect.SetValue("lightPosition", pointLightPositions);
                pelota.ownSphere.Effect.SetValue("lightIntensity", pointLightIntensity);
                pelota.ownSphere.Effect.SetValue("lightAttenuation", pointLightAttenuation);
                pelota.ownSphere.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                pelota.ownSphere.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));

                pelota.ownSphere.Effect.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat4Array(currentCameraPos));
                pelota.ownSphere.Effect.SetValue("shininess", 1);
            }

            pelota.ownSphere.AlphaBlendEnable = false;
            pelota.render();

            if (lightEnable)
            {
                pelota.ownSphere.Effect = sombrasEffect;
                pelota.ownSphere.Technique = "SombrasTechnique";
                mainCarMesh.Effect = sombrasAutoEffect;
                mainCarMesh.Technique = "SombrasTechnique";
                secondCarMesh.Effect = sombrasAutoEffect;
                secondCarMesh.Technique = "SombrasTechnique";

                

                for (int i = 0; i < lightMeshes.Length; i++)
                {
                    int j = (i +1) % 4;
                    TgcBox lightMesh = lightMeshes[j];
                
                    pointLightPositions[j] = TgcParserUtils.vector3ToVector4(lightMesh.Position);


                    pelota.ownSphere.AlphaBlendEnable = true;

                    pelota.ownSphere.Effect.SetValue("matViewProj", d3dDevice.Transform.View * d3dDevice.Transform.Projection);
                    pelota.ownSphere.Effect.SetValue("centroPelota", TgcParserUtils.vector3ToVector4(pelota.pos));
                    pelota.ownSphere.Effect.SetValue("lightPosition", pointLightPositions[j]);
                    pelota.ownSphere.Effect.SetValue("lightOrder", i);
                    pelota.ownSphere.render();
                    

                    pelota.ownSphere.AlphaBlendEnable = false;
                }

                for (int i = 0; i < lightMeshes.Length; i++)
                {
                    int j = (i + 1) % 4;
                    TgcBox lightMesh = lightMeshes[j];

                    pointLightPositions[j] = TgcParserUtils.vector3ToVector4(lightMesh.Position);
                    

                    mainCarMesh.Effect.SetValue("matViewProj", d3dDevice.Transform.View * d3dDevice.Transform.Projection);
                    mainCarMesh.Effect.SetValue("centroPelota", TgcParserUtils.vector3ToVector4(mainCar.pos));
                    mainCarMesh.Effect.SetValue("lightPosition", pointLightPositions[j]);
                    mainCarMesh.Effect.SetValue("lightOrder", i+4);
                    mainCarMesh.render();
                    
                }

                for (int i = 0; i < lightMeshes.Length; i++)
                {
                    int j = (i + 1) % 4;
                    TgcBox lightMesh = lightMeshes[j];

                    pointLightPositions[j] = TgcParserUtils.vector3ToVector4(lightMesh.Position);


                    secondCarMesh.Effect.SetValue("matViewProj", d3dDevice.Transform.View * d3dDevice.Transform.Projection);
                    secondCarMesh.Effect.SetValue("centroPelota", TgcParserUtils.vector3ToVector4(secondCar.pos));
                    secondCarMesh.Effect.SetValue("lightPosition", pointLightPositions[j]);
                    secondCarMesh.Effect.SetValue("lightOrder", i + 8);
                    secondCarMesh.render();

                }


                if (lightEnable)
                {
                    meshPisoAuxiliar.AlphaBlendEnable = true;
                    meshPisoAuxiliar.render();
                }

                //mainCar.obb.render();
                //secondCar.obb.render();
                                

            }


            if (!cubemap)
            { //ningún hud debería aparecer en el cubemap cuando lo estamos generando

                txtScoreVisitante.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");
                txtScoreLocal.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");
                txtDebug.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");
                txtGol.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");
                txtNitroVisitante.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");
                txtNitroLocal.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");

                if(time >= 110f)
                {
                    txtTimer.Color = Color.Red;
                }
                else
                {
                    txtTimer.Color = (Color)GuiController.Instance.Modifiers.getValue("ColorHUD");
                }

                txtTimer.Text = formatAsTime(time);
                txtScoreLocal.render();
                txtScoreVisitante.render();
                txtTimer.render();
                txtNitroVisitante.render();
                txtNitroLocal.render();

                if (golTimer > 0)
                {
                    txtGol.render();
                }

            }
        }

        public String formatAsTime(float f)
        {
            int totalSecs = (int)f;

            int secs = totalSecs % 60;
            int mins = totalSecs / 60;
            String secsString = secs.ToString();
            String minsString = mins.ToString();

            if (secs < 10) secsString = "0" + secsString;
            if (mins < 10) minsString = "0" + minsString;

            return minsString + ":" + secsString;

        }
        #endregion

        #region CAMERAS
        private void ResizeFrustum()
        {
            GuiController.Instance.D3dDevice.Transform.Projection = Matrix.PerspectiveFovLH(
                FastMath.ToRad(45.0f), (float)GuiController.Instance.Panel3d.Width / GuiController.Instance.Panel3d.Height, 1.0f, float.MaxValue);
        }

        private void SetViewport()
        {
            if (GuiController.Instance.D3dInput.keyPressed(Microsoft.DirectX.DirectInput.Key.V))
            {
                if (splitScreen)
                    splitScreen = false;
                else
                    splitScreen = true;
            }

        }

        private void CreateViewports()
        {
            Control panel3d = GuiController.Instance.Panel3d;
            View1 = new Viewport();
            View1.X = 0;
            View1.Y = 0;
            View1.Width = panel3d.Width;
            View1.Height = panel3d.Height / 2;
            View1.MinZ = 0;
            View1.MaxZ = 1;
            View2 = new Viewport();
            View2.X = 0;
            View2.Y = View1.Height;
            View2.Width = panel3d.Width;
            View2.Height = panel3d.Height / 2;
            View2.MinZ = 0;
            View2.MaxZ = 1;

            ViewF = GuiController.Instance.D3dDevice.Viewport;
        }

        private void SetCarCamera()
        {
            var pelotaPos = pelota.pos;
            var autoPos = mainCar.pos;
            var auto2Pos = secondCar.pos;
            ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyDown(Key.LeftShift))
            {
                camaraAuto1.RotationY = mainCar.rotacion;
                camaraAuto1.Target = autoPos;
                camaraActiva1 = camaraAuto1;
            }
            else
            {
                camaraPelota1.FirstTarget = autoPos;
                camaraPelota1.SecondTarget = pelotaPos;
                camaraActiva1 = camaraPelota1;
            }
            /////////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia
            if (input.keyDown(Key.RightAlt))
            {
                camaraAuto2.RotationY = secondCar.rotacion + FastMath.PI;
                camaraAuto2.Target = auto2Pos;
                camaraActiva2 = camaraAuto2;
            }
            else
            {
                camaraPelota2.FirstTarget = auto2Pos;
                camaraPelota2.SecondTarget = pelotaPos;
                camaraActiva2 = camaraPelota2;
            }

        }

        

        private void initCarCameras()
        {
            camaraPelota1 = new TwoTargetsCamera(this);
            camaraAuto1 = new MilanesaThirdPersonCamera();

            camaraPelota2 = new TwoTargetsCamera(this);
            camaraAuto2 = new MilanesaThirdPersonCamera();

            camaraAuto1.setCamera(mainCar.pos, 40, 250);
            camaraPelota1.setCamera(mainCar.pos, 0, 0);            

            camaraAuto2.setCamera(secondCar.pos, 40, 250);
            camaraPelota2.setCamera(secondCar.pos, 0, 0);
        }
        #endregion CAMERAS

        #region GOLES
        public void golLocal()
        {
            scoreLocal++;
            gol();
        }

        public void golVisitante()
        {
            scoreVisitante++;
            gol();
        }

        public void gol()
        {
            golTimer = 2;
            txtScoreLocal.Text = "Equipo Rojo: " + scoreLocal.ToString();
            txtScoreVisitante.Text = "Equipo Azul: " + scoreVisitante.ToString();
            
            pelota.ownSphere.dispose();
            pelota = new Pelota(this);
            SetCarPositions();

        }
        #endregion GOLES

        public override void close()
        {

            //todo creo que falta disposear algunas cosas!
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            d3dDevice.Viewport = ViewF;
            GuiController.Instance.Modifiers.clear();
            GuiController.Instance.Mp3Player.closeFile();
            mainCar.meshAuto.dispose();
            secondCar.meshAuto.dispose();
            pelota.ownSphere.dispose();

            foreach (TgcMesh mesh in meshesCajasEscenario)
            {
                mesh.dispose();
            }

            scene.disposeAll();
            piso.dispose();
            pisoArcoNegativo.dispose();
            pisoArcoPositivo.dispose();

            foreach (TgcBox box in cajasVisiblesEscenario)
            {
                box.dispose();
            }
            foreach (TgcBoundingBox bb in limitesArcos)
            {
                bb.dispose();
            }
            foreach (TgcBoundingBox bb in laterales)
            {
                bb.dispose();
            }
            arcoNegativo.dispose();
            arcoPositivo.dispose();

            foreach(TgcMesh dwight in people)
            {
                dwight.dispose();
            }

            int i = 0;

            while (i < 4)
            {
                lightMeshes[i].dispose();
                i++;
            }

            skyBox.dispose();
            meshPisoAuxiliar.dispose();
        }

        public void resetGame()
        {
            close();
            init();
        }
    }
}
