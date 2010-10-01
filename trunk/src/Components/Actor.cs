
//Namespaces used
using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Class namespace
namespace Klotski.Components {
	/// <summary>
	/// Game main character, Klotski.
	/// </summary>
	class Actor {
		//Members
		private Layer			m_Layer;
		private PositionedModel m_Model;
		private float			m_Velocity;
		private float			m_Angular;
		private Vector3			m_Direction;
		private bool			m_Jumping;
		protected BoundingBox	m_AABB;

		/// <summary>
		/// Actor class constructor.
		/// </summary>
		public Actor(Layer layer) {
			//Set layer
			m_Layer = layer;

			//Empties variable
			m_Model		= null;
			m_Direction = new Vector3(1.0f, 0.0f, 0.0f);
			m_Velocity = 40.0f;
			m_Angular = (float) (Math.PI * 0.5f);
			m_Jumping = false;
		}

		public void Initialize() {
			//Create model
			m_Model = ModelManager.AddModel("Content/Models/Boxie", FlatRedBallServices.GlobalContentManager, true);
			ModelManager.AddToLayer(m_Model, m_Layer);
			m_Model.CurrentAnimation = "Idle";

			//Positions model
			m_Model.Z = 20.0f;
			m_Model.Y = -10.0f;


			//Create AABB
			m_AABB = new BoundingBox();
			Matrix []Transforms = new Matrix[m_Model.XnaModel.Bones.Count];
			m_Model.XnaModel.CopyAbsoluteBoneTransformsTo(Transforms);

			foreach (ModelMesh mesh in m_Model.XnaModel.Meshes) {
				float[] verticesData = new float[mesh.VertexBuffer.SizeInBytes / sizeof(float)];
				mesh.VertexBuffer.GetData<float>(verticesData);
				Vector3 Min = new Vector3(verticesData[0], verticesData[1], verticesData[2]);
				Vector3 Max = Min;

				for (int i = 0; i < mesh.VertexBuffer.SizeInBytes / sizeof(float); i += mesh.MeshParts[0].VertexStride / sizeof(float)) {
					Vector3 pos=new Vector3(verticesData[i], verticesData[i + 1], verticesData[i + 2]);

					Min = Vector3.Min(Min, pos);
					Max = Vector3.Max(Max, pos);
				}
				// We need to take into account the fact that the mesh may have a bone transform
				Min = Vector3.Transform(Min, Transforms[mesh.ParentBone.Index]);
				Max = Vector3.Transform(Max, Transforms[mesh.ParentBone.Index]);

				m_AABB.Min = Vector3.Min(m_AABB.Min, Min);
				m_AABB.Max = Vector3.Max(m_AABB.Max, Max);
			}

			System.Console.WriteLine("Min " + m_AABB.Min + " Max " + m_AABB.Max);
		}

		public void Update(GameTime time) {
			//Rotate with AD
			if (InputManager.Keyboard.KeyDown(Keys.A))
				m_Model.RotationY += m_Angular * time.ElapsedGameTime.Milliseconds / 1000.0f;
			if (InputManager.Keyboard.KeyDown(Keys.D))
				m_Model.RotationY -= m_Angular * time.ElapsedGameTime.Milliseconds / 1000.0f * 0.5f;

			//Calculate direction
			m_Direction = new Vector3(
				(float)Math.Sin(m_Model.RotationY - (Math.PI * 0.5f)),
				0.0f,
				(float)Math.Cos(m_Model.RotationY - (Math.PI * 0.5f)));

			//Moves with ASWD
			if (InputManager.Keyboard.KeyDown(Keys.W))
				m_Model.Position += m_Direction * m_Velocity * time.ElapsedGameTime.Milliseconds / 1000.0f;
			if (InputManager.Keyboard.KeyDown(Keys.S))
				m_Model.Position -= m_Direction * m_Velocity * time.ElapsedGameTime.Milliseconds / 1000.0f;

			//Jump with space
			if (InputManager.Keyboard.KeyDown(Keys.Space) && !m_Jumping) {
				m_Model.CurrentAnimation = "Waving";

				m_Model.YVelocity = 60.0f;
				m_Model.YAcceleration = -80.0f;
				m_Jumping = true;
			}

			SpriteManager.Camera.Position = m_Model.Position - (m_Direction*15.0f) - new Vector3(0, -15.0f, 0);
		}

		public BoundingBox GetBoundingBox() {
			//Create a new bounding box
			return new BoundingBox(m_AABB.Min + m_Model.Position, m_AABB.Max + m_Model.Position);
		}

		public void SetVisibility(bool visible) {
			m_Model.Visible = visible;
		}

		public void OnCollision(Ship ship) {
			//If jumping
			if (m_Jumping) {
				//Stop jumping
				m_Model.YVelocity = 0.0f;
				m_Model.YAcceleration = 0.0f;
				m_Jumping = false;
				m_Model.CurrentAnimation = "Idle";

				//Place it
				m_Model.Y = ship.GetBoundingBox().Max.Y;
			}
		}
	}
}
