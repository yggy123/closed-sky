//ReSharper disable EmptyConstructor

//Class namespace
using System;
using Klotski.Utilities;

namespace Klotski.States {
	//State identifier
	public enum StateID {
		Title,	//Title state
		Story,	//Story board
		Config,	//Config state
		Game,	//Game state
		Editor,
		Credit,	//Credit
		Pause
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
				case StateID.Title:	 return new StateTitle();
                case StateID.Story:  return new StateStory(parameters[0] as string, parameters[1] as string);
				case StateID.Config: return new StateConfig();
				case StateID.Game:	 return new StateGame((Player)parameters[0], parameters[1] as Game.GameData);
				case StateID.Pause:  return new StatePause();
				case StateID.Editor: return new StateEditor((int)parameters[0], (int)parameters[1]);
				case StateID.Credit: return new StateStory(parameters[0] as string, parameters[1] as string);
				default:			throw new Exception(Global.UNKNOWNSTATE_ERROR);
			}
		}
	}
}
