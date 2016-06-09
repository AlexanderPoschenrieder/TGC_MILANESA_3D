using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Collision.ElipsoidCollision;
using TgcViewer.Utils.Shaders;
using System.Windows.Forms;
using TgcViewer.Utils._2D;
using TgcViewer.Utils;

namespace AlumnoEjemplos.Milanesa_3D
{
    public class Menu
    {
        public List<MenuButton> buttons = new List<MenuButton>();
        public TgcSprite header;
        public TgcSprite defaultBackgroud = new TgcSprite();
        public int selectorPosition = 0;
        private System.Windows.Forms.Control panel;

        EjemploAlumno parent;

        public Menu()
        {
        }

        public Menu(EjemploAlumno par)
        {
            parent = par;
            if (GuiController.Instance.FullScreenEnable)
            {
                panel = GuiController.Instance.FullScreenPanel;
            }
            else
            {
                panel = GuiController.Instance.Panel3d;
            }
            
            this.defaultBackgroud.Texture = TgcTexture.createTexture(
                EjemploAlumno.mediaFolder +"\\menu\\fondo.jpg");
            this.defaultBackgroud.Position = new Vector2(0, 0);
            this.defaultBackgroud.Scaling = new Vector2(
                (float)panel.Size.Width / this.defaultBackgroud.Texture.Width,
                (float)panel.Size.Height / this.defaultBackgroud.Texture.Height);
            this.setHeader("ligaCuete.png");

            addButton(1, "jugar.png", (EjemploAlumno ej) => 
            {
                ej.splitScreen = true;
                ej.estadoJuego = EstadoJuego.Juego;
            });
            addButton(2, "controles.png", (EjemploAlumno ej) =>
            {
                ej.estadoJuego = EstadoJuego.MenuControles;
            });
            buttons[0].selected = true;
        }


        public virtual void setHeader(string headerFile)
        {
            this.header = new TgcSprite(); 
            this.header.Texture = TgcTexture.createTexture(EjemploAlumno.mediaFolder + "menu\\"+headerFile); 
            this.header.Scaling = new Vector2(0.5f,0.5f);
            this.header.Position = new Vector2(
                (GuiController.Instance.Panel3d.Size.Width - this.header.Texture.Width/2) / 2 ,50 );
        }

        private void addButton(int position, string texturePath,Action<EjemploAlumno> callback)
        {
            var buttonspath = EjemploAlumno.mediaFolder + "menu\\buttons\\";
            TgcSprite sprite = new TgcSprite();
            sprite.Texture = TgcTexture.createTexture(buttonspath + texturePath);
            buttons.Add(
                new MenuButton(
                    position,
                    sprite,
                    callback));
        }
 
        public void render()
        {
            GuiController.Instance.Drawer2D.beginDrawSprite();
            this.defaultBackgroud.render();
            this.handleKeyboard();
            this.header.render();
            this.buttons.ForEach(button => button.render());
            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public void handleKeyboard()
        {
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            if (d3dInput.keyDown(Key.Down))
            {
                if (selectorPosition == buttons.Count-1)
                {
                    return;
                }

                buttons[selectorPosition].selected = false;
                selectorPosition++;
                buttons[selectorPosition].selected = true;
            }
            else if (d3dInput.keyPressed(Key.Up))
            {
                if (selectorPosition == 0)
                {
                    return;
                }

                buttons[selectorPosition].selected = false;
                selectorPosition--;
                buttons[selectorPosition].selected = true;
            }
            else if (d3dInput.keyPressed(Key.Return))
            {
                buttons[selectorPosition].callback(parent);
            }
        }
    }
}
