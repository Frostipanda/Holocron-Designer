using System;

namespace Star_Wars_D6
{
    public class EquipmentItem
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Cost { get; set; }
    public int PhysicalArmor { get; set; } // pr value
    public int EnergyArmor { get; set; } // er value
    public string PhysicalArmorText { get; set; } // Formatted text for PhysicalArmor
    public string EnergyArmorText { get; set; } // Formatted text for EnergyArmor
        public int Damage { get; set; } // Damage score in pips
        public string Subtype { get; set; } // Weapon subtype (e.g., Ranged or Melee)
        public string Skill { get; set; } // Skill required to use the weapon
        public int Ammo { get; set; } // Ammo count
        public int RangeShort { get; set; } // Short range value
        public int RangeMedium { get; set; } // Medium range value
        public int RangeLong { get; set; } // Long range value
    }

}
