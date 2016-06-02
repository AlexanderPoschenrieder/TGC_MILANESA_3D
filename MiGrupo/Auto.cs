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

        public float rotacionAcumuladaEnElSalto = 0;

        public float velocidadHorizontal;
        public float velocidadVertical;
        public Vector3 pos;
        public float rotacion = 0;
        Matrix matWorld;

        protected float velocidadMaximaReversa = -7500f;
        protected float velocidadMaxima = 1000f;

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
        public Vector3 desvio = new Vector3(0, 0, 0);
        private bool reposicionar=false;

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
            var axis = Vector3.Cross(direccion,new Vector3(0,1,0));
            Matrix originalMatWorld = matWorld;
            rotacion += angle;
            Matrix gotoObjectSpace = Matrix.Invert(matWorld);
            //axisRotation.TransformCoordinate(gotoObjectSpace);
            var rotMat = Matrix.RotationAxis(axisRotation, angle);
            var rotObb = Matrix.RotationAxis(axis, angle);
            matWorld = Matrix.Identity * rotMat;
            matWorld = matWorld * originalMatWorld;
            //Rotación OBB defectuosa
            obb.Orientation = Vector3.TransformCoordinate(obb.Orientation, rotObb);
            
            
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
            chequearColisiones();
            CalcularMovimiento();
            CalcularExtraInvoluntario();

            Saltar();
            MoverMesh();
        }

        protected void CalcularExtraInvoluntario()
        {
            translate(desvio * elapsedTime);
            desvio = desvio * 0.92f;
        }


        protected virtual void CalcularMovimiento()
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyDown(Key.A))
            {
                if (!saltando) Rotar(-1);
            }
            else if (input.keyDown(Key.D))
            {
                if (!saltando) Rotar(1);
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
                else
                {
                    Acelerar(0);
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
                    rotacionAcumuladaEnElSalto = 0;
                    
                    velocidadVertical = 100;
                }
            }
        }

        public void desviar(Vector3 d)
        {
            desvio += d;
        }

        public void Saltar()
        {
            if (reposicionar)
            {
                translate(0, pos.Y * -1, 0);

                reposicionar = false;
            }

            if (!saltando)
            {
                return;
            }

            if (bajando && chocaPiso())
            {
                rotate(new Vector3(1, 0, 0), -rotacionAcumuladaEnElSalto);
                return;
            }

            if (!colisionando) translate(0, velocidadVertical * elapsedTime * CONST_SALTO, 0);

            if (!colisionando) aplicarGravedad(elapsedTime);
            
            rotacionSalto();

        }

        private void rotacionSalto()
        {
            float k = 1;

            k = velocidadVertical * elapsedTime*0.004f;
            rotate(new Vector3(1, 0, 0), CONST_SALTO * k);
            rotacionAcumuladaEnElSalto += CONST_SALTO * k;
            
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

            var rot = (elapsedTime * unaDireccion * elapsedTime*(handling * velocidadHorizontal / 50)); 
            rotate(new Vector3(0, 1, 0), rot);
            obb.setRotation(new Vector3(0f, rotacion, 0f));
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
                saltando = false;
                reposicionar = true;
                return true;
            }

            return false;

        }

        public void choqueFuerteConParedOLateral()
        {
        }

        public void chequearColisiones()
        {
            colisionando = false;

            int i = 0;
            float dt = 0.001f * elapsedTime;

            Vector3 originalPos = pos;
            Vector3 originalObbPos = obb.Position;
            float velocidadHorInicial = velocidadHorizontal;
            float velocidadTotalInicial = Vector3.Length(velocidadHorInicial * direccion + desvio);

            while (i < 5)
            {
                i++;
                //dt = dt / 2;
                Vector3 lastPos = pos;
                Vector3 transVec = (this.velocidadHorizontal * direccion * dt);
                transVec = transVec + this.desvio * dt;
                translate(transVec);

                //pro-tip: no leer los 4 ifs que siguen

                if(TgcCollisionUtils.testObbAABB(obb, parent.limiteLateralPositivo))
                {
                    float perpendicularidadChoque = Vector3.Dot(new Vector3(1, 0, 0), Vector3.Normalize((direccion * velocidadHorInicial) + desvio));
                    translate(-transVec);
                    desviar(new Vector3(1f * Math.Abs(velocidadTotalInicial * perpendicularidadChoque), 0, 0));
                    velocidadHorizontal = velocidadHorInicial * Math.Abs(1 - perpendicularidadChoque * perpendicularidadChoque);
                }

                if (TgcCollisionUtils.testObbAABB(obb, parent.limiteLateralNegativo))
                {
                    float perpendicularidadChoque = Vector3.Dot(new Vector3(1, 0, 0), Vector3.Normalize((direccion * velocidadHorInicial) + desvio));
                    translate(-transVec);
                    desviar(new Vector3(-1f * Math.Abs(velocidadTotalInicial * perpendicularidadChoque), 0, 0));
                    velocidadHorizontal = velocidadHorInicial * Math.Abs(1 - perpendicularidadChoque * perpendicularidadChoque);
                }


                if (TgcCollisionUtils.testObbAABB(obb, parent.limiteArcoPositivo1) || TgcCollisionUtils.testObbAABB(obb, parent.limiteArcoPositivo2) || TgcCollisionUtils.testObbAABB(obb, parent.arcoPositivo)) 
                {
                    float perpendicularidadChoque = Vector3.Dot(new Vector3(0, 0, 1), Vector3.Normalize((direccion * velocidadHorInicial) + desvio));
                    translate(-transVec);
                    desviar(new Vector3(0, 0, -1f * Math.Abs(velocidadTotalInicial * perpendicularidadChoque)));
                    velocidadHorizontal = velocidadHorInicial * Math.Abs(1 - perpendicularidadChoque * perpendicularidadChoque);
                }

                if (TgcCollisionUtils.testObbAABB(obb, parent.limiteArcoNegativo1) || TgcCollisionUtils.testObbAABB(obb, parent.limiteArcoNegativo2) || TgcCollisionUtils.testObbAABB(obb, parent.arcoNegativo))
                {
                    float perpendicularidadChoque = Vector3.Dot(new Vector3(0, 0, 1), Vector3.Normalize((direccion * velocidadHorInicial) + desvio));
                    translate(-transVec);
                    desviar(new Vector3(0, 0, 1f * Math.Abs(velocidadTotalInicial * perpendicularidadChoque)));
                    velocidadHorizontal = velocidadHorInicial * Math.Abs(1 - perpendicularidadChoque * perpendicularidadChoque);
                }


                foreach (var auto in parent.autitus)
                {
                    if (auto.Equals(this))
                    {
                        continue;
                    }

                    if (TgcCollisionUtils.testObbObb(obb, auto.obb))
                    {
                        Vector3 collisionPos;

                        TgcRay ray = new TgcRay(lastPos, auto.pos - lastPos);
                        TgcCollisionUtils.intersectRayObb(ray, auto.obb, out collisionPos);

                        Vector3 d = Vector3.Normalize(lastPos - collisionPos);
                        d = new Vector3(d.X, 0, d.Z); //proyectar en y=0 para que los autos no se levanten ni se hundan entre sí

                        desviar(d * 0.5f);
                        auto.desviar(-d * velocidadTotalInicial * 0.2f);
                        colisionando = true;
                        translate(-transVec);

                        //¿puede mejorarse? sí
                        //¿lo voy a mejorar? no creo
                        

                    }
                }

                if (TgcCollisionUtils.testSphereOBB(parent.pelota.ownSphere.BoundingSphere, obb))
                {

                    Vector3 collisionPos = new Vector3();
                    Vector3 spherePosition = new Vector3();
                    spherePosition = parent.pelota.pos;

                    TgcRay ray = new TgcRay(lastPos, spherePosition - lastPos);
                    TgcCollisionUtils.intersectRayObb(ray, obb, out collisionPos);

                    Vector3 velocidadATransmitir = 0.02f * (spherePosition - collisionPos) * FastMath.Abs(velocidadTotalInicial);
                    velocidadATransmitir = new Vector3(velocidadATransmitir.X, velocidadATransmitir.Y * 0.3f, velocidadATransmitir.Z);

                    //translate(spherePosition - lastPos);
                    parent.pelota.velocity = parent.pelota.velocity + velocidadATransmitir;
                    parent.pelota.mover(elapsedTime);
                    velocidadHorizontal = velocidadHorInicial * (1/i*i);

                    colisionando = true;
                    if (i == 5) velocidadHorizontal = -velocidadHorizontal;
                    
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

        public Vector3 getTranslationComponent(Matrix world)
        {
            Vector3 result = new Vector3();

            result.X = world.M41;
            result.Y = world.M42;
            result.Z = world.M43;

            return result;
        }

        #endregion

    }
}
