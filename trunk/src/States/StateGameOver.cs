
//Namespaces used
using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Graphics;
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;

//Class namespace namespace
namespace Klotski.States {
    /// <summary>
    /// Class description.
    /// </summary>
    public class StateGameOver : State {
    	private readonly int	m_Result;
    	private readonly int	m_Step;
    	private readonly int	m_Visited;
    	private TimeSpan		m_Time;

		/// <summary>
		/// Game over state class constructor
		/// </summary>
		/// <param name="result">The result, -1 is gameover, 0 is player win, 1 is AI win</param>
		/// <param name="time"></param>
		/// <param name="step"></param>
		/// <param name="node"></param>
        public StateGameOver(int result, TimeSpan time, int step, int node) : base(StateID.GameOver) {
			//Set variables
			m_Time		= time;
			m_Step		= step;
			m_Visited	= node;
        	m_Result	= result;
        }

		public override void Initialize() {
			//Reset camera
			SpriteManager.Camera.X = Global.APPCAM_DEFAULTX;
			SpriteManager.Camera.Y = Global.APPCAM_DEFAULTY;
			SpriteManager.Camera.Z = Global.APPCAM_DEFAULTZ;
			SpriteManager.Camera.RotationX = Global.APPCAM_DEFAULTROTX;
			SpriteManager.Camera.RotationY = Global.APPCAM_DEFAULTROTY;
			SpriteManager.Camera.RotationZ = Global.APPCAM_DEFAULTROTZ;

			//Create BG
			Sprite Background = null;

			//If gameover
			if (m_Result == -1) {
				//Create background sprite
				Background = SpriteManager.AddSprite(Global.IMAGE_FOLDER + "GameOver", FlatRedBallServices.GlobalContentManager, m_Layer);
			}
			else {
				//Load background
				string BGFile = "Victory-AI";
				if (m_Result == 0) BGFile = "Victory-Player";
				Background = SpriteManager.AddSprite(Global.IMAGE_FOLDER + BGFile, FlatRedBallServices.GlobalContentManager, m_Layer);

				//Load bitmap font
				BitmapFont BmpFont = new BitmapFont(
					Global.CONTENT_FOLDER + Global.FONT_FOLDER + "SFIVBitmap.tga",
					Global.CONTENT_FOLDER + Global.FONT_FOLDER + "SFIVBitmap.fnt",
					FlatRedBallServices.GlobalContentManager);

				//Load texts
				Text Time = TextManager.AddText(m_Time.TotalSeconds.ToString(), m_Layer);
				Text Step = TextManager.AddText(m_Step.ToString(), m_Layer);
				Step.Font = BmpFont;
				Time.Font = BmpFont;

				//Place text
				Time.X = -3;
				Time.Y =  5;
				Step.X = -3;
				Step.Y = 0.8f;
				Time.SetPixelPerfectScale(SpriteManager.Camera);
				Step.SetPixelPerfectScale(SpriteManager.Camera);

				//Load visited node if AI);
				if (m_Result == 1) {
					Text Visited = TextManager.AddText(m_Visited.ToString(), m_Layer);
					Visited.Font = BmpFont;
					Visited.X = -3;
					Visited.Y = -3.5f;
					Visited.SetPixelPerfectScale(SpriteManager.Camera);
				}

				//Load ranks
				int ranking = 1;
				Sprite Rank = null;
				if (m_Time.Seconds < 600)  ranking = 0;
				if (m_Time.Seconds > 1800) ranking = 2;

				//Load image
				Rank = SpriteManager.AddSprite(
					Global.IMAGE_FOLDER + "Rank" + ranking,
					FlatRedBallServices.GlobalContentManager,
					m_Layer);

				Rank.X = 0;
				Rank.Y = 0;
				Rank.ScaleX = Rank.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
				Rank.ScaleY = Rank.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
			}

			//Resize background
			Background.ScaleX = Background.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
			Background.ScaleY = Background.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;

        }

		public override void OnEnter() {}

        public override void Update(GameTime time) {
        	//If space, return to title
			if (InputManager.Keyboard.KeyPushed(Keys.Space)) Global.StateManager.GoTo(StateID.Title, null);
        }
    }
}
