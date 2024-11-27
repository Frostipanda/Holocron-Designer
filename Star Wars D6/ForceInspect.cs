using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Star_Wars_D6
{
    public partial class ForceInspect : Form
    {
        public ForceInspect()
        {
            InitializeComponent();
        }

        public void LoadForcePower(string powerName)
        {
            // Path to the JSON file
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "force_powers.json");

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Force powers data file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Read the JSON file
                string jsonContent = File.ReadAllText(filePath);

                // Parse the JSON and navigate to the 'items' array
                JObject rootObject = JObject.Parse(jsonContent);
                JArray powersArray = (JArray)rootObject["items"];

                if (powersArray == null)
                {
                    MessageBox.Show("Force powers array not found in the JSON file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Find the selected power
                var power = powersArray.FirstOrDefault(p => p["name"]?.ToString() == powerName);

                if (power != null)
                {
                    string name = power["name"]?.ToString() ?? "Unnamed Power";
                    string description = power["system"]?["description"]?.ToString() ?? "No description available.";

                    // Set the name and description in the form
                    forcePower.Text = name;
                    descriptionWebBrowser.DocumentText = $"<html><body>{description}</body></html>";
                }
                else
                {
                    MessageBox.Show("Force power not found in the data file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading force power: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
