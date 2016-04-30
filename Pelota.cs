using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer.Example;

namespace AlumnoEjemplos.MiGrupo
{
    class Pelota
    {
        EjemploAlumno parent;
        Vector3 pos;
        Vector3 velocity;

        Matrix scale = Matrix.Identity;
        Matrix translate = Matrix. Identity;
        Matrix rotate = Matrix.Identity;
        Matrix move = Matrix.Identity;

        const float DELTA_T = 0.01f;
        const float GRAVEDAD = -9.81f;
        public TgcSphere ownSphere;
        List<TgcMesh> obstaculos = new List<TgcMesh>();


        public Pelota(EjemploAlumno p)
        {
            parent = p;
            velocity = new Vector3(50, 0, 0);
            ownSphere = new TgcSphere();



            ownSphere.Radius = 20;
            ownSphere.setColor(Color.Red);
            ownSphere.Position = new Vector3(0, 80, 0);

            //ownSphere.AutoTransformEnable = false;
        }

        public void aplicarGravedad(float elapsedTime)
        {
            velocity = velocity + new Vector3(0,  GRAVEDAD * 20f * elapsedTime, 0);
        }

        public void chequearColisiones(float elapsedTime)
        {
            Vector3 oldpos = ownSphere.Position;

            ownSphere.move(velocity * DELTA_T);

            if (TgcCollisionUtils.testSphereAABB(ownSphere.BoundingSphere, parent.piso))
            {
                velocity.Y = -(velocity.Y);
                velocity = velocity * 0.85f; //rozamiento con el piso
                ownSphere.Position = oldpos;
            }

            
            
        }

        public void updateValues()
        {
            
            ownSphere.updateValues();
            
        }

        public void mover(float elapsedTime)
        {
            chequearColisiones(elapsedTime);
            aplicarGravedad(elapsedTime);
        }

        public void render()
        {            
            ownSphere.render();
        }


    }
}
