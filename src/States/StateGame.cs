
//Namespaces used
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Lighting;
using Klotski.Utilities;
using Klotski.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

//Class namespace
namespace Klotski.States {
	/// <summary>
	/// Class description.
	/// </summary>
	public class StateGame : State {
		//Members
		private Sky			m_Sky;
		private List<Ship>	m_Ships;

		private Camera m_ChaseCamera;
		private Camera m_BirdsCamera;
 
		/// <summary>
		/// Class constructor.
		/// </summary>
		public StateGame() : base(StateID.Game) {
			//Empties variables
			m_Sky = null;
			m_Ships = new List<Ship>();
			m_BirdsCamera = null;
			m_ChaseCamera = null;
		}

		public override void Initialize() {
			//Set camera
			m_ChaseCamera = SpriteManager.Camera;
			m_BirdsCamera = new Camera(FlatRedBallServices.GlobalContentManager);
			m_BirdsCamera.X = 0;
			m_BirdsCamera.Y = 20.0f;
			m_BirdsCamera.Z = 40.0f;
			//SpriteManager.Camera.SetCameraTo(m_BirdsCamera);
			m_ChaseCamera.BackgroundColor.A = 0;
			Renderer.UseRenderTargets = false;
			

			//Set lighting
			Renderer.LightingEnabled = true;
			Renderer.Lights.SetDefaultLighting(LightCollection.DefaultLightingSetup.Evening);
			Renderer.Lights.DirectionalLights[0].Direction = new Vector3(0.6f, -0.8f, 0);

			//Create sky
			m_Sky = new Sky(Global.TEXTURE_FOLDER + "Sky", SpriteManager.Camera);

			//Create ships
			Ship S = new Ship(m_Layer);
			S.Initialize();
			m_Ships.Add(S);

			S = new Ship(m_Layer);
			S.Initialize(20, -20, 1, 1);
			m_Ships.Add(S);

			S = new Ship(m_Layer);
			S.Initialize(10, -50, 2, 2);
			m_Ships.Add(S);

			//Set camera position
			SpriteManager.Camera.Y = 5.0f;
		}

		public override void OnEnter() {
		}

		public override void Update(GameTime time) {
            SpriteManager.Camera.RotationX -= InputManager.Mouse.YChange / 150.0f;
            SpriteManager.Camera.RotationY -= InputManager.Mouse.XChange / 80.0f;
            if (InputManager.Keyboard.KeyPushed(Keys.Escape))
                Global.StateManager.GoTo(StateID.Title, null);
		}

        public override void OnExit() {
            base.OnExit();

            //Reset camera
            SpriteManager.Camera.X			= 0;
            SpriteManager.Camera.Y			= 0;
            SpriteManager.Camera.Z			= 40;
            SpriteManager.Camera.RotationX  = 0;
            SpriteManager.Camera.RotationY  = 0;
            SpriteManager.Camera.RotationZ  = 0;
        }

		public override void Draw(GameTime time) {
			//Draws the sky
			m_Sky.Draw(time);
		}
	}
}
