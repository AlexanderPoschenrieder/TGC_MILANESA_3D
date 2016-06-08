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
    public class GameOver: Menu
    {
        public List<MenuButton> buttons = new List<MenuButton>();
        public TgcSprite header;
        public TgcSprite resultsFrame;
        public TgcSprite defaultBackgroud = new TgcSprite();
        private System.Windows.Forms.Control panel;
        EjemploAlumno parent;

        TgcText2d ganador = new TgcText2d();
        TgcText2d scoreLocal;
        TgcText2d scoreVisitante;

        public GameOver(EjemploAlumno par)
        {
            parent = par;
            panel = GuiController.Instance.Panel3d;
            this.defaultBackgroud.Texture = TgcTexture.createTexture(
                EjemploAlumno.mediaFolder +"\\menu\\fondo.jpg");
            this.defaultBackgroud.Position = new Vector2(0, 0);
            this.defaultBackgroud.Scaling = new Vector2(
                (float)panel.Size.Width / this.defaultBackgroud.Texture.Width,
                (float)panel.Size.Height / this.defaultBackgroud.Texture.Height);
            this.setHeader("game_over.png");
            this.SetResults();
            

            addButton(6, "volver.png", (EjemploAlumno ej) =>
            {
                this.ganador.dispose();
                this.scoreLocal.dispose();
                this.scoreVisitante.dispose();
                parent.resetGame();
            });
         
        }

        private void SetResults()
        {
            this.resultsFrame = new TgcSprite();
            this.resultsFrame.Texture = TgcTexture.createTexture(EjemploAlumno.mediaFolder + "menu\\" + "cuadro_vacio.png");
            this.resultsFrame.Scaling = new Vector2(0.3f, 0.3f);
            this.resultsFrame.Position = new Vector2(
                (GuiController.Instance.Panel3d.Size.Width - this.header.Texture.Width / 2) / 2 + 100, 100);
        }

        public override void setHeader(string headerFile)
        {
            this.header = new TgcSprite(); 
            this.header.Texture = TgcTexture.createTexture(EjemploAlumno.mediaFolder + "menu\\"+headerFile); 
            this.header.Scaling = new Vector2(0.3f,0.3f);
            this.header.Position = new Vector2(
                (GuiController.Instance.Panel3d.Size.Width - this.header.Texture.Width/2) / 2 +100 ,10 );
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
            createResults();
            GuiController.Instance.Drawer2D.beginDrawSprite();
            this.defaultBackgroud.render();
            this.handleClicks();
            this.header.render();
            this.resultsFrame.render();
            this.buttons.ForEach(button => button.render());
            GuiController.Instance.Drawer2D.endDrawSprite();
            this.ganador.render();
            this.scoreLocal.render();
            this.scoreVisitante.render();
        }

        private void createResults()
        {
            var centerX =GuiController.Instance.Panel3d.Size.Width /2;
            var centerY =GuiController.Instance.Panel3d.Size.Height /2;

            this.ganador.Text = "¡¡Ganador ";
            this.ganador.Size = new Size(300, 100);
            this.ganador.changeFont(new System.Drawing.Font("Calibri", 24, FontStyle.Bold | FontStyle.Italic));
            this.ganador.Color = Color.DarkGreen;
            this.ganador.Position = new Point(centerX-150,centerY -100);
            
            this.scoreLocal = parent.txtScoreLocal;
            this.scoreLocal.Color = Color.DarkGreen;
            this.scoreLocal.Position = new Point(centerX - 150, ganador.Position.Y + 50);

            this.scoreVisitante = parent.txtScoreVisitante;
            this.scoreVisitante.Color = Color.DarkGreen;
            this.scoreVisitante.Position = new Point(centerX - 150, scoreLocal.Position.Y + 30);

            if (parent.scoreLocal > parent.scoreVisitante)
            {
                this.ganador.Text += "Jugador 1!!";
            }
            else if (parent.scoreLocal< parent.scoreVisitante)
            {
                this.ganador.Text += "Jugador 2!!";
            }
            else
            {
                this.ganador.Text = "Empate... ¬¬";
            }

        }

        public void handleClicks()
        {
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            float mouseX = d3dInput.Xpos;
            float mouseY = d3dInput.Ypos;

            if (d3dInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                foreach (MenuButton button in buttons)
                {
                    button.handleClick(parent, mouseX, mouseY);
                }
            }
        }
    }
}
