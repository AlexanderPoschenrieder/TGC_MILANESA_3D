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
using System.Drawing;

namespace AlumnoEjemplos.MiGrupo
{
    class Pelota
    {
        float exposicionAGravedad = 0;
        Vector3 pos;
        Vector3 velocity;

        Matrix scale = Matrix.Identity;
        Matrix translate = Matrix. Identity;
        Matrix rotate = Matrix.Identity;
        Matrix move = Matrix.Identity;

        Matrix temp;

        const float GRAVEDAD = -9.81f;
        public TgcSphere ownSphere;
        List<TgcMesh> obstaculos = new List<TgcMesh>();


        public Pelota()
        {
            ownSphere = new TgcSphere();

            ownSphere.Radius = 50;
            ownSphere.setColor(Color.Red);

            //ownSphere.AutoTransformEnable = false;
            ownSphere.Transform = move;
        }

        public void aplicarGravedad()
        {
            exposicionAGravedad = exposicionAGravedad + 1;
            temp = Matrix.Translation(0, exposicionAGravedad * GRAVEDAD, 0);

            translate = translate * temp;
        }

        public void chequearColisiones()
        {
            
            
        }

        public void render()
        {
            aplicarGravedad();

            move = Matrix.Identity;

            move = move * translate;
            ownSphere.Transform = move;
            ownSphere.updateValues();
            ownSphere.render();
        }


    }
}
