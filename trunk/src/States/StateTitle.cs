
//Namespaces used
using FlatRedBall;
using Klotski.Controls;
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

//Application namespace
namespace Klotski.States {
	/// <summary>
	/// Class description.
	/// </summary>
	public class StateTitle : State {
		//Title buttons
		private CustomButton[] m_Buttons;

		/// <summary>
		/// Class constructor.
		/// </summary>
		public StateTitle() : base(StateID.Title) {
			//Draw cursor
			m_VisibleCursor = true;
			m_Buttons = null;
		}

		public override void Initialize() {
			//Create background sprite
			Sprite Background = SpriteManager.AddSprite(
				Global.IMAGE_FOLDER + "Storm2",
				FlatRedBallServices.GlobalContentManager,
				m_Layer);

			//Create title sprite
			Sprite Title = SpriteManager.AddSprite(
				Global.IMAGE_FOLDER + "Title",
				FlatRedBallServices.GlobalContentManager,
				m_Layer);
                       
            //Resize sprite
			Title.ScaleX		= Title.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
			Title.ScaleY		= Title.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
			Background.ScaleX	= Background.Texture.Width  / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
			Background.ScaleY	= Background.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;

			//Create buttons
			m_Buttons = new CustomButton[Global.TITLE_MENU.Length];
			for (int i = 0; i < m_Buttons.Length; i++) {
				//Create buttons
				m_Buttons[i] = new CustomButton(
					Global.GUIManager,
					Global.TITLE_MENU[i],
					Global.TITLE_MENU_LEFT,
					Global.TITLE_MENU_TOP + (i * Global.TITLE_MENU_SPACE));
				m_Buttons[i].Init(Global.TITLE_MENU_FONT);

				//Add the buttons
				m_Panel.Add(m_Buttons[i]);
				Global.GUIManager.Add(m_Buttons[i]);

				//Set event handler
				m_Buttons[i].Click += MenuClick;
			}
		}

		/// <summary>
		/// Menu click event handler
		/// </summary>
		/// <param name="sender">Who triggers the event?</param>
		/// <param name="e">The event</param>
		private void MenuClick(object sender, EventArgs e) {
            //Prepare parameter
		    object[] Parameters;

            if (sender == m_Buttons[0]) {
                //Create parameter
                Parameters      = new object[2];
                Parameters[0]   = Global.STORY_CAPTION;
                Parameters[1]   = Global.STORY_TEXT;

                //Go to story
                Global.StateManager.GoTo(StateID.Story, Parameters, false);
            }
            if (sender == m_Buttons[1])
            {
                //Create parameter
                Parameters    = new object[2];
                Parameters[0] = 5;
                Parameters[1] = 4;

                //Go to story
                Global.StateManager.GoTo(StateID.Editor, Parameters, false);
            }
            if (sender == m_Buttons[2]) {
                //Create parameter
                Parameters      = new object[2];
                Parameters[0]   = Global.CREDIT_CAPTION;
                Parameters[1]   = Global.CREDIT_TEXT;

                //Go to story
                Global.StateManager.GoTo(StateID.Story, Parameters);
            }
            if (sender == m_Buttons[3]) m_Active = false;
		}

		public override void OnEnter() {
			//Play song
			//Global.SoundManager.PlayBGM("song.mp3");
		}

		public override void Update(GameTime time) {
		}
	}
}
