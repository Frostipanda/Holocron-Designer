using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Star_Wars_D6
{
    public partial class Learning : Form
    {
        public Learning()
        {
            InitializeComponent();



            ConfigureLearningGrid();
            ConfigureLearningGridContextMenu();



            // Load the force powers data
            LoadForcePowers();
        }

        private void LoadForcePowers()
        {
            // Path to the JSON file
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "force_powers.json");

            // Check if the file exists
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

                // Create a list to store the names
                List<string> forcePowers = new List<string>();

                // Extract names from JSON
                foreach (var power in powersArray)
                {
                    string name = power["name"]?.ToString() ?? "Unnamed Power";
                    forcePowers.Add(name);
                }

                // Sort the names alphabetically
                forcePowers.Sort();

                // Populate the DataGridView with the sorted names
                learningGrid.Rows.Clear();
                foreach (string name in forcePowers)
                {
                    learningGrid.Rows.Add(name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading force powers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ConfigureLearningGrid()
        {
            learningGrid.Columns.Clear();

            // Add a single column for displaying the Force Power names
            learningGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ForcePowerName",
                HeaderText = "Force Power Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // Make the DataGridView read-only
            learningGrid.ReadOnly = true;

            // Disable adding and deleting rows by the user
            learningGrid.AllowUserToAddRows = false;
            learningGrid.AllowUserToDeleteRows = false;

            // Enable full-row selection
            learningGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }



        private ContextMenuStrip learningGridContextMenu;

        private void ConfigureLearningGridContextMenu()
        {
            learningGridContextMenu = new ContextMenuStrip();

            // Add "Learn Force Power" menu item
            var learnForcePowerMenuItem = new ToolStripMenuItem("Learn Force Power");
            learnForcePowerMenuItem.Click += LearnForcePowerMenuItem_Click;
            learningGridContextMenu.Items.Add(learnForcePowerMenuItem);

            // Add "Inspect" menu item
            var inspectMenuItem = new ToolStripMenuItem("Inspect");
            inspectMenuItem.Click += InspectMenuItem_Click;
            learningGridContextMenu.Items.Add(inspectMenuItem);

            // Attach the context menu to the DataGridView
            learningGrid.ContextMenuStrip = learningGridContextMenu;
        }

        private void LearnForcePowerMenuItem_Click(object sender, EventArgs e)
        {
            if (learningGrid.SelectedRows.Count > 0)
            {
                string selectedPower = learningGrid.SelectedRows[0].Cells[0].Value?.ToString();

                if (!string.IsNullOrEmpty(selectedPower))
                {
                    // Get the reference to Form1
                    var form1 = (Form1)Application.OpenForms["Form1"];

                    if (form1 != null)
                    {
                        // Add a new Force Power to the Panel on Form1
                        form1.AddForcePowerToPanel(selectedPower);
                    }
                }
            }
        }


        private void InspectMenuItem_Click(object sender, EventArgs e)
        {
            if (learningGrid.SelectedRows.Count > 0)
            {
                string selectedPower = learningGrid.SelectedRows[0].Cells[0].Value?.ToString();

                if (!string.IsNullOrEmpty(selectedPower))
                {
                    // Open the ForceInspect form
                    var inspectForm = new ForceInspect();
                    inspectForm.LoadForcePower(selectedPower);
                    inspectForm.Show();
                }
            }
        }



    }
}
