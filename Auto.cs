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

        protected float aceleracion = 600f;
        const float CONSTANTEFRENAR = 800f;
        const float CONSTANTEMARCHAATRAS = 400f;
        const float ROZAMIENTOCOEF = 200f;
        const float DELTA_T = 0.000000001f;
        const float CONST_SALTO = 0.1f;
        const float ALTURA_MAXIMA = 100;

        float gravedad = -9.81f;
        float handling = 1f;

        #endregion

        #region Atributos

        public float velocidadHorizontal;
        public float velocidadVertical;
        public Vector3 pos;
        Matrix matWorld;

        protected float velocidadMaxima = -7500f;
        protected float velocidadMinima = 1000f;

        public float rotacion;
        public float elapsedTime;
        protected List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> ruedas;
        public TgcMesh meshAuto;
        protected float direccion;
        public TgcObb obb;
        protected EjemploAlumno parent;
        protected bool saltando = false;
        public Vector3 direccionMovimiento = new Vector3(0,0,-1);

        #endregion

        #region Properties


        public bool subiendo
        {
            get
            {
                return velocidadVertical > 0;
            }
        }

        public bool bajando
        {
            get
            {
                return velocidadVertical < 0;
            }
        }

        #endregion

        #region Constructor

        public Auto(TgcMesh mesh, EjemploAlumno p)
        {
            parent = p;
            meshAuto = mesh;
            obb = TgcObb.computeFromAABB(mesh.BoundingBox);
            matWorld = meshAuto.Transform;            
            meshAuto.AutoTransformEnable = false;
        }

        //public Auto(float rot, TgcMesh auto, List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> unasRuedas = null)
        //{
        //    this.rotacion = rot;
        //    this.ruedas = unasRuedas;
        //    meshAuto = auto;
        //    obb = TgcObb.computeFromAABB(auto.BoundingBox);
        //}

        #endregion

        #region BASICMOVEMENT

        public void translate(float x, float y, float z)
        {
            translate(new Vector3(x, y, z));
        }

        public void translate(Vector3 vec)
        {
            if (vec == new Vector3(0, 0, 0))
            {
                return;
            }
            matWorld = matWorld * Matrix.Translation(vec);
            direccionMovimiento = Vector3.Normalize(vec);
            pos = pos + vec;
            obb.Center = pos;
        }

        public void rotate(Vector3 axisRotation, float angle)
        {
            Matrix originalMatWorld = matWorld;

            Matrix gotoObjectSpace = Matrix.Invert(matWorld);
            axisRotation.TransformCoordinate(gotoObjectSpace);

            //GuiController.Instance.UserVars.setValue("Pos Pelota", TgcParserUtils.printVector3(axisRotation));

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
        
        #endregion

        #region Métodos
        public void Mover(float et)
        {
            this.elapsedTime = et;
            CalcularMovimiento();

            //chequearColisiones();
            Saltar();
            MoverMesh();
        }

        protected virtual void CalcularMovimiento()
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyDown(Key.A))
            {
                Rotar(-1);
            }
            else if (input.keyDown(Key.D))
            {
                Rotar(1);
            }

            if (input.keyDown(Key.W))
            {
                if (!saltando)
                {
                    Acelerar(aceleracion);
                }

            }
            else if (input.keyDown(Key.S))
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

            if (!saltando && input.keyDown(Key.LeftControl))
            {
                if (velocidadHorizontal != 0)
                {
                    FrenoDeMano();
                }
                
            }

            if (input.keyDown(Key.Space))
            {
                if (!saltando)
                {
                    saltando = true;
                    velocidadVertical = 100;
                }
            }
        }

        public void Saltar()
        {
            if (!saltando)
            {
                return;
            }
            int i;
            for (i = 0; i <= 5; i++)
            {
                if (bajando && chocaPiso())
                {
                    break;
                }

                translate(0, velocidadVertical * CONST_SALTO, 0);
                aplicarGravedad(elapsedTime);
                break;
            }
        }

        public void aplicarGravedad(float elapsedTime)
        {
            velocidadVertical += gravedad * 20f * elapsedTime;
        }

        private void MoverMesh()
        {
            meshAuto.Transform = matWorld;
            if (this.GetType().Name == "Auto")
            {
                GuiController.Instance.UserVars.setValue("Pos Auto 1", TgcParserUtils.printVector3(meshAuto.Position));
                GuiController.Instance.UserVars.setValue("Pos Obb 1", TgcParserUtils.printVector3(obb.Position));
            }
            else
            {
                GuiController.Instance.UserVars.setValue("Pos Auto 2", TgcParserUtils.printVector3(meshAuto.Position));
                GuiController.Instance.UserVars.setValue("Pos Obb 2", TgcParserUtils.printVector3(obb.Position));
            }

        }

        public void Retroceder()
        {
            if (velocidadHorizontal > 0) Frenar();
            if (velocidadHorizontal <= 0) MarchaAtras();
        }

        public void Rotar(float unaDireccion)
        {
            if (velocidadHorizontal == 0)
            {
                return;
            }

            var rot = (elapsedTime * unaDireccion * (handling * velocidadHorizontal / 1000)); //direccion puede ser 1 o -1, 1 es derecha y -1 izquierda
            rotate(new Vector3(0, 1, 0), rot);
        }
        
        protected void Acelerar(float aumento)
        {
            velocidadHorizontal += (aumento - Rozamiento()) * elapsedTime;
            AjustarVelocidad();
            translate(direccionMovimiento*velocidadHorizontal);
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
            var k = velocidadHorizontal > 0 ? -1 : 1;
            Acelerar(k*CONSTANTEFRENAR * 2.5f);
        }

        public void MarchaAtras()
        {
            Acelerar(-CONSTANTEMARCHAATRAS);
        }

        
        public void render()
        {
            aceleracion = (float) GuiController.Instance.Modifiers["Aceleracion"];
            gravedad = (float)GuiController.Instance.Modifiers["Gravedad"];
            handling =  (float)GuiController.Instance.Modifiers["VelocidadRotacion"];

            meshAuto.render();
            obb.render();
        }


        #endregion

        #region Colisiones

        private bool chocaPiso()
        {
            if (TgcCollisionUtils.testObbAABB(obb, parent.piso.BoundingBox))
            {

                translate(0, meshAuto.Position.Y * -1, 0);

                saltando = false;
                return true;
            }

            return false;

        }

        public void choqueFuerteConParedOLateral()
        {
        }

        public void chequearColisiones()
        {
            int i = 0;
            float dt = 0.001f * elapsedTime;

            Vector3 originalPos = meshAuto.Position;
            Vector3 originalObbPos = obb.Position;

            while (i < 5)
            {
                i++;
                //dt = dt / 2;
                Vector3 lastPos = pos;
                float velocidadHorInicial = velocidadHorizontal;
                translate(-this.velocidadHorizontal*direccionMovimiento* dt);
                
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
                        velocidadHorizontal = velocidadHorizontal - 10 * Math.Sign(velocidadHorInicial);
                    }
                }

                foreach (TgcBoundingBox pared in parent.limitesArcos)
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
                        velocidadHorizontal = velocidadHorizontal - 10 * Math.Sign(velocidadHorInicial);
                    }
                }

                foreach (var auto in parent.autitus)
                {
                    if (auto.Equals(this))
                    {
                        continue;
                    }

                    if (TgcCollisionUtils.testObbObb(obb, auto.obb))
                    {
                        var anguloChoque = Vector3.Dot(direccionMovimiento, auto.direccionMovimiento);

                        //if (FastMath.Abs(anguloChoque) < 0.4)
                        //{
                        velocidadHorizontal = -velocidadHorizontal * 0.5f;
                        //}
                        //else
                        //{
                        //velocidadHorizontal = -velocidadHorizontal * 0.5f;
                        //}
                    }
                }

                if (TgcCollisionUtils.testSphereOBB(parent.pelota.ownSphere.BoundingSphere, obb))
                {

                    Vector3 collisionPos = new Vector3();
                    Vector3 spherePosition = new Vector3();
                    spherePosition = parent.pelota.pos;

                    TgcRay ray = new TgcRay(lastPos, spherePosition - lastPos);
                    TgcCollisionUtils.intersectRayObb(ray, obb, out collisionPos);

                    Vector3 velocidadATransmitir = 0.02f * (spherePosition - collisionPos) * this.velocidadHorizontal;
                    velocidadATransmitir = new Vector3(velocidadATransmitir.X, velocidadATransmitir.Y * 0.3f, velocidadATransmitir.Z);

                    meshAuto.Position = lastPos;
                    parent.pelota.velocity = parent.pelota.velocity + velocidadATransmitir;
                    this.velocidadHorizontal = this.velocidadHorizontal * (i / 3);

                }
                else
                {
                    obb.move(-direccionMovimiento * velocidadHorInicial * dt);
                    meshAuto.Position = lastPos;
                    break;
                }

                obb.move(-direccionMovimiento * velocidadHorInicial * dt);
                meshAuto.Position = lastPos;

                continue;
            }

        }



        #endregion

        #region Auxiliar
        
        public bool isLeft(Vector3 a, Vector3 b, Vector3 c)
        {
            return ((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) > 0;
        }

        #endregion

    }
}
