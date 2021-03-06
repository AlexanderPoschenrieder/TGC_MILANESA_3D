﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.Milanesa_3D
{
    
    public interface IMilanesaCamera
    {
        Matrix GetUpdatedViewMatrix();
        Vector3 getPosition();
    }

    public class MilanesaThirdPersonCamera : TgcCamera, IMilanesaCamera
    {

        static readonly Vector3 UP_VECTOR = new Vector3(0, 1, 0);

        public Matrix GetUpdatedViewMatrix()
        {
            updateCamera();
            return viewMatrix;
        }

        float offsetHeight;
        /// <summary>
        /// Desplazamiento en altura de la camara respecto del target
        /// </summary>
        public float OffsetHeight
        {
            get { return offsetHeight; }
            set { offsetHeight = value; }
        }

        float offsetForward;
        /// <summary>
        /// Desplazamiento hacia adelante o atras de la camara repecto del target.
        /// Para que sea hacia atras tiene que ser negativo.
        /// </summary>
        public float OffsetForward
        {
            get { return offsetForward; }
            set { offsetForward = value; }
        }

        Vector3 targetDisplacement;
        /// <summary>
        /// Desplazamiento final que se le hace al target para acomodar la camara en un cierto
        /// rincon de la pantalla
        /// </summary>
        public Vector3 TargetDisplacement
        {
            get { return targetDisplacement; }
            set { targetDisplacement = value; }
        }

        float rotationY;
        /// <summary>
        /// Rotacion absoluta en Y de la camara
        /// </summary>
        public float RotationY
        {
            get { return rotationY; }
            set { rotationY = value; }
        }

        Vector3 target;
        /// <summary>
        /// Objetivo al cual la camara tiene que apuntar
        /// </summary>
        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }

        Vector3 position;
        /// <summary>
        /// Posicion del ojo de la camara
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        bool enable;
        /// <summary>
        /// Habilita o no el uso de la camara
        /// </summary>
        public bool Enable
        {
            get { return enable; }
            set
            {
                enable = value;

                //Si se habilito la camara, cargar como la cámara actual
                if (value)
                {
                    GuiController.Instance.CurrentCamera = this;
                }
            }
        }

        public Matrix viewMatrix;


        /// <summary>
        /// Crear una nueva camara
        /// </summary>
        public MilanesaThirdPersonCamera()
        {
            resetValues();
        }

        /// <summary>
        /// Carga los valores default de la camara y limpia todos los cálculos intermedios
        /// </summary>
        public void resetValues()
        {
            offsetHeight = 20;
            offsetForward = -120;
            rotationY = 0;
            targetDisplacement = Vector3.Empty;
            target = Vector3.Empty;
            position = Vector3.Empty;
            viewMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Configura los valores iniciales de la cámara
        /// </summary>
        /// <param name="target">Objetivo al cual la camara tiene que apuntar</param>
        /// <param name="offsetHeight">Desplazamiento en altura de la camara respecto del target</param>
        /// <param name="offsetForward">Desplazamiento hacia adelante o atras de la camara repecto del target.</param>
        public void setCamera(Vector3 target, float offsetHeight, float offsetForward)
        {
            this.target = target;
            this.offsetHeight = offsetHeight;
            this.offsetForward = offsetForward;
        }

        /// <summary>
        /// Genera la proxima matriz de view, sin actualizar aun los valores internos
        /// </summary>
        /// <param name="pos">Futura posicion de camara generada</param>
        /// <param name="pos">Futuro centro de camara a generada</param>
        /// <returns>Futura matriz de view generada</returns>
        public Matrix generateViewMatrix(out Vector3 pos, out Vector3 targetCenter)
        {
            //alejarse, luego rotar y lueg ubicar camara en el centro deseado
            targetCenter = Vector3.Add(target, targetDisplacement);
            Matrix m = Matrix.Translation(0, offsetHeight, offsetForward) * Matrix.RotationY(rotationY) * Matrix.Translation(targetCenter);

            //Extraer la posicion final de la matriz de transformacion
            pos.X = m.M41;
            pos.Y = m.M42;
            pos.Z = m.M43;

            //Obtener ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
            return Matrix.LookAtLH(pos, targetCenter, UP_VECTOR);
        }

        public void updateCamera()
        {
            Vector3 targetCenter;
            viewMatrix = generateViewMatrix(out position, out targetCenter);
        }

        /// <summary>
        /// Rotar la camara respecto del eje Y
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateY(float angle)
        {
            rotationY += angle;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public Vector3 getLookAt()
        {
            return target;
        }


        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            if (!enable)
            {
                return;
            }

            d3dDevice.Transform.View = viewMatrix;
        }
    }

    public class TwoTargetsCamera : TgcCamera, IMilanesaCamera
    {
        public int alejamientoMaximo;

        static readonly Vector3 UP_VECTOR = new Vector3(0, 1, 0);
        TgcObb obb;

        float offsetHeight;
        public float alejamiento;
        EjemploAlumno parent;

        private Matrix viewMatrix;

        public Matrix GetUpdatedViewMatrix()
        {
            updateCamera();
            return viewMatrix;
        }


        /// <summary>
        /// Desplazamiento en altura de la camara respecto del target
        /// </summary>
        public float OffsetHeight
        {
            get { return offsetHeight; }
            set { offsetHeight = value; }
        }

        float offsetForward;
        /// <summary>
        /// Desplazamiento hacia adelante o atras de la camara repecto del target.
        /// Para que sea hacia atras tiene que ser negativo.
        /// </summary>
        public float OffsetForward
        {
            get { return offsetForward; }
            set { offsetForward = value; }
        }

        float rotationY;
        /// <summary>
        /// Rotacion absoluta en Y de la camara
        /// </summary>
        public float RotationY
        {
            get { return rotationY; }
            set { rotationY = value; }
        }

        private Vector3 completeRotation;

        public Vector3 CompleteRotation
        {
            get { return completeRotation; }
            set { completeRotation = value; }
        }


        Vector3 firstTarget;
        /// <summary>
        /// Objetivo al cual la camara tiene que apuntar
        /// </summary>
        public Vector3 FirstTarget
        {
            get { return firstTarget; }
            set { firstTarget = value; }
        }

        Vector3 secondTarget;
        /// <summary>
        /// Objetivo al cual la camara tiene que apuntar
        /// </summary>
        public Vector3 SecondTarget
        {
            get { return secondTarget; }
            set { secondTarget = value; }
        }

        Vector3 position;
        /// <summary>
        /// Posicion del ojo de la camara
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        bool enable;
        /// <summary>
        /// Habilita o no el uso de la camara
        /// </summary>
        public bool Enable
        {
            get { return enable; }
            set
            {
                enable = value;

                //Si se habilito la camara, cargar como la cámara actual
                if (value)
                {
                    GuiController.Instance.CurrentCamera = this;
                }
            }
        }


        /// <summary>
        /// Crear una nueva camara
        /// </summary>
        public TwoTargetsCamera(EjemploAlumno ej)
        {
            resetValues();
            obb = TgcObb.computeFromPoints(new Vector3[] { new Vector3(1, 1, 1), new Vector3(-1, -1, -1) });
            parent = ej;
        }

        /// <summary>
        /// Carga los valores default de la camara y limpia todos los cálculos intermedios
        /// </summary>
        public void resetValues()
        {
            offsetHeight = 20;
            offsetForward = -120;
            rotationY = 0;
            secondTarget = Vector3.Empty;
            position = Vector3.Empty;
            viewMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Configura los valores iniciales de la cámara
        /// </summary>
        /// <param name="target">Objetivo al cual la camara tiene que apuntar</param>
        /// <param name="offsetHeight">Desplazamiento en altura de la camara respecto del target</param>
        /// <param name="offsetForward">Desplazamiento hacia adelante o atras de la camara repecto del target.</param>
        public void setCamera(Vector3 target, float offsetHeight, float offsetForward)
        {
            this.firstTarget = target;
            this.offsetHeight = offsetHeight;
            this.offsetForward = offsetForward;
        }

        /// <summary>
        /// Genera la proxima matriz de view, sin actualizar aun los valores internos
        /// </summary>
        /// <param name="pos">Futura posicion de camara generada</param>
        /// <param name="pos">Futuro centro de camara a generada</param>
        /// <returns>Futura matriz de view generada</returns>
        public Matrix generateViewMatrix()
        {
            alejamientoMaximo = (int)GuiController.Instance.Modifiers.getValue("Distancia cámara");
            var lastPos = position;
            var newPos = new Vector3();

            var desplazamiento = new Vector3(0,80, 0);

            var targetCenter = Vector3.Add(firstTarget, desplazamiento);
            var vectorDirector = Vector3.Normalize(secondTarget - targetCenter);
            
            newPos = chequearColisiones(targetCenter,vectorDirector);

            Matrix m = Matrix.Translation(targetCenter - newPos) * Matrix.LookAtLH(targetCenter, secondTarget, UP_VECTOR);

            position = newPos;
            obb.Center = position;


            //Obtener ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
            return m;
        }

        private Vector3 chequearColisiones(Vector3 targetCenter, Vector3 director)
        {

            var newPos = targetCenter - director * (alejamiento+10);
            obb.Center = newPos;
            if (obb.Center == new Vector3(0, 0, 0))
            {
                return newPos;
            }

            if(Colisiona())
            {
                alejamiento -= 2;
                newPos = targetCenter - director * alejamiento;
                obb.Center = newPos;
            }
            if(alejamiento < alejamientoMaximo)
            {
                var valor = alejamiento +2;
                alejamiento = valor > alejamientoMaximo ? alejamientoMaximo : valor;
                newPos = targetCenter - director * alejamiento;
                obb.Center = newPos;
                if (Colisiona())
                {
                    newPos = newPos + director * (alejamiento - 2);
                    obb.Center = newPos;

                }
            }          

            return newPos;
        }

        private bool Colisiona()
        {
            return TgcCollisionUtils.testObbAABB(obb, parent.limiteLateralPositivo) 
                || TgcCollisionUtils.testObbAABB(obb, parent.limiteLateralNegativo)
                || TgcCollisionUtils.testObbAABB(obb, parent.limiteArcoPositivo1) 
                || TgcCollisionUtils.testObbAABB(obb, parent.limiteArcoPositivo2)
                || TgcCollisionUtils.testObbAABB(obb, parent.limiteArcoNegativo1)
                || TgcCollisionUtils.testObbAABB(obb, parent.limiteArcoNegativo2)
                || TgcCollisionUtils.testObbAABB(obb, parent.piso.BoundingBox)
                || TgcCollisionUtils.testObbObb(obb, parent.mainCar.obb)
                || TgcCollisionUtils.testObbObb(obb, parent.secondCar.obb);
        }

        public void updateCamera()
        {
            viewMatrix = generateViewMatrix();
        }

        /// <summary>
        /// Rotar la camara respecto del eje Y
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateY(float angle)
        {
            rotationY += angle;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public Vector3 getLookAt()
        {
            return secondTarget;
        }

        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            if (!enable)
            {
                return;
            }

            d3dDevice.Transform.View = viewMatrix;
        }

    }
}
