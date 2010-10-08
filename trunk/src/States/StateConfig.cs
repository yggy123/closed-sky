
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
    public class StateConfig : State {
        //Members
        private ListBox         m_FileListBox;
        private HeroButton[]    m_HeroButtons;
        private Button[]        m_MenuButtons;

        /// <summary>
        /// Class constructor.
        /// </summary>
        public StateConfig()
            : base(StateID.Config)
        {
            //Draw cursor
            m_VisibleCursor = true;

            //Nulling Value
            m_FileListBox = null;
            m_HeroButtons = null;
            m_MenuButtons = null;
        }

        public override void Initialize()
        {
            //Create background sprite
            Sprite Background = SpriteManager.AddSprite(
                Global.IMAGE_FOLDER + "Storm2",
                FlatRedBallServices.GlobalContentManager,
                m_Layer);

            //Resize sprite
            Background.ScaleX = Background.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
            Background.ScaleY = Background.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;

            #region Initialize Hero Buttons
            m_HeroButtons = new HeroButton[Global.HEROBUTTON_MENU.Length];
            for (int i = 0; i < m_HeroButtons.Length; i++)
            {
                //Create buttons
                m_HeroButtons[i] = new HeroButton(
                    Global.GUIManager,
                    Global.HEROBUTTON_MENU[i],
                    Global.HEROBUTTON_LEFT,
                    Global.HEROBUTTON_TOP + (i * Global.HEROBUTTON_SPACE),
                    "Images/HeroImage",
                    Global.HEROBUTTON_INFO[i]);
                m_HeroButtons[i].Init();

                //Add the buttons
                m_Panel.Add(m_HeroButtons[i]);
                Global.GUIManager.Add(m_HeroButtons[i]);

                //Set event handler
                m_HeroButtons[i].Click += HeroChoose;
            }
            #endregion

            #region Initialize Board File ListBox
            //Create ListBox
            m_FileListBox = new ListBox(Global.GUIManager);

            //Init ListBox
            m_FileListBox.Init();
            m_FileListBox.Left = Global.BOARD_LISTMENU_LEFT;
            m_FileListBox.Top = Global.BOARD_LISTMENU_TOP;
            m_FileListBox.Width = 228;
            m_FileListBox.Height = 400;

            //Fill ListBox with GameBoard file list
            List<string> m_TempList = null;
            m_TempList = FileManager.GetAllFilesInDirectory(Global.LEVEL_FOLDER, Global.BOARD_EXTENSION, 0);

            //Remove Extension and Path 
            for (int i = 0; i < m_TempList.Count; ++i)
            {
                m_TempList[i] = FileManager.RemovePath(m_TempList[i]);
                m_TempList[i] = FileManager.RemoveExtension(m_TempList[i]);
                m_FileListBox.Items.Add(m_TempList[i]);
            }

            //add ListBox to GUI manager and State Panel List
            Global.GUIManager.Add(m_FileListBox);
            m_Panel.Add(m_FileListBox);
            #endregion

            #region Initialize Buttons
            m_MenuButtons = new Button[Global.CONFIG_MENU.Length];
            for (int i = 0; i < m_MenuButtons.Length; i++)
            {
                //Create buttons
                m_MenuButtons[i] = new Button(Global.GUIManager);
                m_MenuButtons[i].Text = Global.CONFIG_MENU[i];
                m_MenuButtons[i].Top = Global.CONFIGBUTTON_TOP + (i * Global.CONFIGBUTTON_SPACE);
                m_MenuButtons[i].Left = Global.CONFIGBUTTON_LEFT;
                m_MenuButtons[i].Width = Global.CONFIGBUTTON_WIDTH;
                m_MenuButtons[i].Init();

                //Add the buttons
                m_Panel.Add(m_MenuButtons[i]);
                Global.GUIManager.Add(m_MenuButtons[i]);

                //Set event handler
                m_MenuButtons[i].Click += MenuChoose;
            }
            #endregion

			//Set default selection
			m_FileListBox.ItemIndex			= 0;
			m_FileListBox.Focused			= true;
        	m_HeroButtons[0].m_Highlight	= true;

        }

		private void HeroChoose(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            foreach (HeroButton h in m_HeroButtons) {
                h.m_Highlight = false;
            }

            ((HeroButton)sender).m_Highlight = true;

            foreach (HeroButton h in m_HeroButtons)
            {
                h.Invalidate();
            }
        }

		private void MenuChoose(object sender, TomShane.Neoforce.Controls.EventArgs e) {
        	//If back, destroy state
            if (sender == m_MenuButtons[0]) m_Active = false;

			//If save the king
			if (sender == m_MenuButtons[1]) {
				//Create parameter
				Object[] Parameters = new object[2];
                Parameters[0] = Player.Klotski;
                Parameters[0] = Player.BFS;
				Parameters[1] = GameData.LoadGameData(m_FileListBox.Items[m_FileListBox.ItemIndex] as string);

				//Go to play state
            	Global.StateManager.GoTo(StateID.Game, Parameters, true);
            }
        }

        public override void OnEnter() {
        }

        public override void Update(GameTime time) {
        }
    }
}
