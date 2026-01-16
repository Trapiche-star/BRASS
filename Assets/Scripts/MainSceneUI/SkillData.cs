using UnityEngine;


    [CreateAssetMenu(fileName = "NewSkill", menuName = "UI/SkillData")]
    public class SkillData : ScriptableObject
    {
        public string skillName;
        public Sprite icon;
        public int mpCost;
        public int cpCost;
        public int gpCost;
        public float cooldown;
    }
