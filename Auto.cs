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

namespace AlumnoEjemplos.MiGrupo
{
    public class Auto
    {
        #region Constantes

        const float CONSTANTEAVANZAR = 600f;
        const float CONSTANTEFRENAR = 800f;
        const float CONSTANTEMARCHAATRAS = 400f;
        const float ROZAMIENTOCOEF = 200f;
        const float DELTA_T = 0.000000001f;
        const float CONST_SALTO = 0.1f;
        const float ALTURA_MAXIMA = 100;
        const float GRAVEDAD = -9.81f;

        #endregion

        #region Atributos

        public float velocidadHorizontal;
        public float velocidadVertical;

        float velocidadMaxima = -1500f;
        float velocidadMinima = 2000f;
        public float rotacion;
        public float elapsedTime;
        List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> ruedas;
        public TgcMesh meshAuto;
        float direccion;
        public TgcObb obb;
        EjemploAlumno parent;
        bool saltando = false;

        public bool subiendo
        {
            get
            {
                return velocidadVertical > 0;
            }
        }
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
        public void Mover(float et)
        {
            this.elapsedTime = et;
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
                    velocidadVertical = 100;
                }
            }


            chequearColisiones();
            MoverMesh();
            Saltar();
        }

        public void Saltar()
        {
            if (!saltando)
            {
                return;
            }

            chequearPiso(elapsedTime);
            aplicarGravedad(elapsedTime);

            if (meshAuto.Position.Y + velocidadVertical * CONST_SALTO < 0)
            {
                meshAuto.move(new Vector3(0, meshAuto.Position.Y * -1, 0));
                obb.move(new Vector3(0, meshAuto.Position.Y * -1, 0));
                saltando = false;
            }
            else
            {
                meshAuto.move(new Vector3(0, velocidadVertical * CONST_SALTO, 0));
                obb.move(new Vector3(0, velocidadVertical * CONST_SALTO, 0));
            }

        }

        public void aplicarGravedad(float elapsedTime)
        {
            velocidadVertical += GRAVEDAD * 20f * elapsedTime;
        }

        private void chequearPiso(float elapsedTime)
        {
            //if (TgcCollisionUtils.testObbAABB(obb, parent.piso))
            //{
            //    if (!subiendo)
            //    {
            //        if (FastMath.Abs(velocidadVertical) < 0.2)
            //        {
            //            saltando = false;
            //        }
            //        velocidadVertical = -(velocidadVertical);
            //        velocidadVertical = velocidadVertical * 0.2f; //rozamiento con el piso
            //    }
            //}
        }


        public void choqueFuerteConParedOLateral()
        {
        }

        public void chequearColisiones()
        {
            int i = 0;
            float dt = DELTA_T * 2 * elapsedTime;

            Vector3 originalPos = meshAuto.Position;
            Vector3 originalObbPos = obb.Position;

            while (i < 5)
            {
                i++;
                dt = dt / 2;

                float velocidadHorInicial = velocidadHorizontal;

                Vector3 lastPos = meshAuto.Position;
                this.meshAuto.Rotation = new Vector3(0f, this.rotacion, 0f);
                meshAuto.moveOrientedY(-this.velocidadHorizontal * dt);
                Vector3 direccionMovimiento = Vector3.Normalize(lastPos - meshAuto.Position);


                obb.move(direccionMovimiento * velocidadHorizontal * dt);

                float gradoDeProyeccionAlLateral = Vector3.Dot(direccionMovimiento, new Vector3(0, 0, 1));
                float gradoDeProyeccionALaPared = Vector3.Dot(direccionMovimiento, new Vector3(1, 0, 0));


                foreach (TgcBoundingBox lateral in parent.laterales)
                {
                    if (TgcCollisionUtils.testObbAABB(obb, lateral))
                    {
                        if (FastMath.Abs(gradoDeProyeccionAlLateral) < 0.4)
                        {
                            velocidadHorizontal = -velocidadHorizontal * 0.5f;
                            choqueFuerteConParedOLateral();
                        }
                        else
                        {
                            if (gradoDeProyeccionAlLateral > 0) rotacion = 0;
                            else rotacion = FastMath.PI;
                            velocidadHorizontal = velocidadHorizontal * FastMath.Abs(gradoDeProyeccionAlLateral);
                        }
                    }
                }

                foreach (TgcBoundingBox pared in parent.paredes)
                {
                    if (TgcCollisionUtils.testObbAABB(obb, pared))
                    {
                        if (FastMath.Abs(gradoDeProyeccionALaPared) < 0.4)
                        {
                            velocidadHorizontal = -velocidadHorizontal * 0.5f;
                            choqueFuerteConParedOLateral();
                        }
                        else
                        {

                            if (gradoDeProyeccionALaPared > 0) rotacion = FastMath.PI_HALF;
                            else rotacion = 3 * FastMath.PI_HALF;
                            velocidadHorizontal = velocidadHorizontal * FastMath.Abs(gradoDeProyeccionALaPared);
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


                    parent.pelota.velocity = parent.pelota.velocity + (0.005f * (spherePosition - collisionPos) * this.velocidadHorizontal);
                    this.velocidadHorizontal = this.velocidadHorizontal * (i / 3);

                }

                else {
                    obb.move(-direccionMovimiento * velocidadHorInicial * dt);
                    meshAuto.Position = lastPos;
                    break;
                }

                obb.move(-direccionMovimiento * velocidadHorInicial * dt);
                meshAuto.Position = lastPos;
                
                continue;
            }
            
        }

        private void MoverMesh()
        {
            Vector3 lastPos = meshAuto.Position;
            this.meshAuto.Rotation = new Vector3(0f, this.rotacion, 0f);
            meshAuto.moveOrientedY(-this.velocidadHorizontal * elapsedTime);
            GuiController.Instance.UserVars.setValue("Velocidad", TgcParserUtils.printFloat(velocidadHorizontal));

            Vector3 newPos = meshAuto.Position;
            obb.setRotation(new Vector3(0f, this.rotacion, 0f));

            obb.move(newPos - lastPos);

        }


        public void Retroceder()
        {
            if (velocidadHorizontal > 0) Frenar();
            if (velocidadHorizontal < 0) MarchaAtras();
        }

        public void Rotar(float unaDireccion)
        {
            if (velocidadHorizontal == 0)
            {
                return;
            }

            direccion = unaDireccion;
            rotacion += (elapsedTime * direccion * (velocidadHorizontal / 1000)); //direccion puede ser 1 o -1, 1 es derecha y -1 izquierda
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
            velocidadHorizontal += (aumento - Rozamiento()) * elapsedTime;
            AjustarVelocidad();
        }

        public void AjustarVelocidad()
        {
            if (velocidadHorizontal > velocidadMinima) velocidadHorizontal = velocidadMinima;
            if (velocidadHorizontal < velocidadMaxima) velocidadHorizontal = velocidadMaxima;
        }

        public void EstablecerVelocidadMáximaEn(float velMaxima)
        {
            velocidadMinima = velMaxima;
        }

        public float Rozamiento()
        {
            return ROZAMIENTOCOEF * Math.Sign(velocidadHorizontal);
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
