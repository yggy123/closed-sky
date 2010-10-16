
//Namespaces used
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Model;
using Klotski.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Class namespace
namespace Klotski.Components {
	/// <summary>
	/// Class that represents object that can be controlled by player.
	/// </summary>
	public abstract class Playable {
		//Members
		protected Layer				m_Layer;
		protected Camera			m_Camera;
		protected BoundingBox		m_AABB;
		protected PositionedModel	m_Model;

		//Data
		protected bool m_Active;

		/// <summary>
		/// Class constructor.
		/// </summary>
		protected Playable(Layer layer) {
			//Set layer
			m_Layer = layer;

			//Empties variables
			m_Model		= null;

			//Initialize variables
			CalculateAABB();
			m_Active = false;
			m_Camera = new Camera(FlatRedBallServices.GlobalContentManager);
		}

		/// <summary>
		/// Initialize the entity by loading and placing its model.
		/// </summary>
		/// <param name="model">Path to the entity's model.</param>
		/// <param name="animated">Is the model animated?</param>
		/// <param name="x">Entity's model's X position</param>
		/// <param name="y">Entity's model's Y position</param>
		/// <param name="z">Entity's model's Z position</param>
		public virtual void Initialize(string model, bool animated, float x, float y, float z) {
			//Create and add model to layer
			m_Model = ModelManager.AddModel(
				Global.MODEL_FOLDER + model,
				FlatRedBallServices.GlobalContentManager,
				animated);
			ModelManager.AddToLayer(m_Model, m_Layer);

			//Calculate model's bounding box
			CalculateAABB();

			//Place the model in position
			m_Model.X = x;
			m_Model.Y = y;
			m_Model.Z = z;
		}

        /// <summary>
        /// Remove the entity
        /// </summary>
        public virtual void Remove() {
            //Remove model
            m_Layer.Remove(m_Model);
            ModelManager.RemoveModel(m_Model);
        }
		
		/// <summary>
		/// Is playable entity currently playable?
		/// </summary>
		/// <returns>m_Active</returns>
		public bool IsActive() {
			return m_Active;
		}

		/// <summary>
		/// Entity's camera accessor.
		/// </summary>
		/// <returns>m_Camera</returns>
		public Camera GetCamera() {
			return m_Camera;
		}

		/// <summary>
		/// Entity's bounding box accessor.
		/// </summary>
		/// <returns>Bounding box located in the position</returns>
		public BoundingBox GetBoundingBox() {
			//Return 0 AAB if there's no model yet
			if (m_Model == null) return m_AABB;

			//Return offseted AABB
			return new BoundingBox(m_AABB.Min + m_Model.Position, m_AABB.Max + m_Model.Position);
		}

		/// <summary>
		/// Entity's visibility mutator.
		/// </summary>
		/// <param name="visibility">Entity's new visibility</param>
		public void SetVisibility(bool visibility) {
			m_Model.Visible = visibility;
		}
		
		/// <summary>
		/// Calculate m_AABB
		/// </summary>
		protected virtual void CalculateAABB() {
			//Initialize AABB
			m_AABB = new BoundingBox(Vector3.Zero, Vector3.Zero);

			//Do nothing if there's no model
			if (m_Model == null) return;

			//Get transformation data
			Matrix []Transformation = new Matrix[m_Model.XnaModel.Bones.Count];
			m_Model.XnaModel.CopyAbsoluteBoneTransformsTo(Transformation);

			//For each mesh in the model
			foreach (ModelMesh mesh in m_Model.XnaModel.Meshes) {
				//Set min and max coordinates
				Vector3 Min = Vector3.Zero;
				Vector3 Max = Vector3.Zero;

				//Get vertex data
				int FloatCount = mesh.VertexBuffer.SizeInBytes / sizeof (float);
				int DataLength = mesh.MeshParts[0].VertexStride / sizeof(float);
				float[] Vertices = new float[FloatCount];
				mesh.VertexBuffer.GetData(Vertices);

				//For each vertex
				for (int i = 0; i < FloatCount; i+= DataLength) {
					//Create position vector
					Vector3 Position = new Vector3(Vertices[i], Vertices[i+1], Vertices[i+2]);

					//Get min and max
					Min = Vector3.Min(Min, Position);
					Max = Vector3.Max(Max, Position);
				}

				//Apply transformation
				Min = Vector3.Transform(Min, Transformation[mesh.ParentBone.Index]);
				Max = Vector3.Transform(Max, Transformation[mesh.ParentBone.Index]);

				//Compare it with current bounding box
				m_AABB.Min = Vector3.Min(m_AABB.Min, Min);
				m_AABB.Max = Vector3.Max(m_AABB.Max, Max);
			}
		}

		//Abstract class
		public abstract void Update(GameTime time);
		public abstract void UpdateControl(GameTime time);
	}
}
