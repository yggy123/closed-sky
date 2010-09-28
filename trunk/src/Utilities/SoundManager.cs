
//Namespaces used
using System.Text;
using Klotski.Utilities.FMOD;

//Class namespace
namespace Klotski.Utilities {
	/// <summary>
	/// Class that manages sound in the application.
	/// </summary>
	public class SoundManager {
		//Members
		private readonly FMOD.System m_System;
		private Sound	m_BGM;
		private Channel m_BGMChannel;

		/// <summary>
		/// Class constructor.
		/// </summary>
		public SoundManager() {
			//Empties variable
			m_System		= null;
			m_BGM			= null;
			m_BGMChannel	= null;

			//Create FMOD system
			CheckError(Factory.System_Create(ref m_System));
			
			//Check version
			uint Version = 0;
			CheckError(m_System.getVersion(ref Version));
			if (Version < VERSION.number) throw new System.Exception(Global.FMODVERSION_ERROR);

			//Check driver
			CheckDriver();
		}

		/// <summary>
		/// Check and fix any problem with hardware.
		/// </summary>
		private void CheckDriver() {
			//Get number of driver available
			int DriverNumber = 0;
			CheckError(m_System.getNumDrivers(ref DriverNumber));

			//Driver checking
			#region Solve hardware problems
			//If there's 0 driver, no sound
			if (DriverNumber <= 0) CheckError(m_System.setOutput(OUTPUTTYPE.NOSOUND));
			else {
				//Get driver capability
				int			MinFreq = 0, MaxFreq = 0;
				CAPS		Capability	= CAPS.NONE;
				SPEAKERMODE Speakermode = SPEAKERMODE.STEREO;
				CheckError(m_System.getDriverCaps(0, ref Capability, ref MinFreq, ref MaxFreq, ref Speakermode));

				//Set spearker mode according to the driver
				CheckError(m_System.setSpeakerMode(Speakermode));

				//Set buffer if not using hardware accceleration
				if ((Capability & CAPS.HARDWARE_EMULATED) == CAPS.HARDWARE_EMULATED)
					CheckError(m_System.setDSPBufferSize(Global.EMULATED_BUFFERSIZE, Global.EMULATED_BUFFERCOUNT));

				//Get user driver data
				GUID			Guid = new GUID();
				StringBuilder	DriverName = new StringBuilder(Global.MAX_DRIVERNAME);
				CheckError(m_System.getDriverInfo(0, DriverName, Global.MAX_DRIVERNAME, ref  Guid));

				//If driver is sigmatel
				if (DriverName.Equals(new StringBuilder((Global.SIGMATEL_DRIVERNAME)))) {
					//Fix crackling
					CheckError(m_System.setSoftwareFormat(
						Global.SIGMATEL_SAMPLERATE,
						SOUND_FORMAT.PCMFLOAT,
						0,
						0,
						DSP_RESAMPLER.LINEAR));
				}
			}
			#endregion
		}

		/// <summary>
		/// Check for any error when using FMOD procedures.
		/// </summary>
		/// <param name="result">The result of the FMOD operatio.</param>
		private void CheckError(RESULT result) {
			//Throw exception if it's not okay
			if (result != RESULT.OK) throw new System.Exception(Error.String(result));
		}

		/// <summary>
		/// Initialize the sound manager and FMOD engine.
		/// </summary>
		public void Initialize() {
			//Initialize FMOD
			RESULT Result = m_System.init(Global.MAX_SOUNDCHANNEL, INITFLAGS.NORMAL, (System.IntPtr) null);

			//If speaker mode is not supporterd
			if (Result == RESULT.ERR_OUTPUT_CREATEBUFFER) {
				//Reset to stereo
				CheckError(m_System.setSpeakerMode(SPEAKERMODE.STEREO));
				CheckError(m_System.init(Global.MAX_SOUNDCHANNEL, INITFLAGS.NORMAL, (System.IntPtr)null));
			}

			//Logging
			Global.Logger.AddLine("Sound manager initialized.");
		}

		/// <summary>
		/// Play a new BGM in the BGM channel.
		/// </summary>
		/// <param name="file">The file containing the BGM</param>
		public void PlayBGM(string file) {
			//If file doesn exist
			if (!FlatRedBall.IO.FileManager.FileExists(Global.BGM_FOLDER + file)) {
				//Log it then get out
				Global.Logger.AddLine(Global.NOFILE_ERROR + file);
				return;
			}

			//Stop bgm if it exist
			if (m_BGMChannel != null) CheckError(m_BGMChannel.stop());

			//Create and play bgm
			#region BGM Playing
			CheckError(m_System.createSound(Global.BGM_FOLDER + file, MODE.LOOP_NORMAL | MODE._2D | MODE.HARDWARE, ref m_BGM));
			CheckError(m_System.playSound(CHANNELINDEX.REUSE, m_BGM, true, ref m_BGMChannel));
			CheckError(m_BGMChannel.setPaused(false)); 
			#endregion

			//Logging info
			Global.Logger.AddLine("BGM file " + file + " is loaded and played.");
		}

		/// <summary>
		/// Stop/pause currently playing BGM.
		/// </summary>
		public void StopBGM() {
			//Pause BGM channel if exist
			if (m_BGMChannel != null) CheckError(m_BGMChannel.setPaused(true));
		}

		/// <summary>
		/// Updates sound manager.
		/// </summary>
		/// <param name="time">Data containing game time information.</param>
		public void Update(Microsoft.Xna.Framework.GameTime time) {
			//Updates FMOD
			CheckError(m_System.update());
		}
	}
}
