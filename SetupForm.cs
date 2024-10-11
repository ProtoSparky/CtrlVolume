using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CtrlVolume
{
    public partial class SetupForm : Form
    {
        public SetupForm(List <string>SecondaryDevices)
        {
            InitializeComponent(); //initialize the form

            
            foreach (var Device in SecondaryDevices)
            {
                deviceDropdown.Items.Add(Device); 
            }

        }

        private void SetupFormValidate(object sender, EventArgs e)
        {
            if(deviceDropdown.SelectedIndex != -1)
            {
                //device actually selected and ready to be saved
                deviceDropDownSave.Enabled = true; 
            }
        }

        private void SetupFormSaveDevice(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Saving");
            Console.WriteLine(deviceDropdown.SelectedItem);
            var GenericOps = new Generic();
            var FileOps = new FileOperations();

            
            Settings settings = new Settings
            {
                CtrlFriendlyName = deviceDropdown.SelectedItem.ToString()
            };
            GenericOps.SettingsPath(out string DefaultSettingsPath); //get path for settings file            
            FileOps.WriteFile(DefaultSettingsPath, settings); //Write to settings file 

            //Check if there is any data in the file
            if (!FileOps.CheckFile(DefaultSettingsPath))
            {
                //file does not exist
                Globals.FailedSetup = true;
                errorMessage.Text = "Error: Failed to write to the config file '" + DefaultSettingsPath + "' at the same location as this executable!.\nThe setup cannot continue!\n Setup will start every time if it cannot access the setting file";
            }
            var WrittenContents = FileOps.ReadFile<Settings>(DefaultSettingsPath);
            if (WrittenContents.CtrlFriendlyName == "" || WrittenContents.CtrlFriendlyName == " ")
            {
                //contents do not exist
                Globals.FailedSetup = true;
                errorMessage.Text = "Error: Successfully wrote the config file, but contents seem empty!\nThe setup cannot continue! \nVerify that the program has the necesary permissions to run and access the file '" + DefaultSettingsPath + "' in the same location as this executable. \nSetup will start every time if it cannot access the setting file";
            }

            if (!Globals.FailedSetup)
            {
                Globals.NeedsSetup = false; //mark setup done so monitor could start
                Close(); //close if there are no problems
            }

        }
    }
}
