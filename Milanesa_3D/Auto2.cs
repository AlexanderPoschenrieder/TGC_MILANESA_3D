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

namespace AlumnoEjemplos.Milanesa_3D
{
    public class Auto2 : Auto
    {
        #region Constructor

        override public void setearNitroHUD()
        {
            this.nitroHUD = parent.txtNitroVisitante;
        }

        public Auto2(TgcMesh mesh, EjemploAlumno p)
            : base(mesh, p)
        {
            rotateYaw(FastMath.PI);
            direccion = -direccion;
        }

        #endregion
        #region Métodos

        protected override void CalcularMovimiento()
        {
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyDown(Key.Left))
            {
                Rotar(-1);
            }
            else if (input.keyDown(Key.Right))
            {
                Rotar(1);
            }

            if (input.keyDown(Key.Up))
            {
                if (!saltando)
                {
                    Acelerar(aceleracion);
                }
                else
                {
                    rotacionPitch(1);
                    Acelerar(0);
                }

            }
            else if (input.keyDown(Key.Down))
            {
                if (!saltando)
                {
                    Retroceder();
                }
                else
                {
                    rotacionPitch(-1);
                    Acelerar(0);
                }

            }
            else
            {
                Acelerar(0);
            }

            if (!saltando && input.keyDown(Key.RightControl))
            {
                if (velocidadHorizontal != 0)
                {
                    FrenoDeMano();
                }

            }

            if (input.keyPressed(Key.RightShift))
            {
                if (saltando)
                {
                    usarNitro();
                }

                else
                {
                    saltando = true;
                    direccionPreSalto = direccion;
                    pitchAcumuladoEnElSalto = 0;
                    yawAcumuladoEnElSalto = 0;
                    velocidadVertical = 100;
                }
            }

            if (input.keyPressed(Key.Period))
            {
                usarNitro();
            }
        }
        #endregion

    }
}
