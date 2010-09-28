
//Namespaces used
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

//Class namespace
namespace Klotski.Controls {
	/// <summary>
	/// A custom button used for title menu.
	/// </summary>
	public class CustomButton : Button {
		//Members
		protected SpriteFont	m_Font;

		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="manager">GUI manager of the control.</param>
		/// <param name="text">The button's text</param>
		/// <param name="left">How far is the left side from the left of the form?</param>
		/// <param name="top">How far is the top side from the top of the form?</param>
		public CustomButton(Manager manager, string text, int left, int top) : base(manager) {
			//Set values
			Top		= top;
			Left	= left;
			Text	= text;

			//Nulling stuff
			m_Font = null;
		}

		public void Init(string font) {
			//Original initialization
			base.Init();

			//Load font
			m_Font = Global.StateManager.Content.Load<SpriteFont>(font);
			
			//Calculate width and height
			Width  = (int)m_Font.MeasureString(Text).X;
			Height = (int)m_Font.MeasureString(Text).Y;
		}

		protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime) {
			//Set color based on state
			Color FontColor = Color.White;
			if (ControlState == ControlState.Hovered) FontColor = Color.Gold;
			if (ControlState == ControlState.Pressed) FontColor = Color.Gray;

			//Set offset based on state
			Rectangle Rect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
			if (ControlState == ControlState.Pressed) {
				Rect.X += 4; Rect.Y += 4;
			}

			//Draw text only
			if (m_Font != null) renderer.DrawString(m_Font, Text, Rect, FontColor, Alignment.MiddleLeft );
		}
	}
}
