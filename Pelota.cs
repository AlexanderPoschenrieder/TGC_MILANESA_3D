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
        
        const float GRAVEDAD = -9.81f;
        public TgcSphere ownSphere;
        List<TgcMesh> obstaculos = new List<TgcMesh>();


        public Pelota()
        {
            ownSphere = new TgcSphere();

            ownSphere.Radius = 20;
            ownSphere.setColor(Color.Red);
            ownSphere.Position = new Vector3(0, 80, 0);

            //ownSphere.AutoTransformEnable = false;
        }

        public void aplicarGravedad(float elapsedTime)
        {
            exposicionAGravedad = exposicionAGravedad + 1;
            ownSphere.move(0, exposicionAGravedad * GRAVEDAD * elapsedTime * 0.25f, 0);
        }

        public void chequearColisiones()
        {
            
            
        }

        public void updateValues()
        {
            
            ownSphere.updateValues();
            
        }

        public void render()
        {
            ownSphere.render();
        }


    }
}
