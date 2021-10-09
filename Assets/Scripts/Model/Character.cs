﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {

    
    public delegate void CharacterChangedHandler(Character c);
    public delegate void CharacterUsedSpellDelegate(Character c);
    public delegate void CharacterNewSpellDelegate(Character c);
    public delegate void CharacterDieDelegate(Character c);


    public abstract class Character : MonoBehaviour
    {
        public event CharacterUsedSpellDelegate CharacterUsedSpellEvent;
        public event CharacterNewSpellDelegate CharacterNewSpellEvent;
        public event CharacterDieDelegate CharacterDieEvent;

        public HealthBar HBPrefab;
        public NextMarker IsNextPrefab;
        protected HealthBar healthBar;
        protected NextMarker isNextMarker;
        public Animator animator;
        public SkillBase[] skillPrefabs = new SkillBase[4];
        private List<SkillBase> skills;

        private bool isSelected = false;
        private int health = 100;
        public int speed;
        public int defaultLevel;
        public int baseDamage;
        public int baseArmor;
        public float baseCrit;
        public float baseStunResist;
        public float baseDodgeChance;
        public float baseAccuracy;

        public SkillBase SelectedSkill { get; set; }


        public bool IsSelected {
            get { return isSelected; }
        } 
        
        public bool IsNext
        {
            get;
            set;
        }

        public int Health {
            get { return health; }
            set {
                if(value <= 0)
                {
                    health = 0;
                }
                if(value >= 100)
                {
                    health = 100;
                }
                 if( value > 0 && value < 100){
                    health = value;
                }
            }
        }

        public int Level{
            get { return defaultLevel; }
            set {
                if(value >= 0 && value <= 10){
                    defaultLevel = value;
                }
            }
        }

        protected virtual void Awake() {
            skills = new List<SkillBase>(4);

            var transform = this.GetComponent<Transform>();

            healthBar = Instantiate<HealthBar>(
                    HBPrefab,
                    new Vector3(
                        transform.position.x,
                        transform.position.y + HBPrefab.GetComponent<RectTransform>().rect.height * 2 + 0.15f,
                        0),
                     Quaternion.identity);
        }

        protected virtual void Start() {
             foreach(SkillBase skillPref in skillPrefabs){
                 if(skillPref != null){
                    SkillBase skill = Instantiate(skillPref);
                    skills.Add(skill);
                    // Notify the character when the skill selected
                    skill.SkillSelected += this.SelectSkill;
                 } else {
                     skills.Add(null);
                 }
            }
        }

        protected virtual void Update()
        {
            if (this.IsNext)
            {
                if (isNextMarker == null)
                {
                    isNextMarker = Instantiate(
                    IsNextPrefab,
                    new Vector3(
                        transform.position.x,
                        transform.position.y + HBPrefab.GetComponent<RectTransform>().rect.height * 2 + 0.5f,
                        0.5f),
                     Quaternion.identity);
                }
                isNextMarker.gameObject.SetActive(true);
            }
            if (!this.IsNext && isNextMarker != null)
            {
                isNextMarker.gameObject.SetActive(false);
            }

            healthBar.SetValue(Health / 100.0f);
        }

        public void Hit(int damage)
        {
            // TODO: Show information to user
            Health -= damage;
            if(Health <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            // TODO: Show information to user
            Health += amount;
        }

        public void CastSkill(Character[] targets)
        {
            if (SelectedSkill)
            {
                SelectedSkill.CastSkill(this, targets);
                SelectedSkill = null;
                CharacterUsedSpellEvent(this);
            }
            else
            {
                Debug.LogError("Selected skill is Null");
            }
        }

        public void SelectSkill(SkillBase skill)
        {
            if (IsNext)
            {
               SelectedSkill = skill;
               CharacterNewSpellEvent(this);
            }
        }

        public void setSkill(SkillBase skill, int position){
            if(position < 0 && position > 3){
                throw new System.Exception("Wrong position given!");
            }
            skills[position] = skill;
        }

        public virtual void Die()
        {
            if (CharacterDieEvent != null) {
                CharacterDieEvent(this);
            } else
            {
                Debug.Log($"Event is: {CharacterDieEvent}");
            }
            if (isNextMarker != null)
            {
                Destroy(isNextMarker.gameObject);
            }
            if (healthBar != null) {
                Destroy(healthBar.gameObject);
            }
            Destroy(this.gameObject);
        }

        public void Select(){
            isSelected = true;
        }

         public void UnSelect(){
            isSelected = false;
        }

        public List<SkillBase> GetSkills(){
            return skills;
        }
    }
}
