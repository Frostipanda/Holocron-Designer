using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Star_Wars_D6
{
    public static class VehicleIO
    {
        public static void SaveVehicle(VehicleControl vehicleControl)
        {
            try
            {
                // Create a JSON object to store vehicle data
                var vehicleData = new JObject
                {
                    ["craft"] = vehicleControl.craftBox.Text,
                    ["type"] = vehicleControl.typeBox.Text,
                    ["scale"] = vehicleControl.scaleBox.Text,
                    ["length"] = vehicleControl.lengthBox.Text,
                    ["pilotSkill"] = vehicleControl.pilotSkill.Text,
                    ["crewSize"] = vehicleControl.crewSize.Text,
                    ["crewSkill"] = vehicleControl.crewSkill.Text,
                    ["passengers"] = vehicleControl.passBox.Text,
                    ["cargoCapacity"] = vehicleControl.cargoCap.Text,
                    ["consumables"] = vehicleControl.conBox.Text,
                    ["cost"] = vehicleControl.costBox.Text,
                    ["hull"] = vehicleControl.hullMod.Text,
                    ["shields"] = vehicleControl.shieldBox.Text,
                    ["hyperdrive"] = vehicleControl.hyperBox.Text,
                    ["backupHyperdrive"] = vehicleControl.backHyper.Text,
                    ["navComputer"] = vehicleControl.navComp.SelectedItem?.ToString() ?? "No",
                    ["maneuverability"] = vehicleControl.manBox.Text,
                    ["spaceMove"] = vehicleControl.spaceMove.Text,
                    ["atmoMove"] = vehicleControl.atmoMove.Text,
                    ["passive1"] = vehicleControl.passive1.Text,
                    ["passive2"] = vehicleControl.passive2.Text,
                    ["scan1"] = vehicleControl.scan1.Text,
                    ["scan2"] = vehicleControl.scan2.Text,
                    ["search1"] = vehicleControl.search1.Text,
                    ["search2"] = vehicleControl.search2.Text,
                    ["focus1"] = vehicleControl.focus1.Text,
                    ["focus2"] = vehicleControl.focus2.Text,
                    ["shipNotes"] = vehicleControl.shipNotes.Text,
                    ["shipImagePath"] = vehicleControl.SelectedShipImagePath ?? string.Empty,
                    ["vehicleWeapons"] = GetVehicleWeaponsData(vehicleControl.vehicleWeapons)
                };

                // Show Save File dialog
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Ship Files (*.ship)|*.ship";
                    saveFileDialog.Title = "Save Ship File";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, vehicleData.ToString(Newtonsoft.Json.Formatting.Indented));
                        MessageBox.Show("Ship saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save ship: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void LoadVehicle(VehicleControl vehicleControl)
        {
            try
            {
                // Show Open File dialog
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Ship Files (*.ship)|*.ship";
                    openFileDialog.Title = "Load Ship File";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string jsonContent = File.ReadAllText(openFileDialog.FileName);
                        var vehicleData = JObject.Parse(jsonContent);

                        // Populate the fields on the VehicleControl
                        vehicleControl.craftBox.Text = vehicleData["craft"]?.ToString() ?? string.Empty;
                        vehicleControl.typeBox.Text = vehicleData["type"]?.ToString() ?? string.Empty;
                        vehicleControl.scaleBox.Text = vehicleData["scale"]?.ToString() ?? string.Empty;
                        vehicleControl.lengthBox.Text = vehicleData["length"]?.ToString() ?? string.Empty;
                        vehicleControl.pilotSkill.Text = vehicleData["pilotSkill"]?.ToString() ?? string.Empty;
                        vehicleControl.crewSize.Text = vehicleData["crewSize"]?.ToString() ?? string.Empty;
                        vehicleControl.crewSkill.Text = vehicleData["crewSkill"]?.ToString() ?? string.Empty;
                        vehicleControl.passBox.Text = vehicleData["passengers"]?.ToString() ?? string.Empty;
                        vehicleControl.cargoCap.Text = vehicleData["cargoCapacity"]?.ToString() ?? string.Empty;
                        vehicleControl.conBox.Text = vehicleData["consumables"]?.ToString() ?? string.Empty;
                        vehicleControl.costBox.Text = vehicleData["cost"]?.ToString() ?? string.Empty;
                        vehicleControl.hullMod.Text = vehicleData["hull"]?.ToString() ?? string.Empty;
                        vehicleControl.shieldBox.Text = vehicleData["shields"]?.ToString() ?? string.Empty;
                        vehicleControl.hyperBox.Text = vehicleData["hyperdrive"]?.ToString() ?? string.Empty;
                        vehicleControl.backHyper.Text = vehicleData["backupHyperdrive"]?.ToString() ?? string.Empty;
                        vehicleControl.navComp.SelectedItem = vehicleData["navComputer"]?.ToString() ?? "No";
                        vehicleControl.manBox.Text = vehicleData["maneuverability"]?.ToString() ?? string.Empty;
                        vehicleControl.spaceMove.Text = vehicleData["spaceMove"]?.ToString() ?? string.Empty;
                        vehicleControl.atmoMove.Text = vehicleData["atmoMove"]?.ToString() ?? string.Empty;
                        vehicleControl.passive1.Text = vehicleData["passive1"]?.ToString() ?? string.Empty;
                        vehicleControl.passive2.Text = vehicleData["passive2"]?.ToString() ?? string.Empty;
                        vehicleControl.scan1.Text = vehicleData["scan1"]?.ToString() ?? string.Empty;
                        vehicleControl.scan2.Text = vehicleData["scan2"]?.ToString() ?? string.Empty;
                        vehicleControl.search1.Text = vehicleData["search1"]?.ToString() ?? string.Empty;
                        vehicleControl.search2.Text = vehicleData["search2"]?.ToString() ?? string.Empty;
                        vehicleControl.focus1.Text = vehicleData["focus1"]?.ToString() ?? string.Empty;
                        vehicleControl.focus2.Text = vehicleData["focus2"]?.ToString() ?? string.Empty;
                        vehicleControl.shipNotes.Text = vehicleData["shipNotes"]?.ToString() ?? string.Empty;

                        // Load the image if the path exists
                        string shipImagePath = vehicleData["shipImagePath"]?.ToString();
                        if (!string.IsNullOrEmpty(shipImagePath) && File.Exists(shipImagePath))
                        {
                            vehicleControl.shipImageDisplay.Image = Image.FromFile(shipImagePath);
                            vehicleControl.shipImageDisplay.SizeMode = PictureBoxSizeMode.Zoom;
                            vehicleControl.SelectedShipImagePath = shipImagePath;
                        }
                        else
                        {
                            vehicleControl.shipImageDisplay.Image = null;
                        }

                        // Load the vehicle weapons data
                        LoadVehicleWeaponsData(vehicleControl.vehicleWeapons, vehicleData["vehicleWeapons"] as JArray);

                        MessageBox.Show("Ship loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load ship: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static JArray GetVehicleWeaponsData(DataGridView vehicleWeapons)
        {
            var weaponsArray = new JArray();

            foreach (DataGridViewRow row in vehicleWeapons.Rows)
            {
                if (!row.IsNewRow)
                {
                    var weaponData = new JObject();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.OwningColumn != null && cell.Value != null)
                        {
                            weaponData[cell.OwningColumn.Name] = cell.Value.ToString();
                        }
                    }
                    weaponsArray.Add(weaponData);
                }
            }

            return weaponsArray;
        }

        private static void LoadVehicleWeaponsData(DataGridView vehicleWeapons, JArray weaponsArray)
        {
            if (weaponsArray == null) return;

            vehicleWeapons.Rows.Clear();

            foreach (var weaponToken in weaponsArray)
            {
                var weaponData = weaponToken as JObject;
                if (weaponData != null)
                {
                    int rowIndex = vehicleWeapons.Rows.Add();
                    var row = vehicleWeapons.Rows[rowIndex];

                    foreach (var property in weaponData.Properties())
                    {
                        if (row.Cells[property.Name] != null)
                        {
                            row.Cells[property.Name].Value = property.Value.ToString();
                        }
                    }
                }
            }
        }
    }
}
