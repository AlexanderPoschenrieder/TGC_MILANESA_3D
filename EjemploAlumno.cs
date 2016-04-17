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
        TgcSphere sphere;
        TgcScene scene;
        SphereCollisionManager collisionManager;
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
            scene = loader.loadSceneFromFile(sceneFolder + "PuebloJapones\\PuebloJapones-TgcScene.xml");


            objetosColisionables.Clear();
            foreach (TgcMesh mesh in scene.Meshes)
            {
                objetosColisionables.Add(mesh.BoundingBox);
            }

            collisionManager = new SphereCollisionManager();
            collisionManager.GravityEnabled = true;
            collisionManager.GravityForce = new Vector3(0, -1, 0);


            sphere = new TgcSphere();
            sphere.Radius = 5;
            sphere.Position = new Vector3(0, 10, 0);
            sphere.setColor(Color.Blue);


            Vector3 movementVector = Vector3.Empty;




            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas
            Device d3dDevice = GuiController.Instance.D3dDevice;


            GuiController.Instance.RotCamera.CameraDistance = 100;
            
        
        }

        
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {

            float moveForward = 0;
            float jump = 0;


            //Device de DirectX para renderizar
            Device d3dDevice = GuiController.Instance.D3dDevice;

            
            //conviene deshabilitar ambas camaras para que no haya interferencia

            //Capturar Input teclado 
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.F))
            {
                moveForward = 100*elapsedTime;
                //Tecla F apretada
            }

            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Space))
            {
                jump = 100 *elapsedTime;
            }

            //Capturar Input Mouse
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Boton izq apretado
            }
            

            Vector3 movementVector = new Vector3(
                    FastMath.Sin(sphere.Rotation.Y) * moveForward,
                    jump,
                    FastMath.Cos(sphere.Rotation.Y) * moveForward
                    );

            Vector3 realMovement = collisionManager.moveCharacter(sphere.BoundingSphere, movementVector, objetosColisionables);
            sphere.move(realMovement);


            sphere.updateValues();
            scene.renderAll();
            sphere.render();
            GuiController.Instance.RotCamera.CameraCenter = sphere.Position;
            
        }
        
        public override void close()
        {
            sphere.dispose();
        }

    }
}
