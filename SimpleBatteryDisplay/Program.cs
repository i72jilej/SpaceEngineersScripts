using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;
using System.Text.RegularExpressions;

namespace IngameScript{
	partial class Program : MyGridProgram{
		#region mdk preserve
		//=================
		//--Configuration--
		//=================

		//Name of the battery group to monitor
		const string batteriesGroupName = "Battery Group Name";
 
		//Add this label anywhere in the LCD panels you want to display results
		const string LCDLabel = "[SBD]";

		//=====================
		//--End Configuration--
		//=====================

		//DO NOT MODIFY ANYTHING BEYOND THIS POINT
		#endregion

		float _currCharge = 0;
		float _prevCharge = 0;

		MyIni _ini = new MyIni();

		public Program(){
			Runtime.UpdateFrequency = UpdateFrequency.Update100;
		}

		public void Save(){
		}

		public void Main(string argument, UpdateType updateSource){
			
			List<IMyBatteryBlock> batteriesList = new List<IMyBatteryBlock>();

			IMyBlockGroup batteriesGroup = GridTerminalSystem.GetBlockGroupWithName(batteriesGroupName);
			if (batteriesGroup == null) {
				Echo(string.Format("Batteries group \"{0}\" not found.", batteriesGroupName));
				Echo("Check the script code and modify the \"batteriesGroupName\" variable in the Configuration section");
				return;
			}
			batteriesGroup.GetBlocksOfType<IMyBatteryBlock>(batteriesList);

			for(int i = batteriesList.Count - 1; i >= 0; i--) {
				_currCharge += batteriesList[i].CurrentStoredPower;
			}
			_currCharge = _currCharge / batteriesList.Count;

			string status;
			if(_currCharge < _prevCharge) {
				status = "Discharging";
			}
			else if(_currCharge > _prevCharge) {
				status = "Recharging";
			}
			else {
				status = "Stable";
			}
			
			Echo(string.Format("Curr: {0}", _currCharge.ToString("P")));
			Echo(string.Format("Prev: {0}", _prevCharge.ToString("P")));
			Echo(string.Format("State: {0}", status));

			IMyTextSurface PB = Me.GetSurface(0);
			PB.ContentType = ContentType.TEXT_AND_IMAGE;
			PB.FontSize = 1.5f;
			PB.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;

			PB.WriteText(string.Format("{0}\n----\n{1}\n{2}", batteriesGroupName, _currCharge.ToString("P"), status));

			_prevCharge = _currCharge;
			_currCharge = 0;
		}
	}
}
