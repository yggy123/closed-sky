
//namespace used
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

//Class namespace
namespace Klotski.Controls {
	/// <summary>
	/// A custom button used for choosing Hero in Configuration State
	/// </summary>
	public class HeroButton : Button {
		//Members
		protected string m_Info;//string for Hero Information
        protected string m_Image;//string representing Hero Image File

        protected SpriteFont    m_Font;
        protected Texture2D     m_HeroImage;
        protected Texture2D     m_ButtonTexture;
        protected Texture2D     m_Layer;
        protected bool          m_InfoVisible;

        public bool             m_Highlight;

		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="manager">GUI manager of the control.</param>
		/// <param name="text">The button's text</param>
		/// <param name="left">How far is the left side from the left of the form?</param>
		/// <param name="top">How far is the top side from the top of the form?</param>
        /// <param name="image">Image file path representation of the Hero?</param>
        /// <param name="info">Information or Story aboue the Hero?</param>
        public HeroButton(Manager manager, string text, int left, int top, string image, string info) 
            : base(manager) {
			//Set values
			Top		= top;
			Left	= left;
			Text	= text;

            m_Image = image;
            m_Info = info;

            m_InfoVisible = false;
            m_Highlight   = false;

            //Nulling Stuffs
            m_Font          = null;
            m_HeroImage     = null;
            m_ButtonTexture = null;
            m_Layer         = null;
		}

		public override void Init() {
			//Original initialization
			base.Init();

            //Init Font
            m_Font = Global.StateManager.Content.Load<SpriteFont>(Global.HEROBUTTON_FONT);

            //Init Button Texture
            m_ButtonTexture = Global.StateManager.Content.Load<Texture2D>(Global.HEROBUTTON_TEXTURE);

            //Init Image
            m_HeroImage = Global.StateManager.Content.Load<Texture2D>(m_Image);

            //Init Layer
            m_Layer = Global.StateManager.Content.Load<Texture2D>(Global.HEROBUTTONOVERLAY_TEXTURE);
			
			//Set Button Width and Height
            Width   = Global.HEROBUTTON_WIDTH;
            Height  = Global.HEROBUTTON_HEIGHT;
		}

		protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime) {
            //Draw Base Control
            //base.DrawControl(renderer, rect, gameTime);
            Color FontColor = Color.White;

			//Set Display based on ControlState
            if (ControlState == ControlState.Hovered) m_InfoVisible = true;
            else m_InfoVisible = false;

            if (m_Highlight) FontColor = Color.Gold;
            else FontColor = Color.White;

			//Set offset based on state
			Rectangle Rect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
			if (ControlState == ControlState.Pressed) {
				Rect.X += 4; Rect.Y += 4;
			}

			//Draw Button
            renderer.Draw(m_ButtonTexture, Rect, Color.White);

            //Draw Information
            Rectangle HeroRect = new Rectangle(Rect.X + 20, Rect.Y + 20, Global.HEROBUTTON_IMAGEWIDTH, Global.HEROBUTTON_IMAGEHEIGHT);
            renderer.Draw(m_HeroImage, HeroRect, Color.White);

            renderer.DrawString(m_Font, Text, HeroRect.X + 135, HeroRect.Y, FontColor);

            if (m_InfoVisible) renderer.DrawString(m_Font, m_Info, HeroRect.X + 135, HeroRect.Y + 30, FontColor);

            //Draw Overlay
            if (ControlState == ControlState.Hovered || m_Highlight) renderer.Draw(m_Layer, Rect, Color.White);
		}
	}
}
