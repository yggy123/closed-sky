//Namespaces used
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace Klotski.States
{
    public class StatePause : State {
        //Member
        private Window		m_Window;
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
            m_Window = new Window(Global.GUIManager);
            m_Window.Init();
            m_Window.CloseButtonVisible = false;
            m_Window.Movable = false;
            m_Window.StayOnBack = true;
            m_Window.Resizable = false;
          
            m_Window.Text = "Pause Game";
            m_Window.Top = 150; 
            m_Window.Left = 170;
            m_Window.Width = 450;
            m_Window.Height = 330;
            m_Panel.Add(m_Window);
            Global.GUIManager.Add(m_Window);
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

            //TODO: Fix layout.
        }

        private void PauseChoose(object sender, EventArgs e) {
            //Resume Button
            if (sender == m_PauseButtons[0]) m_Active=false;

            //Restart Button
            if (sender == m_PauseButtons[1]) {
                //Restart previous state
                Global.StateManager.GetPreviousState(this).Initialize();

                //return
                m_Active = false;
            }

            //To title Buttonze
            if (sender == m_PauseButtons[2]) Global.StateManager.GoTo(StateID.Title, null);

            //Exit Button
            if (sender == m_PauseButtons[3]) Global.StateManager.Quit();
        }

         public override void OnEnter() {
         }

         public override void Update(GameTime time) {
         }
    }
}