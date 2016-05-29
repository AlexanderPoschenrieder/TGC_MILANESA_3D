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
            rotate(new Vector3(0, 1, 0), FastMath.PI);
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

            if (input.keyDown(Key.RightAlt))
            {
                if (!saltando)
                {
                    saltando = true;
                    ejeRotacionSalto = Vector3.Cross(new Vector3(0, 1, 0), direccion);
                    velocidadVertical = 100;
                }
            }
        
        }

        #endregion

    }
}
