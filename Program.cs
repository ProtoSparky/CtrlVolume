using System;
using System.Collections.Generic;
using NAudio.CoreAudioApi;
using System.IO;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;


static class Globals
{
    public static bool FailedSetup = false; //if the setup fails, this gets flagged as true
    public static bool NeedsSetup = false;
}
namespace CtrlVolume
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*check if need to run setup*/
            var generic = new Generic();
            generic.SettingsPath(out string DefaultSettingsPath);
            
            var FileOps = new FileOperations();            
            if (!FileOps.CheckFile(DefaultSettingsPath))
            {
                Globals.NeedsSetup = true; 
                generic.AskUserForDevice();
            }
            else if(FileOps.ReadFile<Settings>(DefaultSettingsPath).CtrlFriendlyName == "" || FileOps.ReadFile<Settings>(DefaultSettingsPath).CtrlFriendlyName == " ")
            {
                //checks if file is invalid and then starts the setup again.
                Globals.NeedsSetup = true;
                generic.AskUserForDevice();
            }

            if (!Globals.NeedsSetup)
            {
                //only start the volume monitor if there are no erros in the setup
                Thread NotificationThread = new Thread(generic.SpawnNotification);
                //start volume monitor and notification in different threads so both can run simultaniously
                var vol_monitor = new Monitor();
                Thread Monitor = new Thread(vol_monitor.VolumeChange);
                NotificationThread.Start();
                Monitor.Start();

            }
            else
            {
                MessageBox.Show("The setup was not completed.\nCtrlVolume will not work without being set up.\nThe setup will start the next time you launch the program.");
            }

        }
    }
    public class GetDevices
    {
        private MMDeviceEnumerator _enumerator;
        public GetDevices()
        {
            _enumerator = new MMDeviceEnumerator(); //make an enumerator all methods can use in GetDevices so i wouldnt have to define them for every method || var enumerator = new MMDeviceEnumerator(); removed from all methods
        }

        public void All(out List<string> AllAudioDevices)
        {
            //gets all devices
            List<string> AudioDevices = new List<string>();
            var devices = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            foreach (var device in devices)
            {
                AudioDevices.Add(device.ToString());
            }
            AllAudioDevices = AudioDevices;
        }
        public void Default(out string DefaultAudioDevice)
        {
            //gets the default windows device
            var DefaultAudio = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            DefaultAudioDevice = DefaultAudio.ToString();
        }
    }
    public class Volume
    {
        private MMDeviceEnumerator _enumerator; 
        public Volume()
        {
            _enumerator = new MMDeviceEnumerator(); //make an enumerator all methods can use in Volume so i wouldnt have to define them for every method || var enumerator = new MMDeviceEnumerator(); removed from all methods
        }
        public void GetVolume(string DeviceFriendlyName, out float Volume)
        {
            Volume = -1;
            var allDevices = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            // Find the device by friendly name
            foreach (var device in allDevices)
            {
                if (device.FriendlyName == DeviceFriendlyName)
                {   // Get the audio session manager and master volume
                    var audioMeter = device.AudioEndpointVolume;
                    Volume = (audioMeter.MasterVolumeLevelScalar * 100);
                    break;
                }
            }
        }
        public void WriteVolume(string DeviceFriendlyName, float Volume)
        {
            var devices = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            foreach (var device in devices)
            {
                if (device.FriendlyName == DeviceFriendlyName)
                {
                    var volume = device.AudioEndpointVolume;
                    volume.MasterVolumeLevelScalar = Volume / 100.0f;
                    break;
                }
            }
        }
        public void MuteUnmuteDevice(string DeviceFriendlyName, bool Mute)
        {
            //true to mute, false to unmute
            var devices = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            foreach (var device in devices)
            {
                if (device.FriendlyName == DeviceFriendlyName)
                {
                    var volume = device.AudioEndpointVolume;
                    volume.Mute = Mute;
                    break;
                }
            }
        }
    }
    public class FileOperations
    {
        public bool CheckFile(string FilePath)
        {
            return File.Exists(FilePath); //checks if the file/directory exists
        }
        public void WriteFile(string FilePath, object Data)
        {
            string JsonString = JsonSerializer.Serialize(Data);
            File.WriteAllText(FilePath, JsonString);
        }
        public T ReadFile<T>(string FilePath)
        {
            string JsonString = File.ReadAllText(FilePath);
            if(JsonString == "")
            {
                JsonString = "{\"CtrlFriendlyName\":\"\"}";
            }
            return JsonSerializer.Deserialize<T>(JsonString);
        }
    }
    public class Settings
    {
        public string CtrlFriendlyName { get; set; }
    }
    public class Generic
    {
        public void AskUserForDevice()
        {
            var devices = new GetDevices();
            devices.All(out List<string> AllAudioDevices);
            devices.Default(out string DefaultAudioDevice);
            //subtract default device
            AllAudioDevices.Remove(DefaultAudioDevice);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SetupForm(AllAudioDevices));

        }
        public void SettingsPath(out string DefaultSettingsPath)
        {
            DefaultSettingsPath = "./CtrlVolume_settings.json";
        }
        public T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
        public void SpawnNotification()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            NotifyIcon notifyIcon = new NotifyIcon();//new notification icon
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); //use the same icon as the application uses but now for the notification icon
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Stop CtrlVolume", null, (sender, e) =>
            {
                // Clean up and close the application.
                notifyIcon.Dispose();
                Application.Exit();
                Environment.Exit(0);
            });
            notifyIcon.ContextMenuStrip.Items.Add("Open Settings file", null, (sender, e) =>
            {
                SettingsPath(out string DefaultSettingsPath);
                Process.Start(@"" + Environment.CurrentDirectory + "\\" + DefaultSettingsPath.Remove(0, 1));
            });
            notifyIcon.ContextMenuStrip.Items.Add("Made with ❤ by Sparky", null, (sender,e)=>
            {
                Process.Start("https://protosparky.uk"); //open url in browser
            });
            notifyIcon.Text = "CtrlVolume is active";
            notifyIcon.Visible = true;
            Application.Run();
        }
    }
    
    public class Monitor
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        // Define the virtual key code for the Ctrl key
        private const int VK_CONTROL = 0x11;

        public void VolumeChange()
        {
            var volume = new Volume();
            var AudioDevices = new GetDevices();
            var FileOPs = new FileOperations();
            var GenericOPs = new Generic();
            GenericOPs.SettingsPath(out string DefaultSettingsPath); 
            AudioDevices.Default(out string DefaultAudioDevice);
            volume.GetVolume(DefaultAudioDevice, out float PreviousDefaultDeviceVolume);
            string SelectedAudioDevice = FileOPs.ReadFile<Settings>(DefaultSettingsPath).CtrlFriendlyName;

            bool ctrlPressed = false;
            bool deviceExists = true; 
            while (true)
            {                
                if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0)
                {
                    if (!ctrlPressed)
                    {
                        ctrlPressed = true;
                        volume.GetVolume(DefaultAudioDevice, out PreviousDefaultDeviceVolume);  //update previous volume if ctrl key is pressed
                        if(PreviousDefaultDeviceVolume == -1) { deviceExists = false; }//set device flag as not false if any of the audio output devices cannot be found
                    }

                    volume.GetVolume(DefaultAudioDevice, out float CurrentDefaultDeviceVolume); //get the volume of the default audio device // used to check volume change and apply that to the select device
                    if (CurrentDefaultDeviceVolume == -1) { deviceExists = false; }//set device flag as not false if any of the audio output devices cannot be found

                    if (PreviousDefaultDeviceVolume < CurrentDefaultDeviceVolume)
                    {
                        int VolumeChange = (int)(Math.Round(PreviousDefaultDeviceVolume, 0) - Math.Round(CurrentDefaultDeviceVolume, 0));
                        VolumeChange = VolumeChange * -1;
                        if (deviceExists) { volume.WriteVolume(DefaultAudioDevice, CurrentDefaultDeviceVolume - VolumeChange); }//freeze default audio device if it exists
                        volume.GetVolume(SelectedAudioDevice, out float SelectedAudioDeviceVolume); //get volume of selected device
                        if(SelectedAudioDeviceVolume == -1) { deviceExists = false; }//set device flag as not false if any of the audio output devices cannot be found
                        int SelectedVolume = (int)Math.Round(SelectedAudioDeviceVolume, 0) + VolumeChange; //convert to int and apply volume change
                        SelectedVolume = GenericOPs.Clamp(SelectedVolume, 0, 100);
                        if (deviceExists) { volume.WriteVolume(SelectedAudioDevice, SelectedVolume); } //write new volume to the selected audio device
                    }
                    else if (PreviousDefaultDeviceVolume > CurrentDefaultDeviceVolume)
                    {
                        int VolumeChange = (int)(Math.Round(PreviousDefaultDeviceVolume, 0) - Math.Round(CurrentDefaultDeviceVolume, 0));
                        VolumeChange = VolumeChange * -1;

                        if (CurrentDefaultDeviceVolume == 0) {
                            //IF the volume reaches 0%, windows will try to mute the default audio device. The problem is that once the value to cancel the volume adjustment is written to the default device
                            // it'll stay muted even though the value is not 0% anymore.
                            //This unmutes the device if it ever reaches 0% during the volume adjustment
                            //Multiple unmute commands are written to be EXTRA SURE that the device is unmuted again (unmute is sticky and doenst always trigger in windows)
                            //I fucking hate that i had to do this but here we are
                            volume.MuteUnmuteDevice(DefaultAudioDevice, false);
                            volume.MuteUnmuteDevice(DefaultAudioDevice, false);
                            volume.MuteUnmuteDevice(DefaultAudioDevice, false);
                        }

                        if (deviceExists) { volume.WriteVolume(DefaultAudioDevice, CurrentDefaultDeviceVolume - VolumeChange); }//freeze default audio device if it exists
                        volume.GetVolume(SelectedAudioDevice, out float SelectedAudioDeviceVolume); //get volume of selected device
                        if(SelectedAudioDeviceVolume == -1) { deviceExists = false; }//set device flag as not false if any of the audio output devices cannot be found
                        int SelectedVolume = (int)Math.Round(SelectedAudioDeviceVolume, 0) + VolumeChange; //convert to int and apply volume change
                        SelectedVolume = GenericOPs.Clamp(SelectedVolume, 0, 100);
                        if (deviceExists) { volume.WriteVolume(SelectedAudioDevice, SelectedVolume); }//write new volume to the selected audio device
                    }
                    if(deviceExists == false)
                    {
                        Console.WriteLine("One or more of the configured audio devices do not exist. Skipping volume adjustment!"); 
                    }
                    deviceExists = true; 
                }
                else
                {
                    ctrlPressed = false; 
                }
                Thread.Sleep(50);
            }
        }
    }


}
