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
        #region DECLARATIONS
        EjemploAlumno parent;
        public Vector3 velocity;
        public Vector3 pos;

        Matrix matWorld;
        

        const float DELTA_T = 0.01f;
        public float gravedad = -9.81f;
        public TgcSphere ownSphere;

        #endregion

        #region CONSTRUCTOR
        public Pelota(EjemploAlumno p)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            parent = p;
            velocity = new Vector3(50, 0, 0);
            ownSphere = new TgcSphere();
            pos = new Vector3(0, 0, 0);
            matWorld = Matrix.Identity;

            ownSphere.AutoTransformEnable = false;

            String ballTexture = GuiController.Instance.AlumnoEjemplosDir  + "Media\\MILANESA_3D\\textures\\football.png";

            scale(50);
            translate(-400, 200, 2300);
            ownSphere.BoundingSphere.setValues(new Vector3(-400, 200, 2300), 50);
            ownSphere.setColor(Color.Red);
            ownSphere.setTexture(TgcTexture.createTexture(d3dDevice, ballTexture));


            ownSphere.Transform = matWorld;
            


        }
        #endregion

        #region BASICMOVEMENT


        public void translate(float  x, float y, float z)
        {
            translate(new Vector3(x, y, z));
        }
        public void translate(Vector3 vec)
        {
            matWorld = matWorld * Matrix.Translation(vec);
            pos = pos + vec;
            ownSphere.BoundingSphere.moveCenter(vec);
        }

        public void rotate(Vector3 axisRotation, float angle)
        {
            Matrix originalMatWorld = matWorld;

            Matrix gotoObjectSpace = Matrix.Invert(matWorld);
            axisRotation.TransformCoordinate( gotoObjectSpace);
            

            matWorld = Matrix.Identity * Matrix.RotationAxis(axisRotation, angle);
            matWorld = matWorld * originalMatWorld;
        }

        
        public void scale(float k)
        {
            scale(new Vector3(k, k, k));
        }

        public void scale(float x, float y, float z)
        {
            scale(new Vector3(x, y, z));
        }

        public void scale(Vector3 vec)
        {
            matWorld = matWorld * Matrix.Scaling(vec);
            

        }

        public void applyMovement()
        {
            ownSphere.Transform = matWorld;
            
            ownSphere.updateValues();
        }

        #endregion



        public void girar(float elapsedTime)
        {
            Vector3 velocidadHorizontal = velocity;
            velocidadHorizontal = new Vector3(velocidadHorizontal.X, 0, velocidadHorizontal.Z);
            Vector3 direccionHorizontal = Vector3.Normalize(velocidadHorizontal);
            
            rotate(Vector3.Cross(new Vector3(0, 1, 0), direccionHorizontal)+pos, elapsedTime * velocidadHorizontal.Length() * 0.005f);

        }


        public void aplicarGravedad(float elapsedTime)
        {
            velocity = velocity + new Vector3(0,  gravedad * 20f * elapsedTime, 0);
        }

        public void chequearColisiones(float elapsedTime)
        {


            Vector3 oldpos = pos;

            Vector3 transVec = velocity * DELTA_T;

            translate(transVec);
        

            foreach (Auto a in parent.autitus)
            {
                if (TgcCollisionUtils.testSphereOBB(ownSphere.BoundingSphere, a.obb))

                {
                    Vector3 collisionPos = new Vector3();
                    //translate(-transVec);
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
                translate(-transVec);
            }
             

            foreach (TgcBoundingBox pared in parent.limitesArcos)
            {
                
                if (TgcCollisionUtils.testSphereAABB(ownSphere.BoundingSphere, pared))
                {
                    velocity.Z = -(velocity.Z);
                    velocity = velocity * 0.9f;
                    translate(-transVec);
                }
            }

            foreach (TgcBoundingBox lateral in parent.laterales)
            {
                if (TgcCollisionUtils.testSphereAABB(ownSphere.BoundingSphere, lateral))
                {
                    velocity.X = -(velocity.X);
                    velocity = velocity * 0.9f;
                    translate(-transVec);
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
            //GuiController.Instance.UserVars.setValue("Pos Pelota", TgcParserUtils.printVector3(pos));
        }

        public void mover(float elapsedTime)
        {
            gravedad = (float)GuiController.Instance.Modifiers["Gravedad"];
            chequearColisiones(elapsedTime);
            aplicarGravedad(elapsedTime);
            girar(elapsedTime);
            applyMovement();
        }

        public void render()
        {            
            ownSphere.render();
        }


    }
}
