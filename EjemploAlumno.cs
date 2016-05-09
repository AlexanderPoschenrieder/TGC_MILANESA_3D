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

namespace AlumnoEjemplos.MiGrupo
{

    public class EjemploAlumno : TgcExample
    {
        TgcSkyBox skyBox;
        TgcScene scene;
        TgcMesh mainCarMesh,secondCarMesh;
        Auto mainCar;
        Auto2 secondCar;
        TgcCamera camaraActiva1, camaraActiva2;

        TwoTargetsCamera camaraPelota1 = new TwoTargetsCamera();
        TgcThirdPersonCamera camaraAuto1 = new TgcThirdPersonCamera();

        TwoTargetsCamera camaraPelota2 = new TwoTargetsCamera();
        TgcThirdPersonCamera camaraAuto2 = new TgcThirdPersonCamera();

        public Pelota pelota;
        TgcText2d txtScoreLocal = new TgcText2d();
        TgcText2d txtScoreVisitante = new TgcText2d();
        Viewport View1, View2, ViewF;
        bool splitScreen = false;

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
            
            paredArcoNegativo1 = TgcBox.fromExtremes(new Vector3(-4000, 0, -11050), new Vector3(-500, 2000, -11000));
            paredArcoNegativo2 = TgcBox.fromExtremes(new Vector3(500, 0, -11050), new Vector3(4000, 2000, -11000));
            paredArcoNegativo3 = TgcBox.fromExtremes(new Vector3(-500, 400, -11050), new Vector3(500, 2000, -11000));

            paredArcoPositivo1 = TgcBox.fromExtremes(new Vector3(-4000, 0, 11000), new Vector3(-500, 2000, 11050));
            paredArcoPositivo2 = TgcBox.fromExtremes(new Vector3(500, 0, 11000), new Vector3(4000, 2000, 11050));
            paredArcoPositivo3 = TgcBox.fromExtremes(new Vector3(-500, 400, 11000), new Vector3(500, 2000, 11050));

            lateralPositivo = TgcBox.fromExtremes(new Vector3(-4050, 0, -11000), new Vector3(-4000, 2000, 11000));
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

            TgcTexture cemento = TgcTexture.createTexture(d3dDevice, mediaFolder + "textures\\paredRugosa.jpg");
            lateralNegativo.setTexture(cemento);
            lateralPositivo.setTexture(cemento);
            lateralNegativo.UVTiling = new Vector2(100, 10);
            lateralPositivo.UVTiling = new Vector2(100, 10);

            lateralNegativo.updateValues();
            lateralPositivo.updateValues();

            TgcSceneLoader loader = new TgcSceneLoader();
            //scene = loader.loadSceneFromFile(sceneFolder + "ss2\\IndoorSoccerField--TgcScene.xml");
            
            mainCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Auto\\Auto-TgcScene.xml").Meshes[0];
            secondCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Hummer\\Hummer-TgcScene.xml").Meshes[0];

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

            SetCarPositions();
            CreateViewports();
            initCarCameras();
        }

        private void SetCarPositions()
        {
            var direccion = arcoNegativo.Position - arcoPositivo.Position;
            var pos1=Vector3.Add(arcoPositivo.Position, Vector3.Multiply(direccion,0.1f));
            var pos2 = Vector3.Add(arcoNegativo.Position, Vector3.Multiply(direccion, -0.1f));

            mainCarMesh.Position = new Vector3(pos1.X, 0, pos1.Z);
            mainCar = new Auto(mainCarMesh, this);
            autitus.Add(mainCar);

            secondCarMesh.Position = new Vector3(pos2.X, 0, pos2.Z);
            secondCar = new Auto2(secondCarMesh, this);
            autitus.Add(secondCar);
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



        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            
            //Mover Auto
            mainCar.elapsedTime = elapsedTime;
            mainCar.Mover(elapsedTime);
            secondCar.elapsedTime = elapsedTime;
            secondCar.Mover(elapsedTime);

            pelota.mover(elapsedTime);
            pelota.updateValues();
 			skyBox.Center = mainCar.meshAuto.Position;
            skyBox.updateValues();
            SetCarCamera();
            SetViewport();
            RenderAll();
        }

        private void RenderAll()
        {
            if (splitScreen)
            {
                
                GuiController.Instance.D3dDevice.Viewport = View1;
                camaraActiva1.Enable=true;
                RenderAllObjects();

                GuiController.Instance.D3dDevice.Viewport = View2;
                camaraActiva2.Enable = true;
                RenderAllObjects();

            }
            else
            {
                camaraActiva1.updateCamera();
                GuiController.Instance.D3dDevice.Viewport = ViewF;
                camaraActiva1.Enable = true;
                RenderAllObjects();
            }
        }

        private void RenderAllObjects()
        {
            pelota.render();
            foreach (TgcBoundingBox p in paredes)
            {
                p.render();
            }
           

            arcoPositivo.BoundingBox.render();
            arcoNegativo.BoundingBox.render();
           // scene.renderAll();
            piso.render();
            lateralPositivo.render();
            lateralNegativo.render();

            mainCar.render();
            mainCar.obb.render();
            secondCar.render();
            secondCar.obb.render();
            skyBox.render();
            txtScoreLocal.render();
            txtScoreVisitante.render();
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

        private void SetCarCamera()
        {
            var pelotaPos = pelota.ownSphere.Position;
            var autoPos = mainCarMesh.Position;
            var auto2Pos = secondCarMesh.Position;
             ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyDown(Key.LeftShift))
            {
                camaraPelota1.FirstTarget = autoPos;
                camaraPelota1.SecondTarget = pelotaPos;
                camaraActiva1 = camaraPelota1;
            }
            else
            {
                camaraAuto1.RotationY = mainCar.rotacion;
                camaraAuto1.Target = mainCar.meshAuto.Position;
                camaraActiva1 = camaraAuto1;
            }
            /////////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia
            if (input.keyDown(Key.RightShift))
            {
                camaraPelota2.FirstTarget = auto2Pos;
                camaraPelota2.SecondTarget = pelotaPos;
                camaraActiva2 = camaraPelota2;
            }
            else
            {
                camaraAuto2.RotationY = secondCar.rotacion;
                camaraAuto2.Target = secondCar.meshAuto.Position;
                camaraActiva2 = camaraAuto2;
            }
            
            
        }

        private void initCarCameras()
        {
            camaraAuto1.setCamera(mainCarMesh.Position, 40, 250);
            camaraPelota1.setCamera(mainCarMesh.Position, 0, 0);
            camaraAuto1.Enable = true;

            camaraAuto2.setCamera(secondCarMesh.Position, 40, 250);
            camaraPelota2.setCamera(secondCarMesh.Position, 0, 0);
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