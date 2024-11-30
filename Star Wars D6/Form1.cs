using System;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Drawing.Printing;
using System.Text;
using System.Text.RegularExpressions;


namespace Star_Wars_D6
{
    public partial class Form1 : Form
    {
        

        
        public Form1 mainForm;
        private PrintDocument printDocument = new PrintDocument();
        private List<Bitmap> tabImages = new List<Bitmap>();
        private int currentPageIndex = 0;

        public Form1()
        {
            InitializeComponent();
            imageSelect.Click += imageSelect_Click;
            foundExport.Click += FoundExport_Click;
            mainForm = this;
            this.openShop.Click += new System.EventHandler(this.openShop_Click);
            var vehicleControl = new VehicleControl();
            openLearning.Click += openLearning_Click;
            var equipmentListForm = new Star_Wars_D6.EquipmentList();
            tabVehicle.Controls.Add(vehicleControl);
            vehicleControl.Dock = DockStyle.Fill;
            this.forcePowersPanel.ContextMenuStrip = null; 
            AttachMouseWheelHandler(this);
            if (weapInv == null) weapInv = new Panel();
            if (armorInv == null) armorInv = new Panel();
            if (gearInv == null) gearInv = new Panel();
            dexAtt.MouseClick += AttributeBox_MouseClick;
            knowAtt.MouseClick += AttributeBox_MouseClick;
            techAtt.MouseClick += AttributeBox_MouseClick;
            mechAtt.MouseClick += AttributeBox_MouseClick;
            perAtt.MouseClick += AttributeBox_MouseClick;
            strAtt.MouseClick += AttributeBox_MouseClick;
            checkBoxCharacterCreation.CheckedChanged += checkBoxCharacterCreation_CheckedChanged;
            previousAttributeValues[dexAtt] = "0D";
            previousAttributeValues[knowAtt] = "0D";
            previousAttributeValues[techAtt] = "0D";
            previousAttributeValues[mechAtt] = "0D";
            previousAttributeValues[perAtt] = "0D";
            previousAttributeValues[strAtt] = "0D";
            specialConvert.CheckedChanged += specialConvert_CheckedChanged;
            dexSkills.MouseClick += SkillList_MouseClick;
            knowSkills.MouseClick += SkillList_MouseClick;
            techSkills.MouseClick += SkillList_MouseClick;
            mechSkills.MouseClick += SkillList_MouseClick;
            perSkills.MouseClick += SkillList_MouseClick;
            strSkills.MouseClick += SkillList_MouseClick;
            dexSkills.MouseClick += SkillList_RightClick;
            knowSkills.MouseClick += SkillList_RightClick;
            techSkills.MouseClick += SkillList_RightClick;
            mechSkills.MouseClick += SkillList_RightClick;
            perSkills.MouseClick += SkillList_RightClick;
            strSkills.MouseClick += SkillList_RightClick;
            forceEditButton.Click += forceEditButton_Click;
            ConfigureXpLog();
            this.discordLink.Click += new EventHandler(this.discordLink_Click);
            this.buyCoffee.Click += new EventHandler(this.buyCoffee_Click);
            this.od6.LinkClicked += new LinkLabelLinkClickedEventHandler(this.od6_LinkClicked);
            this.od6sw.LinkClicked += new LinkLabelLinkClickedEventHandler(this.od6sw_LinkClicked);
            UpdateChecker.CheckForUpdates();
            ConfigureContextMenus();



            // Campaign wires
            dexRename.TextChanged += dexRename_TextChanged;
            knowRename.TextChanged += knowRename_TextChanged;
            mechRename.TextChanged += mechRename_TextChanged;
            techRename.TextChanged += techRename_TextChanged;
            perRename.TextChanged += perRename_TextChanged;
            strRename.TextChanged += strRename_TextChanged;
            racialCampList.TextChanged += racialCampList_TextChanged;
            ConfigureListView(dexSkills);
            ConfigureListView(knowSkills);
            ConfigureListView(techSkills);
            ConfigureListView(mechSkills);
            ConfigureListView(perSkills);
            ConfigureListView(strSkills);
            PopulateListViewFromListBox(dexSkills, campDex);
            PopulateListViewFromListBox(knowSkills, campKnow);
            PopulateListViewFromListBox(techSkills, campTech);
            PopulateListViewFromListBox(mechSkills, campMech);
            PopulateListViewFromListBox(perSkills, campPer);
            PopulateListViewFromListBox(strSkills, campStr);
            racialBase.TextChanged += racialBase_TextChanged;
            attAdd.TextChanged += attAdd_TextChanged;

        }




        public string selectedImagePath;


        private void imageSelect_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = openFileDialog.FileName;  // Store the selected image path
                    imageDisplay.Image = Image.FromFile(selectedImagePath);
                    imageDisplay.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private void PrepareTabsForPrinting()
        {
            tabImages.Clear();

            // List of specific tabs to print
            var tabsToPrint = new[] { "tabMain", "tabForce", "tabEquipment", "tabBackground" };

            foreach (TabPage tab in tabControl.TabPages)
            {
                if (!tabsToPrint.Contains(tab.Name))
                    continue;

                // Switch to the target tab to ensure proper rendering
                tabControl.SelectedTab = tab;
                tabControl.Refresh(); // Force UI to refresh

                Bitmap bmp = new Bitmap(tab.ClientSize.Width, tab.ClientSize.Height);
                tab.DrawToBitmap(bmp, tab.ClientRectangle);

                // Scale bitmap to Letter paper size (8.5 x 11 inches at 96 DPI)
                Bitmap scaledBmp = new Bitmap(10200, 13200);
                using (Graphics g = Graphics.FromImage(scaledBmp))
                {
                    g.DrawImage(bmp, 0, 0, scaledBmp.Width, scaledBmp.Height);
                }

                tabImages.Add(scaledBmp);
            }
        }


        private void PrintCharacter_Click(object sender, EventArgs e)
        {
            // Hide buttons before starting the print process
            ToggleControlsVisibility(false);

            try
            {
                PrepareTabsForPrinting();

                PrintDialog printDialog = new PrintDialog
                {
                    Document = printDocument
                };

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.PrintPage += PrintDocument_PrintPage;
                    currentPageIndex = 0; // Reset page index
                    printDocument.Print();
                }
            }
            finally
            {
                // Restore button visibility after printing
                ToggleControlsVisibility(true);
            }
        }

        private void ToggleControlsVisibility(bool isVisible)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    control.Visible = isVisible;
                }

                // Check inside containers like tabs and panels
                if (control.HasChildren)
                {
                    ToggleChildControlsVisibility(control, isVisible);
                }
            }
        }

        private void ToggleChildControlsVisibility(Control parent, bool isVisible)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is Button)
                {
                    child.Visible = isVisible;
                }

                if (child.HasChildren)
                {
                    ToggleChildControlsVisibility(child, isVisible);
                }
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (currentPageIndex < tabImages.Count)
            {
                // Draw the current page image
                e.Graphics.DrawImage(tabImages[currentPageIndex], e.MarginBounds);

                // Move to the next page
                currentPageIndex++;
                e.HasMorePages = currentPageIndex < tabImages.Count;
            }
            else
            {
                e.HasMorePages = false;
            }
        }

        private void AttachMouseWheelHandler(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is ListBox listBox)
                {
                    listBox.MouseWheel += ListBox_MouseWheel;
                }
                else if (control.HasChildren)
                {
                    AttachMouseWheelHandler(control); // Recursively attach to child controls
                }
            }
        }

        private void ListBox_MouseWheel(object sender, MouseEventArgs e)
        {
            this.OnMouseWheel(e);
        }



        // Helper method to gather skills dynamically with correct types
        private List<KeyValuePair<string, string>> GatherSkills(ListBox skillsList, ListBox valuesList)
        {
            List<KeyValuePair<string, string>> skills = new List<KeyValuePair<string, string>>();

            // Log to check counts and identify potential mismatches
            Console.WriteLine($"SkillsList Count: {skillsList.Items.Count}, ValuesList Count: {valuesList.Items.Count}");

            // Check for mismatched item counts before gathering skills
            if (skillsList.Items.Count != valuesList.Items.Count)
            {
                MessageBox.Show("Mismatch between skills and values list counts. Check entries to ensure they are properly aligned.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Mismatch found - exiting GatherSkills without processing items.");
                return skills; // Exit if counts don't match to avoid accessing out-of-range indices
            }

            for (int i = 0; i < skillsList.Items.Count; i++)
            {
                string skillName = skillsList.Items[i].ToString();

                // Handle unassigned base skills by defaulting to "0D" if no value exists
                string skillValueText = valuesList.Items[i]?.ToString();
                if (string.IsNullOrWhiteSpace(skillValueText))
                {
                    skillValueText = "0D"; // Default to "0D" if no value is set for base skill
                }

                skills.Add(new KeyValuePair<string, string>(skillName, skillValueText));
            }

            Console.WriteLine($"Successfully gathered {skills.Count} skills.");
            return skills;
        }


        private void loadFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON Files (*.json)|*.json";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string jsonContent = File.ReadAllText(openFileDialog.FileName);
                    LoadFromJson(jsonContent);  // Directly calling LoadFromJson, not Export.LoadFromJson
                }
            }
        }


        private void LoadFromJson(string jsonContent)
        {
            JObject jsonObject = JObject.Parse(jsonContent);

            // Load racialBase value first to avoid "Please enter racial attribute dice" message
            racialBase.Text = jsonObject["system"]["racialBase"]?.ToString();

            // Now proceed to load other data
            characterName.Text = jsonObject["name"]?.ToString();
            typeBox.Text = jsonObject["system"]["chartype"]["content"]?.ToString();

            // Load attributes and other data after racialBase is set
            dexAtt.Text = ConvertFromPips((int?)jsonObject["system"]["attributes"]["agi"]["base"] ?? 0);
            knowAtt.Text = ConvertFromPips((int?)jsonObject["system"]["attributes"]["kno"]["base"] ?? 0);
            mechAtt.Text = ConvertFromPips((int?)jsonObject["system"]["attributes"]["mec"]["base"] ?? 0);
            perAtt.Text = ConvertFromPips((int?)jsonObject["system"]["attributes"]["per"]["base"] ?? 0);
            techAtt.Text = ConvertFromPips((int?)jsonObject["system"]["attributes"]["tec"]["base"] ?? 0);
            strAtt.Text = ConvertFromPips((int?)jsonObject["system"]["attributes"]["str"]["base"] ?? 0);

            // Load species, move, and points
            speciesBox.Text = jsonObject["system"]["species"]["content"]?.ToString();
            moveAtt.Text = jsonObject["system"]["move"]["value"]?.ToString();
            forcePoints.Text = jsonObject["system"]["fatepoints"]["value"]?.ToString();
            dsPoints.Text = jsonObject["system"]["custom1"]["value"]?.ToString();
            charPoints.Text = jsonObject["system"]["characterpoints"]["value"]?.ToString();

            // Load fate point effect
            fateBox.Checked = jsonObject["system"]["fatepointeffect"]?.ToObject<bool>() ?? false;

            // Load the image if available
            string imagePath = jsonObject["img"]?.ToString();
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                imageDisplay.Image = Image.FromFile(imagePath);
                imageDisplay.SizeMode = PictureBoxSizeMode.Zoom;
                selectedImagePath = imagePath;  // Store the loaded image path in case of re-export
            }

            
        }

        private void PopulateSkills(JObject jsonObject, ListBox skillsList, ListBox valuesList, string attributeKey)
        {
            JArray itemsArray = (JArray)jsonObject["items"];
            skillsList.Items.Clear();
            valuesList.Items.Clear();

            foreach (JObject item in itemsArray)
            {
                string skillType = item["type"]?.ToString();
                string attribute = item["system"]["attribute"]?.ToString();

                if (skillType == "skill" && attribute == attributeKey)
                {
                    string skillName = item["name"]?.ToString();
                    int basePips = item["system"]["base"]?.ToObject<int>() ?? 0;
                    string baseDiceValue = ConvertFromPips(basePips);

                    skillsList.Items.Add(skillName);
                    valuesList.Items.Add(baseDiceValue);
                }
                else if (skillType == "specialization" && attribute == attributeKey)
                {
                    string specializationName = "-" + item["name"]?.ToString();
                    string parentSkill = item["system"]["skill"]?.ToString();
                    int basePips = item["system"]["base"]?.ToObject<int>() ?? 0;
                    string baseDiceValue = ConvertFromPips(basePips);

                    skillsList.Items.Add(specializationName);
                    valuesList.Items.Add(baseDiceValue);
                }
            }
        }

        private string ConvertFromPips(int pips)
        {
            int dice = pips / 3;
            int remainingPips = pips % 3;
            return remainingPips > 0 ? $"{dice}D+{remainingPips}" : $"{dice}D";
        }


        private void FoundExport_Click(object sender, EventArgs e)
        {
            string character = characterName.Text;
            string type = typeBox.Text;
            string species = speciesBox.Text;
            string move = moveAtt.Text;
            string fatepoints = forcePoints.Text;
            string custom1 = dsPoints.Text;
            string characterpoints = charPoints.Text;
            bool fatepointeffect = fateBox.Checked;
            string racialBaseValue = racialBase.Text;

            // Get credits value from playerCreditsTextBox directly
            string credits = playerCreditsTextBox?.Text ?? "0";

            // Call GenerateJson with all collected parameters
            Export.GenerateJson(
                character,
                type,
                selectedImagePath,
                this,  // Pass the current form
                species,
                move,
                fatepoints,
                custom1,
                characterpoints,
                fatepointeffect,
                racialBaseValue,
                credits
            );
        }


        public bool IsLoadingCharacter { get; set; } = false;

        // Method to load character data
        public void LoadCharacterData(string filePath)
        {
            IsLoadingCharacter = true; // Set loading state to true
            CharacterIO.LoadCharacter(this, filePath); // Pass both the form and file path
            IsLoadingCharacter = false; // Reset loading state
        }




        private void Form1_Load(object sender, EventArgs e)
        {
            saveCharacter.Click += (s, ev) =>
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Holo Files (*.holo)|*.holo";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        CharacterIO.SaveCharacter(this, saveFileDialog.FileName);
                        MessageBox.Show("Character saved successfully.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };

            loadCharacter.Click += (s, ev) =>
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Holo Files (*.holo)|*.holo";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        CharacterIO.LoadCharacter(this, openFileDialog.FileName);
                        MessageBox.Show("Character loaded successfully.", "Load", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                
            };
        }

        private void discordLink_Click(object sender, EventArgs e)
        {
            string discordUrl = "https://discord.gg/Vt7rhHsPWf";
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = discordUrl,
                    UseShellExecute = true // Ensures it opens in the default browser
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open the link. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buyCoffee_Click(object sender, EventArgs e)
        {
            string coffeeUrl = "https://buymeacoffee.com/pandacreates";
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = coffeeUrl,
                    UseShellExecute = true // Ensures it opens in the default browser
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open the link. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void od6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string od6Url = "https://gitlab.com/vtt2/opend6-space";
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = od6Url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open the link. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void od6sw_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string od6swUrl = "https://github.com/algnc/od6s-star-wars";
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = od6swUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open the link. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        ///////////////////////////////////////////Everything below is new Character Creator Code after the great purge/////////////////////////////////////////////


        private bool characterCreation = false;

        private void checkBoxCharacterCreation_CheckedChanged(object sender, EventArgs e)
        {
            // Set the characterCreation flag based on the checkbox's state
            characterCreation = checkBoxCharacterCreation.Checked;

            // Debug: Print the current state to the console
            Console.WriteLine($"Character Creation Flag: {characterCreation}");
        }

        private string ShowDiceInputPopup()
        {
            // Create a new form
            Form popup = new Form
            {
                Text = "Assign Attribute Dice",
                Size = new System.Drawing.Size(300, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false
            };

            // Label
            Label label = new Label
            {
                Text = "Enter Dice Value:",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true
            };

            // TextBox for input
            TextBox inputBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 40),
                Width = 260
            };

            // Buttons
            Button applyButton = new Button
            {
                Text = "Apply",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(10, 80)
            };

            Button cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(110, 80)
            };

            // Add controls to the form
            popup.Controls.Add(label);
            popup.Controls.Add(inputBox);
            popup.Controls.Add(applyButton);
            popup.Controls.Add(cancelButton);

            // Set the Accept and Cancel buttons
            popup.AcceptButton = applyButton;
            popup.CancelButton = cancelButton;

            // Show the dialog and get the result
            if (popup.ShowDialog() == DialogResult.OK)
            {
                return inputBox.Text.Trim(); // Return the entered value
            }

            return null; // Return null if canceled
        }

        private Dictionary<TextBox, string> previousAttributeValues = new Dictionary<TextBox, string>();

        private void AttributeBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return; // Only handle left-clicks

            TextBox attributeBox = sender as TextBox;
            if (attributeBox == null) return;

            if (!characterCreation)
            {
                // Logic when characterCreation is false (existing behavior)
                HandleAttributeBoxDuringGameplay(attributeBox);
            }
            else
            {
                // Logic when characterCreation is true (new behavior)
                HandleAttributeBoxDuringCharacterCreation(attributeBox);
            }
        }

        private void HandleAttributeBoxDuringGameplay(TextBox attributeBox)
        {
            // Show the dice input popup
            string enteredValue = ShowDiceInputPopup();

            if (!string.IsNullOrEmpty(enteredValue))
            {
                // Validate the entered dice value
                if (IsValidDiceValue(enteredValue))
                {
                    // Get the previous value from the dictionary
                    string previousValue = previousAttributeValues[attributeBox];

                    // Calculate the difference between the new and previous values
                    string currentValue = currentAttribute.Text.Trim();
                    string afterAddingBack;
                    string newCurrentAttribute;

                    if (TryAddDiceValues(currentValue, previousValue, out afterAddingBack) &&
                        TrySubtractDiceValues(afterAddingBack, enteredValue, out newCurrentAttribute))
                    {
                        // Update the currentAttribute with the new value
                        currentAttribute.Text = newCurrentAttribute;

                        // Update the target textbox with the entered value
                        attributeBox.Text = enteredValue;

                        // Update the dictionary with the new value
                        previousAttributeValues[attributeBox] = enteredValue;
                    }
                    else
                    {
                        MessageBox.Show("Not enough attribute dice available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid dice value. Please enter a valid value like '1D+1' or '2D'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void HandleAttributeBoxDuringCharacterCreation(TextBox attributeBox)
        {
            string currentValue = attributeBox.Text.Trim();
            if (!IsValidDiceValue(currentValue))
            {
                MessageBox.Show("Please assign a valid attribute value before increasing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Parse the current value
            int currentDice, currentPips;
            ParseDiceValue(currentValue, out currentDice, out currentPips);

            // Calculate the cost to upgrade by 1 pip
            int cost = currentDice * 10;

            // Confirm the upgrade
            string message = $"Do you want to increase {attributeBox.Name.Replace("Att", "")} to {currentDice}D+{currentPips + 1}? This will cost {cost} Character Points.";
            DialogResult result = MessageBox.Show(message, "Increase Attribute", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            // Deduct Character Points and validate
            int charPointsRemaining = int.TryParse(charPoints.Text.Trim(), out int points) ? points : 0;
            if (charPointsRemaining < cost)
            {
                MessageBox.Show("Not enough Character Points available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            charPointsRemaining -= cost;
            charPoints.Text = charPointsRemaining.ToString();

            // Store the previous value for logging
            string previousValue = currentValue;

            // Increase the attribute by 1 pip
            int totalPips = (currentDice * 3) + currentPips + 1;
            int newDice = totalPips / 3;
            int newPips = totalPips % 3;
            string newValue = newPips > 0 ? $"{newDice}D+{newPips}" : $"{newDice}D";

            attributeBox.Text = newValue;

            // Update corresponding skills
            UpdateSkillsForAttribute(attributeBox.Name.Replace("Att", ""), totalPips);

            // Log the upgrade in xpLog
            LogAttributeUpgrade(attributeBox.Name.Replace("Att", ""), previousValue, newValue, cost);
        }


        private void UpdateSkillsForAttribute(string attributeName, int totalPips)
        {
            ListView skillList = GetSkillListView(attributeName);
            if (skillList == null) return;

            foreach (ListViewItem item in skillList.Items)
            {
                string skillValue = item.SubItems[1].Text; // Get the die column value
                if (string.IsNullOrWhiteSpace(skillValue)) continue; // Skip empty skills

                int skillDice, skillPips;
                ParseDiceValue(skillValue, out skillDice, out skillPips);

                int skillTotalPips = (skillDice * 3) + skillPips + 1; // Increase by 1 pip
                int newSkillDice = skillTotalPips / 3;
                int newSkillPips = skillTotalPips % 3;

                item.SubItems[1].Text = newSkillPips > 0 ? $"{newSkillDice}D+{newSkillPips}" : $"{newSkillDice}D"; // Update skill die column
            }
        }

        private ListView GetSkillListView(string attributeName)
        {
            switch (attributeName.ToLower())
            {
                case "dex":
                    return dexSkills;
                case "know":
                    return knowSkills;
                case "tech":
                    return techSkills;
                case "mech":
                    return mechSkills;
                case "per":
                    return perSkills;
                case "str":
                    return strSkills;
                default:
                    return null;
            }
        }



        private bool TryAddDiceValues(string baseValue, string addValue, out string result)
        {
            result = baseValue;

            // Parse the base value
            int baseDice, basePips;
            ParseDiceValue(baseValue, out baseDice, out basePips);

            // Parse the value to add
            int addDice, addPips;
            ParseDiceValue(addValue, out addDice, out addPips);

            // Perform addition
            int totalPips = (baseDice * 3 + basePips) + (addDice * 3 + addPips);
            int finalDice = totalPips / 3; // Convert back to dice
            int finalPips = totalPips % 3;

            // Format the result
            result = finalPips > 0 ? $"{finalDice}D+{finalPips}" : $"{finalDice}D";
            return true;
        }


        private bool IsValidDiceValue(string value)
        {
            // Normalize to uppercase to handle both "d" and "D"
            value = value.ToUpper();

            // Match formats like "1D", "1D+1", "2D+2", or "+1", "+2"
            return System.Text.RegularExpressions.Regex.IsMatch(value, @"^(\d+D(\+\d)?|\+\d)$");
        }

        public void specialConvert_CheckedChanged(object sender, EventArgs e)
        {
            // Only allow this method to work if character creation is set to false
            if (characterCreation)
            {
                return; // Do nothing if character creation is active
            }

            string skillDiceValue = skillDice.Text.Trim();
            string specialDiceValue = specialDice.Text.Trim();
            string newSkillDiceValue, newSpecialDiceValue;

            if (specialConvert.Checked)
            {
                // Try to subtract 1D from skillDice and add 3D to specialDice when checked
                if (TrySubtractDiceValues(skillDiceValue, "1D", out newSkillDiceValue))
                {
                    skillDice.Text = newSkillDiceValue;

                    // Add 3D to specialDice
                    if (TryAddDiceValues(specialDiceValue, "3D", out newSpecialDiceValue))
                    {
                        specialDice.Text = newSpecialDiceValue;
                    }
                    else
                    {
                        // Rollback if adding to specialDice fails (unlikely)
                        MessageBox.Show("Error adding 3D to specialties.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        specialConvert.Checked = false; // Reset checkbox
                        skillDice.Text = skillDiceValue; // Rollback skillDice
                    }
                }
                else
                {
                    // Not enough dice to subtract, show an error and reset the checkbox
                    MessageBox.Show("Not enough dice to convert skills to specialties.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    specialConvert.Checked = false; // Reset checkbox
                }
            }
            else
            {
                // Try to add 1D back to skillDice and subtract 3D from specialDice when unchecked
                if (TryAddDiceValues(skillDiceValue, "1D", out newSkillDiceValue))
                {
                    skillDice.Text = newSkillDiceValue;

                    // Subtract 3D from specialDice
                    if (TrySubtractDiceValues(specialDiceValue, "3D", out newSpecialDiceValue))
                    {
                        specialDice.Text = newSpecialDiceValue;
                    }
                    else
                    {
                        // Rollback if subtracting from specialDice fails
                        MessageBox.Show("Not enough specialty dice to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        specialConvert.Checked = true; // Reset checkbox
                        skillDice.Text = skillDiceValue; // Rollback skillDice
                    }
                }
                else
                {
                    // Unexpected failure to add dice back to skillDice
                    MessageBox.Show("Error adding dice back to skills.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }





        private bool TrySubtractDiceValues(string baseValue, string subtractValue, out string result)
        {
            result = baseValue;

            // Parse the base value
            int baseDice, basePips;
            ParseDiceValue(baseValue, out baseDice, out basePips);

            // Parse the value to subtract
            int subtractDice, subtractPips;
            ParseDiceValue(subtractValue, out subtractDice, out subtractPips);

            // Convert to total pips
            int baseTotalPips = baseDice * 3 + basePips;
            int subtractTotalPips = subtractDice * 3 + subtractPips;

            // Check if the subtraction would result in a negative value
            if (subtractTotalPips > baseTotalPips)
            {
                return false; // Not enough dice
            }

            // Perform subtraction
            int resultTotalPips = baseTotalPips - subtractTotalPips;
            int finalDice = resultTotalPips / 3; // Convert back to dice
            int finalPips = resultTotalPips % 3;

            // Format the result
            result = finalPips > 0 ? $"{finalDice}D+{finalPips}" : $"{finalDice}D";
            return true;
        }



        private void AttributeBox_TextChanged(object sender, EventArgs e)
        {
            Console.WriteLine($"Character Creation Flag: {characterCreation}");
            if (!characterCreation)
            {
                TextBox attributeBox = sender as TextBox;
                if (attributeBox != null)
                {
                    // Get the entered value
                    string enteredValue = attributeBox.Text.Trim();

                    // Get the current value of currentAttribute
                    string currentValue = currentAttribute.Text.Trim();

                    // Attempt to subtract the entered value from currentAttribute
                    string newValue;
                    if (TrySubtractDiceValues(currentValue, enteredValue, out newValue))
                    {
                        // Update currentAttribute with the new value
                        currentAttribute.Text = newValue;
                    }
                    else
                    {
                        // Show an alert and revert the change
                        MessageBox.Show("Not enough attribute dice available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Clear the invalid input in the attribute box
                        attributeBox.Text = "";
                    }
                }
            }
        }

        private Dictionary<ListViewItem, string> assignedDiceValues = new Dictionary<ListViewItem, string>();

        private void SkillList_MouseClick(object sender, MouseEventArgs e)
        {
            // Get the ListView and ensure it's a left-click
            ListView listView = sender as ListView;
            if (listView == null || e.Button != MouseButtons.Left) return;

            // Get the clicked row
            ListViewItem selectedItem = listView.GetItemAt(e.X, e.Y);
            if (selectedItem == null) return;

            string skillName = selectedItem.SubItems[0].Text;

            // Handle behavior based on the character creation flag
            if (!characterCreation)
            {
                // Specializations do not allow skill dice assignment during gameplay
                if (skillName.StartsWith("-"))
                {
                    MessageBox.Show("Specializations cannot have skill dice assigned directly.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Get the corresponding attribute value
                TextBox attributeBox = GetAttributeBoxFromListView(listView);
                string attributeValue = attributeBox?.Text.Trim() ?? "0D";

                int attributeDice, attributePips;
                ParseDiceValue(attributeValue, out attributeDice, out attributePips);

                // Ensure attribute has a value
                if (attributeDice == 0 && attributePips == 0)
                {
                    MessageBox.Show("Please assign Attribute Dice first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get the current assigned dice value
                string currentAssignedDice = assignedDiceValues.ContainsKey(selectedItem) ? assignedDiceValues[selectedItem] : "0D";

                // Handle skill dice assignment during gameplay
                HandleSkillDiceDuringGameplay(listView, selectedItem, skillName, currentAssignedDice, attributeValue);
            }
            else
            {
                // Allow specializations to be upgraded with Character Points
                HandleSkillUpgradeWithCharacterPoints(selectedItem, skillName);
            }
        }


        private void HandleSkillDiceDuringGameplay(ListView listView, ListViewItem selectedItem, string skillName, string currentAssignedDice, string attributeValue)
        {
            // Show a popup to assign dice
            string input = ShowDiceInputPopup($"How many skill dice would you like to assign to {skillName}?");
            input = string.IsNullOrWhiteSpace(input) ? "0D" : input; // Treat empty input as 0D

            if (!IsValidDiceValue(input)) return;

            // Parse the input dice value and the current assigned dice value
            int inputDice, inputPips, currentDice, currentPips;
            ParseDiceValue(input, out inputDice, out inputPips);
            ParseDiceValue(currentAssignedDice, out currentDice, out currentPips);

            // Calculate the difference between the new assigned dice and the previous assigned dice
            int inputTotalPips = (inputDice * 3) + inputPips;
            int currentTotalPips = (currentDice * 3) + currentPips;
            int difference = inputTotalPips - currentTotalPips;

            // Get the current skillDice value
            string skillDiceValue = skillDice.Text.Trim();
            int skillDiceRemaining, skillPipsRemaining;
            ParseDiceValue(skillDiceValue, out skillDiceRemaining, out skillPipsRemaining);
            int skillTotalPips = (skillDiceRemaining * 3) + skillPipsRemaining;

            // Handle refund or subtraction
            if (difference > 0)
            {
                // Subtract the difference from skillDice
                if (difference > skillTotalPips)
                {
                    MessageBox.Show("Not enough dice in your skill pool to assign.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                skillTotalPips -= difference;
            }
            else
            {
                // Refund the difference to skillDice
                skillTotalPips += Math.Abs(difference);
            }

            // Update the skillDice textbox
            int newSkillDice = skillTotalPips / 3;
            int newSkillPips = skillTotalPips % 3;
            skillDice.Text = newSkillPips > 0 ? $"{newSkillDice}D+{newSkillPips}" : $"{newSkillDice}D";

            // Calculate the total dice value (attribute + assigned)
            int attributeDice, attributePips;
            ParseDiceValue(attributeValue, out attributeDice, out attributePips);

            int totalSkillPips = (attributeDice * 3 + attributePips) + inputTotalPips;
            int finalDice = totalSkillPips / 3;
            int finalPips = totalSkillPips % 3;

            // If the total equals the attribute value, clear the Die column
            if (totalSkillPips == (attributeDice * 3 + attributePips))
            {
                selectedItem.SubItems[1].Text = ""; // Clear the Die column
            }
            else
            {
                // Otherwise, update the Die column with the new total
                selectedItem.SubItems[1].Text = finalPips > 0 ? $"{finalDice}D+{finalPips}" : $"{finalDice}D";
            }

            // Update the assigned dice value for the row
            assignedDiceValues[selectedItem] = input;
        }



        private void HandleSkillUpgradeWithCharacterPoints(ListViewItem selectedItem, string skillName)
        {
            // Get the associated attribute for the skill
            TextBox attributeBox = GetAttributeBoxFromListView(selectedItem.ListView);
            if (attributeBox == null)
            {
                MessageBox.Show($"Could not determine the attribute for skill: {skillName}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Parse the attribute value
            string attributeValue = attributeBox.Text.Trim();
            int attributeDice, attributePips;
            ParseDiceValue(attributeValue, out attributeDice, out attributePips);

            if (attributeDice == 0 && attributePips == 0)
            {
                MessageBox.Show("Please assign Attribute Dice first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the current skill value from the Die column
            string skillValue = selectedItem.SubItems[1].Text.Trim(); // SubItems[1] corresponds to the Die column
            int skillDice = 0, skillPips = 0;

            if (string.IsNullOrEmpty(skillValue))
            {
                // Use the attribute value if the skill value is empty
                skillDice = attributeDice;
                skillPips = attributePips;
            }
            else
            {
                // Parse the skill value if it exists
                ParseDiceValue(skillValue, out skillDice, out skillPips);
            }

            // Calculate the current total pips
            int currentTotalPips = (skillDice * 3) + skillPips;

            // Check if the skill is a specialty
            bool isSpecialty = skillName.StartsWith("-");

            // Calculate the cost based on the dice value before the upgrade
            int diceBeforeUpgrade = currentTotalPips / 3;
            int cost = isSpecialty
                ? (int)Math.Ceiling(diceBeforeUpgrade / 2.0) // Specialty cost is half the dice value, rounded up
                : diceBeforeUpgrade;

            // Calculate the new total pips after the upgrade
            int newTotalPips = currentTotalPips + 1;

            // Calculate the new dice and pips values
            int newDice = newTotalPips / 3;
            int newPips = newTotalPips % 3;

            // Determine the new skill value
            string newSkillValue = newPips > 0 ? $"{newDice}D+{newPips}" : $"{newDice}D";

            // Prompt the user to confirm the cost
            DialogResult result = MessageBox.Show(
                $"Increase {skillName} to {newSkillValue} for {cost} Character Points?",
                "Confirm Skill Upgrade",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            // Ensure enough Character Points are available
            int characterPoints = 0; // Default to 0 if parsing fails
            if (!int.TryParse(charPoints.Text.Trim(), out characterPoints))
            {
                characterPoints = 0; // Handle invalid or empty input by defaulting to 0
            }

            if (characterPoints < cost)
            {
                MessageBox.Show("Not enough Character Points.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            // Deduct the cost and update the Character Points
            characterPoints -= cost;
            charPoints.Text = characterPoints.ToString();

            // Update the skill value in the Die column
            selectedItem.SubItems[1].Text = newSkillValue;

            // Log the transaction in the XP Log
            xpLog.Items.Add(new ListViewItem(new[]
            {
        skillName,
        isSpecialty ? "Specialty" : "Skill",
        string.IsNullOrEmpty(skillValue) ? $"{attributeDice}D" : skillValue, // Log initial value
        newSkillValue,
        cost.ToString()
    }));
        }














        private TextBox GetAttributeBoxFromListView(ListView listView)
        {
            if (listView == dexSkills) return dexAtt;
            if (listView == knowSkills) return knowAtt;
            if (listView == techSkills) return techAtt;
            if (listView == mechSkills) return mechAtt;
            if (listView == perSkills) return perAtt;
            if (listView == strSkills) return strAtt;

            return null;
        }

        private string ShowDiceInputPopup(string prompt)
        {
            Form popup = new Form
            {
                Text = "Assign Skill Dice",
                Size = new System.Drawing.Size(300, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false
            };

            Label label = new Label
            {
                Text = prompt,
                AutoSize = true,
                Location = new System.Drawing.Point(10, 10)
            };

            TextBox inputBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 40),
                Width = 260
            };

            Button applyButton = new Button
            {
                Text = "Apply",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(10, 80)
            };

            Button cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(110, 80)
            };

            popup.Controls.Add(label);
            popup.Controls.Add(inputBox);
            popup.Controls.Add(applyButton);
            popup.Controls.Add(cancelButton);

            popup.AcceptButton = applyButton;
            popup.CancelButton = cancelButton;

            if (popup.ShowDialog() == DialogResult.OK)
            {
                return inputBox.Text.Trim();
            }

            return null;
        }

        private void AddSpecialization(ListView listView, ListViewItem parentItem, string specializationName)
        {
            // Get the corresponding attribute value
            TextBox attributeBox = GetAttributeBoxFromListView(listView);
            string attributeValue = attributeBox?.Text.Trim() ?? "0D";

            int attributeDice, attributePips;
            ParseDiceValue(attributeValue, out attributeDice, out attributePips);
            if (attributeDice == 0 && attributePips == 0)
            {
                MessageBox.Show("Please assign Attribute Dice first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the parent's current Die value
            string parentDiceValue = parentItem.SubItems[1].Text;
            int parentDice, parentPips;
            ParseDiceValue(parentDiceValue, out parentDice, out parentPips);

            // Calculate the specialization's starting value (1D more than the parent)
            int totalPips = (parentDice * 3 + parentPips) + 3; // Add 1D (3 pips)
            int specializationDice = totalPips / 3;
            int specializationPips = totalPips % 3;
            string specializationValue = specializationPips > 0 ? $"{specializationDice}D+{specializationPips}" : $"{specializationDice}D";

            // Reduce specialDice by 1D
            string specialDiceValue = specialDice.Text.Trim();
            string newSpecialDice;
            if (!TrySubtractDiceValues(specialDiceValue, "1D", out newSpecialDice))
            {
                MessageBox.Show("Not enough dice to create specialization.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            specialDice.Text = newSpecialDice;

            // Create a new ListViewItem for the specialization
            ListViewItem specializationItem = new ListViewItem($"-{specializationName}");
            specializationItem.SubItems.Add(specializationValue);

            // Insert the specialization immediately below the parent
            int parentIndex = listView.Items.IndexOf(parentItem);
            listView.Items.Insert(parentIndex + 1, specializationItem);

            // Highlight the new specialization
            specializationItem.Selected = true;
        }



        private void RemoveSpecialization(ListView listView, ListViewItem specializationItem)
        {
            // Refund 1D to specialDice
            string specialDiceValue = specialDice.Text.Trim();
            string newSpecialDice;
            if (TryAddDiceValues(specialDiceValue, "1D", out newSpecialDice))
            {
                specialDice.Text = newSpecialDice;
            }

            // Remove the specialization row
            listView.Items.Remove(specializationItem);
        }


        private void SkillList_RightClick(object sender, MouseEventArgs e)
        {
            if (characterCreation) return; // Only allow this when character creation is false

            ListView listView = sender as ListView;
            if (listView == null || e.Button != MouseButtons.Right) return;

            // Get the clicked row
            ListViewItem selectedItem = listView.GetItemAt(e.X, e.Y);
            if (selectedItem == null) return;

            // Check if the item is a specialization (starts with '-')
            bool isSpecialization = selectedItem.SubItems[0].Text.StartsWith("-");
            if (isSpecialization)
            {
                // Prompt to remove specialization
                DialogResult result = MessageBox.Show($"Remove specialization {selectedItem.SubItems[0].Text}?",
                                                      "Remove Specialization",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    RemoveSpecialization(listView, selectedItem);
                }
            }
            else
            {
                // Handle adding a new specialization
                string skillName = selectedItem.SubItems[0].Text;

                string specializationName = ShowSpecializationPopup(skillName);
                if (string.IsNullOrWhiteSpace(specializationName)) return; // Exit if no input provided

                AddSpecialization(listView, selectedItem, specializationName);
            }
        }

        private string ShowSpecializationPopup(string skillName)
        {
            Form popup = new Form
            {
                Text = $"Create Specialization for {skillName}",
                Size = new System.Drawing.Size(400, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false
            };

            Label label = new Label
            {
                Text = $"Enter a specialization for {skillName}:",
                AutoSize = true,
                Location = new System.Drawing.Point(10, 10)
            };

            TextBox inputBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 40),
                Width = 360
            };

            Button applyButton = new Button
            {
                Text = "Apply",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(10, 80)
            };

            Button cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(100, 80)
            };

            popup.Controls.Add(label);
            popup.Controls.Add(inputBox);
            popup.Controls.Add(applyButton);
            popup.Controls.Add(cancelButton);

            popup.AcceptButton = applyButton;
            popup.CancelButton = cancelButton;

            if (popup.ShowDialog() == DialogResult.OK)
            {
                return inputBox.Text.Trim();
            }

            return null;
        }

        // Force Page Stuff below for character creation

        private void forceEditButton_Click(object sender, EventArgs e)
        {
            // Step 1: Ask the user which Force Skill to modify
            string[] forceSkills = { "Control", "Sense", "Alter" };
            string selectedForceSkill = ShowOptionPopup("What Force Skill would you like to modify?", forceSkills);
            if (string.IsNullOrEmpty(selectedForceSkill)) return; // User canceled

            // Step 2: Ask how to modify the Force Skill
            string[] modificationMethods = { "Attribute Dice", "Skill Dice", "Character Points" };
            string selectedMethod = ShowOptionPopup("How are we modifying this skill?", modificationMethods);
            if (string.IsNullOrEmpty(selectedMethod)) return; // User canceled

            // Step 3: If Attribute or Skill Dice is selected, ask to increase or decrease
            string modificationType = null;
            if (selectedMethod == "Attribute Dice" || selectedMethod == "Skill Dice")
            {
                string[] increaseOrDecrease = { "Increase", "Decrease" };
                modificationType = ShowOptionPopup("Would you like to increase or decrease this skill?", increaseOrDecrease);
                if (string.IsNullOrEmpty(modificationType)) return; // User canceled
            }

            // Handle the selected method
            if (selectedMethod == "Attribute Dice")
            {
                if (modificationType == "Increase")
                {
                    IncreaseForceSkillByAttributeDice(selectedForceSkill);
                }
                else if (modificationType == "Decrease")
                {
                    DecreaseForceSkillByAttributeDice(selectedForceSkill);
                }
            }
            else if (selectedMethod == "Skill Dice")
            {
                if (modificationType == "Increase")
                {
                    IncreaseForceSkillBySkillDice(selectedForceSkill);
                }
                else if (modificationType == "Decrease")
                {
                    DecreaseForceSkillBySkillDice(selectedForceSkill);
                }
            }
            else if (selectedMethod == "Character Points")
            {
                IncreaseForceSkillByCharacterPoints(selectedForceSkill);
            }
        }

        private void DecreaseForceSkillByAttributeDice(string forceSkill)
        {
            // Prompt the user for a dice value to decrease
            string input = ShowDiceInputPopup($"Enter the dice value to decrease from {forceSkill}:");
            if (string.IsNullOrWhiteSpace(input)) return; // User canceled

            if (!IsValidDiceValue(input)) return;

            // Parse the input dice value
            int inputDice, inputPips;
            ParseDiceValue(input, out inputDice, out inputPips);
            int inputTotalPips = (inputDice * 3) + inputPips;

            // Get the corresponding Force skill textbox
            TextBox forceSkillBox = GetForceSkillBox(forceSkill);
            if (forceSkillBox == null) return;

            // Get the current value of the Force skill
            string currentValue = forceSkillBox.Text.Trim();
            int currentDice, currentPips;
            ParseDiceValue(currentValue, out currentDice, out currentPips);
            int currentTotalPips = (currentDice * 3) + currentPips;

            // Ensure the Force skill has enough dice to decrease
            if (inputTotalPips > currentTotalPips)
            {
                MessageBox.Show("Not enough dice in the Force skill to decrease.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Calculate the new total pips for the Force skill
            int newTotalPips = currentTotalPips - inputTotalPips;

            // Add the difference back to the currentAttribute
            string currentAttributeValue = currentAttribute.Text.Trim();
            int attributeDice, attributePips;
            ParseDiceValue(currentAttributeValue, out attributeDice, out attributePips);
            int attributeTotalPips = (attributeDice * 3) + attributePips + inputTotalPips;

            // Update currentAttribute
            int newAttributeDice = attributeTotalPips / 3;
            int newAttributePips = attributeTotalPips % 3;
            currentAttribute.Text = newAttributePips > 0 ? $"{newAttributeDice}D+{newAttributePips}" : $"{newAttributeDice}D";

            // Update the Force skill value
            int newForceDice = newTotalPips / 3;
            int newForcePips = newTotalPips % 3;
            forceSkillBox.Text = newForcePips > 0 ? $"{newForceDice}D+{newForcePips}" : $"{newForceDice}D";
        }

        private void DecreaseForceSkillBySkillDice(string forceSkill)
        {
            // Prompt the user for a dice value to decrease
            string input = ShowDiceInputPopup($"Enter the dice value to decrease from {forceSkill}:");
            if (string.IsNullOrWhiteSpace(input)) return; // User canceled

            if (!IsValidDiceValue(input)) return;

            // Parse the input dice value
            int inputDice, inputPips;
            ParseDiceValue(input, out inputDice, out inputPips);
            int inputTotalPips = (inputDice * 3) + inputPips;

            // Get the corresponding Force skill textbox
            TextBox forceSkillBox = GetForceSkillBox(forceSkill);
            if (forceSkillBox == null) return;

            // Get the current value of the Force skill
            string currentValue = forceSkillBox.Text.Trim();
            int currentDice, currentPips;
            ParseDiceValue(currentValue, out currentDice, out currentPips);
            int currentTotalPips = (currentDice * 3) + currentPips;

            // Ensure the Force skill has enough dice to decrease
            if (inputTotalPips > currentTotalPips)
            {
                MessageBox.Show("Not enough dice in the Force skill to decrease.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Calculate the new total pips for the Force skill
            int newTotalPips = currentTotalPips - inputTotalPips;

            // Add the difference back to the skillDice pool
            string skillDiceValue = skillDice.Text.Trim();
            int skillDiceRemaining, skillPipsRemaining;
            ParseDiceValue(skillDiceValue, out skillDiceRemaining, out skillPipsRemaining);
            int skillTotalPips = (skillDiceRemaining * 3) + skillPipsRemaining + inputTotalPips;

            // Update skillDice
            int newSkillDice = skillTotalPips / 3;
            int newSkillPips = skillTotalPips % 3;
            skillDice.Text = newSkillPips > 0 ? $"{newSkillDice}D+{newSkillPips}" : $"{newSkillDice}D";

            // Update the Force skill value
            int newForceDice = newTotalPips / 3;
            int newForcePips = newTotalPips % 3;
            forceSkillBox.Text = newForcePips > 0 ? $"{newForceDice}D+{newForcePips}" : $"{newForceDice}D";
        }


        private void IncreaseForceSkillByCharacterPoints(string forceSkill)
        {
            // Get the corresponding Force skill textbox
            TextBox forceSkillBox = GetForceSkillBox(forceSkill);
            if (forceSkillBox == null) return;

            // Parse the current Force skill value
            string currentValue = forceSkillBox.Text.Trim();
            int currentDice, currentPips;
            ParseDiceValue(currentValue, out currentDice, out currentPips);

            // Calculate the cost (number before the D, i.e., currentDice)
            int cost = currentDice > 0 ? currentDice : 1;

            // Calculate the new value (increase by 1 pip)
            int currentTotalPips = (currentDice * 3) + currentPips;
            int newTotalPips = currentTotalPips + 1;
            int newDice = newTotalPips / 3;
            int newPips = newTotalPips % 3;
            string newValue = newPips > 0 ? $"{newDice}D+{newPips}" : $"{newDice}D";

            // Ask for confirmation
            var result = MessageBox.Show(
                $"Increase {forceSkill} to {newValue} for {cost} Character Points?",
                "Confirm Upgrade",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                // User canceled the action
                return;
            }

            // Check if the user has enough Character Points
            string charPointsValue = charPoints.Text.Trim();
            int charPointsAvailable = int.TryParse(charPointsValue, out int points) ? points : 0;

            if (charPointsAvailable < cost)
            {
                MessageBox.Show($"Not enough Character Points. You need {cost} Character Points to increase {forceSkill}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Deduct the Character Points
            charPointsAvailable -= cost;
            charPoints.Text = charPointsAvailable.ToString();

            // Update the Force skill value
            forceSkillBox.Text = newValue;

            // Log the change in the xpLog
            AddToXPLog(forceSkill, "Force", currentValue, newValue, cost);
        }



        private string ShowOptionPopup(string message, string[] options)
        {
            using (Form popup = new Form())
            {
                popup.Text = "Select an Option";
                popup.Size = new Size(300, 200);
                popup.StartPosition = FormStartPosition.CenterParent;

                // Create the question label
                Label label = new Label
                {
                    Text = message,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(10, 10),
                    Size = new Size(280, 30) // Ensures the label is properly sized
                };
                popup.Controls.Add(label);

                // Create the dropdown
                ComboBox comboBox = new ComboBox
                {
                    Location = new Point(10, label.Bottom + 10), // Place the ComboBox below the label
                    Size = new Size(260, 30), // Appropriately size the dropdown
                    DataSource = options, // Set options as data source
                    DropDownStyle = ComboBoxStyle.DropDownList // Make it a dropdown list
                };
                popup.Controls.Add(comboBox);

                // Create the confirm button
                Button confirmButton = new Button
                {
                    Text = "Confirm",
                    Size = new Size(100, 30),
                    Location = new Point(popup.ClientSize.Width / 2 - 50, comboBox.Bottom + 20), // Center below the dropdown
                    Anchor = AnchorStyles.Bottom // Ensures the button stays in place if the window resizes
                };
                confirmButton.Click += (s, e) => popup.DialogResult = DialogResult.OK;
                popup.Controls.Add(confirmButton);

                popup.AcceptButton = confirmButton;

                // Show the dialog and return the selected item, or null if canceled
                return popup.ShowDialog() == DialogResult.OK ? comboBox.SelectedItem.ToString() : null;
            }
        }



        private void IncreaseForceSkillByAttributeDice(string forceSkill)
        {
            // Prompt the user for a dice value
            string input = ShowDiceInputPopup($"Enter the dice value to add to {forceSkill}:");
            if (string.IsNullOrWhiteSpace(input)) return; // User canceled

            if (!IsValidDiceValue(input)) return;

            // Parse the input dice value
            int inputDice, inputPips;
            ParseDiceValue(input, out inputDice, out inputPips);
            int inputTotalPips = (inputDice * 3) + inputPips;

            // Get the corresponding Force skill textbox
            TextBox forceSkillBox = GetForceSkillBox(forceSkill);
            if (forceSkillBox == null) return;

            // Get the current value of the Force skill
            string currentValue = forceSkillBox.Text.Trim();
            int currentDice, currentPips;
            ParseDiceValue(currentValue, out currentDice, out currentPips);
            int currentTotalPips = (currentDice * 3) + currentPips;

            // Calculate the new total pips for the Force skill
            int newTotalPips = currentTotalPips + inputTotalPips;

            // Adjust the currentAttribute based on the input value
            string currentAttributeValue = currentAttribute.Text.Trim();
            int attributeDice, attributePips;
            ParseDiceValue(currentAttributeValue, out attributeDice, out attributePips);
            int attributeTotalPips = (attributeDice * 3) + attributePips;

            // Check if there are enough attribute dice
            if (inputTotalPips > attributeTotalPips)
            {
                MessageBox.Show("Not enough Attribute Dice available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Deduct the input pips from the currentAttribute
            attributeTotalPips -= inputTotalPips;
            int newAttributeDice = attributeTotalPips / 3;
            int newAttributePips = attributeTotalPips % 3;
            currentAttribute.Text = newAttributePips > 0 ? $"{newAttributeDice}D+{newAttributePips}" : $"{newAttributeDice}D";

            // Update the Force skill value
            int newForceDice = newTotalPips / 3;
            int newForcePips = newTotalPips % 3;
            forceSkillBox.Text = newForcePips > 0 ? $"{newForceDice}D+{newForcePips}" : $"{newForceDice}D";
        }



        private void IncreaseForceSkillBySkillDice(string forceSkill)
        {
            // Prompt the user for a dice value
            string input = ShowDiceInputPopup($"Enter the dice value to add to {forceSkill}:");
            if (string.IsNullOrWhiteSpace(input)) return; // User canceled

            if (!IsValidDiceValue(input)) return;

            // Parse the input dice value
            int inputDice, inputPips;
            ParseDiceValue(input, out inputDice, out inputPips);
            int inputTotalPips = (inputDice * 3) + inputPips;

            // Get the corresponding Force skill textbox
            TextBox forceSkillBox = GetForceSkillBox(forceSkill);
            if (forceSkillBox == null) return;

            // Get the current value of the Force skill
            string currentValue = forceSkillBox.Text.Trim();
            int currentDice, currentPips;
            ParseDiceValue(currentValue, out currentDice, out currentPips);
            int currentTotalPips = (currentDice * 3) + currentPips;

            // Calculate the new total pips for the Force skill
            int newTotalPips = currentTotalPips + inputTotalPips;

            // Adjust the skillDice based on the input value
            string skillDiceValue = skillDice.Text.Trim();
            int skillDiceRemaining, skillPipsRemaining;
            ParseDiceValue(skillDiceValue, out skillDiceRemaining, out skillPipsRemaining);
            int skillTotalPips = (skillDiceRemaining * 3) + skillPipsRemaining;

            // Check if there are enough skill dice
            if (inputTotalPips > skillTotalPips)
            {
                MessageBox.Show("Not enough Skill Dice available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Deduct the input pips from the skillDice pool
            skillTotalPips -= inputTotalPips;
            int newSkillDice = skillTotalPips / 3;
            int newSkillPips = skillTotalPips % 3;
            skillDice.Text = newSkillPips > 0 ? $"{newSkillDice}D+{newSkillPips}" : $"{newSkillDice}D";

            // Update the Force skill value
            int newForceDice = newTotalPips / 3;
            int newForcePips = newTotalPips % 3;
            forceSkillBox.Text = newForcePips > 0 ? $"{newForceDice}D+{newForcePips}" : $"{newForceDice}D";
        }


        private TextBox GetForceSkillBox(string forceSkill)
        {
            switch (forceSkill)
            {
                case "Control":
                    return forceCon;
                case "Sense":
                    return forceSense;
                case "Alter":
                    return forceAlter;
                default:
                    return null;
            }
        }




        private void UpdateForceSkillTextbox(string forceSkill, string diceValue)
        {
            TextBox targetTextbox = null;

            switch (forceSkill)
            {
                case "Control":
                    targetTextbox = forceCon;
                    break;
                case "Sense":
                    targetTextbox = forceSense;
                    break;
                case "Alter":
                    targetTextbox = forceAlter;
                    break;
            }

            if (targetTextbox != null)
            {
                string currentValue = targetTextbox.Text.Trim();
                string newValue;

                // Add the new dice value to the current value
                if (TryAddDiceValues(currentValue, diceValue, out newValue))
                {
                    targetTextbox.Text = newValue;
                }
                else
                {
                    MessageBox.Show("Error updating the Force skill value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        ///////////////////////////////////////////CP Log/////////////////////////////////////////////

        private void ConfigureXpLog()
        {
            xpLog.View = View.Details; // Enable details view
            xpLog.FullRowSelect = true; // Enable full-row selection
            xpLog.Columns.Clear();

            // Add columns
            xpLog.Columns.Add("Name", 150); // Attribute name
            xpLog.Columns.Add("Type", 100); // Type (e.g., Attribute)
            xpLog.Columns.Add("Previous Value", 150); // Previous dice value
            xpLog.Columns.Add("New Value", 150); // New dice value
            xpLog.Columns.Add("Cost", 100); // Character Points spent
        }


        private void LogAttributeUpgrade(string attributeName, string previousValue, string newValue, int cost)
        {
            // Create a new ListViewItem
            ListViewItem logItem = new ListViewItem(attributeName); // Name column
            logItem.SubItems.Add("Attribute"); // Type column
            logItem.SubItems.Add(previousValue); // Previous Value column
            logItem.SubItems.Add(newValue); // New Value column
            logItem.SubItems.Add(cost.ToString()); // Cost column

            // Add the item to xpLog
            xpLog.Items.Add(logItem);
        }


        private void LogSkillUpgrade(string skillName, string previousValue, string newValue, int cost)
        {
            ListViewItem logEntry = new ListViewItem(skillName); // Skill Name
            logEntry.SubItems.Add(skillName.StartsWith("-") ? "Specialization" : "Skill"); // Type
            logEntry.SubItems.Add(previousValue);                // Previous Value
            logEntry.SubItems.Add(newValue);                     // New Value
            logEntry.SubItems.Add(cost.ToString());              // Cost

            xpLog.Items.Add(logEntry);
        }

        private void AddToXPLog(string name, string type, string previousValue, string newValue, int cost)
        {
            var listViewItem = new ListViewItem(name)
            {
                SubItems =
        {
            type,
            previousValue,
            newValue,
            cost.ToString()
        }
            };

            xpLog.Items.Add(listViewItem);
        }


        ///////////////////////////////////////////Everything below is Campaign Editing Data/////////////////////////////////////////////

        private void dexRename_TextChanged(object sender, EventArgs e)
        {
            dexLabel.Text = dexRename.Text;
            dexSkillCamp.Text = dexRename.Text;
        }

        private void knowRename_TextChanged(object sender, EventArgs e)
        {
            knowLabel.Text = knowRename.Text;
            knowSkillCamp.Text = knowRename.Text;
        }

        private void mechRename_TextChanged(object sender, EventArgs e)
        {
            mechLabel.Text = mechRename.Text;
            mechSkillCamp.Text = mechRename.Text;
        }

        private void techRename_TextChanged(object sender, EventArgs e)
        {
            techLabel.Text = techRename.Text;
            techSkillCamp.Text = techRename.Text;
        }

        private void perRename_TextChanged(object sender, EventArgs e)
        {
            perLabel.Text = perRename.Text;
            perSkillCamp.Text = perRename.Text;
        }

        private void strRename_TextChanged(object sender, EventArgs e)
        {
            strLabel.Text = strRename.Text;
            strSkillCamp.Text = strRename.Text;
        }


        private void racialCampList_TextChanged(object sender, EventArgs e)
        {
            // Clear the ComboBox to avoid duplicates
            speciesBox.Items.Clear();

            // Add each line from the TextBox to the ComboBox
            string[] lines = racialCampList.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            speciesBox.Items.AddRange(lines);
        }


        private void ConfigureListView(ListView listView)
        {
            listView.View = View.Details;
            listView.Columns.Clear();
            listView.Columns.Add("Skill", 140); // First column for skill names
            listView.Columns.Add("Dice", 45); // Second column for dice values (initially empty)
            listView.FullRowSelect = true;
        }

        private void PopulateListViewFromListBox(ListView listView, ListBox listBox)
        {
            // Clear existing items in the ListView
            listView.Items.Clear();

            // Iterate over all items in the ListBox
            foreach (var item in listBox.Items)
            {
                string skillName = item.ToString().Trim();

                if (!string.IsNullOrEmpty(skillName))
                {
                    // Add the skill name to the ListView
                    var listViewItem = new ListViewItem(skillName); // First column (Skill)
                    listViewItem.SubItems.Add(""); // Leave the Dice column empty
                    listView.Items.Add(listViewItem);
                }
            }
        }



        private void campDex_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateListViewFromListBox(dexSkills, campDex); // Correct pairing: ListView (dexSkills), ListBox (campDex)
        }

        private void campKnow_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateListViewFromListBox(knowSkills, campKnow); // Correct pairing: ListView (knowSkills), ListBox (campKnow)
        }

        private void campTech_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateListViewFromListBox(techSkills, campTech); // Correct pairing: ListView (techSkills), ListBox (campTech)
        }

        private void campMech_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateListViewFromListBox(mechSkills, campMech); // Correct pairing: ListView (mechSkills), ListBox (campMech)
        }

        private void campPer_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateListViewFromListBox(perSkills, campPer); // Correct pairing: ListView (perSkills), ListBox (campPer)
        }

        private void campStr_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateListViewFromListBox(strSkills, campStr); // Correct pairing: ListView (strSkills), ListBox (campStr)
        }



        private void ConfigureContextMenus()
        {
            // List of all ListBoxes
            var listBoxes = new[] { campStr, campPer, campTech, campMech, campKnow, campDex };

            foreach (var listBox in listBoxes)
            {
                // Create a ContextMenuStrip
                var contextMenu = new ContextMenuStrip();

                // Create the "Add Skill" menu item
                var addSkillMenuItem = new ToolStripMenuItem("Add Skill");
                addSkillMenuItem.Click += (sender, e) => AddSkillToListBox(listBox);

                // Add the menu item to the ContextMenuStrip
                contextMenu.Items.Add(addSkillMenuItem);

                // Attach the ContextMenuStrip to the ListBox
                listBox.ContextMenuStrip = contextMenu;
            }
        }

        private void AddSkillToListBox(ListBox listBox)
        {
            // Prompt the user to enter a skill name
            using (var inputForm = new Form())
            {
                inputForm.Text = "Add Skill";
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.StartPosition = FormStartPosition.CenterParent;

                var label = new Label
                {
                    Text = "Name the skill you would like to add:",
                    AutoSize = true,
                    Top = 20,
                    Left = 10
                };

                var textBox = new TextBox
                {
                    Top = 50,
                    Left = 10,
                    Width = 260
                };

                var confirmButton = new Button
                {
                    Text = "Confirm",
                    DialogResult = DialogResult.OK,
                    Top = 80,
                    Left = 10
                };

                inputForm.Controls.Add(label);
                inputForm.Controls.Add(textBox);
                inputForm.Controls.Add(confirmButton);

                inputForm.AcceptButton = confirmButton;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    string skillName = textBox.Text.Trim();

                    // Validate the input
                    if (!string.IsNullOrEmpty(skillName))
                    {
                        listBox.Items.Add(skillName);
                    }
                    else
                    {
                        MessageBox.Show("Skill name cannot be empty.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }





        ///////////////////////////////////////////Everything below is Equipment tab data because I got tired of fighting the save/load function/////////////////////////////////////////////

        private void openShop_Click(object sender, EventArgs e)
        {
            // Create an instance of the EquipmentList form
            EquipmentList equipmentListForm = new EquipmentList();

            // Show the form as a dialog or non-modal window
            equipmentListForm.Show(); // Use ShowDialog() if you want it to block interaction with Form1
        }


        public void AddToInventory(EquipmentItem item, int quantity)
        {
            // Select the appropriate panel based on item type
            Panel targetPanel = null;

            if (item.Type.Equals("weapon", StringComparison.OrdinalIgnoreCase))
            {
                targetPanel = weapInv;
            }
            else if (item.Type.Equals("armor", StringComparison.OrdinalIgnoreCase))
            {
                targetPanel = armorInv;
            }
            else
            {
                targetPanel = gearInv;
            }

            if (targetPanel == null) return;

            // Check for duplicate items in the target panel
            foreach (Control control in targetPanel.Controls)
            {
                if (control is GroupBox existingGroupBox && existingGroupBox.Text == item.Name)
                {
                    MessageBox.Show("This item is already in your inventory.", "Duplicate Item", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            // Search for the full item details in the JSON files
            string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            string[] jsonFiles = { "Equipment.json", "General_Goods.json", "Weapons.json" };
            JObject fullItemDetails = null;

            foreach (var fileName in jsonFiles)
            {
                string filePath = Path.Combine(dataDirectory, fileName);
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    JObject rootObject = JObject.Parse(jsonContent);
                    if (rootObject["items"] is JArray itemsArray)
                    {
                        var matchingItem = itemsArray.FirstOrDefault(obj => obj["name"]?.ToString() == item.Name);
                        if (matchingItem != null)
                        {
                            fullItemDetails = (JObject)matchingItem;
                            break;
                        }
                    }
                }
            }

            if (fullItemDetails == null)
            {
                MessageBox.Show($"Could not find details for item: {item.Name}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Parse item details from the JSON
            string details = "";
            if (item.Type.Equals("weapon", StringComparison.OrdinalIgnoreCase))
            {
                string subtype = fullItemDetails["system"]?["subtype"]?.ToString() ?? "Unknown";
                int damageScore = fullItemDetails["system"]?["damage"]?["score"]?.ToObject<int>() ?? 0;
                string damageText = subtype.Equals("Melee", StringComparison.OrdinalIgnoreCase)
                    ? $"STR+{ConvertToDiceNotation(damageScore)}"
                    : ConvertToDiceNotation(damageScore);

                string skill = fullItemDetails["system"]?["stats"]?["skill"]?.ToString() ?? "Unknown";
                int ammo = fullItemDetails["system"]?["ammo"]?.ToObject<int>() ?? 0;
                int rangeShort = fullItemDetails["system"]?["range"]?["short"]?.ToObject<int>() ?? 0;
                int rangeMedium = fullItemDetails["system"]?["range"]?["medium"]?.ToObject<int>() ?? 0;
                int rangeLong = fullItemDetails["system"]?["range"]?["long"]?.ToObject<int>() ?? 0;

                string ammoText = ammo > 0 ? $"Ammo: {ammo}" : "";
                string rangeText = (rangeShort > 0 || rangeMedium > 0 || rangeLong > 0)
                    ? $"Short: {rangeShort} Medium: {rangeMedium} Long: {rangeLong}"
                    : "";

                details = $"{subtype}\nDamage: {damageText}\nSkill: {skill}\n" +
                          $"{(string.IsNullOrEmpty(ammoText) ? "" : ammoText + "\n")}" +
                          $"{(string.IsNullOrEmpty(rangeText) ? "" : rangeText + "\n")}" +
                          $"Quantity: {quantity}";
            }
            else if (item.Type.Equals("armor", StringComparison.OrdinalIgnoreCase))
            {
                int physicalArmor = fullItemDetails["system"]?["pr"]?.ToObject<int>() ?? 0;
                int energyArmor = fullItemDetails["system"]?["er"]?.ToObject<int>() ?? 0;

                string physicalArmorText = physicalArmor > 0 ? $"Physical Armor: {ConvertToDiceNotation(physicalArmor)}" : "";
                string energyArmorText = energyArmor > 0 ? $"Energy Armor: {ConvertToDiceNotation(energyArmor)}" : "";

                details = $"{(string.IsNullOrEmpty(physicalArmorText) ? "" : physicalArmorText + "\n")}" +
                          $"{(string.IsNullOrEmpty(energyArmorText) ? "" : energyArmorText + "\n")}" +
                          $"Cost: {item.Cost}\nQuantity: {quantity}";
            }
            else
            {
                details = $"Cost: {item.Cost}\nQuantity: {quantity}";
            }

            // Create the GroupBox for the item
            var groupBox = new GroupBox
            {
                Text = item.Name,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(10),
                Margin = new Padding(10),
                Font = new Font("Arial", 9, FontStyle.Bold),
                Dock = DockStyle.Top // Stack vertically
            };

            // Create a Label for the item details
            var detailsLabel = new Label
            {
                Text = details,
                AutoSize = true,
                Font = new Font("Arial", 8, FontStyle.Regular),
                MaximumSize = new Size(targetPanel.Width - 20, 0), // Word wrap within the panel's width
                Dock = DockStyle.Top
            };

            // Add the Label to the GroupBox
            groupBox.Controls.Add(detailsLabel);

            // Add a ContextMenuStrip to the GroupBox for additional options
            var contextMenu = new ContextMenuStrip();
            var removeMenuItem = new ToolStripMenuItem("Remove Item");
            removeMenuItem.Click += (s, e) => targetPanel.Controls.Remove(groupBox);

            contextMenu.Items.Add(removeMenuItem);
            groupBox.ContextMenuStrip = contextMenu;

            // Add the GroupBox to the target panel
            targetPanel.Controls.Add(groupBox);

            // Refresh the panel layout to ensure proper display
            targetPanel.Controls.SetChildIndex(groupBox, 0); // Ensures the latest item appears at the top
            targetPanel.PerformLayout();
        }

        // Helper method to convert pips to dice notation
        private string ConvertToDiceNotation(int pips)
        {
            int dice = pips / 3;  // 1D for every 3 pips
            int remainder = pips % 3;  // Remaining pips
            if (dice > 0 && remainder > 0)
            {
                return $"{dice}D+{remainder}";
            }
            else if (dice > 0)
            {
                return $"{dice}D";
            }
            else
            {
                return $"+{remainder}";
            }
        }



        private string AddDiceValues(string baseValue, string addValue)
        {
            // Parse the first value
            int baseDice, basePips;
            ParseDiceValue(baseValue, out baseDice, out basePips);

            // Parse the second value
            int addDice, addPips;
            ParseDiceValue(addValue, out addDice, out addPips);

            // Add the dice and pips
            int totalPips = (baseDice * 3 + basePips) + (addDice * 3 + addPips);
            int finalDice = totalPips / 3; // Convert total pips to dice
            int finalPips = totalPips % 3; // Remainder are pips

            // Return the result in Star Wars D6 format
            if (finalPips > 0)
                return $"{finalDice}D+{finalPips}";
            else
                return $"{finalDice}D";
        }

        private void ParseDiceValue(string value, out int dice, out int pips)
        {
            dice = 0;
            pips = 0;

            if (string.IsNullOrEmpty(value)) return;

            // Normalize to uppercase for consistency
            value = value.ToUpper();

            // Match full dice and pip notation (e.g., "1D+2", "2D", or "+1")
            var match = System.Text.RegularExpressions.Regex.Match(value, @"^(\d+)?D?(\+\d)?$");
            if (match.Success)
            {
                // Parse dice (if present)
                if (match.Groups[1].Success)
                {
                    dice = int.Parse(match.Groups[1].Value);
                }

                // Parse pips (if present)
                if (match.Groups[2].Success)
                {
                    pips = int.Parse(match.Groups[2].Value.Trim('+'));
                }
            }
        }



        public bool isLoading = false; // Flag to indicate loading state

        private void UpdateCurrentAttribute()
        {
            // Skip execution if loading state is active
            if (isLoading) return;

            // Get the values from the textboxes
            string baseValue = racialBase.Text.Trim();
            string addValue = attAdd.Text.Trim();

            // Calculate the total dice value
            string totalValue = AddDiceValues(baseValue, addValue);

            // Display the result in currentAttribute
            currentAttribute.Text = totalValue;
        }


        private void racialBase_TextChanged(object sender, EventArgs e)
        {
            if (isLoading) return;

            UpdateCurrentAttribute();
        }

        private void attAdd_TextChanged(object sender, EventArgs e)
        {
            if (isLoading) return;

            UpdateCurrentAttribute();
        }





        //////////////////////////////////////////////////////////// Force Powers Code Below //////////////////////////////////////////////////////////////////////////


        private List<Panel> forcePowerPanels = new List<Panel>();
        private int currentPanelIndex = 0;

        private void InitializeForcePanels()
        {
            // Add the initial panel to the list
            forcePowerPanels.Add(forcePowersPanel);
        }

        private void DebugInitialSetup()
        {
            Console.WriteLine("Checking initial setup...");

            // Check if forcePowersPanel is part of tabForce
            if (forcePowersPanel.Parent != null && forcePowersPanel.Parent == tabForce)
            {
                Console.WriteLine("forcePowersPanel is correctly placed inside tabForce.");
            }
            else
            {
                Console.WriteLine("forcePowersPanel is not correctly placed inside tabForce.");
            }

            // Check if tabForce is part of a TabControl
            if (tabForce.Parent is TabControl parentTabControl)
            {
                Console.WriteLine("tabForce is correctly placed inside a TabControl.");
                tabControl = parentTabControl; // Ensure tabControl is correctly referenced
            }
            else
            {
                Console.WriteLine("tabForce is not correctly placed inside a TabControl.");
            }
        }



        // Add a Force Power to the Panel
        public void AddForcePower(string powerName, string descriptionHtml)
        {
            Console.WriteLine($"Adding Force Power: {powerName}");

            // Parse a short description
            string shortDescription = ExtractShortDescription(descriptionHtml);

            // Create GroupBox
            var groupBox = new GroupBox
            {
                Text = powerName,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(10),
                Margin = new Padding(10),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            // Create Label
            var descriptionLabel = new Label
            {
                Text = shortDescription,
                AutoSize = true,
                Font = new Font("Arial", 8, FontStyle.Regular),
                MaximumSize = new Size(forcePowersPanel.Width - 40, 0)
            };
            groupBox.Controls.Add(descriptionLabel);

            Console.WriteLine("Created GroupBox. Adding to current panel...");
            AddGroupBoxToCurrentPanel(groupBox);
        }




        /// <summary>
        /// Adds the group box to the current panel or creates a new tab if needed.
        /// </summary>
        private void CreateNewTabAndPanel()
        {
            // Increment the current panel index
            currentPanelIndex++;

            // Create a new TabPage
            var newTab = new TabPage($"Force Page {currentPanelIndex + 1}");

            // Create a new panel for the new tab
            var newPanel = new Panel
            {
                Name = $"forcePowersPanel{currentPanelIndex + 1}",
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Size = forcePowersPanel.Size // Match the size of the original panel
            };

            // Add the panel to the new TabPage
            newTab.Controls.Add(newPanel);

            // Add the TabPage to the TabControl
            tabControl.TabPages.Add(newTab);

            // Add the new panel to the list of panels
            forcePowerPanels.Add(newPanel);

            Console.WriteLine($"Created new tab and panel: {newTab.Text}");
        }



        private void AddGroupBoxToCurrentPanel(GroupBox groupBox)
        {
            // Ensure we have at least one panel
            if (forcePowerPanels.Count == 0)
            {
                Console.WriteLine("No panels found. Adding the initial panel.");
                forcePowerPanels.Add(forcePowersPanel); // Add the initial panel
                currentPanelIndex = 0;
            }

            // Get the current panel
            var currentPanel = forcePowerPanels[currentPanelIndex];

            // Add the GroupBox to the current panel
            currentPanel.Controls.Add(groupBox);

            // Measure total height of all controls
            int totalHeight = currentPanel.Controls.Cast<Control>().Sum(c => c.Height + c.Margin.Vertical);

            Console.WriteLine($"Adding GroupBox. Total Height: {totalHeight}, Panel Height: {currentPanel.Height}");

            // Check if adding this group box exceeds the current panel's height
            if (totalHeight > currentPanel.Height)
            {
                Console.WriteLine($"Overflow detected. Creating a new panel and moving group box to Force Page {currentPanelIndex + 2}.");

                // Remove the overflowing GroupBox
                currentPanel.Controls.Remove(groupBox);

                // Create a new panel and tab
                CreateNewTabAndPanel();

                // Add the GroupBox to the new panel
                forcePowerPanels[currentPanelIndex].Controls.Add(groupBox);

                Console.WriteLine($"GroupBox moved to new panel: Force Page {currentPanelIndex + 2}");
            }

            // Refresh the layout
            currentPanel.PerformLayout();
        }













        // Extract a short description from the HTML
        private string ExtractShortDescription(string htmlDescription)
        {
            // Use the HTMLConversion class to extract difficulties
            var difficulties = HTMLConversion.ExtractDifficulties(htmlDescription);

            // Combine extracted difficulties into a short description
            if (difficulties.ContainsKey("Error"))
            {
                return difficulties["Error"];
            }

            return string.Join(Environment.NewLine, difficulties.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        }


        public void AddForcePowerToPanel(string powerName)
        {
            // Check if the forcePowersPanel already has this power
            foreach (var panel in forcePowerPanels)
            {
                foreach (Control existingControl in panel.Controls)
                {
                    if (existingControl is GroupBox existingGroupBox && existingGroupBox.Text == powerName)
                    {
                        MessageBox.Show("This Force Power is already added.", "Duplicate Power", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }

            // Path to the JSON file
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "force_powers.json");

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Force powers data file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string description = "No description available.";
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                JObject rootObject = JObject.Parse(jsonContent);
                JArray powersArray = (JArray)rootObject["items"];

                var power = powersArray.FirstOrDefault(p => p["name"]?.ToString() == powerName);
                if (power != null)
                {
                    description = ExtractShortDescription(power["system"]?["description"]?.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading force power: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create the GroupBox for the force power
            var groupBox = new GroupBox
            {
                Text = powerName, // Set the Force Power name as the GroupBox title
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(10),
                Margin = new Padding(10),
                Font = new Font("Arial", 9, FontStyle.Bold),
                Dock = DockStyle.Top
            };

            // Create a Label for the short description
            var descriptionLabel = new Label
            {
                Text = description,
                AutoSize = true,
                Font = new Font("Arial", 8, FontStyle.Regular),
                MaximumSize = new Size(forcePowersPanel.Width - 40, 0), // Word wrap within the panel's width
                Dock = DockStyle.Top
            };
            groupBox.Controls.Add(descriptionLabel);

            // Add a ContextMenuStrip to the GroupBox for additional options
            var contextMenu = new ContextMenuStrip();
            var removeMenuItem = new ToolStripMenuItem("Remove Force Power");
            removeMenuItem.Click += (s, e) =>
            {
                foreach (var panel in forcePowerPanels)
                {
                    if (panel.Controls.Contains(groupBox))
                    {
                        panel.Controls.Remove(groupBox); // Remove the GroupBox from the panel
                        panel.PerformLayout(); // Refresh the layout
                        break;
                    }
                }
            };

            contextMenu.Items.Add(removeMenuItem);
            groupBox.ContextMenuStrip = contextMenu;

            // Add the GroupBox to the current panel
            AddGroupBoxToCurrentPanel(groupBox);
        }






        private void openLearning_Click(object sender, EventArgs e)
        {
            // Create and show the Learning form
            var learningForm = new Learning();
            learningForm.Show();
        }



    }
}





