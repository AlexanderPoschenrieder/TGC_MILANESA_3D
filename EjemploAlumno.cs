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

namespace AlumnoEjemplos.MiGrupo
{
    
    public class EjemploAlumno : TgcExample
    {
        TgcSkyBox skyBox;
        TgcScene scene;
        TgcMesh mainCarMesh,secondCarMesh,iaCarMesh;
        Auto mainCar;
        Auto2 secondCar;
        AutoIA iaCar;
        TgcCamera camaraActiva1, camaraActiva2;
        float farPlane = 100.0f;

        Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

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
        public List<TgcBoundingBox> limitesArcos; 
        public List<TgcBoundingBox> laterales;

        public List<TgcBox> cajasVisiblesEscenario;


        public TgcBox piso;
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


        static string rootDir = GuiController.Instance.AlumnoEjemplosDir;
        static string mediaFolder = rootDir + "MiGrupo\\media\\";
        static string sceneFolder = mediaFolder + "meshes\\scenes\\";
        private TgcBox rejaArcoPositivo1;
        private TgcBox rejaArcoPositivo2;
        private TgcBox rejaSuperiorArcoPositivo;
        private TgcBox rejaArcoNegativo1;
        private TgcBox rejaArcoNegativo2;
        private TgcBox rejaSuperiorArcoNegativo;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Grupo 99";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string getDescription()
        {
            return "Rocket League - Futbol de autos. Auto principal: movimiento con W, A, S, D, Ctrl izq, Espacio. Auto secundario: movimiento con flechas, Ctrl der y Alt der. Shift izq apunta la cámara hacia la pelota.";
        }

        public void crearEscenario()
        {
            piso = TgcBox.fromExtremes(new Vector3(-2600, -100, -8000), new Vector3(2600, 0, 8000));

            arcoPositivo = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-500, 0, 8000), new Vector3(500, 400, 9000) });
            arcoNegativo = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-500, 0, -9000), new Vector3(500, 400, -8000) });
            

            limiteArcoNegativo1 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-2600, 0, -8050), new Vector3(-500, 1000, -8000) });
            limiteArcoNegativo2 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(500, 0, -8050), new Vector3(2600, 1000, -8000) });
            limiteArcoNegativo3 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-500, 400, -8050), new Vector3(500, 1000, -8000) });

            paredArcoNegativo1 = TgcBox.fromExtremes(new Vector3(-2600, 0, -8050), new Vector3(-500, 100, -8000));
            paredArcoNegativo2 = TgcBox.fromExtremes(new Vector3(500, 0, -8050), new Vector3(2600, 100, -8000));

            rejaArcoNegativo1 = TgcBox.fromExtremes(new Vector3(-2600, 100, -8000), new Vector3(-500, 400, -8000));
            rejaArcoNegativo2 = TgcBox.fromExtremes(new Vector3(500, 100, -8000), new Vector3(2600, 400, -8000));
            rejaSuperiorArcoNegativo = TgcBox.fromExtremes(new Vector3(-2600, 400, -8000), new Vector3(2600, 1000, -8000));

            limiteArcoPositivo1 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-2600, 0, 8000), new Vector3(-500, 1000, 8050) });
            limiteArcoPositivo2 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(500, 0, 8000), new Vector3(2600, 1000, 8050) });
            limiteArcoPositivo3 = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-500, 400, 8000), new Vector3(500, 1000, 8050) });

            paredArcoPositivo1 = TgcBox.fromExtremes(new Vector3(-2600, 0, 8000), new Vector3(-500, 100, 8050));
            paredArcoPositivo2 = TgcBox.fromExtremes(new Vector3(500, 0, 8000), new Vector3(2600, 100, 8050));

            rejaArcoPositivo1 = TgcBox.fromExtremes(new Vector3(-2600, 100, 8000), new Vector3(-500, 400, 8000));
            rejaArcoPositivo2 = TgcBox.fromExtremes(new Vector3(500, 100, 8000), new Vector3(2600, 400, 8000));
            rejaSuperiorArcoPositivo = TgcBox.fromExtremes(new Vector3(-2600, 400, 8000), new Vector3(2600, 1000, 8000));

            limiteLateralPositivo = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-2650, 0, -8000), new Vector3(-2600, 1000, 8000) });
            limiteLateralNegativo = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(2600, 0, -8000), new Vector3(2650, 1000, 8000) });

            paredLateralNegativa = TgcBox.fromExtremes(new Vector3(-2600, 0, -8000), new Vector3(-2600, 100, 8000));
            paredLateralPositiva = TgcBox.fromExtremes(new Vector3(2600, 0, -8000), new Vector3(2600, 100, 8000));

            rejaLateralNegativa = TgcBox.fromExtremes(new Vector3(-2600, 100, -8000), new Vector3(-2600, 1000, 8000));
            rejaLateralPositiva = TgcBox.fromExtremes(new Vector3(2600, 100, -8000), new Vector3(2600, 1000, 8000));


            TgcTexture pasto = TgcTexture.createTexture(d3dDevice, mediaFolder + "textures\\pasto.jpg");
            piso.setTexture(pasto);
            piso.UVTiling = new Vector2(150, 150);

            cajasVisiblesEscenario.Add(piso);



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


            foreach (TgcBox box in cajasVisiblesEscenario)
            {
                box.updateValues();
            }

        }

        public override void init()
        {

            cajasVisiblesEscenario = new List<TgcBox>();
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

           
         

            TgcSceneLoader loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(sceneFolder + "predio\\predio-TgcScene.xml");
            
            
            
            mainCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Auto\\Auto-TgcScene.xml").Meshes[0];
            secondCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Auto\\Auto-TgcScene.xml").Meshes[0];
            secondCarMesh.setColor(Color.Red);
            iaCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Auto\\Auto-TgcScene.xml").Meshes[0];
            iaCarMesh.setColor(Color.Green);

            GuiController.Instance.UserVars.addVar("Velocidad");
            GuiController.Instance.UserVars.addVar("Pos Auto 1");
            GuiController.Instance.UserVars.addVar("Pos Obb 1");
            GuiController.Instance.UserVars.addVar("Pos Auto 2");
            GuiController.Instance.UserVars.addVar("Pos Obb 2");
            GuiController.Instance.UserVars.addVar("Pos Pelota");

            GuiController.Instance.Modifiers.addFloat("Gravedad", -50, 0, -9.81f);
            GuiController.Instance.Modifiers.addFloat("Aceleracion", 100f, 1000f, 500f);
            GuiController.Instance.Modifiers.addFloat("VelocidadRotacion", 0f, 5f, 1.5f);

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

            pelota = new Pelota(this);

            autitus = new List<Auto>();

            SetCarPositions();
            CreateViewports();
            initCarCameras();
            //ResizeFrustum();
        }

        private void SetCarPositions()
        {
            var direccion = arcoNegativo.Position - arcoPositivo.Position;
            var pos1=Vector3.Add(arcoPositivo.Position, Vector3.Multiply(direccion,0.1f));
            var pos2 = Vector3.Add(arcoNegativo.Position, Vector3.Multiply(direccion, -0.1f));
            var pos3 = Vector3.Add(pos1, new Vector3(300,0,0));

            mainCarMesh.Position = new Vector3(pos1.X, 0, pos1.Z);
            mainCar = new Auto(mainCarMesh, this);
            autitus.Add(mainCar);

            secondCarMesh.Position = new Vector3(pos2.X, 0, pos2.Z);
            secondCar = new Auto2(secondCarMesh, this);

            secondCarMesh.Position = new Vector3(pos2.X, 0, pos2.Z);
            secondCar = new Auto2(secondCarMesh, this);

            iaCarMesh.Position = new Vector3(pos3.X, 0, pos3.Z);
            iaCar = new AutoIA(iaCarMesh, this);

            autitus.Add(secondCar);
            //autitus.Add(iaCar);
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
            foreach (var auto in autitus)
            {
                auto.elapsedTime = elapsedTime;
                auto.Mover(elapsedTime);
            }

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


                skyBox.Center = secondCar.meshAuto.Position;
                skyBox.updateValues();
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
           
            arcoPositivo.render();
            arcoNegativo.render();
            scene.renderAll();

            foreach (var auto in autitus)
            {
                auto.render();
            }

            skyBox.render();

            foreach (TgcBox box in cajasVisiblesEscenario)
            {
                box.render();
            }

            txtScoreLocal.render();
            txtScoreVisitante.render();
        }

        private void ResizeFrustum(float aspectRatio)
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

            //todo creo que falta disposear algunas cosas!
            mainCar.meshAuto.dispose();
            secondCar.meshAuto.dispose();
            pelota.ownSphere.dispose();

            scene.disposeAll();
            piso.dispose();

            foreach (TgcBox box in cajasVisiblesEscenario)
            {
                box.dispose();
            }

            skyBox.dispose();
        }

    }
}