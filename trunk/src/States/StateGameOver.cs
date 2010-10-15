
//Namespaces used
using System;
using FlatRedBall;
using FlatRedBall.IO;
using Klotski.Controls;
using Klotski.Utilities;
using Klotski.States.Game;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using System.Collections.Generic;

//Application namespace
namespace Klotski.States
{
    /// <summary>
    /// Class description.
    /// </summary>
    public class StateGameOver : State
    {
        private Button[] m_GameOver;
        private int winning;
        private int playtime;
        private Label m_displaytime;
      
        
  
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// parameter 1 = win or lose, win = 1, lose = 0
        /// parameter 2 = jumlah detik permainan
        public StateGameOver(int win, int time)
            : base(StateID.GameOver)
        {
            //Draw cursor
            m_VisibleCursor = true;

            //Nulling Value
            m_GameOver = null;
            winning = win;
            playtime = time;
   
        }

        public override void Initialize()
        {

            //Check win or lose
            if (winning == 1)
            {
                //Create background sprite
                Sprite Background = SpriteManager.AddSprite(
                    Global.IMAGE_FOLDER + "Winning",
                    FlatRedBallServices.GlobalContentManager,
                    m_Layer);

                #region check rank
                //Check rank based time played
                if (playtime < 100)
                {
                    Sprite Rank = SpriteManager.AddSprite(
                    Global.RANK_FOLDER + "A",
                    FlatRedBallServices.GlobalContentManager,
                    m_Layer);

                    //Resize
                    Rank.ScaleX = Rank.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
                    Rank.ScaleY = Rank.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
                }
                else if (playtime < 200)
                {
                    Sprite Rank = SpriteManager.AddSprite(
                    Global.RANK_FOLDER + "B",
                    FlatRedBallServices.GlobalContentManager,
                    m_Layer);

                    //Resize
                    Rank.ScaleX = Rank.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
                    Rank.ScaleY = Rank.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
                }
                else if (playtime >= 200)
                {
                    Sprite Rank = SpriteManager.AddSprite(
                    Global.RANK_FOLDER + "C",
                    FlatRedBallServices.GlobalContentManager,
                    m_Layer);

                    //Resize
                    Rank.ScaleX = Rank.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
                    Rank.ScaleY = Rank.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
                }
                #endregion

                //Resize sprite
                Background.ScaleX = Background.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
                Background.ScaleY = Background.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
               

            }

            else if (winning == 0)
            {
                //Create background sprite
                Sprite Background = SpriteManager.AddSprite(
                    Global.IMAGE_FOLDER + "lost",
                    FlatRedBallServices.GlobalContentManager,
                    m_Layer);
                //Resize sprite
                Background.ScaleX = Background.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
                Background.ScaleY = Background.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
            }

          

            #region Initialize Buttons
            m_GameOver = new Button[3];

            //Create Buttons
            m_GameOver[0] = new Button(Global.GUIManager);
            m_GameOver[1] = new Button(Global.GUIManager);
           
            m_GameOver[0].Text = "Play Again!";
            m_GameOver[1].Text = "To Title!";

            for (int i = 0; i < 2; i++)
            {
                //Initialize Properties
                m_GameOver[i].Top = 530;
                m_GameOver[i].Left = 240 + (i*200);
                m_GameOver[i].Width = 100;
                m_GameOver[i].Init();
                //Add the buttons
                m_Panel.Add(m_GameOver[i]);
                Global.GUIManager.Add(m_GameOver[i]);

                //Set event handler
                m_GameOver[i].Click += OverChoose;
            }
            #endregion

            //Create Time label
            m_displaytime = new Label(Global.GUIManager);
            m_displaytime.Init();
            TimeSpan t = TimeSpan.FromSeconds(playtime);
            m_displaytime.Text = t.ToString();
            m_displaytime.Top = 80;
            m_displaytime.Left = 300;
            m_displaytime.Width = 300;
            m_displaytime.Height = 280;

            //Add it to the
            m_Panel.Add(m_displaytime);
            Global.GUIManager.Add(m_displaytime);
        }


        private void OverChoose(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            //Restart Button
            if (sender == m_GameOver[0])
            {
                //Restart previous state
                Global.StateManager.GetPreviousState(this).Initialize();

                //return
                m_Active = false;
            }

            //To title Button
            if (sender == m_GameOver[1]) Global.StateManager.GoTo(StateID.Title, null);    
        }

        public override void OnEnter()
        {
        }

        public override void Update(GameTime time)
        {
        }
    }
}
