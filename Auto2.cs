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
    public class Auto2: Auto
    {
        #region Constructor

        public Auto2(TgcMesh mesh, EjemploAlumno p)
            : base(mesh, p)
        {
        }

        #endregion
        #region Métodos

        protected override void CalcularMovimiento()
        {
            ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia
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

            }
            else if (input.keyDown(Key.Down))
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

            if (!saltando && input.keyDown(Key.RightControl))
            {
                FrenoDeMano();
            }

            if (input.keyDown(Key.RightAlt))
            {
                if (!saltando)
                {
                    saltando = true;
                    velocidadVertical = 100;
                }
            }
        }


        #endregion

    }
}
