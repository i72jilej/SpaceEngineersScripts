using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
	partial class Program : MyGridProgram {

		#region mdk preserve
		//=================
		//--Configuration--
		//=================

		//Add the containers you want to monitor to a group and set it to the containersGroupName variable
		string containersGroupName = "Container Group";

		//Add the next label to the LCDs name where you want to show the script output.
		//If you are monitoring more than one set of containers with different scripts, set a different label for each one
		const string LCDLabel = "[SISD]";

		//=====================
		//--End Configuration--
		//=====================

		//DO NOT MODIFY ANYTHING BEYOND THIS POINT
		#endregion

		public Program() {
			Runtime.UpdateFrequency = UpdateFrequency.Update100;
			if (string.IsNullOrEmpty(Me.CustomData)) {
				Me.CustomData = "group=Group Name\n";
			}
		}

		public void Save() {}

		public void Main(string argument, UpdateType updateSource) {

			//Generic list for retrieving first the containers and then the LCDs
			List<IMyTerminalBlock> blockList = new List<IMyTerminalBlock>();

			//1 Look for the containers
			//2 Calculate max capacity
			//3 Calculate current capacity
			float maxVolume = 0;
			float currVolume = 0;

			IMyBlockGroup containersGroup = GridTerminalSystem.GetBlockGroupWithName(containersGroupName);
			if (containersGroup == null) {
				Echo(string.Format("Containers group \"{0}\" not found.", containersGroupName));
				Echo("Check the script code and modify the \"containersGroupName\" variable in the Configuration section");
				return;
			}

			containersGroup.GetBlocksOfType<IMyTerminalBlock>(blockList);
			int actualCount = blockList.Count();

			for (int i = blockList.Count - 1; i >= 0 ; i--) {
				if(blockList[i].GetInventory() != null) {
					maxVolume += (float)blockList[i].GetInventory().MaxVolume;
					currVolume += (float)blockList[i].GetInventory().CurrentVolume;
				}
				else {
					actualCount--;
				}
			}

			Echo(string.Format("Found \"{0}\" containers group with {1} inventories", containersGroupName, actualCount));
			if (actualCount != blockList.Count) {
				Echo("WARNING: Looks like there are items without an inventory in the group.");
			}
			Echo("-----");

			//4 Look for the LCDs
			blockList.Clear();
			GridTerminalSystem.GetBlocks(blockList);
			//Echo("DEBUG: All?: " + blockList.Count);
			for(int i = blockList.Count -1; i>= 0; i--) {
				if (blockList[i].CustomName.Contains(LCDLabel)) {
					//Echo("DEBUG: Detected " + blockList[i].CustomName);
					//((IMyTextSurface) blockList[i])
				}
			}
			IMyTextSurface PB = Me.GetSurface(0);
			PB.ContentType = ContentType.TEXT_AND_IMAGE;
			PB.FontSize = 1.5f;
			PB.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
			
			//5 Calculate output
			float percentage = 0;
			if(maxVolume != 0) {
				percentage = (currVolume * 100) / maxVolume;
			}

			//6 Print output
			Echo(string.Format("Maximum Volume: {0} kl", (float) Math.Round((double) maxVolume, 2)));
			Echo(string.Format("Used Volume: {0} kl", (float)Math.Round((double) currVolume, 2)));
			Echo(string.Format("% of used Volume: {0}%", (float)Math.Round((double) percentage, 2)));

			string text = string.Format("{0}\n----\nMaximum Volume:\n{1} kl\n\nUsed Volume:\n{2} kl\n\n% of used Volume:\n{3}%", containersGroup, (float)Math.Round((double)maxVolume, 2), (float)Math.Round((double)currVolume, 2), (float)Math.Round((double)percentage, 2));
			PB.WriteText(text);
		}
	}
}
