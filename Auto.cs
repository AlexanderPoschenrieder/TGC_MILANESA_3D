﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MiGrupo
{
    public class Auto
    {
        #region Constantes

        const float CONSTANTEAVANZAR = 600f;
        const float CONSTANTEFRENAR = 800f;
        const float CONSTANTEMARCHAATRAS = 400f;
        const float ROZAMIENTOCOEF = 200f;
        const float DELTA_T = 0.1f;
        const float ALTURA_MAXIMA = 100;

        #endregion

        #region Atributos

        public float velocidad;
        float velocidadMaxima = -1500f;
        float velocidadMinima = 2000f;
        public float rotacion;
        public float elapsedTime;
        List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> ruedas;
        public TgcMesh meshAuto;
        float direccion;
        public TgcObb obb;
        EjemploAlumno parent;
        bool subiendo=false;
        bool saltando = false;

        #endregion

        #region Constructor
        
        public Auto(TgcMesh mesh, EjemploAlumno p)
        {
            parent = p;
            meshAuto = mesh;
            obb = TgcObb.computeFromAABB(mesh.BoundingBox);
        }

        public Auto(float rot, TgcMesh auto, List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> unasRuedas = null)
        {
            this.rotacion = rot;
            this.ruedas = unasRuedas;
            meshAuto = auto;
            obb = TgcObb.computeFromAABB(auto.BoundingBox);
        }

        #endregion

        #region Métodos
        public void Mover()
        {
            ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                Rotar(-1);
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                Rotar(1);
            }

            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                if (!saltando)
                {
                    Acelerar(CONSTANTEAVANZAR);    
                }
                
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                if (!saltando)
                {
                    Retroceder();
                }
                
            }
            else
            {
                Acelerar(0);    
            }

            if (input.keyDown(Key.Space))
            {
                if (!saltando)
                {
                    saltando = true;
                    subiendo = true;
                }
            }

            
            chequearColisiones();
            MoverMesh();
            Saltar();
        }

        private void Saltar()
        {
            if(!saltando)
            {
                return;
            }
            
            float posX =meshAuto.Position.X;
            float posY =meshAuto.Position.Y;
            float posZ =meshAuto.Position.Z;

            float movY=0;
            if (subiendo)
            {
                if (posY < ALTURA_MAXIMA)
                {
                    movY = elapsedTime*100;
                }
                else
                {
                    subiendo = false;
                }
            }
            else if (!subiendo)
            {
                if (posY > 0)
                {
                    movY = -1* elapsedTime * 100;
                }
                else
                {
                    saltando=false;
                }
            }

            meshAuto.move(new Vector3(0,movY,0));
            obb.move(new Vector3(0,movY,0));


        }

        public void choqueFuerteConParedOLateral()
        {

        }

        public void chequearColisiones()
        {
            int i = 0;
            float dt = DELTA_T * 2;

            while (i < 5)
            {
                i++;
                dt = dt / 2;

                Vector3 lastPos = meshAuto.Position;
                this.meshAuto.Rotation = new Vector3(0f, this.rotacion, 0f);
                meshAuto.moveOrientedY(-this.velocidad * dt);
                Vector3 direccionMovimiento = Vector3.Normalize(lastPos - meshAuto.Position);
                
                
                obb.move(direccionMovimiento * velocidad * dt);

                float gradoDeProyeccionAlLateral = Vector3.Dot(direccionMovimiento, new Vector3(0, 0, 1));
                float gradoDeProyeccionALaPared = Vector3.Dot(direccionMovimiento, new Vector3(1, 0, 0));


                foreach (TgcBoundingBox lateral in parent.laterales)
                {
                    if(TgcCollisionUtils.testObbAABB(obb, lateral))
                    {
                        if (FastMath.Abs(gradoDeProyeccionAlLateral) < 0.4)
                        {
                            velocidad = -velocidad * 0.5f;
                            choqueFuerteConParedOLateral();
                        }
                        else
                        {
                            if (gradoDeProyeccionAlLateral > 0) rotacion = 0;
                            else rotacion = FastMath.PI;
                            velocidad = velocidad * FastMath.Abs(gradoDeProyeccionAlLateral);
                        }
                    }
                }

                foreach (TgcBoundingBox pared in parent.paredes)
                {
                    if(TgcCollisionUtils.testObbAABB(obb, pared))
                    {
                        if (FastMath.Abs(gradoDeProyeccionALaPared) < 0.4)
                        {
                            velocidad = -velocidad * 0.5f;
                            choqueFuerteConParedOLateral();
                        }
                        else
                        {

                            if (gradoDeProyeccionALaPared > 0) rotacion = FastMath.PI_HALF;
                            else rotacion = 3 * FastMath.PI_HALF;
                            velocidad = velocidad * FastMath.Abs(gradoDeProyeccionALaPared);
                        }
                    }
                }

                if (TgcCollisionUtils.testSphereOBB(parent.pelota.ownSphere.BoundingSphere, obb))
                {

                    Vector3 collisionPos = new Vector3();
                    Vector3 spherePosition = new Vector3();
                    spherePosition = parent.pelota.ownSphere.Position;

                    TgcRay ray = new TgcRay(lastPos, spherePosition - lastPos);
                    TgcCollisionUtils.intersectRayObb(ray, obb, out collisionPos);
                    

                    parent.pelota.velocity = parent.pelota.velocity + (0.005f*(spherePosition - collisionPos) * this.velocidad);
                    this.velocidad = this.velocidad * (i/3);
                    
                }

                obb.move(-direccionMovimiento * velocidad * dt);
                meshAuto.Position = lastPos;


                continue;
            }
        }

        private void MoverMesh()
        {
            Vector3 lastPos = meshAuto.Position;
            this.meshAuto.Rotation = new Vector3(0f, this.rotacion, 0f);
            meshAuto.moveOrientedY(-this.velocidad * elapsedTime);
            GuiController.Instance.UserVars.setValue("Velocidad", TgcParserUtils.printFloat(velocidad));

            Vector3 newPos = meshAuto.Position;
            obb.setRotation(new Vector3(0f, this.rotacion, 0f));
            
            obb.move(newPos - lastPos);
           
        }


        public void Retroceder()
        {
            if (velocidad > 0) Frenar();
            if (velocidad < 0) MarchaAtras();
        }

        public void Rotar(float unaDireccion)
        {
            if (velocidad == 0)
            {
                return;
            }

            direccion = unaDireccion;
            rotacion += (elapsedTime * direccion * (velocidad / 1000)); //direccion puede ser 1 o -1, 1 es derecha y -1 izquierda
            AjustarRotacion();
            
        }

        public float RotarRueda(int i)
        {
            if (i == 0 || i == 2)
            {
                return 0.5f * direccion;
            }
            return 0;
        }
        //Metodos Internos
        //De Velocidad

        private void Acelerar(float aumento)
        {
            velocidad += (aumento - Rozamiento()) * elapsedTime;
            AjustarVelocidad();
        }

        public void AjustarVelocidad()
        {
            if (velocidad > velocidadMinima) velocidad = velocidadMinima;
            if (velocidad < velocidadMaxima) velocidad = velocidadMaxima;
        }

        public void EstablecerVelocidadMáximaEn(float velMaxima)
        {
            velocidadMinima = velMaxima;
        }

        public float Rozamiento()
        {
            return ROZAMIENTOCOEF * Math.Sign(velocidad);
        }

        private void Frenar()
        {
            Acelerar(-CONSTANTEFRENAR);
        }

        public void FrenoDeMano()
        {
            Acelerar(-CONSTANTEFRENAR * 2.5f);
        }

        public void MarchaAtras()
        {
            Acelerar(-CONSTANTEMARCHAATRAS);
        }


        //DeRotacion
        private void AjustarRotacion()
        {
            while (rotacion > FastMath.TWO_PI * 100)
            {
                rotacion -= FastMath.TWO_PI * 100;
            }
        }

        //para que el auto se quede quieto cuando perdio el jugador
        public void Estatico()
        {
            velocidadMinima = 0f;
            velocidadMaxima = 0f;
            AjustarVelocidad();
        }

        public float GetRotacion()
        {
            return this.rotacion;
        }

        #endregion
    }
}
