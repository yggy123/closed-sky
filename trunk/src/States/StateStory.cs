
//Namespaces used
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using FlatRedBall.Graphics;
using Klotski.Utilities;
using Microsoft.Xna.Framework;

//Application namespace
namespace Klotski.States {
	/// <summary>
	/// Class description.
	/// </summary>
	class StateStory : State {
		//Members
		private Layer m_TextLayer;

		/// <summary>
		/// Class constructor.
		/// </summary>
		public StateStory() : base(StateID.Story) {
		}

		public override void Initialize() {
			//Create background sprite
			Sprite Background = SpriteManager.AddSprite(
				Global.IMAGE_FOLDER + "Storm1.png",
				FlatRedBallServices.GlobalContentManager,
				m_Layer);

			//Resize sprite
			Background.ScaleX = Background.Texture.Width / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;
			Background.ScaleY = Background.Texture.Height / SpriteManager.Camera.PixelsPerUnitAt(0) / 2.0f;


			Text T = TextManager.AddText("raka gan!\r\n Hahahaha sidhjasjkh ksajhd kahljkd salkjd \r\n kasdja s", m_Layer);
			T.Font = new BitmapFont(Global.CONTENT_FOLDER + Global.FONT_FOLDER + "TahomaBitmap.tga", Global.CONTENT_FOLDER + Global.FONT_FOLDER + "TahomaBitmap.fnt", FlatRedBallServices.GlobalContentManager);
			T.HorizontalAlignment = HorizontalAlignment.Center;
			T.X = 0;
			T.Y = -6;
			T.Z = 25.0f;
			T.RotationX = (float) -(Math.PI * 0.25);
			T.ZVelocity = -1.5f;
			T.YVelocity = 1.5f;
		}

		public override void OnEnter() {
		}

		public override void Update(GameTime time) {}
	}
}
