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
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            parent = p;
            velocity = new Vector3(50, 0, 0);
            ownSphere = new TgcSphere();

            String ballTexture = GuiController.Instance.AlumnoEjemplosDir  + "MiGrupo\\media\\textures\\football.png";

            ownSphere.Radius = 50;
            ownSphere.setColor(Color.Red);
            ownSphere.Position = new Vector3(0, 200, -100);
            ownSphere.setTexture(TgcTexture.createTexture(d3dDevice, ballTexture));
            ownSphere.Rotation = new Vector3(0, 0, 0);

            
        }

        public void girar(float elapsedTime)
        {
            Vector3 sentidoMovimiento = velocity;
            sentidoMovimiento = new Vector3(sentidoMovimiento.X, 0, sentidoMovimiento.Z);

            
            ownSphere.Rotation = ownSphere.Rotation + Vector3.Cross(sentidoMovimiento, new Vector3(0, -1, 0)) * elapsedTime * 0.01f;

       
          
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

                    velocity = velocity + (0.1f * (oldpos - collisionPos));
                }

            }

            if (TgcCollisionUtils.testSphereAABB(ownSphere.BoundingSphere, parent.piso.BoundingBox))
            {
                velocity.Y = -(velocity.Y);
                velocity = velocity * 0.75f; //rozamiento con el piso
                ownSphere.Position = oldpos;
            }

            foreach (TgcBoundingBox pared in parent.limitesArcos)
            {
                if (TgcCollisionUtils.testSphereAABB(ownSphere.BoundingSphere, pared))
                {
                    velocity.Z = -(velocity.Z);
                    velocity = velocity * 0.9f;
                    ownSphere.Position = oldpos;
                }
            }

            foreach (TgcBoundingBox lateral in parent.laterales)
            {
                if (TgcCollisionUtils.testSphereAABB(ownSphere.BoundingSphere, lateral))
                {
                    velocity.X = -(velocity.X);
                    velocity = velocity * 0.9f;
                    ownSphere.Position = oldpos;
                }
            }

            if (TgcCollisionUtils.testSphereAABB(ownSphere.BoundingSphere, parent.arcoNegativo))
            {
                parent.golLocal();
            }

            if (TgcCollisionUtils.testSphereAABB(ownSphere.BoundingSphere, parent.arcoPositivo))
            {
                parent.golVisitante();
                
            }

        }

        public void updateValues()
        {
            
            ownSphere.updateValues();
            GuiController.Instance.UserVars.setValue("Pos Pelota", TgcParserUtils.printVector3(ownSphere.Position));
        }

        public void mover(float elapsedTime)
        {
            chequearColisiones(elapsedTime);
            aplicarGravedad(elapsedTime);
            girar(elapsedTime);
        }

        public void render()
        {            
            ownSphere.render();
        }


    }
}
