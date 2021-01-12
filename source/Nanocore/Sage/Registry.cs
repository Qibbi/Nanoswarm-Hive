using Nanocore.Native;

namespace Nanocore.Sage
{
    public class Registry
    {
        private const string _mainRegistryKey = "SOFTWARE\\Electronic Arts\\Electronic Arts\\Red Alert 3";
        private const string _secondaryRegistryKey = "SOFTWARE\\Electronic Arts\\Electronic Arts\\Red Alert 3";

        public string UserDataLeafName { get; } = "Red Alert 3";
        public string ScreenshotsFolderName { get; } = "Screenshots";
        public string ReplayFolderName { get; } = "Replays";
        public string ProfileFolderName { get; } = "Profiles";
        public string SaveFolderName { get; } = "SaveGames";
        public bool UseLocalUserMaps { get; } = true;
        public int MapPackVersion { get; } = 0x00010000;
        public int Version { get; } = 0x00010000;
        public uint Hash { get; } = 0x00000000u;
        public string Language { get; } = "english";
        public string BaseURL { get; } = "http://na.llnet.eadownloads.ea.com/u/f/eagames/redalert3/patches/";
        public string InstallPath { get; } = null;
        public string Readme { get; } = null;
        public string DisplayName { get; } = "Red Alert 3";
        public string Ergc { get; } = "unknown";

        public Registry()
        {
            if (RegistryClass.OpenKey(RegistryClass.HKEY_LOCAL_MACHINE, _mainRegistryKey, out HKey registryKey))
            {
                if (RegistryClass.GetStringW(registryKey, nameof(UserDataLeafName), out string str))
                {
                    UserDataLeafName = str;
                }
                if (RegistryClass.GetStringW(registryKey, nameof(ScreenshotsFolderName), out str))
                {
                    ScreenshotsFolderName = str;
                }
                if (RegistryClass.GetStringW(registryKey, nameof(ReplayFolderName), out str))
                {
                    ReplayFolderName = str;
                }
                if (RegistryClass.GetStringW(registryKey, nameof(ProfileFolderName), out str))
                {
                    ProfileFolderName = str;
                }
                if (RegistryClass.GetStringW(registryKey, nameof(SaveFolderName), out str))
                {
                    SaveFolderName = str;
                }
                if (RegistryClass.GetDWord(registryKey, nameof(UseLocalUserMaps), out int intValue))
                {
                    UseLocalUserMaps = intValue != 0 ? true : false;
                }
                if (RegistryClass.GetDWord(registryKey, nameof(MapPackVersion), out intValue))
                {
                    MapPackVersion = intValue;
                }
                if (RegistryClass.GetDWord(registryKey, nameof(Version), out intValue))
                {
                    Version = intValue;
                }
                if (RegistryClass.GetDWord(registryKey, nameof(Hash), out intValue))
                {
                    Hash = (uint)intValue;
                }
                if (RegistryClass.OpenKey(RegistryClass.HKEY_CURRENT_USER, _mainRegistryKey, out HKey userRegistryKey))
                {
                    if (RegistryClass.GetStringA(userRegistryKey, nameof(Language), out str))
                    {
                        Language = str;
                    }
                    else if (RegistryClass.GetStringA(registryKey, nameof(Language), out str))
                    {
                        Language = str;
                    }
                    AdvApi32.RegCloseKey(userRegistryKey);
                }
                if (RegistryClass.GetStringA(registryKey, nameof(BaseURL), out str))
                {
                    BaseURL = str;
                }
                if (RegistryClass.GetStringW(registryKey, "Install Dir", out str))
                {
                    InstallPath = str;
                }
                if (RegistryClass.GetStringW(registryKey, nameof(Readme), out str))
                {
                    Readme = str;
                }
                if (RegistryClass.OpenKey(registryKey, "ergc", out HKey ergcKey))
                {
                    if (RegistryClass.GetStringA(ergcKey, string.Empty, out str))
                    {
                        Ergc = str;
                    }
                    AdvApi32.RegCloseKey(ergcKey);
                }
                AdvApi32.RegCloseKey(registryKey);
            }
            if (RegistryClass.OpenKey(RegistryClass.HKEY_LOCAL_MACHINE, _secondaryRegistryKey, out HKey keyDisplayName))
            {
                if (RegistryClass.GetStringW(registryKey, nameof(DisplayName), out string str))
                {
                    DisplayName = str;
                }
                AdvApi32.RegCloseKey(keyDisplayName);
            }
        }
    }
}
