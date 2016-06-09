using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.Milanesa_3D
{
    public delegate void ButtonCallback();
    public class MenuButton
    {
        private TgcSprite sprite;
        public Action<EjemploAlumno> callback;
        public TgcSprite selector = new TgcSprite();
        public bool selected = false;
        private System.Windows.Forms.Control panel;

        public MenuButton(float index, TgcSprite sprite, Action<EjemploAlumno> callback)
        {
            panel = GuiController.Instance.Panel3d;
            this.sprite = sprite;
            this.callback = callback;
            this.sprite.Scaling = new Vector2(.14f,.14f);
            this.sprite.Position = new Vector2(
                (panel.Size.Width - this.width()) / 2,
                150 + (index * ((float)panel.Size.Height/400) * this.height()));
            createSelector();
        }


        private void createSelector()
        {
            selector.Texture = TgcTexture.createTexture(EjemploAlumno.mediaFolder + "menu\\selector.png");
            selector.Scaling = this.sprite.Scaling;
            selector.Position = this.sprite.Position;
        }


        private float width()
        {
            return this.sprite.Texture.Width * this.sprite.Scaling.X;
        }

        private float height()
        {
            return this.sprite.Texture.Height * this.sprite.Scaling.Y;
        }

        public void render()
        {
            this.sprite.render();
            if (selected)
            {
                selector.render();
            }

        }

        public float topX()
        {
            return this.bottomX() + this.width();
        }

        public float topY()
        {
            return this.bottomY() + this.height();
        }

        public float bottomX()
        {
            return this.sprite.Position.X;
        }

        public float bottomY()
        {
            return this.sprite.Position.Y;
        }

    }
}
