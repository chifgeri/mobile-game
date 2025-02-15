﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {

    public enum WeaponType {
        LongSword,
        CurvedSword,
        Saber,
        KnightSword
    }

    [Serializable]
    public class Weapon : Item
        {
            // Weapon type
            [SerializeField]
            private WeaponType type;
            [SerializeField]
            private int damage;

            public int Damage {
                get => damage;
            }
            // Buffs
            public Weapon(string _name, int damage, WeaponType _type, int _price): base(_name, _price, 1, false){
                this.type = _type;
                this.damage = damage;
            }

            public override Sprite GetSprite()
            {
                switch(type){
                    case WeaponType.LongSword:
                         return WeaponSprites.Instance.longSword;
                    case WeaponType.Saber:
                        return WeaponSprites.Instance.Saber;
                    case WeaponType.CurvedSword:
                        return WeaponSprites.Instance.curvedSword;
                    case WeaponType.KnightSword:
                        return WeaponSprites.Instance.KnightSword;
                    default: 
                        return null;
                }
            }

        public override Enum GetItemType()
        {
          return type;
        }

        public override Item Clone(int amount = 1){
                return new Weapon(this.Name,this.damage, this.type, this.Price);
            }

        public override ItemAttribute GetItemAttributes()
        {
            return new ItemAttribute()
                {
                    AttributeName = "Damage",
                    ValueString = Damage.ToString()
                };
        }

        public override void Use(PlayerCharacter target)
        {
            target.EquipWeapon(this);
        }
    }


}
