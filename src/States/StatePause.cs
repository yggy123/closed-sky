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
        private Window		window;
        private Button[]	m_PauseButtons;
        private Label		m_Help;

        //Class Constructor
        public StatePause()
            : base(StateID.Pause)
        {
            //Draw cursor
            m_VisibleCursor = true;
        	m_PopUp = true;
            
            //Nulling value
            m_Help = null;
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

			//Create Help label
			m_Help = new Label(Global.GUIManager);
			m_Help.Init();
        	m_Help.Text = Global.PAUSE_HELP;
        	m_Help.Top = 190;
        	m_Help.Left = 200;
        	m_Help.Width = 300;
        	m_Help.Height = 280;          
			
			//Add it to the
        	m_Panel.Add(m_Help);
        	Global.GUIManager.Add(m_Help);
        }

        private void PauseChoose(object sender, EventArgs e)
        {
            //Resume Button
            if (sender == m_PauseButtons[0]) m_Active=false;

            //Restart Button
            //if (sender == m_PauseButtons[1]) ;

            //To title Button
            if (sender == m_PauseButtons[2]) Global.StateManager.GoTo(StateID.Title, null);

            //Exit Button
            if (sender == m_PauseButtons[3]) Global.StateManager.Quit();
        }

         public override void OnEnter()
        {
        }

         public override void Update(GameTime time)
         {
         }
    }
}