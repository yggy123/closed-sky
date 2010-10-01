
//Namespaces used
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Graphics;
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

//Application namespace
namespace Klotski.States {
	/// <summary>
	/// Class description.
	/// </summary>
	class StateStory : State {
		//Members
	    private string m_Text;
	    private string m_Caption;
		private float  m_Seconds;

		/// <summary>
		/// Class constructor.
		/// </summary>
		public StateStory(string caption, string text) : base(StateID.Story) {
			//Initialize member
			m_Seconds = 0;
		    m_Caption = caption;
		    m_Text    = text;
		}

		public override void Initialize() {
            //SpriteManager.Camera.Position = Vector3.Zero;
            System.Console.WriteLine(SpriteManager.Camera.X);
            System.Console.WriteLine(SpriteManager.Camera.Y);
            System.Console.WriteLine(SpriteManager.Camera.Z);

			//Create background sprite
			Sprite Background = SpriteManager.AddSprite(
				Global.IMAGE_FOLDER + "Storm1",
				FlatRedBallServices.GlobalContentManager,
				m_Layer);

			//Resize sprite
			Background.ScaleX = Background.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
			Background.ScaleY = Background.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;

			//Create story
			Text Story = TextManager.AddText(m_Text, m_Layer);
			Story.HorizontalAlignment = HorizontalAlignment.Center;

			//Set story's font
			Story.Font = new BitmapFont(
				Global.CONTENT_FOLDER + Global.FONT_FOLDER + Global.STORY_FONT + ".tga",
				Global.CONTENT_FOLDER + Global.FONT_FOLDER + Global.STORY_FONT + ".fnt",
				FlatRedBallServices.GlobalContentManager);

			//Set position
			Story.Y = Global.STORY_Y;
			Story.Z = Global.STORY_Z;
			Story.RotationX = Global.STORY_ANGLE;
			Story.ZVelocity = -Global.STORY_SPEED;
			Story.YVelocity = Global.STORY_SPEED;
		}

		public override void OnEnter() {
			//Play BGM
			Global.SoundManager.PlayBGM(Global.STORY_BGM);
		}

        private void ChangeState() {
            //Based on the story
            if (m_Caption == Global.STORY_CAPTION)  Global.StateManager.GoTo(StateID.Config, null, true);
            if (m_Caption == Global.CREDIT_CAPTION) m_Active = false;
        }

		public override void Update(GameTime time) {
			//Increase time counter
			m_Seconds += time.ElapsedGameTime.Milliseconds / 1000.0f;

			//Check time
			if (m_Seconds > Global.STORY_TIME) ChangeState();

			//Check input
			if (InputManager.Keyboard.KeyPushed(Keys.Space)	||
				InputManager.Keyboard.KeyPushed(Keys.Escape) ||
                InputManager.Keyboard.KeyPushed(Keys.Enter)) ChangeState();
		}
	}
}
