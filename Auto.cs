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
        const float CONST_SALTO = 7f;
        //Luego de pasar a matrices esta constante simplifica todo
        const float CONSTANTE_LOCA = 0.01f;
        

        float gravedad = -9.81f;
        float handling = 5f;

        #endregion

        #region Atributos

        public float velocidadHorizontal;
        public float velocidadVertical;
        public Vector3 pos;
        public float rotacionY=0;
        Matrix matWorld;

        protected float velocidadMaximaReversa = -7500f;
        protected float velocidadMaxima = 1000f;

        public float rotacion;
        public float elapsedTime;
        protected List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> ruedas;
        public TgcMesh meshAuto;
        public TgcObb obb;
        protected EjemploAlumno parent;
        protected bool saltando = false;
        public Vector3 direccion;
        public Vector3 ejeRotacionSalto;
        protected float alturaObb;
        protected bool colisionando = false;

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
            alturaObb = obb.Position.Y;
            matWorld = meshAuto.Transform;
            iniciarDirMovimiento();
            meshAuto.AutoTransformEnable = false;
        }

        protected void iniciarDirMovimiento()
        {
            var pos1 = meshAuto.Position;
            meshAuto.moveOrientedY(0.1f);
            var pos2 = meshAuto.Position;
            direccion = Vector3.Normalize(pos1 - pos2);
        }

        #endregion

        #region BASICMOVEMENT

        public void setPosition(float x, float y, float z)
        {
            var vec = new Vector3(x, y, z);

            matWorld = matWorld * Matrix.Translation(vec);
            pos = pos + vec;
            obb.Center = pos + new Vector3(0, alturaObb, 0);
        }


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
            pos = pos + vec;
            obb.Center = pos + new Vector3(0, alturaObb, 0);
        }

        public void rotate(Vector3 axisRotation, float angle)
        {
            Matrix originalMatWorld = matWorld;
            rotacionY += angle;
            Matrix gotoObjectSpace = Matrix.Invert(matWorld);
            //axisRotation.TransformCoordinate(gotoObjectSpace);

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

            chequearColisiones();
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
                else
                {
                    Acelerar(0);
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
                    ejeRotacionSalto = Vector3.Cross(new Vector3(0, 1, 0), direccion);
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

            if (bajando && chocaPiso())
            {
                return;
            }

            translate(0, velocidadVertical * elapsedTime * CONST_SALTO, 0);
            aplicarGravedad(elapsedTime);
            
            //rotacionSalto();

        }

        private void rotacionSalto()
        {
            float k = 1;
            //if (FastMath.Abs(velocidadVertical) > 50)
            //{
                k = velocidadVertical*elapsedTime;
            //}
            //else
            //{
            //    k = -velocidadVertical * 0.01f;
            //}
                rotate(ejeRotacionSalto, elapsedTime * CONST_SALTO * k);
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
                GuiController.Instance.UserVars.setValue("Pos Auto 1", TgcParserUtils.printVector3(pos));
                GuiController.Instance.UserVars.setValue("Pos Obb 1", TgcParserUtils.printVector3(obb.Position));
            }
            else
            {
                GuiController.Instance.UserVars.setValue("Pos Auto 2", TgcParserUtils.printVector3(pos));
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

            var rot = (elapsedTime * unaDireccion * elapsedTime*(handling * velocidadHorizontal / 10)); //direccion puede ser 1 o -1, 1 es derecha y -1 izquierda
            rotate(new Vector3(0, 1, 0), rot);
            obb.setRotation(new Vector3(0f, rotacionY, 0f));
            direccion.TransformCoordinate(Matrix.RotationAxis(new Vector3(0, 1, 0), rot));
            direccion.Normalize();
        }

        protected void Acelerar(float aumento)
        {
            velocidadHorizontal += (aumento - Rozamiento()) * elapsedTime;
            AjustarVelocidad();
            translate(direccion * velocidadHorizontal*CONSTANTE_LOCA);
        }

        public void AjustarVelocidad()
        {
            if (velocidadHorizontal > velocidadMaxima) velocidadHorizontal = velocidadMaxima;
            if (velocidadHorizontal < velocidadMaximaReversa) velocidadHorizontal = velocidadMaximaReversa;
        }

        public void EstablecerVelocidadMáximaEn(float velMaxima)
        {
            velocidadMaxima = velMaxima;
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
            Acelerar(k * CONSTANTEFRENAR * 2.5f);
        }

        public void MarchaAtras()
        {
            Acelerar(-CONSTANTEMARCHAATRAS);
        }


        public void render()
        {
            aceleracion = (float)GuiController.Instance.Modifiers["Aceleracion"];
            gravedad = (float)GuiController.Instance.Modifiers["Gravedad"];
            handling = (float)GuiController.Instance.Modifiers["VelocidadRotacion"];

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

            Vector3 originalPos = pos;
            Vector3 originalObbPos = obb.Position;

            while (i < 5)
            {
                i++;
                //dt = dt / 2;
                Vector3 lastPos = pos;
                float velocidadHorInicial = velocidadHorizontal;
                translate(-this.velocidadHorizontal * direccion * dt);

                float gradoDeProyeccionAlLateral = Vector3.Dot(direccion, new Vector3(0, 0, 1));
                float gradoDeProyeccionALaPared = Vector3.Dot(direccion, new Vector3(1, 0, 0));


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
                        var anguloChoque = Vector3.Dot(direccion, auto.direccion);

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

                    //translate(lastPos);
                    parent.pelota.velocity = parent.pelota.velocity + velocidadATransmitir;
                    this.velocidadHorizontal = this.velocidadHorizontal * (i / 3);

                }
                else
                {
                    //obb.move(-direccion * velocidadHorInicial * dt);
                    //translate(lastPos);
                    break;
                }

                //translate(lastPos);

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
