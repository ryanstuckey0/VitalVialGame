using System.Collections.Generic;

namespace ViralVial.Player.TechTreeCode
{
    public class AbilityLevel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string[] prereqs { get; set; }
        public Dictionary<string, float> stats { get; set; }
        public string effect { get; set; }
        public string[] affectedEnemies { get; set; }
        public Dictionary<string, string> childAbility;
        public int costSkillPoints { get; set; }
    }
}
