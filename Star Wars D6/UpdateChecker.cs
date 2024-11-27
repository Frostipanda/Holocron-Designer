using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Star_Wars_D6
{
    public class UpdateChecker
    {
        private const string VersionUrl = "https://raw.githubusercontent.com/Frostipanda/Holocron-Designer/master/version.txt";
        private const string GitHubUrl = "https://github.com/Frostipanda/Holocron-Designer/tree/master";

        public static async void CheckForUpdates()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Fetch the latest version from GitHub
                    string latestVersion = await client.GetStringAsync(VersionUrl);

                    // Get the current version of the program
                    string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    // Compare versions
                    if (IsNewerVersion(latestVersion.Trim(), currentVersion))
                    {
                        // Prompt the user
                        DialogResult result = MessageBox.Show(
                            "A new update is available. Would you like to visit the download page?",
                            "Update Available",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(GitHubUrl);
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
