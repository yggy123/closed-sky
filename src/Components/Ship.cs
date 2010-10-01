
//Namespace used
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Class namespace
namespace Klotski.Components {
	/// <summary>
	/// Ship class, a block in klotski puzzle.
	/// </summary>
	public class Ship {
        //Members
        protected Layer				m_Layer;
        protected PositionedModel	m_Model;
		protected BoundingBox		m_AABB;

		/// <summary>
		/// Ship class constructor.
		/// </summary>
		/// <param name="layer">FRB layer for the ship model.</param>
        public Ship(Layer layer) {
            //
            m_Layer = layer;
		}

        public void Initialize() {
            //Create and add model to layer
            m_Model = ModelManager.AddModel("Content/Models/Blocks/Balloon12", FlatRedBallServices.GlobalContentManager, false);
            ModelManager.AddToLayer(m_Model, m_Layer);

			m_Model.X = 40.0f;
			m_Model.Z = -40.0f;
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

				for (int i = 0; i < mesh.VertexBuffer.SizeInBytes / sizeof(float); i+= mesh.MeshParts[0].VertexStride / sizeof(float) ) {
					Vector3 pos=new Vector3(verticesData[i], verticesData[i+1], verticesData[i+2]);
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

		public void Initialize(float X, float Y, int Width, int Height) {
			//Create and add model to layer
			m_Model = ModelManager.AddModel("Content/Models/Blocks/Balloon" + Width + Height, FlatRedBallServices.GlobalContentManager, false);
			ModelManager.AddToLayer(m_Model, m_Layer);

			m_Model.X = X;
			m_Model.Z = Y;
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
        //
		
		public BoundingBox GetBoundingBox() {
			//Create a new bounding box
			return new BoundingBox(m_AABB.Min + m_Model.Position, m_AABB.Max + m_Model.Position);
		}
    }
}
