
//Namespaces used
using FlatRedBall;
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

//Class namespace
namespace Klotski.States {
	/// <summary>
	/// Class description.
	/// </summary>
	public class StateGame : State {
		/// <summary>
		/// Class constructor.
		/// </summary>
		public StateGame() : base(StateID.Game) {
		}

		public override void Initialize() {
			//Sprite S = SpriteManager.AddSprite("redball.bmp");
			//S.X = -15;
			//SpriteManager.AddToLayer(S, m_Layer);
			Button A = new Button(Global.GUIManager);
			A.Init();
			A.Click += AClicked;
			A.Left = 0;
			Global.GUIManager.Add(A);
			m_Panel.Add(A);
		}

		private void AClicked(object sender, EventArgs e) {
			//
			Global.StateManager.GoTo(StateID.Title, null);
		}

		public override void OnEnter() {
			//
		}

		public override void Update(GameTime time) {
		}
	}
}
