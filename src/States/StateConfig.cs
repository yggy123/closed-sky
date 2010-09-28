
//Namespaces used
using FlatRedBall;
using Klotski.Controls;
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using System.Collections.Generic;
using FlatRedBall.IO;

//Application namespace
namespace Klotski.States
{
    /// <summary>
    /// Class description.
    /// </summary>
    public class StateConfig : State
    {
        //Members
        private ListBox m_FileListBox;

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
        }

        public override void Initialize()
        {

            //Create background sprite
            Sprite Background = SpriteManager.AddSprite(
                Global.IMAGE_FOLDER + "Storm2.png",
                FlatRedBallServices.GlobalContentManager,
                m_Layer);

            //Resize sprite
            Background.ScaleX = Background.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
            Background.ScaleY = Background.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;

            #region Initialize Board File ListBox
            //Create ListBox
            m_FileListBox = new ListBox(Global.GUIManager);

            //Init ListBox
            m_FileListBox.Init();
            m_FileListBox.Left = Global.BOARD_LISTMENU_LEFT;
            m_FileListBox.Top = Global.BOARD_LISTMENU_TOP;
            m_FileListBox.Width = 100;
            m_FileListBox.Height = 200;

            //Fill ListBox with GameBoard file list
            List<string> m_TempList = null;
            m_TempList = FileManager.GetAllFilesInDirectory(Global.BOARD_FOLDER, Global.BOARD_EXTENSION, 0);

            //Remove Extension and Path 
            for (int i = 0; i < m_TempList.Count; ++i)
            {
                m_TempList[i] = FileManager.RemovePath(m_TempList[i]);
                m_TempList[i] = FileManager.RemoveExtension(m_TempList[i]);
                m_FileListBox.Items.Add(m_TempList[i]);
            }

            //add ListBox to GUI manager
            Global.GUIManager.Add(m_FileListBox);
            #endregion
        }

        public override void OnEnter()
        {
        }

        public override void Update(GameTime time)
        {
        }
    }
}
