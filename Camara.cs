﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MiGrupo
{
    class TwoTargetsCamera : TgcCamera
    {
 static readonly Vector3 UP_VECTOR = new Vector3(0, 1, 0);


        float offsetHeight;

        Vector3 manualPosition;
        /// <summary>
        /// Posicion del ojo de la camara
        /// </summary>
        public Vector3 ManualPosition
        {
            get { return manualPosition; }
            set { manualPosition = value; }
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

        Matrix viewMatrix;


        /// <summary>
        /// Crear una nueva camara
        /// </summary>
        public TwoTargetsCamera()
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
        public Matrix generateViewMatrix(out Vector3 pos, out Vector3 targetCenter)
        {
            var desplazamiento = new Vector3(0, 80, 0);
            //alejarse, luego rotar y lueg ubicar camara en el centro deseado
            targetCenter = Vector3.Add(firstTarget, desplazamiento);
            var diff = (secondTarget - targetCenter);
            diff.Normalize();
            Matrix m = Matrix.Translation(diff*200) * Matrix.LookAtLH(targetCenter,secondTarget,UP_VECTOR);
            
            //Extraer la posicion final de la matriz de transformacion
            pos.X = m.M41;
            pos.Y = m.M42;
            pos.Z = m.M43;
            
            //Obtener ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
            return m;
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
