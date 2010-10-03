
//Namespaces used
using FlatRedBall;
using FlatRedBall.Graphics;
using Klotski.Utilities;
using Microsoft.Xna.Framework.Graphics;

//Class namespace
namespace Klotski.Components {
	/// <summary>
	/// Sky sphere for environment.
	/// </summary>
	public class Sky : IDrawableBatch {
		//Members
		protected Effect	m_FX;
		protected Model		m_Sphere;

		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="texture">Cube texture for sky.</param>
		public Sky(string texture) {
			//Create and set effect
			m_FX = Global.StateManager.Content.Load<Effect>(Global.SKY_SHADER);
			m_FX.Parameters[Global.SKYTEX_PARAMETER].SetValue(Global.StateManager.Content.Load<TextureCube>(texture));

			//Create model
			m_Sphere = Global.StateManager.Content.Load<Model>(Global.SKY_MODEL);
			foreach (ModelMesh mesh in m_Sphere.Meshes) foreach (ModelMeshPart part in mesh.MeshParts)
					part.Effect = m_FX;

			Z = -1000.0f;

			//Logging
			Global.Logger.AddLine("Skysphere created.");
		}

		/// <summary>
		/// Used to draw assets
		///             Batch is sorted by Z with sprites and text
		/// </summary>
		/// <param name="camera">The currently drawing camera</param>
		public void Draw(Camera camera) {
			//Set effect
			m_FX.Parameters[Global.SKYVIEW_PARAMETER].SetValue(camera.View);
			m_FX.Parameters[Global.SKYPROJ_PARAMETER].SetValue(camera.Projection);

			//Draw the sky
			foreach (ModelMesh mesh in m_Sphere.Meshes) mesh.Draw();

			//Reset render state
			Global.StateManager.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
			Global.StateManager.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
		}

		/// <summary>
		/// Interface implementations (not used)
		/// </summary>
		public void Destroy() { }
		public void Update() {}
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		/// <summary>
		/// Whether or not this batch should be updated
		/// </summary>
		public bool UpdateEveryFrame { get { return false; } }
	}
}
