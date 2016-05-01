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
        TgcBox box;


<<<<<<< Updated upstream
        TgcScene scene;
        TgcMesh mainCarMesh;
        Auto mainCar;
        public Pelota pelota;

        public List<Auto> autitus = new List<Auto>();

        public TgcBoundingBox piso = TgcBoundingBox.computeFromPoints(new Vector3[] { new Vector3(-100000, -100, -100000), new Vector3(0, 0, 0), new Vector3(100000, 0, 10000)});
        

        static string rootDir = GuiController.Instance.AlumnoEjemplosDir;
        static string mediaFolder = rootDir + "MiGrupo\\media\\";
        static string sceneFolder = mediaFolder + "meshes\\scenes\\";


=======
        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a ver en el árbol de la derecha de la pantalla.
        /// </summary>
>>>>>>> Stashed changes
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


<<<<<<< Updated upstream
            pelota = new Pelota(this);
        
            mainCar = new Auto(mainCarMesh, this);
            autitus.Add(mainCar);


            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas
            Device d3dDevice = GuiController.Instance.D3dDevice;
            GuiController.Instance.RotCamera.CameraDistance = 100;
            initCarCamera();
        
=======

            box = new TgcBox();
            
           


            //Carpeta de archivos Media del alumno
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;

            


            ///////////////CONFIGURAR CAMARA ROTACIONAL//////////////////
            //Es la camara que viene por default, asi que no hace falta hacerlo siempre
            GuiController.Instance.RotCamera.Enable = true;
            //Configurar centro al que se mira y distancia desde la que se mira
            GuiController.Instance.RotCamera.setCamera(new Vector3(0, 0, 0), 100);


            /*
            ///////////////CONFIGURAR CAMARA PRIMERA PERSONA//////////////////
            //Camara en primera persona, tipo videojuego FPS
            //Solo puede haber una camara habilitada a la vez. Al habilitar la camara FPS se deshabilita la camara rotacional
            //Por default la camara FPS viene desactivada
            GuiController.Instance.FpsCamera.Enable = true;
            //Configurar posicion y hacia donde se mira
            GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 0, -20), new Vector3(0, 0, 0));
            */

>>>>>>> Stashed changes
        }


        
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            
            //Device de DirectX para renderizar
            Device d3dDevice = GuiController.Instance.D3dDevice;
<<<<<<< Updated upstream

            
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

=======
            box.setTexture(TgcTexture.createTexture(GuiController.Instance.ExamplesMediaDir + "\\Texturas\\madera.jpg"));
            GuiController.Instance.RotCamera.CameraDistance = 50;
            box.render();
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(mainCar.meshAuto.Position, 40, 250);
            GuiController.Instance.RotCamera.CameraDistance = 50;
        }

        public override void close()
        {
            mainCar.meshAuto.dispose();
            pelota.ownSphere.dispose();
            scene.disposeAll();
=======
            box.dispose();
>>>>>>> Stashed changes
        }

    }
}
