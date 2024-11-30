using System;
using System.Net.Http;
using System.Windows.Forms;

namespace Star_Wars_D6
{
    public class UpdateChecker
    {
        // Define the current version here
        private const string CurrentVersion = "0.9.0.5"; // Change this whenever you update the program version
        private const string VersionUrl = "https://raw.githubusercontent.com/Frostipanda/Holocron-Designer/master/Star%20Wars%20D6/Resources/version.txt";
        private const string GitHubUrl = "https://github.com/Frostipanda/Holocron-Designer";

        public static async void CheckForUpdates()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Fetch the latest version from GitHub
                    string latestVersion = await client.GetStringAsync(VersionUrl);

                    // Compare versions
                    if (IsNewerVersion(latestVersion.Trim(), CurrentVersion))
                    {
                        // Prompt the user
                        DialogResult result = MessageBox.Show(
                            "A new update is available. Would you like to visit the download page?",
                            "Update Available",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = GitHubUrl,
                                UseShellExecute = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to check for updates: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool IsNewerVersion(string latestVersion, string currentVersion)
        {
            try
            {
                Version latest = new Version(latestVersion);
                Version current = new Version(currentVersion);

                return latest > current;
            }
            catch
            {
                // If parsing fails, assume no update
                return false;
            }
        }
    }
}
