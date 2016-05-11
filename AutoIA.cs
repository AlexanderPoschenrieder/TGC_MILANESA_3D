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
    public class AutoIA : Auto
    {
        #region Constructor

        public AutoIA(TgcMesh mesh, EjemploAlumno p)
            : base(mesh, p)
        {
        }

        #endregion

        protected override void CalcularMovimiento()
        {
            var pelota = parent.pelota.ownSphere.Position;
            var posActual = this.meshAuto.Position;
            var direccionPelota = pelota - posActual;
            var direccionMov = Vector3.Normalize(direccionMovimiento);

            if (Vector3.Cross(direccionPelota, direccionMov) == new Vector3(0, 0, 0))
            {
                if (direccionPelota.Length() > 50)
                {
                    Acelerar(aceleracion);
                }
                if (direccionPelota.Length() <= 50)
                {
                    Acelerar(0);
                }
                if (direccionPelota.Length() < 10 && pelota.Z > 0)
                {
                    Saltar();
                }

            }
            else
            {
                var left = isLeft(posActual, posActual + direccionMov, pelota);
                if (left)
                {
                    Rotar(-1);
                    Acelerar(aceleracion);
                }
                else
                {
                    Rotar(1);
                    Acelerar(aceleracion);
                }
            }

        }

        public bool isLeft(Vector3 a, Vector3 b, Vector3 c)
        {
            return ((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) > 0;
        }

    }
}
