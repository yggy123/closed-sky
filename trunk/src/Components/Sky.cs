
//Namespaces used
using FlatRedBall;
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Class namespace
namespace Klotski.Components {
	/// <summary>
	/// Sky sphere for environment.
	/// </summary>
	public class Sky {
		//Members
		protected Effect	m_FX;
		protected Model		m_Sphere;
		protected Camera	m_Camera;

		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="texture">Cube texture for sky.</param>
		/// <param name="camera">Default camera for sky.</param>
		public Sky(string texture, Camera camera) {
			//Set camera
			m_Camera = camera;

			//Create and set effect
			m_FX = Global.StateManager.Content.Load<Effect>(Global.SKY_SHADER);
			m_FX.Parameters[Global.SKYTEX_PARAMETER].SetValue(Global.StateManager.Content.Load<TextureCube>(texture));

			//Create model
			m_Sphere = Global.StateManager.Content.Load<Model>(Global.SKY_MODEL);
			foreach (ModelMesh mesh in m_Sphere.Meshes) foreach (ModelMeshPart part in mesh.MeshParts)
				part.Effect = m_FX;

			//Logging
			Global.Logger.AddLine("Skysphere created.");
		}

		/// <summary>
		/// Camera mutator.
		/// </summary>
		/// <param name="camera">The new camera that should be used as basis.</param>
		public void SetCamera(Camera camera) {
			m_Camera = camera;
		}

		public void Draw(GameTime time) {
			//Set effect
			m_FX.Parameters[Global.SKYVIEW_PARAMETER].SetValue(m_Camera.View);
			m_FX.Parameters[Global.SKYPROJ_PARAMETER].SetValue(m_Camera.Projection);

			//Draw the sky
			foreach (ModelMesh mesh in m_Sphere.Meshes) mesh.Draw();

			//Reset render state
			Global.StateManager.GraphicsDevice.RenderState.CullMode					= CullMode.CullCounterClockwiseFace;
			Global.StateManager.GraphicsDevice.RenderState.DepthBufferWriteEnable	= true;
		}
	}
}
