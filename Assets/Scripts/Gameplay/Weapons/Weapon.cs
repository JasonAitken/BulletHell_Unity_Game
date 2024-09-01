using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UtilityFuctions;

public class Weapon : MonoBehaviour
{
    /* Design
     * Each Weapon has its own defined "Skill" / Attributes
     * IDEA: Has soctets that can modify weapon is large way eg. split projectile on hit, explodes on impact ...)
     */
    public struct Attributes
    {
        public UtilityFuctions.Pair<bool, GameObject> hasProjectile;
        public UtilityFuctions.Pair<bool, float> hasAOE;
        public UtilityFuctions.Pair<bool, int> canPierce;
        public UtilityFuctions.Pair<bool, /*TDB eg. BonusAttribute*/ string> hasBonusEffect;

        public float attackSpeed;
        public int damage;
        public float projectileSpeed;
        public float range; 

        public Attributes(BaseAttributes baseAttributes, BonusAttributes bonusAttributes)
        {
            this.hasProjectile = bonusAttributes.hasProjectile;
            this.hasAOE = bonusAttributes.hasAOE;
            this.canPierce = bonusAttributes.canPierce;
            this.hasBonusEffect = bonusAttributes.hasBonusEffect;

            this.attackSpeed = baseAttributes.attackSpeed;
            this.damage = baseAttributes.damage;
            this.projectileSpeed = baseAttributes.projectileSpeed;
            this.range = baseAttributes.range;
        }
    }

    public struct BaseAttributes 
    {
        public float attackSpeed;
        public int damage;
        public float projectileSpeed;
        public float range;

        public BaseAttributes(float attackSpeed, int damage, float projectileSpeed, float range)
        {
            this.attackSpeed = attackSpeed;
            this.damage = damage;
            this.projectileSpeed = projectileSpeed;
            this.range = range;
        }
    }

    public struct BonusAttributes
    {
        public UtilityFuctions.Pair<bool, GameObject> hasProjectile; // First hasProjectile, Second theProcetilesGameObject
        public UtilityFuctions.Pair<bool, float> hasAOE; // First hasAOE, Second sizeOfArea as a float
        public UtilityFuctions.Pair<bool, int> canPierce; // First canPierce, Second numberOfTimesCanPierce
        public UtilityFuctions.Pair<bool, /*TDB eg. BonusAttribute*/ string> hasBonusEffect; // First hasBonusEffect, Second TBD theBonusEffectItHas

        public BonusAttributes(
            UtilityFuctions.Pair<bool, GameObject> hasProjectile, 
            UtilityFuctions.Pair<bool, float> hasAOE,
            UtilityFuctions.Pair<bool, int> canPierce,
            UtilityFuctions.Pair<bool, /*TDB eg. BonusAttribute*/ string> hasBonusEffect)
        {
            this.hasProjectile = hasProjectile;
            this.hasAOE = hasAOE;
            this.canPierce = canPierce;
            this.hasBonusEffect = hasBonusEffect;
        }
    }

    public Attributes attributes;

    /* TODO
     * Define spell type
     * Define attributes (eg. pierce, AOE ...)
     * createBonusEffects (eg. splits, DOT's, lifesteal ...)
     */

    float lastFired = 0.0f;

    [SerializeField]
    GameObject projectile;
    [SerializeField]
    bool hasAoe = false;
    [SerializeField]
    float aoeSize = 0.0f;
    [SerializeField]
    bool canPierce = false;
    [SerializeField]
    int noOfPierce = 0;

    private void Awake()
    {
        setAttributes();
    }

    //public float getAttackSpeed() { return attributes.attackSpeed; }

    //public int getDamage() { return attributes.damage; }

    //public float getProjectileSpeed() 
    //{
    //    if (attributes.hasProjectile) 
    //    {
    //        return attributes.projectileSpeed;  
    //    }
    //    return 0.0F;
    //}

    //public float getRange() { return attributes.range; }

    //public GameObject getProjectile()
    //{
    //    if (attributes.hasProjectile)
    //    {
    //        return projectile;
    //    }
    //    return null;
    //}

    public bool cast(GameObject target) 
    {
        if ((Time.fixedTime - lastFired) < attributes.attackSpeed)
        {
            return false;
        }

        if (attributes.hasProjectile.First) 
        {
            lastFired = Time.fixedTime;
            GameObject newProjectile = Instantiate(attributes.hasProjectile.Second, transform.position, target.transform.rotation);
            newProjectile.GetComponent<Projectile>().setTarget(target);
            newProjectile.GetComponent<Projectile>().setAttributes(attributes);
            newProjectile.GetComponent<Projectile>().weapon = this;

            return true;
        }
        return false;
    }

    private void setAttributes() 
    {
         UtilityFuctions.Pair<bool, GameObject> hasProjectile = new UtilityFuctions.Pair<bool, GameObject>(true, projectile);
         UtilityFuctions.Pair<bool, float> hasAOE = new UtilityFuctions.Pair<bool, float>(this.hasAoe, this.aoeSize);
         UtilityFuctions.Pair<bool, int> canPierce = new UtilityFuctions.Pair<bool, int>(this.canPierce, this.noOfPierce);
         UtilityFuctions.Pair<bool, /*TDB eg. BonusAttribute*/ string> hasBonusEffect = new UtilityFuctions.Pair<bool, string>(false, "NULL");

        BonusAttributes bonusAttributes = new BonusAttributes(hasProjectile, hasAOE, canPierce, hasBonusEffect);
        BaseAttributes baseAttributes = new BaseAttributes(/*attackSpeed*/1.0f, /*damage*/10, /*projectileSpeed*/3.0f, /*range*/20.0f);
        this.attributes = new Attributes(baseAttributes, bonusAttributes);
    }

}
