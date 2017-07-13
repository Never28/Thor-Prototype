using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStates : MonoBehaviour {

    public int health;

    public CharacterStats characterStats;

    public bool canBeParried = true;
    public bool parryIsOn = true;
    //public bool doParry = false;
    public bool isInvincible;
    public bool canMove;
    public bool isDead;
    public bool dontDoAnything;

    public StateManager parriedBy;

    public Animator anim;
    EnemyTarget enemyTarget;
    AnimatorHook a_hook;
    public Rigidbody rigid;
    public float delta;
    public float poiseDegrade = 2;

    List<Rigidbody> ragdollRigids = new List<Rigidbody>();
    List<Collider> ragdollColliders = new List<Collider>();

    public delegate void SpellEffect_Loop();
    public SpellEffect_Loop spellEffect_Loop;

    float timer;

    void Start()
    {
        health = 10000000;
        anim = GetComponentInChildren<Animator>();
        enemyTarget = GetComponent<EnemyTarget>();
        enemyTarget.Init(this);

        rigid = GetComponent<Rigidbody>();

        a_hook = anim.GetComponent<AnimatorHook>();
        if (a_hook == null)
            a_hook = anim.gameObject.AddComponent<AnimatorHook>();
        a_hook.Init(null, this);

        InitRagdoll();
        parryIsOn = false;
    }

    void InitRagdoll() { 
        Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigs.Length; i++) {
            if (rigs[i] == rigid)
                continue;
            ragdollRigids.Add(rigs[i]);
            rigs[i].isKinematic = true;

            Collider col = rigs[i].gameObject.GetComponent<Collider>();
            col.isTrigger = true;
            ragdollColliders.Add(col);
        }
    }

    public void EnableRagdoll() {
        for (int i = 0; i < ragdollRigids.Count; i++)
        {
            ragdollRigids[i].isKinematic = false;
            ragdollColliders[i].isTrigger = false;
        }

        Collider controllerCollider = rigid.gameObject.GetComponent<Collider>();
        controllerCollider.enabled = false;
        rigid.isKinematic = true;

        StartCoroutine("CloseAnimator");
    }

    IEnumerator CloseAnimator()
    {
        yield return new WaitForEndOfFrame();
        anim.enabled = false;
        this.enabled = false;
    }

    void Update() {
        delta = Time.deltaTime;
        canMove = anim.GetBool(StaticStrings.canMove);

        if (spellEffect_Loop != null) {
            spellEffect_Loop();
        }

        if (dontDoAnything) {
            dontDoAnything = !canMove;
            return;
        }

        if (health <= 0) {
            if (!isDead)
            {
                isDead = true;
                EnableRagdoll();
            }
        }

        if (isInvincible) {
            isInvincible = !canMove;
        }

        if (parriedBy != null && !parryIsOn) {
            //parriedBy.parryTarget = null;
            parriedBy = null;
        }

        if (canMove) {
            anim.applyRootMotion = false;

            //Debug
            timer += Time.deltaTime;
            if (timer > 2) {
                DoAction();
                timer = 0;
            }
        }

        characterStats.poise -= poiseDegrade * delta;
        if (characterStats.poise < 0)
            characterStats.poise = 0;

    }

    void DoAction() {
        anim.Play("oh_attack_1");
        anim.applyRootMotion = true;
        anim.SetBool(StaticStrings.canMove, false);
    }

    public void DoDamage(Action a) {
        if (isInvincible)
            return;

        int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats);
        
        characterStats.poise += damage;
        health -= damage;
        
        if(canMove || characterStats.poise > 100){
            if (a.overrideDamageAnim)
                anim.Play(a.damageAnim);
            else {
                int ran = Random.Range(0, 100);
                string tA = (ran > 50) ? StaticStrings.damage_1 : StaticStrings.damage_2;
                anim.Play(tA);
            }
        }

        Debug.Log("Damage :" + damage + " Poise:" + characterStats.poise);

        isInvincible = true;
        
        anim.applyRootMotion = true;
        anim.SetBool(StaticStrings.canMove, false);
    }

    public void DoDamage_() {
        if (isInvincible)
            return;
        anim.Play("damage_3");
    }

    public void CheckForParry(Transform target, StateManager st) {
        if (!canBeParried || !parryIsOn || isInvincible)
            return;

        //check if enemy is attacking from behind and entered the trigger from behind
        Vector3 dir = transform.position - target.position;
        dir.Normalize();
        float dot = Vector3.Dot(target.forward, dir);
        if (dot < 0)
            return; 

        isInvincible = true;
        anim.Play("attack_interrupt");
        anim.applyRootMotion = true;
        anim.SetBool(StaticStrings.canMove, false);
        //st.parryTarget = this;
        parriedBy = st;
        return;
    }

    public void IsGettingParried(Action a)
    {
        int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats, a.parryMultiplier);
        health -= damage;
        dontDoAnything = true;
        anim.SetBool(StaticStrings.canMove, false);
        anim.Play(StaticStrings.parry_received);
    }

    public void IsGettingBackstabbed(Action a)
    {
        int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats, a.backstabMultiplier);
        health -= damage;
        dontDoAnything = true;
        anim.SetBool(StaticStrings.canMove, false);
        anim.Play(StaticStrings.backstabbed);
    }

    public ParticleSystem fireParticle;
    float _t;

    public void OnFire() {
        if (fireParticle == null)
            return;

        if (_t < 3)
        {
            _t += Time.deltaTime;
            fireParticle.Emit(1);
        }
        else {
            _t = 0;
            spellEffect_Loop = null;
        }
    }
}
