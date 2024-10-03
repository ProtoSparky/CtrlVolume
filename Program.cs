using System;
using System.Collections.Generic;
using NAudio.CoreAudioApi;
using System.IO;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Threading; 
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
                Console.WriteLine("File does not exist. Starting setup!");                
                generic.AskUserForDevice();                
            }
            var Updated_Settings = FileOps.ReadFile<Settings>(DefaultSettingsPath);
            if(Updated_Settings.CtrlFriendlyName == "" || Updated_Settings.CtrlFriendlyName == " ")
            {
                //checks if file is invalid and then starts the setup again.
                generic.AskUserForDevice();
            }

            var vol_monitor = new Monitor();
            vol_monitor.VolumeChange(); 


        }
    }
    public class GetDevices
    {

        public void All(out List<string> AllAudioDevices)
        {
            //gets all devices
            List<string> AudioDevices = new List<string>();
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            foreach (var device in devices)
            {
                AudioDevices.Add(device.ToString());
            }
            AllAudioDevices = AudioDevices;

        }
        public void Default(out string DefaultAudioDevice)
        {
            //gets the default windows device
            var enumerator = new MMDeviceEnumerator();
            var DefaultAudio = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            DefaultAudioDevice = DefaultAudio.ToString();
        }
    }
    public class Volume
    {
        public void GetVolume(string DeviceFriendlyName, out float Volume)
        {
            Volume = 0;
            var enumerator = new MMDeviceEnumerator();
            var allDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
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
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
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
    }
    public class FileOperations
    {
        public bool CheckFile(string FilePath)
        {
            return File.Exists(FilePath);
        }
        public void WriteFile(string FilePath, object Data)
        {
            string JsonString = JsonSerializer.Serialize(Data);
            File.WriteAllText(FilePath, JsonString);
        }
        public T ReadFile<T>(string FilePath)
        {
            string JsonString = File.ReadAllText(FilePath);
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
            Console.WriteLine("Type the number of the device you want to select for CtrlVolume");
            Console.WriteLine("----------------------------------------------------------------");
            int pointer = 0;
            foreach (var SelectOutputDevice in AllAudioDevices)
            {
                Console.WriteLine(pointer.ToString() + " | " + SelectOutputDevice);
                pointer++; 
            }
            Console.WriteLine("Enter number (0-" + AllAudioDevices.Count.ToString() + ")"); 
            string UserInput = Console.ReadLine();
            int failed_pointer = 0;
            for (int p2 = 0; p2 < pointer; p2++)
            {
                if(UserInput == p2.ToString())
                {
                    //input valid
                    
                    var FileOps = new FileOperations();
                    Settings settings = new Settings
                    {
                        CtrlFriendlyName = AllAudioDevices[Convert.ToInt32(UserInput)]
                    };
                    var generic = new Generic();
                    generic.SettingsPath(out string DefaultSettingsPath); //get path for settings file
                    FileOps.WriteFile(DefaultSettingsPath, settings);                   

                }
                else
                {
                    failed_pointer++; 
                }
            }
            if(failed_pointer == pointer)
            {
                //input not valid
                Console.Clear();
                Console.WriteLine("input Wrong!" + "\nValid Inputs are: 0-" + pointer);
                AskUserForDevice();
            }

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

            while (true)
            {
                volume.GetVolume(DefaultAudioDevice, out float CurrentDefaultDeviceVolume);
                if ((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0)
                {
                    if (PreviousDefaultDeviceVolume < CurrentDefaultDeviceVolume)
                    {
                        int VolumeChange = (int)(Math.Round(PreviousDefaultDeviceVolume, 0) - Math.Round(CurrentDefaultDeviceVolume, 0));
                        VolumeChange = VolumeChange * -1;
                        volume.WriteVolume(DefaultAudioDevice, CurrentDefaultDeviceVolume - VolumeChange); //freeze default audio device
                        volume.GetVolume(SelectedAudioDevice, out float SelectedAudioDeviceVolume); //get volume of selected device
                        int SelectedVolume = (int)Math.Round(SelectedAudioDeviceVolume, 0) + VolumeChange; //convert to int and apply volume change
                        SelectedVolume = GenericOPs.Clamp(SelectedVolume, 0, 100);
                        volume.WriteVolume(SelectedAudioDevice, SelectedVolume);
                        //Console.WriteLine("Increase" + VolumeChange.ToString());
                    }
                    else if (PreviousDefaultDeviceVolume > CurrentDefaultDeviceVolume)
                    {
                        int VolumeChange = (int)(Math.Round(PreviousDefaultDeviceVolume, 0) - Math.Round(CurrentDefaultDeviceVolume, 0));
                        VolumeChange = VolumeChange * -1;
                        volume.WriteVolume(DefaultAudioDevice, CurrentDefaultDeviceVolume - VolumeChange); //freeze default audio device
                        volume.GetVolume(SelectedAudioDevice, out float SelectedAudioDeviceVolume); //get volume of selected device
                        int SelectedVolume = (int)Math.Round(SelectedAudioDeviceVolume, 0) + VolumeChange; //convert to int and apply volume change
                        SelectedVolume = GenericOPs.Clamp(SelectedVolume, 0, 100);
                        volume.WriteVolume(SelectedAudioDevice, SelectedVolume);
                        //Console.WriteLine("Decrease" + VolumeChange.ToString());
                    }

                }

                Thread.Sleep(50);
            }
        }
    }


}
