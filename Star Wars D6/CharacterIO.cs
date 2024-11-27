using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Star_Wars_D6;

public class CharacterIO
{
    private Form1 mainForm;

    public CharacterIO(Form1 form)
    {
        mainForm = form;
    }

    // Save character data to a JSON file
    public static void SaveCharacter(Form1 form, string filePath)
    {
        var characterData = new
        {
            Name = form.characterName.Text,
            Age = form.charAge.Text,
            Height = form.charHeight.Text,
            Weight = form.charWeight.Text,
            CharPoints = form.charPoints.Text,
            ForcePoints = form.forcePoints.Text,
            DSPoints = form.dsPoints.Text,
            CurrentAttribute = form.currentAttribute.Text,
            SkillDice = form.skillDice.Text,
            SpecialDice = form.specialDice.Text,
            Dexterity = form.dexAtt.Text,
            Knowledge = form.knowAtt.Text,
            Mechanical = form.mechAtt.Text,
            Perception = form.perAtt.Text,
            Strength = form.strAtt.Text,
            Technical = form.techAtt.Text,
            RacialBase = form.racialBase.Text,
            Quote = form.charQuote.Text,
            MoveAttribute = form.moveAtt.Text,
            RacialAbilities = form.raceAbil.Text,
            RacialAbilitiesLong = form.racialAbil.Text,
            PerBack = form.perBack.Text,
            PhyBack = form.phyBack.Text,
            AlliesBack = form.alliesBack.Text,
            EnemiesBack = form.enemiesBack.Text,
            BackgroundBack = form.backgroundBack.Text,
            EquipNotes = form.equipNotes.Text,
            PlayerCredits = form.playerCreditsTextBox.Text,
            FateBox = form.fateBox.Checked,
            CheckBoxCharacterCreation = form.checkBoxCharacterCreation.Checked,
            SpecialConvert = form.specialConvert.Checked,
            Species = form.speciesBox.SelectedItem?.ToString(),
            Type = form.typeBox.Text,
            Image = form.imageDisplay.Image != null
                ? Convert.ToBase64String((byte[])new ImageConverter().ConvertTo(form.imageDisplay.Image, typeof(byte[])))
                : null,
            DexSkills = GetSkillData(form.dexSkills),
            KnowSkills = GetSkillData(form.knowSkills),
            MechSkills = GetSkillData(form.mechSkills),
            TechSkills = GetSkillData(form.techSkills),
            PerSkills = GetSkillData(form.perSkills),
            StrSkills = GetSkillData(form.strSkills),
            ForceCon = form.forceCon.Text,
            ForceSense = form.forceSense.Text,
            ForceAlter = form.forceAlter.Text,
            ForcePowers = form.forcePowersPanel.Controls
                .OfType<GroupBox>()
                .Select(g => g.Text)
                .ToList(),
            Weapons = form.weapInv.Controls
                .OfType<GroupBox>()
                .Select(g => g.Text)
                .ToList(),
            Armor = form.armorInv.Controls
                .OfType<GroupBox>()
                .Select(g => g.Text)
                .ToList(),
            Gear = form.gearInv.Controls
                .OfType<GroupBox>()
                .Select(g => g.Text)
                .ToList()
        };

        string json = JsonConvert.SerializeObject(characterData, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }






    private static List<object> GetSkillData(ListView listView)
    {
        var skills = new List<object>();
        foreach (ListViewItem item in listView.Items)
        {
            skills.Add(new
            {
                Name = item.SubItems[0].Text,
                Dice = item.SubItems[1].Text
            });
        }
        return skills;
    }


    // Load method
    public static void LoadCharacter(Form1 form, string filePath)
    {
        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        dynamic characterData = JsonConvert.DeserializeObject(json);

        form.characterName.Text = characterData.Name;
        form.charAge.Text = characterData.Age;
        form.charHeight.Text = characterData.Height;
        form.charWeight.Text = characterData.Weight;
        form.charPoints.Text = characterData.CharPoints;
        form.forcePoints.Text = characterData.ForcePoints;
        form.dsPoints.Text = characterData.DSPoints;
        form.currentAttribute.Text = characterData.CurrentAttribute;
        form.skillDice.Text = characterData.SkillDice;
        form.specialDice.Text = characterData.SpecialDice;
        form.dexAtt.Text = characterData.Dexterity;
        form.knowAtt.Text = characterData.Knowledge;
        form.mechAtt.Text = characterData.Mechanical;
        form.perAtt.Text = characterData.Perception;
        form.strAtt.Text = characterData.Strength;
        form.techAtt.Text = characterData.Technical;
        form.racialBase.Text = characterData.RacialBase;
        form.charQuote.Text = characterData.Quote;
        form.moveAtt.Text = characterData.MoveAttribute;
        form.raceAbil.Text = characterData.RacialAbilities;
        form.racialAbil.Text = characterData.RacialAbilitiesLong;
        form.perBack.Text = characterData.PerBack;
        form.phyBack.Text = characterData.PhyBack;
        form.alliesBack.Text = characterData.AlliesBack;
        form.enemiesBack.Text = characterData.EnemiesBack;
        form.backgroundBack.Text = characterData.BackgroundBack;
        form.equipNotes.Text = characterData.EquipNotes;
        form.playerCreditsTextBox.Text = characterData.PlayerCredits;
        form.fateBox.Checked = characterData.FateBox;
        form.checkBoxCharacterCreation.Checked = characterData.CheckBoxCharacterCreation;
        form.specialConvert.Checked = characterData.SpecialConvert;
        form.typeBox.Text = characterData.Type;

        string species = characterData.Species?.ToString();
        if (!string.IsNullOrEmpty(species) && form.speciesBox.Items.Contains(species))
        {
            form.speciesBox.SelectedItem = species;
        }
        else
        {
            form.speciesBox.SelectedIndex = -1;
        }

        if (characterData.Image != null)
        {
            byte[] imageBytes = Convert.FromBase64String(characterData.Image.ToString());
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                form.imageDisplay.Image = Image.FromStream(ms);
                form.imageDisplay.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }
        else
        {
            form.imageDisplay.Image = null;
            form.imageDisplay.SizeMode = PictureBoxSizeMode.Normal;
        }

        PopulateSkillData(form.dexSkills, characterData.DexSkills);
        PopulateSkillData(form.knowSkills, characterData.KnowSkills);
        PopulateSkillData(form.mechSkills, characterData.MechSkills);
        PopulateSkillData(form.techSkills, characterData.TechSkills);
        PopulateSkillData(form.perSkills, characterData.PerSkills);
        PopulateSkillData(form.strSkills, characterData.StrSkills);

        form.forceCon.Text = characterData.ForceCon;
        form.forceSense.Text = characterData.ForceSense;
        form.forceAlter.Text = characterData.ForceAlter;

        form.forcePowersPanel.Controls.Clear();
        if (characterData.ForcePowers != null)
        {
            foreach (var powerName in characterData.ForcePowers)
            {
                form.AddForcePowerToPanel((string)powerName);
            }
        }

        form.weapInv.Controls.Clear();
        if (characterData.Weapons != null)
        {
            foreach (var weaponName in characterData.Weapons)
            {
                form.AddToInventory(new EquipmentItem { Name = (string)weaponName, Type = "weapon" }, 1);
            }
        }

        form.armorInv.Controls.Clear();
        if (characterData.Armor != null)
        {
            foreach (var armorName in characterData.Armor)
            {
                form.AddToInventory(new EquipmentItem { Name = (string)armorName, Type = "armor" }, 1);
            }
        }

        form.gearInv.Controls.Clear();
        if (characterData.Gear != null)
        {
            foreach (var gearName in characterData.Gear)
            {
                form.AddToInventory(new EquipmentItem { Name = (string)gearName, Type = "gear" }, 1);
            }
        }
    }





    private static void PopulateSkillData(ListView listView, dynamic skillData)
    {
        listView.Items.Clear();
        foreach (var skill in skillData)
        {
            ListViewItem item = new ListViewItem(skill.Name.ToString());
            item.SubItems.Add(skill.Dice.ToString());
            listView.Items.Add(item);
        }
    }



    private JArray GetListViewData(ListView listView)
    {
        var data = new JArray();
        foreach (ListViewItem item in listView.Items)
        {
            data.Add(new JObject
            {
                ["Skill"] = item.SubItems[0].Text,
                ["Dice"] = item.SubItems[1].Text
            });
        }
        return data;
    }

    private void LoadListViewData(ListView listView, JArray data)
    {
        listView.Items.Clear();
        foreach (var obj in data)
        {
            var skill = (string)obj["Skill"];
            var dice = (string)obj["Dice"];
            listView.Items.Add(new ListViewItem(new[] { skill, dice }));
        }
    }

    private JArray GetXPLogData(ListView listView)
    {
        var data = new JArray();
        foreach (ListViewItem item in listView.Items)
        {
            data.Add(new JObject
            {
                ["Name"] = item.SubItems[0].Text,
                ["Type"] = item.SubItems[1].Text,
                ["PreviousValue"] = item.SubItems[2].Text,
                ["NewValue"] = item.SubItems[3].Text,
                ["Cost"] = item.SubItems[4].Text
            });
        }
        return data;
    }

    private void LoadXPLogData(ListView listView, JArray data)
    {
        listView.Items.Clear();
        foreach (var obj in data)
        {
            var name = (string)obj["Name"];
            var type = (string)obj["Type"];
            var previousValue = (string)obj["PreviousValue"];
            var newValue = (string)obj["NewValue"];
            var cost = (string)obj["Cost"];
            listView.Items.Add(new ListViewItem(new[] { name, type, previousValue, newValue, cost }));
        }
    }
}
