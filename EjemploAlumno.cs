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
using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.MiGrupo
{

    public class EjemploAlumno : TgcExample
    {
        TgcSkyBox skyBox;
        TgcScene scene;
        TgcMesh mainCarMesh;
        Auto mainCar;
        TwoTargetsCamera camaraPelota = new TwoTargetsCamera();
        TgcThirdPersonCamera camaraAuto = GuiController.Instance.ThirdPersonCamera;
        public Pelota pelota;
        TgcText2d txtScoreLocal = new TgcText2d();
        TgcText2d txtScoreVisitante = new TgcText2d();


        public int scoreLocal = 0;
        public int scoreVisitante = 0;

        public List<Auto> autitus;
        public List<TgcBoundingBox> paredes; 
        public List<TgcBoundingBox> laterales;


        public TgcBox piso;
        public TgcBox arcoPositivo;
        public TgcBox arcoNegativo;

        public TgcBox paredArcoNegativo1;
        public TgcBox paredArcoNegativo2;
        public TgcBox paredArcoNegativo3;

        public TgcBox paredArcoPositivo1;
        public TgcBox paredArcoPositivo2;
        public TgcBox paredArcoPositivo3;

        public TgcBox lateralPositivo;
        public TgcBox lateralNegativo;

        


        static string rootDir = GuiController.Instance.AlumnoEjemplosDir;
        static string mediaFolder = rootDir + "MiGrupo\\media\\";
        static string sceneFolder = mediaFolder + "meshes\\scenes\\";


        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Grupo 99";
        }

        public override string getDescription()
        {
            return "Rocket League - Futbol de autos";
        }

        public void crearEscenario()
        {
            piso = TgcBox.fromExtremes(new Vector3(-4000, -100, -11000), new Vector3(4000, 0, 11000));
            arcoPositivo = TgcBox.fromExtremes(new Vector3(-500, 0, 11000), new Vector3(500, 400, 12000));
            arcoNegativo = TgcBox.fromExtremes(new Vector3(-500, 0, -12000), new Vector3(500, 400, -11000));
            
            paredArcoNegativo1 = TgcBox.fromExtremes(new Vector3(-4000, 0, -11000), new Vector3(-500, 2000, -11050));
            paredArcoNegativo2 = TgcBox.fromExtremes(new Vector3(4000, 0, -11000), new Vector3(500, 2000, -11050));
            paredArcoNegativo3 = TgcBox.fromExtremes(new Vector3(-500, 400, -11000), new Vector3(500, 2000, -11050));

            paredArcoPositivo1 = TgcBox.fromExtremes(new Vector3(-4000, 0, 11000), new Vector3(-500, 2000, 11050));
            paredArcoPositivo2 = TgcBox.fromExtremes(new Vector3(4000, 0, 11000), new Vector3(500, 2000, 11050));
            paredArcoPositivo3 = TgcBox.fromExtremes(new Vector3(-500, 400, 11000), new Vector3(500, 2000, 11050));

            lateralPositivo = TgcBox.fromExtremes(new Vector3(-4000, 0, -11000), new Vector3(-4050, 2000, 11000));
            lateralNegativo = TgcBox.fromExtremes(new Vector3(4000, 0, -11000), new Vector3(4050, 2000, 11000));
        }

        public override void init()
        {
            var d3dDevice = GuiController.Instance.D3dDevice;
            crearEscenario();

            

            txtScoreLocal.Text = scoreLocal.ToString();
            txtScoreLocal.Position = new Point(300, 100);
            txtScoreLocal.Size = new Size(300, 100);
            
            txtScoreVisitante.Text = scoreVisitante.ToString();
            txtScoreVisitante.Position = new Point(600, 100);
            txtScoreVisitante.Size = new Size(300, 100);

            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 200, 0);
            skyBox.Size = new Vector3(10000, 10000, 10000);
            string texturesPath = mediaFolder + "textures\\SkyBox3\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "Up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "Down.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "Left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "Right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "Back.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "Front.jpg");
            skyBox.updateValues();

            TgcTexture pasto = TgcTexture.createTexture(d3dDevice, mediaFolder + "textures\\pasto.jpg");
            piso.setTexture(pasto);
            piso.UVTiling = new Vector2(150, 150);
            piso.updateValues();

            TgcSceneLoader loader = new TgcSceneLoader();
            //scene = loader.loadSceneFromFile(sceneFolder + "ss2\\IndoorSoccerField--TgcScene.xml");
            
            mainCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Auto\\Auto-TgcScene.xml").Meshes[0];

            GuiController.Instance.UserVars.addVar("Velocidad");

            GuiController.Instance.Modifiers.addFloat("Gravedad", -50, 0, -9.81f);
            GuiController.Instance.Modifiers.addFloat("Aceleracion", 200f, 1000f, 600f);
            GuiController.Instance.Modifiers.addFloat("VelocidadRotacion", 0f, 5f, 1f);

            paredes = new List<TgcBoundingBox>();
            laterales = new List<TgcBoundingBox>();

            paredes.Add(paredArcoNegativo3.BoundingBox);
            paredes.Add(paredArcoNegativo2.BoundingBox);
            paredes.Add(paredArcoNegativo1.BoundingBox);
            paredes.Add(paredArcoPositivo3.BoundingBox);
            paredes.Add(paredArcoPositivo2.BoundingBox);
            paredes.Add(paredArcoPositivo1.BoundingBox);
            laterales.Add(lateralNegativo.BoundingBox);
            laterales.Add(lateralPositivo.BoundingBox);

            pelota = new Pelota(this);

            autitus = new List<Auto>();

            mainCar = new Auto(mainCarMesh, this);
            autitus.Add(mainCar);


            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas
            
            GuiController.Instance.RotCamera.CameraDistance = 100;
            initCarCamera();

        }



        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {

            //Device de DirectX para renderizar
            var d3dDevice = GuiController.Instance.D3dDevice;


            //conviene deshabilitar ambas camaras para que no haya interferencia

            //Capturar Input teclado 
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.F))
            {

                //Tecla F apretada
            }

            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Space))
            {

            }

            //Capturar Input Mouse
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Boton izq apretado
            }



            //Mover Auto
            mainCar.elapsedTime = elapsedTime;
            mainCar.Mover(elapsedTime);
            skyBox.Center = mainCar.meshAuto.Position;
            skyBox.updateValues();
            //
            

            pelota.mover(elapsedTime);
            pelota.updateValues();
            pelota.render();

            foreach(TgcBoundingBox p in paredes)
            {
                p.render();
            }

            foreach (TgcBoundingBox l in laterales)
            {
                l.render();
            }

            arcoPositivo.BoundingBox.render();
            arcoNegativo.BoundingBox.render();
           // scene.renderAll();
            piso.render();
            mainCar.render();
            mainCar.obb.render();
            skyBox.render();
            txtScoreLocal.render();
            txtScoreVisitante.render();
            SetCarCamera();
        }

        private void SetCarCamera()
        {
            var pelotaPos = pelota.ownSphere.Position;
            var autoPos = mainCarMesh.Position;
             ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyDown(Key.LeftShift))
            {
                camaraPelota.FirstTarget = autoPos;
                camaraPelota.SecondTarget = pelotaPos;
                camaraPelota.Enable=true;;
                camaraPelota.updateCamera();
            }
            else
            {
                camaraAuto.Enable = true;
                camaraAuto.RotationY = mainCar.rotacion;
                camaraAuto.Target = mainCar.meshAuto.Position;
                camaraAuto.updateCamera();
            }
            
        }

        private void initCarCamera()
        {
            camaraAuto.setCamera(mainCarMesh.Position, 40, 250);
            camaraAuto.Enable = true;

            camaraPelota.setCamera(mainCar.meshAuto.Position, 0, 0);
            camaraPelota.TargetDisplacement = new Vector3(0, 40, 0);
        }


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
            txtScoreLocal.Text = scoreLocal.ToString();
            txtScoreVisitante.Text = scoreVisitante.ToString();

            mainCar.meshAuto.Position = new Vector3(0,0,0);
            mainCar.obb = TgcObb.computeFromAABB(mainCar.meshAuto.BoundingBox);
            pelota.ownSphere.dispose();
            pelota = new Pelota(this);

        }

        public override void close()
        {
            mainCar.meshAuto.dispose();
            pelota.ownSphere.dispose();
          //  scene.disposeAll();
            skyBox.dispose();
        }

    }
}