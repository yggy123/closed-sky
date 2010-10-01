//Namespaces used
using FlatRedBall;
using Klotski.Controls;
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using System.Collections.Generic;
using FlatRedBall.IO;


namespace Klotski.States
{
    public class StatePause : State
    {
        //Member
        private Window window;
        private Button[] m_PauseButtons;
        private Label[] m_Label;

        //Class Constructor
        public StatePause()
            : base(StateID.Pause)
        {
            //Draw cursor
            m_VisibleCursor = true;
            
            //Nulling value
            m_Label = null;
            m_PauseButtons = null;
        }

        public override void Initialize()
        {
          

            #region Initialize Window
            //Create Window
            window = new Window(Global.GUIManager);
            window.Init();
            window.CloseButtonVisible = false;
            window.Movable = false;
            window.StayOnBack = true;
            window.Resizable = false;
          
            window.Text = "Pause Game";
            window.Top = 150; 
            window.Left = 170;
            window.Width = 450;
            window.Height = 330;
            m_Panel.Add(window);
            Global.GUIManager.Add(window);
            #endregion

            #region Initialize Buttons
            m_PauseButtons = new Button[Global.PAUSE_MENU.Length];
            for (int i = 0; i < m_PauseButtons.Length; i++)
            {
                //Create buttons
                m_PauseButtons[i] = new Button(Global.GUIManager);
                m_PauseButtons[i].Text = Global.PAUSE_MENU[i];
                m_PauseButtons[i].Top = Global.PAUSEBUTTON_TOP + (i * Global.PAUSEBUTTON_SPACE);
                m_PauseButtons[i].Left = Global.PAUSEBUTTON_LEFT;
                m_PauseButtons[i].Width = Global.PAUSEBUTTON_WIDTH;
                m_PauseButtons[i].Init();

                //Add the buttons
                m_Panel.Add(m_PauseButtons[i]);
                Global.GUIManager.Add(m_PauseButtons[i]);

                //Set event handler
                m_PauseButtons[i].Click += PauseChoose;
            }
            #endregion

            #region Initialize Label
            m_Label = new Label[Global.LABEL_MENU.Length];
            for (int i = 0; i < m_Label.Length; i++)
            {
                //Create Lbel
                m_Label[i] = new Label(Global.GUIManager);
                m_Label[i].Text = Global.LABEL_MENU[i];
                m_Label[i].Top = 220 + i*30;
                m_Label[i].Left = 200;
                m_Label[i].Width = 250;

                //Add the label
                m_Panel.Add(m_Label[i]);
                //window.Add(m_Label[i]);
                Global.GUIManager.Add(m_Label[i]);
            }
 
            #endregion
        }

        private void PauseChoose(object sender, EventArgs e)
        {
            //Resume Button
            if (sender == m_PauseButtons[0]) m_Active=false;

            //Restart Button
            if (sender == m_PauseButtons[1]) ;

            //To title Button
            if (sender == m_PauseButtons[2]) Global.StateManager.GoTo(StateID.Title, null);

            //Exit Button
            if (sender == m_PauseButtons[3]) Global.StateManager.Exit();
        }

         public override void OnEnter()
        {
        }

         public override void Update(GameTime time)
         {
         }
    }
}

/*di global aku tambahi

        //Pause::MenuButton
        public static readonly string[] PAUSE_MENU = new string[4] { "Resume" , "Restart", "To Title", "Exit" };
        public const int                PAUSEBUTTON_LEFT = 470;
        public const int                PAUSEBUTTON_TOP = 230;
        public const int                PAUSEBUTTON_SPACE = 50;
        public const int                PAUSEBUTTON_WIDTH = 100;
       
        //Pause::Help
        public static readonly string[] LABEL_MENU = new string[6] { "Help", 
                    "vasdasdweqweqweasdasdasd", 
                    "12313easdasdasczxc", 
                    "asdsdasdwfqwqweqweqwe",
                    "asdasdzczxcvdssfad",
        "asdasdadasd"};

*/