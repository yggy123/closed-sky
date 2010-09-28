
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
				Global.IMAGE_FOLDER + "Storm2.png",
				FlatRedBallServices.GlobalContentManager,
				m_Layer);

			//Create title sprite
			Sprite Title = SpriteManager.AddSprite(
				Global.IMAGE_FOLDER + "Title.png",
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

		private void MenuClick(object sender, EventArgs e) {

			if (((CustomButton)sender).Text == Global.TITLE_MENU[0]) Global.StateManager.GoTo(StateID.Story, null);
			if (((CustomButton)sender).Text == Global.TITLE_MENU[3]) m_Active = false;
		}

		public override void OnEnter() {
			//Play song
			//Global.SoundManager.PlayBGM("song.mp3");
		}

		public override void Update(GameTime time) {
		}
	}
}
