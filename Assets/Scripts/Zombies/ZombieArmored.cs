using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Zombie
{
    public abstract class ZombieArmor
    {
        protected float armorHealth;
        public AudioClip[] hitSounds;
        protected Sprite armorFine;
        protected Sprite armorSlightBroken;
        protected Sprite armorAlmostBroken;
        protected SpriteRenderer armor;
        public bool isExist;
        public abstract float ArmorHealth { get; set; }
        public ZombieArmor(ZombieArmorConfig config, SpriteRenderer armor)
        {
            hitSounds = config.hitSound;
            armorHealth = config.armorHealth;
            armorFine = config.armorFine;
            armorSlightBroken = config.armorSlightBroken;
            armorAlmostBroken = config.armorAlmostBroken;
            this.armor = armor;
            armor.sprite = armorFine;
            isExist = true;
        }
    }

    public class ArmorHead : ZombieArmor
    {
        float almostBroken;
        float slightBroken;
        public override float ArmorHealth
        {
            get => armorHealth;
            set
            {
                armorHealth = value;
                if (armorHealth < 0) { GameObject.Destroy(armor); isExist = false; }
                else if (armorHealth < 30) { armor.sprite = armorAlmostBroken; }
                else if (armorHealth < 70) { armor.sprite = armorSlightBroken; }
            }
        }
        public ArmorHead(ZombieArmorConfig config, SpriteRenderer armor) : base(config, armor)
        {
            slightBroken = armorHealth * 0.7f;
            almostBroken = armorHealth * 0.3f;
        }
    }

    public class ZombieArmored : ZombieBase
    {
        [SerializeField] ZombieArmorConfig armorConfig;
        ZombieArmor armor;

        protected override void Init()
        {
            base.Init();
            armor = new ArmorHead(armorConfig, transform.GetChild(1).GetComponent<SpriteRenderer>());
        }

        public override void TakeDamage(float damage, AudioClip hitSound = null)
        {
            if (hitSound != null) SoundPlay(hitSound);
            else if (armor.isExist) SoundPlay(armor.hitSounds[Random.Range(0, armor.hitSounds.Length)]);
            else SoundPlay(soundPack.hitSounds[Random.Range(0, soundPack.hitSounds.Length)]);

            if (health <= 0) return;// 死去的僵尸可能还在承伤

            if (armor.isExist) armor.ArmorHealth -= damage;
            else health -= damage;

            if (health <= 0) TransitionState(ZombieState.Die);
        }
    }
}