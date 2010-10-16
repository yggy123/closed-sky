
//Namespaces used
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

//Application namespace
namespace Klotski.Controls.Game {
	/// <summary>
	/// Class description.
	/// </summary>
	class Counter : Control {
		//Members
		protected int			m_Counter1;
		protected int			m_Counter2;
		protected bool			m_ShowNode;
		protected SpriteFont	m_Font;

		/// <summary>
		/// Class constructor.
		/// </summary>
		public Counter(Manager manager) : base(manager) {
			//Initialize stuff
			m_Font		= null;
			m_Counter1	= 0;
			m_Counter2	= 0;
			m_ShowNode	= true;
		}

		public void Init(string font, bool node) {
			//Original initialization
			base.Init();

			//Shows node?
			m_ShowNode = node;

			//Load font
			m_Font = Global.StateManager.Content.Load<SpriteFont>(font);

			//Calculate width and height
			Width  = (int)m_Font.MeasureString("Visited node 10000").X;
			Height = (int)m_Font.MeasureString("Visited node 10000").Y * 2;
		}

		public void SetCounter(int value1, int value2) {
			//Set counter
			m_Counter1 = value1;
			m_Counter2 = value2;

			//Invalidate
			Invalidate();
		}

		protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime time) {
			//Create string
			string Txt = "Step " + m_Counter1;
			if (m_ShowNode) Txt += "\r\nVisited nodes " + m_Counter2;
		//	Txt = "raka";

			//Draw string
			renderer.DrawString(m_Font, Txt, rect, Color.White, Alignment.BottomLeft);
		}


	}
}
