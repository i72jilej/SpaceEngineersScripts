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
        //BEGIN CONFIGURATION
        //You can modify these settings

            //Name of the battery group to monitor
        const string groupName = "Battery Group Name";

            //Add this label anywhere in the LCD panels you want to display results
        const string LCDLabel = "[SBD]";

            //Set to true if you want to use the programmable block as a display
        const bool useProgrammableBlockDisplay = true;

        //END CONFIGURATION
        //Beyond this point modify at your own risk 

        float prevCharge = 0;
        const string charging = ">>>";
        const string discharging = "<<<";
        const string noCharging = "===";
        private int chargeSpriteIndex = 0;

        public Program(){
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save(){
        }

        public void Main(string argument, UpdateType updateSource){
            //Looking for the batteries group
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
           
            //Looking fot the LCD panels
            List<IMyTerminalBlock> panels = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(LCDLabel, panels);
            Echo(string.Format("Found {0} LCD panels", panels.Count));
            Echo("-----");

            //Doing calculations
            float currCharge = 0;
            
            batteryList.ForEach(delegate(IMyBatteryBlock obj){ 
                currCharge += obj.CurrentStoredPower;
            });
            
            Echo(string.Format("Current group charge: {0}%", (currCharge / batteryList.Count).ToString("P")));
            if (currCharge > prevCharge) Echo("Batteries are recharging.");
            else if (currCharge < prevCharge) Echo("Batteries are draining");
            else Echo("Batteries are not charging");

            //Showing results
            //TODO Use LCDs

            if (useProgrammableBlockDisplay){
                IMyTextSurface PBSurface = Me.GetSurface(0);
                PBSurface.BackgroundColor = Color.Black;
                PBSurface.Alignment = TextAlignment.CENTER;

                string chargeText;
                if (currCharge > prevCharge)
                {
                    PBSurface.FontColor = Color.Green;
                    chargeText = charging;
                }
                else if (currCharge < prevCharge)
                {
                    PBSurface.FontColor = Color.Red;
                    chargeText = discharging;
                }
                else
                {
                    PBSurface.FontColor = Color.White;
                    chargeText = noCharging;
                }

                string text = groupName + "\n--------\n";
                text += (currCharge / batteryList.Count).ToString("P");
                text += "\n";
                text += chargeText;

                PBSurface.WriteText(text);
                prevCharge = currCharge;
            }

            /*
            panels.ForEach(delegate (IMyTextPanel obj)
            {
                obj.GetSurface(0);
            });
            */
        }
    }
}
