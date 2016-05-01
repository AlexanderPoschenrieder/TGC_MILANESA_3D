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

namespace AlumnoEjemplos.MiGrupo
{

    public class EjemploAlumno : TgcExample
    {

        TgcScene scene;
        TgcMesh mainCarMesh;
        Auto mainCar;
        public Pelota pelota;

        public List<Auto> autitus = new List<Auto>();

        public TgcBoundingBox piso = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-100000, -100, -100000), new Vector3(0, 0, 0), new Vector3(100000, 0, 10000)});
        

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

        
        public override void init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(sceneFolder + "Futbol\\indoor+fieldx150-TgcScene.xml");
            mainCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Auto\\Auto-TgcScene.xml").Meshes[0];


            pelota = new Pelota(this);
        
            mainCar = new Auto(mainCarMesh, this);
            autitus.Add(mainCar);


            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas
            Device d3dDevice = GuiController.Instance.D3dDevice;
            GuiController.Instance.RotCamera.CameraDistance = 100;
            initCarCamera();
        
        }


        
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            
            //Device de DirectX para renderizar
            Device d3dDevice = GuiController.Instance.D3dDevice;

            
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
            mainCar.Mover();
            //

            scene.renderAll();


            pelota.mover(elapsedTime);
            pelota.updateValues();
            pelota.render();

            piso.render();
            mainCar.meshAuto.render();
            mainCar.obb.render();
            SetCarCamera();
        }

        private void SetCarCamera()
        {
            GuiController.Instance.ThirdPersonCamera.RotationY = mainCar.rotacion;
            GuiController.Instance.ThirdPersonCamera.Target = mainCar.meshAuto.Position;
            GuiController.Instance.ThirdPersonCamera.updateCamera();
        }

        private void initCarCamera()
        {
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(mainCar.meshAuto.Position, 40, 250);
            GuiController.Instance.RotCamera.CameraDistance = 50;
        }

        public override void close()
        {
            mainCar.meshAuto.dispose();
            pelota.ownSphere.dispose();
            scene.disposeAll();
        }

    }
}
