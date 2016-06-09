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
        public TgcSprite header;
        public TgcSprite resultsFrame;
        public TgcSprite disclaimerFrame;
        public TgcSprite defaultBackgroud = new TgcSprite();
        public TgcSprite milanesaBackgroud = new TgcSprite();
        private System.Windows.Forms.Control panel;
        EjemploAlumno parent;

        TgcText2d ganador = new TgcText2d();
        TgcText2d scoreLocal;
        TgcText2d scoreVisitante;
        private bool disclaimer=false;

        public GameOver(EjemploAlumno par)
        {

            parent = par;
            //if (GuiController.Instance.FullScreenEnable)
            //{
            //    panel = GuiController.Instance.FullScreenPanel;
            //}
            //else
            //{
                panel = GuiController.Instance.Panel3d;
            //}
            
            this.defaultBackgroud.Texture = TgcTexture.createTexture(
                EjemploAlumno.mediaFolder +"\\menu\\fondo.jpg");
            this.defaultBackgroud.Position = new Vector2(0, 0);
            this.defaultBackgroud.Scaling = new Vector2(
                (float)panel.Size.Width / this.defaultBackgroud.Texture.Width,
                (float)panel.Size.Height / this.defaultBackgroud.Texture.Height);
            this.setHeader("game_over.png");
            this.SetResults();
            initDisclaimer();

            addButton(6, "volver.png", (EjemploAlumno ej) =>
            {
                disclaimer = true;
            });
            buttons[0].selected = true;
        }

        private void initDisclaimer()
        {
            this.milanesaBackgroud.Texture = TgcTexture.createTexture(
               EjemploAlumno.mediaFolder + "\\menu\\milanesa.jpg");
            this.milanesaBackgroud.Position = new Vector2(0, 0);
            this.milanesaBackgroud.Scaling = new Vector2(
                (float)panel.Size.Width / this.defaultBackgroud.Texture.Width,
                (float)panel.Size.Height / this.defaultBackgroud.Texture.Height);
            this.SetDisclaimer();
        }

        private void SetResults()
        {
            this.resultsFrame = new TgcSprite();
            this.resultsFrame.Texture = TgcTexture.createTexture(EjemploAlumno.mediaFolder + "menu\\" + "cuadro_vacio.png");
            this.resultsFrame.Scaling = new Vector2(0.4f, 0.3f);
            this.resultsFrame.Position = new Vector2(
                (panel.Size.Width - this.header.Texture.Width / 2) / 2 + 50, 100);
        }

        private void SetDisclaimer()
        {
            this.disclaimerFrame = new TgcSprite();
            this.disclaimerFrame.Texture = TgcTexture.createTexture(EjemploAlumno.mediaFolder + "menu\\" + "disclaimer.png");
            this.disclaimerFrame.Scaling = new Vector2(0.4f, 0.3f);
            this.disclaimerFrame.Position = new Vector2(
                (panel.Size.Width - this.header.Texture.Width / 2) / 2 + 50, 100);
        }

        public override void setHeader(string headerFile)
        {
            this.header = new TgcSprite(); 
            this.header.Texture = TgcTexture.createTexture(EjemploAlumno.mediaFolder + "menu\\"+headerFile); 
            this.header.Scaling = new Vector2(0.3f,0.3f);
            this.header.Position = new Vector2(
                (panel.Size.Width - this.header.Texture.Width/2) / 2 +100 ,10 );
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

            if (disclaimer)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();
                this.milanesaBackgroud.render();
                this.disclaimerFrame.render();
                GuiController.Instance.Drawer2D.endDrawSprite();
            }
            else
            {
                createResults();
                GuiController.Instance.Drawer2D.beginDrawSprite();
                this.defaultBackgroud.render();
                this.handleKeyboard();
                this.header.render();
                this.resultsFrame.render();
                this.buttons.ForEach(button => button.render());
                GuiController.Instance.Drawer2D.endDrawSprite();
                this.ganador.render();
                this.scoreLocal.render();
                this.scoreVisitante.render();
            }
        }

        private void detectInput()
        {
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            if (d3dInput.keyPressed(Key.Space) ||
                d3dInput.keyPressed(Key.Return) ||
                d3dInput.keyPressed(Key.Escape))
            {
                parent.close();
            }
        }

        private void createResults()
        {
            var centerX =(int)resultsFrame.Position.X+50;
            var centerY = (int)resultsFrame.Position.Y + 100;

            this.ganador.Text = "¡¡Ganador ";
            this.ganador.Size = new Size(300, 100);
            this.ganador.changeFont(new System.Drawing.Font("Calibri", 24, FontStyle.Bold | FontStyle.Italic));
            this.ganador.Color = Color.DarkGreen;
            this.ganador.Position = new Point(centerX,centerY);
            
            this.scoreLocal = parent.txtScoreLocal;
            this.scoreLocal.Color = Color.DarkGreen;
            this.scoreLocal.Position = new Point(centerX, ganador.Position.Y + 50);

            this.scoreVisitante = parent.txtScoreVisitante;
            this.scoreVisitante.Color = Color.DarkGreen;
            this.scoreVisitante.Position = new Point(centerX, scoreLocal.Position.Y + 30);

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

    }
}
