using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AemulusModManager.Utilities
{
    public class Platform
    {
        public static void OpenUrl(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                // .NET Core defaults UseShellExecute to false, causing a runtime exception when opening urls on Windows.
                // Manually setting UseShellExecute to true fixes this.
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
        }

        public static void PlayNotificationSound()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                bool found = false;
                try
                {
                    using Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"AppEvents\Schemes\Apps\.Default\Notification.Default\.Current");
                    if (key != null)
                    {
                        Object o = key.GetValue(null); // pass null to get (Default)
                        if (o != null)
                        {
                            System.Media.SoundPlayer theSound = new System.Media.SoundPlayer((String)o);
                            theSound.Play();
                            found = true;
                        }
                    }
                }
                catch
                { }
                if (!found)
                    System.Media.SystemSounds.Beep.Play(); // consolation prize
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string DesktopEnvironment = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP");
                if (DesktopEnvironment.Contains("KDE")) // TODO: Detect KDE notification sound instead of hardcoding it
                {
                    Process.Start("paplay", "/usr/share/sounds/Oxygen-Sys-App-Message.ogg");
                }
                else
                {
                    Process.Start("canberra-gtk-play", "--id=dialog-question");
                }

            }

        }

        public static bool InstallGBHandler()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string AppPath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".exe");
                Console.WriteLine(Assembly.GetExecutingAssembly().Location);
                string protocolName = $"Aemulus";
                try
                {
                    var reg = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Classes\aemulus");
                    reg.SetValue("", $"URL:{protocolName}");
                    reg.SetValue("URL Protocol", "");
                    reg = reg.CreateSubKey(@"shell\open\command");
                    reg.SetValue("", $"\"{AppPath}\" \"%1\"");
                    reg.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    string assemblyPath = Assembly.GetExecutingAssembly().Location;
                    string appPath = assemblyPath.Remove(assemblyPath.Length - 4);
                    string localConfigDir = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                    string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string mimeDir;

                    // Detect Linux mime config dir
                    if (!string.IsNullOrWhiteSpace(localConfigDir))
                    {
                        mimeDir = Path.Join(localConfigDir, "applications/");
                    }
                    else
                    {
                        mimeDir = Path.Join(homeDir, ".local/share/applications/");
                    }
                    System.IO.Directory.CreateDirectory(mimeDir);

                    // Create URL Handler mime type
                    string[] desktopEntry =
                    {
                    "[Desktop Entry]",
                    "Type=Application",
                    "Name=Aemulus 1-Click Scheme Handler",
                    "Exec=" + appPath + " %u",
                    "StartupNotify=false",
                    "MimeType=x-scheme-handler/aemulus;"
                    };
                    File.WriteAllLines(Path.Join(mimeDir, "aemulus-1click.desktop"), desktopEntry);

                    // Register URL handler
                    Process.Start("xdg-mime", "default aemulus-1click.desktop x-scheme-handler/aemulus");
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            else // Running on unsupported operating system
            {
                return false;
            }
        }
    }

}
