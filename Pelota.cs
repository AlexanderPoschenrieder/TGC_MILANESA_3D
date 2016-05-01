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
    public class Pelota
    {
        EjemploAlumno parent;
        Vector3 pos;
       public  Vector3 velocity;

        Matrix scale = Matrix.Identity;
        Matrix translate = Matrix. Identity;
        Matrix rotate = Matrix.Identity;
        Matrix move = Matrix.Identity;

        SphereCollisionManager collisionManager = new SphereCollisionManager();

        const float DELTA_T = 0.01f;
        const float GRAVEDAD = -9.81f;
        public TgcSphere ownSphere;


        public Pelota(EjemploAlumno p)
        {
            parent = p;
            velocity = new Vector3(50, 0, 0);
            ownSphere = new TgcSphere();
            
            ownSphere.Radius = 50;
            ownSphere.setColor(Color.Red);
            ownSphere.Position = new Vector3(0, 200, -100);

            
        }

        public void aplicarGravedad(float elapsedTime)
        {
            velocity = velocity + new Vector3(0,  GRAVEDAD * 20f * elapsedTime, 0);
        }

        public void chequearColisiones(float elapsedTime)
        {
            Vector3 oldpos = ownSphere.Position;

            ownSphere.move(velocity * DELTA_T);

            foreach (Auto a in parent.autitus)
            {
                if (TgcCollisionUtils.testSphereOBB(ownSphere.BoundingSphere, a.obb))
                {
                    Vector3 collisionPos = new Vector3();
                    ownSphere.Position = oldpos;
                    Vector3 autoPosition = new Vector3();
                    autoPosition = a.obb.Position;
                    
                    TgcRay ray = new TgcRay(oldpos, autoPosition - oldpos);
                    TgcCollisionUtils.intersectRayObb(ray, a.obb, out collisionPos);
                    
                    velocity = velocity + (0.02f*(oldpos - collisionPos));
                    velocity = velocity + new Vector3(0, 3, 0);
                }

            }

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
