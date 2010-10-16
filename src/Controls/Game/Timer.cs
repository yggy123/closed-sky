
//Namespaces used
using System;
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

//Class namespace
namespace Klotski.Controls.Game {
	/// <summary>
	/// Class description.
	/// </summary>
	class Timer : Control {
		//Member
		protected TimeSpan		m_Time;
		protected SpriteFont	m_Font;
		/// <summary>
		/// Class constructor.
		/// </summary>
		public Timer(Manager manager) : base(manager) {
			//Initialize stuff
			m_Font = null;
			m_Time = TimeSpan.Zero;
		}

		public void Init(string font) {
			//Original initialization
			base.Init();

			//Load font
			m_Font = Global.StateManager.Content.Load<SpriteFont>(font);

			//Calculate width and height
			Width	= (int)m_Font.MeasureString(new TimeSpan(0, 0, 0, 0, 100).ToString()).X + 32;
			Height	= (int)m_Font.MeasureString(new TimeSpan(0, 0, 0, 0, 100).ToString()).Y;
		}

		public TimeSpan GetTime() {
			return m_Time;
		}

		public void Increase(TimeSpan time) {
			//Increase time
			m_Time += time;

			//Invalidate
			Invalidate();
		}

		protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime time) {
			//Draw the time
			renderer.DrawString(m_Font, m_Time.ToString(), rect, Color.White, Alignment.MiddleLeft);
		}
	}
}
