//ReSharper disable DoNotCallOverridableMethodsInConstructor

//Namespaces used
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

//Class namespace
namespace Klotski.Controls.Game {
	/// <summary>
	/// A GUI showing actor's life bar.
	/// </summary>
	public class LifeBar : Control {
		//Value
		private int m_Life;

		//Image
		private readonly Texture2D m_BarImage;
		private readonly Texture2D m_LifeImage;

		/// <summary>
		/// Class constructor.
		/// </summary>
		public LifeBar(Manager manager) : base(manager) {
			//Initialize life
			m_Life = 0;

			//Load images
			m_LifeImage = Global.StateManager.Content.Load<Texture2D>(Global.LIFE_TEXTURE);
			m_BarImage	= Global.StateManager.Content.Load<Texture2D>(Global.LIFEBAR_TEXTURE);

			//Set width and height
			Width  = m_BarImage.Width;
			Height = m_BarImage.Height;
		}

		/// <summary>
		/// Set the amount of life left.
		/// </summary>
		/// <param name="life"></param>
		public void SetLife(int life) {
			//Change life
			m_Life = life;
		}

		protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime time) {
			//Draw the lifebar
			renderer.Draw(m_BarImage, rect, Color.White);

			//For each life
			for (int i = 0; i < m_Life; i++) {
				//Calculate position
				Rectangle LifeRect	 = new Rectangle(rect.Left, rect.Top, m_LifeImage.Width, m_LifeImage.Height);
				LifeRect.X			+= Global.LIFE_X + (i * (m_LifeImage.Width - 2));
				LifeRect.Y			+= Global.LIFE_Y;

				//Draw it
				renderer.Draw(m_LifeImage, LifeRect, Color.White);
			}
		}
	}
}
