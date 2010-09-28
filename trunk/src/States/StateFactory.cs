//ReSharper disable EmptyConstructor

//Class namespace
using System;
using Klotski.Utilities;

namespace Klotski.States {
	//State identifier
	public enum StateID {
		Title,	//Title state
		Game,	//Game state
        Config, //Config state
		Story
	}

	/// <summary>
	/// Singleton factory that produces various states.
	/// </summary>
	public sealed class StateFactory {
		//Singleton
		private static readonly StateFactory m_Instance = new StateFactory();

		/// <summary>
		/// Private class constructor.
		/// </summary>
		static StateFactory()	{}
		private StateFactory()	{}

		/// <summary>
		/// Access to the only instance of StateFactory.
		/// </summary>
		/// <returns>StateFactory's only instance</returns>
		public static StateFactory Instance() {
			return m_Instance;
		}

		/// <summary>
		/// Create a new state based on state identifier.
		/// </summary>
		/// <param name="id">State type identifier.</param>
		/// <param name="parameters">Parameter that is needed by the state constructor</param>
		/// <returns></returns>
		public State CreateState(StateID id, object[] parameters) {
			//Return state based on ID
			switch (id) {
				case StateID.Title :	 return new StateTitle();
				case StateID.Game :	     return new StateGame();
				case StateID.Story :     return new StateStory();
                case StateID.Config :    return new StateConfig();
				default:			throw new Exception(Global.UNKNOWNSTATE_ERROR);
			}
		}
	}
}
