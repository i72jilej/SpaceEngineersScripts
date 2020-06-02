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
		//BEGIN CONFIGURATION
		//You can modify these settings

		//Name of the battery group to monitor
		const string groupName = "Battery Group Name";
 
		//Add this label anywhere in the LCD panels you want to display results
		const string LCDLabel = "[SBD]";

		//Set to true if you want to use the programmable block as a display
		const bool useProgrammableBlockDisplay = true;

		//END CONFIGURATION
		//Beyond this point, modify at your own risk.
		#endregion

		float currCharge = 0;
		float prevCharge = 0;

		public Program(){
			Runtime.UpdateFrequency = UpdateFrequency.Update100;
		}

		public void Save(){
		}

		public void Main(string argument, UpdateType updateSource){
			//Looking for the batteries groups
			IMyBlockGroup batteryGroup = GridTerminalSystem.GetBlockGroupWithName(groupName);
			if (batteryGroup == null){
				Echo(string.Format("Battery group \"{0}\" not found.", groupName));
				Echo("Check the script code and modify the \"groupName\" variable in the CONFIGURATION section");
				return;
			}
			List<IMyBatteryBlock> batteryList = new List<IMyBatteryBlock>();
			batteryGroup.GetBlocksOfType<IMyBatteryBlock>(batteryList);
			Echo(string.Format("Found \"{0}\" battery group with {1} batteries", groupName, batteryList.Count));
			Echo("-----");
		   
			//Looking for the LCD panels
			List<IMyTerminalBlock> panels = new List<IMyTerminalBlock>();
			GridTerminalSystem.SearchBlocksOfName(LCDLabel, panels, panel => panel is IMyTextPanel);
			Echo(string.Format("Found {0} LCD panels", panels.Count));
			Echo("-----");

			//Doing calculations
			currCharge = 0;
			batteryList.ForEach(delegate(IMyBatteryBlock obj){ 
				currCharge += obj.CurrentStoredPower;
			});
			currCharge = currCharge / batteryList.Count;
			
			Echo(string.Format("Current group charge: {0}", currCharge.ToString("P")));
			if (currCharge > prevCharge)
				Echo("Batteries are recharging.");
			else if (currCharge < prevCharge)
				Echo("Batteries are draining");
			else
				Echo("Batteries are not charging");

			//Displaying info

			if (useProgrammableBlockDisplay){
				IMyTextSurface PBSurface = Me.GetSurface(0);
				drawSurface(ref PBSurface);
			}

			panels.ForEach(delegate (IMyTerminalBlock obj){
				IMyTextSurface LCDSurface = (IMyTextSurface)obj;
				drawSurface(ref LCDSurface);
			});

			prevCharge = currCharge;
		}

		void drawSurface(ref IMyTextSurface surf){
			//Draws the information on a text surface

			string chargeText;

			surf.BackgroundColor = new Color(0, 88, 151, 255);
			surf.Alignment = TextAlignment.CENTER;
			surf.FontColor = new Color(179, 237, 255, 255);

			if (currCharge > prevCharge){
				chargeText = "+Recharging+";
			}
			else if (currCharge < prevCharge){
				chargeText = "-Discharging-";
			}
			else{
				chargeText = "=Static=";
			}

			string text = groupName + "\n--------\n";
			text += currCharge.ToString("P");
			text += "\n";
			text += chargeText;

			surf.WriteText(text);
		}
	}
}
