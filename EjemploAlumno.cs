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
        Pelota pelota;
        List<TgcBoundingBox> objetosColisionables = new List<TgcBoundingBox>();

        

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
            return "Rocket League";
        }

        
        public override void init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(sceneFolder + "Futbol\\indoor+fieldx150-TgcScene.xml");
            mainCarMesh = loader.loadSceneFromFile(mediaFolder + "meshes\\objects\\Auto\\Auto-TgcScene.xml").Meshes[0];

            pelota = new Pelota();
        

            mainCar = new Auto(mainCarMesh);

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

            pelota.aplicarGravedad(elapsedTime);
            pelota.updateValues();
            pelota.render();

            mainCar.meshAuto.render();
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
            GuiController.Instance.ThirdPersonCamera.setCamera(mainCar.meshAuto.Position, 50, 200);
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
