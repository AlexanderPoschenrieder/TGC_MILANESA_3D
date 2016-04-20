using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.MiGrupo
{
    public class Auto
    {
        public float velocidad;
        float rozamientoCoef = 200f;
        public float rotacion;
        public float elapsedTime;
        float aceleracionAvanzar = 600f;
        float aceleracionFrenar = 800f;
        float aceleracionMarchaAtras = 400f;
        float velocidadMinima = -2000f;
        float velocidadMaxima = 3000f;
        List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> ruedas;
        public TgcMesh meshAuto;
        float direccion;

        //Interfaz de usuario
        public Auto(TgcMesh mesh)
        {
            meshAuto = mesh;
        }


        public Auto(float rot, TgcMesh auto, List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> unasRuedas = null)
        {
            this.rotacion = rot;
            this.ruedas = unasRuedas;
            meshAuto = auto;
        }

        public void Mover()
        {
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
                Avanzar();
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                Retroceder();
            }
            //else
            //{
            //    auto.noMover();
            //}
        }

        public void Avanzar()
        {
            Acelerar(aceleracionAvanzar);
            meshAuto.moveOrientedY(-this.velocidad * elapsedTime);
        }

        public void Retroceder()
        {
            if (velocidad > 0) Frenar();
            if (velocidad < 0) MarchaAtras();
            meshAuto.moveOrientedY(-this.velocidad * elapsedTime);
        }

        public void NoMover()
        {
            Acelerar(0);
        }

        public void Rotar(float unaDireccion)
        {
            direccion = unaDireccion;
            rotacion += (elapsedTime * direccion * (velocidad / 1000)); //direccion puede ser 1 o -1, 1 es derecha y -1 izquierda
            AjustarRotacion();
            this.meshAuto.Rotation = new Vector3(0f, this.rotacion, 0f);
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

            velocidad += (aumento - Rozamiento()) * elapsedTime;
            AjustarVelocidad();

        }

        public void AjustarVelocidad()
        {
            if (velocidad > velocidadMaxima) velocidad = velocidadMaxima;
            if (velocidad < velocidadMinima) velocidad = velocidadMinima;
        }

        public void EstablecerVelocidadMáximaEn(float velMaxima)
        {
            velocidadMaxima = velMaxima;
        }

        public float Rozamiento()
        {
            return rozamientoCoef * Math.Sign(velocidad);
        }

        private void Frenar()
        {
            Acelerar(-aceleracionFrenar);
        }

        public void FrenoDeMano()
        {
            Acelerar(-aceleracionFrenar * 2.5f);
        }

        public void MarchaAtras()
        {
            Acelerar(-aceleracionMarchaAtras);
        }


        //DeRotacion
        private void AjustarRotacion()
        {
            while (rotacion > 360)
            {
                rotacion -= 360;
            }
        }

        //para que el auto se quede quieto cuando perdio el jugador
        public void Estatico()
        {
            velocidadMaxima = 0f;
            velocidadMinima = 0f;
            AjustarVelocidad();
        }

        public float GetRotacion()
        {
            return this.rotacion;
        }

    }
}
