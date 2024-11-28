using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Star_Wars_D6
{
    public static class Export
    {
        public static void GenerateJson(
            string characterName,
            string type,
            string imagePath,
            Form1 mainForm,
            string species,
            string move,
            string fatepoints,
            string custom1,
            string characterpoints,
            bool fatepointeffect,
            string racialBase,
            string credits)
        {
            // Load the JSON template from the Data folder
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Export_Template.json");
            string jsonContent = File.ReadAllText(templatePath);

            // Parse the JSON content
            JObject jsonObject = JObject.Parse(jsonContent);

            // Access tabMain for attributes and skills
            var tabMain = mainForm.tabControl.TabPages["tabMain"];
            var playerCredits = mainForm.playerCreditsTextBox.Text;

            // Get attributes (assuming TextBox controls inside tabMain)
            string dexAtt = mainForm.dexAtt.Text;
            string knowAtt = mainForm.knowAtt.Text;
            string mechAtt = mainForm.mechAtt.Text;
            string perAtt = mainForm.perAtt.Text;
            string techAtt = mainForm.techAtt.Text;
            string strAtt = mainForm.strAtt.Text;

            // Get skills from ListViews
            var dexSkills = GetSkillsFromListView(mainForm.dexSkills);
            var knowSkills = GetSkillsFromListView(mainForm.knowSkills);
            var mechSkills = GetSkillsFromListView(mainForm.mechSkills);
            var perSkills = GetSkillsFromListView(mainForm.perSkills);
            var techSkills = GetSkillsFromListView(mainForm.techSkills);
            var strSkills = GetSkillsFromListView(mainForm.strSkills);

            // Set general properties
            jsonObject["name"] = characterName;
            jsonObject["type"] = "character";
            jsonObject["system"]["chartype"]["content"] = type;
            jsonObject["system"]["species"]["content"] = species;
            jsonObject["system"]["move"]["value"] = ParseIntOrDefault(move);
            jsonObject["system"]["fatepoints"]["value"] = ParseIntOrDefault(fatepoints);
            jsonObject["system"]["custom1"]["value"] = ParseIntOrDefault(custom1);
            jsonObject["system"]["characterpoints"]["value"] = ParseIntOrDefault(characterpoints);
            jsonObject["prototypeToken"]["name"] = characterName;
            

                // Export fatepointeffect based on the checkbox in the form
                jsonObject["system"]["fatepointeffect"] = fatepointeffect;

            // Update metaphysics extranormal logic
            UpdateMetaphysicsExtranormal(jsonObject, mainForm);

            // Add credits to JSON
            jsonObject["system"]["credits"] = new JObject
            {
                ["type"] = "Number",
                ["label"] = "OD6S.CHAR_CREDITS",
                ["value"] = ParseIntOrDefault(playerCredits)
            };

            // Set attributes
            jsonObject["system"]["attributes"]["agi"]["base"] = ConvertToPips(dexAtt);
            jsonObject["system"]["attributes"]["kno"]["base"] = ConvertToPips(knowAtt);
            jsonObject["system"]["attributes"]["mec"]["base"] = ConvertToPips(mechAtt);
            jsonObject["system"]["attributes"]["per"]["base"] = ConvertToPips(perAtt);
            jsonObject["system"]["attributes"]["tec"]["base"] = ConvertToPips(techAtt);
            jsonObject["system"]["attributes"]["str"]["base"] = ConvertToPips(strAtt);

            // Add skills and specializations
            AddSkills(jsonObject, "agi", ConvertToPips(dexAtt), dexSkills);
            AddSkills(jsonObject, "kno", ConvertToPips(knowAtt), knowSkills);
            AddSkills(jsonObject, "mec", ConvertToPips(mechAtt), mechSkills);
            AddSkills(jsonObject, "per", ConvertToPips(perAtt), perSkills);
            AddSkills(jsonObject, "tec", ConvertToPips(techAtt), techSkills);
            AddSkills(jsonObject, "str", ConvertToPips(strAtt), strSkills);

            // Update force skill scores
            UpdateForceSkillScores(jsonObject, mainForm);

            // add force powers and items
            AddDynamicItemsToExport(jsonObject, mainForm);

            // Update the background content
            string backgroundText = mainForm.backgroundBack.Text.Trim(); // Get the text from the textbox
            if (!string.IsNullOrEmpty(backgroundText))
            {
                // Wrap the text in <p> tags
                string formattedBackground = $"<p>{System.Net.WebUtility.HtmlEncode(backgroundText)}</p>";

                // Set the content in the JSON
                jsonObject["system"]["background"]["content"] = formattedBackground;
            }
            else
            {
                // If the textbox is empty, clear the content field
                jsonObject["system"]["background"]["content"] = "<p></p>";
            }

            // Update the description content
            string descriptionText = mainForm.phyBack.Text.Trim(); // Get the text from the phyBack textbox
            if (!string.IsNullOrEmpty(descriptionText))
            {
                // Wrap the text in <p> tags
                string formattedDescription = $"<p>{System.Net.WebUtility.HtmlEncode(descriptionText)}</p>";

                // Set the content in the JSON
                jsonObject["system"]["description"]["content"] = formattedDescription;
            }
            else
            {
                // If the textbox is empty, clear the content field
                jsonObject["system"]["description"]["content"] = "<p></p>";
            }

            // Update the personality content
            string personalityText = mainForm.perBack.Text.Trim(); // Get the text from the perBack textbox
            if (!string.IsNullOrEmpty(personalityText))
            {
                // Wrap the text in <p> tags
                string formattedPersonality = $"<p>{System.Net.WebUtility.HtmlEncode(personalityText)}</p>";

                // Set the content in the JSON
                jsonObject["system"]["personality"]["content"] = formattedPersonality;
            }
            else
            {
                // If the textbox is empty, clear the content field
                jsonObject["system"]["personality"]["content"] = "<p></p>";
            }

            // Update the gender content
            string genderText = mainForm.genderBox.Text.Trim(); // Get the text from the genderBox textbox
            jsonObject["system"]["gender"]["content"] = string.IsNullOrEmpty(genderText) ? "" : genderText;

            // Update the age content
            string ageText = mainForm.charAge.Text.Trim(); // Get the text from the charAge textbox
            jsonObject["system"]["age"]["content"] = string.IsNullOrEmpty(ageText) ? "" : ageText;

            // Update the height content
            string heightText = mainForm.charHeight.Text.Trim(); // Get the text from the charHeight textbox
            jsonObject["system"]["height"]["content"] = string.IsNullOrEmpty(heightText) ? "" : heightText;

            // Update the weight content
            string weightText = mainForm.charWeight.Text.Trim(); // Get the text from the charWeight textbox
            jsonObject["system"]["weight"]["content"] = string.IsNullOrEmpty(weightText) ? "" : weightText;

            // Save the modified JSON
            SaveJsonFile(jsonObject, characterName);

            



        }

        private static void UpdateMetaphysicsExtranormal(JObject jsonObject, Form1 mainForm)
        {
            var metaphysicsSection = jsonObject["system"]?["metaphysicsextranormal"];
            if (metaphysicsSection != null)
            {
                metaphysicsSection["value"] = mainForm.fateBox.Checked;
            }
        }

        private static List<KeyValuePair<string, string>> GetSkillsFromListView(ListView skillListView)
        {
            var skills = new List<KeyValuePair<string, string>>();
            foreach (ListViewItem item in skillListView.Items)
            {
                string skillName = item.Text; // Skill Name column
                string skillValue = item.SubItems[1].Text; // Die Value column
                skills.Add(new KeyValuePair<string, string>(skillName, skillValue));
            }
            return skills;
        }

        private static void AddSkills(JObject jsonObject, string attributeKey, int attributePips, List<KeyValuePair<string, string>> skills)
        {
            JArray itemsArray = (JArray)jsonObject["items"];
            JObject parentSkill = null;

            foreach (var skill in skills)
            {
                string skillName = skill.Key.Trim();
                int skillPips = ConvertToPips(skill.Value);

                int baseValue = skillPips - attributePips;
                if (baseValue < 0) baseValue = 0;

                if (skillName.StartsWith("-"))
                {
                    string specializationName = skillName.Substring(1).Trim();

                    if (parentSkill != null)
                    {
                        int specializationBaseValue = skillPips - attributePips;
                        if (specializationBaseValue < 0) specializationBaseValue = 0;

                        JObject specialization = new JObject
                        {
                            ["name"] = specializationName,
                            ["type"] = "specialization",
                            ["system"] = new JObject
                            {
                                ["description"] = specializationName,
                                ["attribute"] = attributeKey,
                                ["base"] = specializationBaseValue,
                                ["skill"] = parentSkill["name"]
                            },
                            ["img"] = "icons/svg/item-bag.svg"
                        };

                        itemsArray.Add(specialization);
                    }
                }
                else
                {
                    parentSkill = itemsArray.FirstOrDefault(item =>
                        (string)item["name"] == skillName && (string)item["type"] == "skill") as JObject;

                    if (parentSkill == null)
                    {
                        parentSkill = new JObject
                        {
                            ["name"] = skillName,
                            ["type"] = "skill",
                            ["system"] = new JObject
                            {
                                ["description"] = skillName,
                                ["attribute"] = attributeKey,
                                ["base"] = baseValue
                            },
                            ["img"] = "icons/svg/item-bag.svg"
                        };
                        itemsArray.Add(parentSkill);
                    }
                    else
                    {
                        parentSkill["system"]["base"] = baseValue;
                    }
                }
            }
        }

        private static void UpdateForceSkillScores(JObject jsonObject, Form1 mainForm)
        {
            // Map force skill names to their textboxes
            var forceSkillsMap = new Dictionary<string, TextBox>
    {
        { "Control", mainForm.forceCon },
        { "Sense", mainForm.forceSense },
        { "Alter", mainForm.forceAlter }
    };

            // Iterate through each force skill
            foreach (var skillEntry in forceSkillsMap)
            {
                string skillName = skillEntry.Key;
                TextBox skillTextBox = skillEntry.Value;

                // Check if the textbox has a value
                if (!string.IsNullOrWhiteSpace(skillTextBox.Text))
                {
                    // Convert dice notation to pips
                    int baseValue = ConvertToPips(skillTextBox.Text);

                    // Locate the corresponding item in the JSON and update the base field
                    var skillItem = ((JArray)jsonObject["items"])
                        .FirstOrDefault(item => (string)item["name"] == skillName);

                    if (skillItem != null)
                    {
                        skillItem["system"]["base"] = baseValue;
                    }
                }
            }
        }

        private static void AddDynamicItemsToExport(JObject jsonObject, Form1 mainForm)
        {
            // File paths for JSON data
            string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            string[] jsonFiles = { "Equipment.json", "force_powers.json", "General_Goods.json", "Weapons.json" };

            // Panels for equipment, armor, gear, and weapons
            var fixedPanels = new Dictionary<string, Panel>
    {
        { "Weapons", mainForm.weapInv },
        { "Armor", mainForm.armorInv },
        { "Gear", mainForm.gearInv }
    };

            // Process fixed panels
            foreach (var panelEntry in fixedPanels)
            {
                string panelName = panelEntry.Key;
                Panel panel = panelEntry.Value;

                foreach (GroupBox groupBox in panel.Controls.OfType<GroupBox>())
                {
                    string itemName = groupBox.Text;

                    // Search for the item in the JSON files
                    JObject matchingItem = FindItemInJsonFiles(itemName, jsonFiles, baseDirectory);

                    if (matchingItem != null)
                    {
                        // Extract the quantity from the GroupBox details
                        int quantity = ExtractQuantityFromGroupBox(groupBox);

                        // Update the item's quantity field
                        matchingItem["system"]["quantity"] = quantity;

                        // Add the found item to the export's items array
                        ((JArray)jsonObject["items"]).Add(matchingItem);
                    }
                    else
                    {
                        MessageBox.Show($"Item '{itemName}' from {panelName} not found in the data files.", "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            // Dynamically find all panels that contain Force Powers
            List<Panel> forcePanels = new List<Panel>();
            foreach (TabPage tabPage in mainForm.tabControl.TabPages)
            {
                foreach (Control control in tabPage.Controls)
                {
                    if (control is Panel panel && panel.Name.StartsWith("forcePowersPanel"))
                    {
                        forcePanels.Add(panel);
                    }
                }
            }

            // Process all Force Power panels
            foreach (var panel in forcePanels)
            {
                foreach (GroupBox groupBox in panel.Controls.OfType<GroupBox>())
                {
                    string itemName = groupBox.Text;

                    // Search for the item in the JSON files
                    JObject matchingItem = FindItemInJsonFiles(itemName, jsonFiles, baseDirectory);

                    if (matchingItem != null)
                    {
                        // Extract the quantity from the GroupBox details
                        int quantity = ExtractQuantityFromGroupBox(groupBox);

                        // Update the item's quantity field
                        matchingItem["system"]["quantity"] = quantity;

                        // Add the found item to the export's items array
                        ((JArray)jsonObject["items"]).Add(matchingItem);
                    }
                    else
                    {
                        MessageBox.Show($"Force Power '{itemName}' not found in the data files.", "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }


        // Helper: Extract the quantity value from a GroupBox
        private static int ExtractQuantityFromGroupBox(GroupBox groupBox)
        {
            // Iterate through all controls in the GroupBox
            foreach (Control control in groupBox.Controls)
            {
                // Check if the control is a Label
                if (control is Label label)
                {
                    // Split the Label text by lines to account for multi-line content
                    var lines = label.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    // Search each line for "Quantity:"
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("Quantity:", StringComparison.OrdinalIgnoreCase))
                        {
                            // Extract and parse the quantity value
                            string[] parts = line.Split(':');
                            if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out int quantity))
                            {
                                return quantity; // Return the parsed quantity
                            }
                        }
                    }
                }
            }

            // Default quantity if not found or parsing fails
            return 1;
        }


        // Helper: Search for an item in JSON files
        private static JObject FindItemInJsonFiles(string itemName, string[] jsonFiles, string baseDirectory)
        {
            foreach (string fileName in jsonFiles)
            {
                string filePath = Path.Combine(baseDirectory, fileName);

                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    JObject rootObject = JObject.Parse(jsonContent);

                    // Check for a matching item in the "items" array
                    var matchingItem = rootObject["items"]?
                        .FirstOrDefault(item => (string)item["name"] == itemName);

                    if (matchingItem != null)
                    {
                        // Return a deep clone of the matching item
                        return (JObject)matchingItem.DeepClone();
                    }
                }
            }
            return null; // Return null if not found
        }




        private static int ConvertToPips(string diceValue)
        {
            int totalPips = 0;
            diceValue = diceValue.ToUpper();

            if (diceValue.Contains('D'))
            {
                string[] parts = diceValue.Split('D');
                if (int.TryParse(parts[0], out int dice))
                    totalPips += dice * 3;

                if (parts.Length > 1 && int.TryParse(parts[1].Replace("+", ""), out int pips))
                    totalPips += pips;
            }

            return totalPips;
        }

        private static int ParseIntOrDefault(string value, int defaultValue = 0)
        {
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        private static void SaveJsonFile(JObject jsonObject, string characterName)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON files (*.json)|*.json";
                saveFileDialog.FileName = $"{characterName}.json";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, jsonObject.ToString(Newtonsoft.Json.Formatting.Indented));
                    MessageBox.Show("File saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
