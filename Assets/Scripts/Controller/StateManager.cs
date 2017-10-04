using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{

    [Header("Init")]
    public GameObject activeModel;

    [Header("Stats")]
    public Attributes attributes;
    public CharacterStats characterStats;

    [Header("Inputs")]
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public Vector3 moveDir;
    public bool rt, rb, lt, lb;
    public bool rollInput;
    public bool itemInput;

    [Header("Stats")]
    public float moveSpeed = 2;
    public float runSpeed = 3.5f;
    public float rotationSpeed = 5;
    public float distanceToGround = 0.5f;
    public float rollSpeed = 1;
    public float parryOffset = 1.5f;
    public float backstabOffset = 1.5f;

    [Header("States")]
    public bool onGround;
    public bool run;
    public bool lockOn;
    public bool inAction;
    public bool damageIsOn;
    public bool canRotate;
    public bool canMove;
    public bool canAttack;
    public bool isSpellCasting;
    public bool enableIK;
    public bool isTwoHanded;
    public bool usingItem;
    public bool canBeParried;
    public bool parryIsOn;
    public bool isBlocking;
    public bool isLeftHand;
    public bool onEmpty;

    [Header("Other")]
    public EnemyTarget lockonTarget;
    public Transform lockonTransform;
    public AnimationCurve rollCurve;
    //public EnemyStates parryTarget;

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Rigidbody rigid;
    [HideInInspector]
    public AnimatorHook a_hook;
    [HideInInspector]
    public ActionManager actionManager;
    [HideInInspector]
    public InventoryManager inventoryManager;

    [HideInInspector]
    public float delta;
    [HideInInspector]
    public LayerMask ignoreLayers;

    [HideInInspector]
    public Action currentAction;

    [HideInInspector]
    public float airTimer;
    public ActionInput storePrevActionInput;
    public ActionInput storeActionInput;

    float _actionDelay;

    public void Init()
    {
        SetupAnimator();

        rigid = GetComponent<Rigidbody>();
        rigid.angularDrag = 999;
        rigid.drag = 4;
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        inventoryManager = GetComponent<InventoryManager>();
        inventoryManager.Init(this);

        actionManager = GetComponent<ActionManager>();
        actionManager.Init(this);

        a_hook = activeModel.GetComponent<AnimatorHook>();
        if(a_hook == null)        
            a_hook = activeModel.AddComponent<AnimatorHook>();
        a_hook.Init(this, null);

        gameObject.layer = 8;
        ignoreLayers = ~(1 << 9);
         
        anim.SetBool(StaticStrings.onGround, true);

        characterStats.InitCurrent();
        UIManager UI = UIManager.singleton;
        UI.AffectAll(characterStats.health, characterStats.focus, characterStats.stamina);
        UI.InitSouls(characterStats._souls);
    }

    void SetupAnimator()
    {
        if (!activeModel)
        {
            anim = GetComponentInChildren<Animator>();
            if (anim)
            {
                Debug.Log("No model found");
            }
            else
            {
                activeModel = anim.gameObject;
            }
        }

        if (!anim)
        {
            anim = activeModel.GetComponent<Animator>();
        }
    }

    public void FixedTick(float d)
    {
        delta = d;

        isBlocking = false;
        usingItem = anim.GetBool(StaticStrings.interacting);
        anim.SetBool(StaticStrings.spellCasting, isSpellCasting);
        if (inventoryManager.rightHandWeapon != null)
            inventoryManager.rightHandWeapon.weaponModel.SetActive(!usingItem);
        if (inventoryManager.currentConsumable != null) {
            if (inventoryManager.currentConsumable.itemModel != null)
                inventoryManager.currentConsumable.itemModel.SetActive(usingItem);
        }

        if (!isBlocking && !isSpellCasting)
        {
            enableIK = false; //commentare per lasciare l'animazione bloccata nell'ik e regolare gli helper ed aggiustare l'animazione
        }

        //a_hook.useIK = true; //scommentare per lasciare l'animazione bloccata nell'ik e regolare gli helper ed aggiustare l'animazione

        if (inAction)
        {
            anim.applyRootMotion = true;
            _actionDelay += delta;
            if (_actionDelay > 0.3f)
            { //crossfade value
                inAction = false;
                _actionDelay = 0;
            }
            else
            {
                return;
            }
        }

        onEmpty = anim.GetBool(StaticStrings.onEmpty);

        if (onEmpty) {
            canAttack = true;
            canMove = true;
        }

        if (canRotate) {
            HandleRotation();
        }

        if (!onEmpty && !canMove && !canAttack) //animation is playing
        {
            return;
        }

        if (canMove && !onEmpty) {
            if (moveAmount > 0.3f) {
                anim.CrossFade("Empty Override", 0.1f);
                onEmpty = true;
            }
        }

        if (canAttack) {
            DetectAction();
        }
        if (canMove)
        {
            DetectItemAction();        
        }


        //a_hook.rootMotionMultiplier = 1;

        anim.applyRootMotion = false;

        rigid.drag = (moveAmount > 0 || !onGround) ? 0 : 4;

        float targetSpeed = moveSpeed;

        if (usingItem || isSpellCasting) {
            run = false;
            moveAmount = Mathf.Clamp(moveAmount, 0, 0.5f);
        }

        if (run)
            targetSpeed = runSpeed;

        if (onGround && canMove)
            rigid.velocity = moveDir * (targetSpeed * moveAmount);

        if (run)
            lockOn = false;


        anim.SetBool(StaticStrings.lockon, lockOn);

        if (lockOn)
        {
            HandleLockonAnimations(moveDir);
        }
        else
        {
            HandleMovementAnimations();
        }

        a_hook.useIK = enableIK; //commentare per lasciare l'animazione bloccata nell'ik e regolare gli helper ed aggiustare l'animazione
        //anim.SetBool(StaticStrings.blocking, isBlocking);
        anim.SetBool(StaticStrings.isLeft, isLeftHand);

        HandleBlocking();

        if (isSpellCasting) {
            HandleSpellCasting();
            return;
        }
        
        a_hook.CloseRoll();
        HandleRolls();

    }

    public bool IsInput() {
        if (rt || rb || lt || lb || rollInput)
            return true;

        return false;
    }

    void HandleRotation() {
        Vector3 targetDir = (lockOn == false) ? moveDir :
         (lockonTransform != null) ?
         lockonTransform.position - transform.position :
         moveDir;
        targetDir.y = 0;
        if (targetDir == Vector3.zero)
        {
            targetDir = transform.forward;
        }
        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotationSpeed);
        transform.rotation = targetRotation;

        isBlocking = false;
    }

    public void DetectItemAction() {
        if (!onEmpty || usingItem || isBlocking)
            return;
        if (!itemInput)
            return;

        if (inventoryManager.currentConsumable == null)
            return;

        if (inventoryManager.currentConsumable.itemCount < 1 && !inventoryManager.currentConsumable.unlimitedCount)
            return;

        RuntimeConsumable slot = inventoryManager.currentConsumable;
        string targetAnim = slot.instance.targetAnim;

        if (string.IsNullOrEmpty(targetAnim))
            return;

        usingItem = true;
        anim.Play(targetAnim);
    }

    public void DetectAction()
    {
        if (!canAttack && (!onEmpty || usingItem || isSpellCasting))
            return;

        if (!rb && !rt && !lt && !!!lb)
            return;

        ActionInput targetInput = actionManager.GetActionInput(this);
        storeActionInput = targetInput;
        if (!onEmpty)
        {
            a_hook.killDelta = true;
            targetInput = storePrevActionInput;
        }
        storePrevActionInput = targetInput;

        Action slot = actionManager.GetActionFromInput(targetInput);
        if (slot == null)
            return;

        switch (slot.type)
        {
            case ActionType.attack:
                AttackAction(slot);
                break;
            case ActionType.block:
                BlockAction(slot);
                break;
            case ActionType.spell:
                SpellAction(slot);
                break;
            case ActionType.parry:
                ParryAction(slot);
                break;
            default:
                break;
        }
    }

    void AttackAction(Action slot) {

        if (characterStats._stamina < slot.staminaCost)
            return;

        if (CheckForParry(slot))
            return;
        if (CheckForBackstab(slot))
            return;

        string targetAnim = null;
        targetAnim = slot.GetActionStep(ref actionManager.actionIndex).targetAnim; ;

        if (string.IsNullOrEmpty(targetAnim))
            return;

        currentAction = slot;

        canAttack = false;
        onEmpty = false;
        canMove = false;
        inAction = true;
        float targetSpeed = 1;
        if (slot.changeSpeed)
        {
            targetSpeed = slot.animSpeed;
            if (targetSpeed <= 0)
                targetSpeed = 1;
        }
        anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
        anim.SetBool(StaticStrings.mirror, slot.mirror);
        anim.CrossFade(targetAnim, 0.2f);
        characterStats._stamina -= slot.staminaCost;
    }

    void SpellAction(Action slot) {
        if(characterStats._stamina < slot.staminaCost)
            return;
        if (slot.spellClass != inventoryManager.currentSpell.instance.spellClass || characterStats._focus < slot.focusCost) {
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade("cant_spell", 0.2f);
            canAttack = false;
            canMove = false;
            inAction = true;
        }

        ActionInput inp = actionManager.GetActionInput(this);
        if (inp == ActionInput.lb)
            inp = ActionInput.rb;
        if (inp == ActionInput.lt)
            inp = ActionInput.rt;
        
        Spell s_inst = inventoryManager.currentSpell.instance;
        SpellAction s_slot = s_inst.GetAction(s_inst.spellActions, inp);
        if (s_slot == null)
            return;

        SpellEffectManager.singleton.UseSpellEffect(s_inst.spell_effect, this);

        isSpellCasting = true;
        spellCastTime = 0;
        maxSpellCastTime = s_slot.castTime;
        spellTargetAnim = s_slot.throwAnim;
        spellIsMirrored = slot.mirror;
        curSpellType = s_inst.spellType;

        string targetAnim = s_slot.targetAnim;
        if (spellIsMirrored)
            targetAnim += StaticStrings._l;
        else
            targetAnim += StaticStrings._r;

        projectileCandidate = inventoryManager.currentSpell.instance.projectile;
        inventoryManager.CreateSpellParticle(inventoryManager.currentSpell, spellIsMirrored, s_inst.spellType == SpellType.looping);

        anim.SetBool(StaticStrings.spellCasting, true);
        anim.SetBool(StaticStrings.mirror, slot.mirror);
        anim.CrossFade(targetAnim, 0.2f);

        curFocusCost = s_slot.focusCost;
        curStaminaCost = s_slot.staminaCost;

        a_hook.InitIKForBreathSpell(spellIsMirrored);

        if (spellCast_Start != null)
            spellCast_Start();
    }

    float curFocusCost;
    float curStaminaCost;
    float spellCastTime;
    float maxSpellCastTime;
    string spellTargetAnim;
    bool spellIsMirrored;
    SpellType curSpellType;
    GameObject projectileCandidate;

    public delegate void SpellCast_Start();
    public delegate void SpellCast_Loop();
    public delegate void SpellCast_Stop();
    public SpellCast_Start spellCast_Start;
    public SpellCast_Loop spellCast_Loop;
    public SpellCast_Stop spellCast_Stop;

    void EmptySpellCastDelegate() {
        spellCast_Start = null;
        spellCast_Loop = null;
        spellCast_Stop = null;
    }

    void HandleSpellCasting() {

        if (curSpellType == SpellType.looping) {

            enableIK = true;
            a_hook.currentHand = (spellIsMirrored) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;

            if ((rb == false && lb == false) || characterStats._focus < 2)
            {
                isSpellCasting = false;

                enableIK = false;

                inventoryManager.breathCollider.SetActive(false);
                inventoryManager.blockCollider.SetActive(false);

                if (spellCast_Stop != null)
                    spellCast_Stop();

                EmptySpellCastDelegate();

                return;
            }

            if (spellCast_Loop != null)
                spellCast_Loop();

            characterStats._focus -= 1;

            return;
        }

        spellCastTime += delta;
        if (inventoryManager.currentSpell.currentParticle != null)
            inventoryManager.currentSpell.currentParticle.SetActive(true);
    
        if (spellCastTime > maxSpellCastTime) {
            onEmpty = false;
            canAttack = false;
            canMove = false;
            inAction = true;
            isSpellCasting = false;

            string targetAnim = spellTargetAnim;
            anim.SetBool(StaticStrings.mirror, spellIsMirrored);
            anim.CrossFade(targetAnim, 0.2f);
        }
    }
    
    bool blockAnim;
    string block_idle_anim;
    void HandleBlocking()
    {

        if (!isBlocking)
        {
            if (blockAnim) 
            {
                anim.CrossFade(block_idle_anim, 0.1f);
                blockAnim = false;
            }
        }
        else { 
            
        }

    }

    public void ThrowProjectile() {
        if (projectileCandidate == null)
            return;
        GameObject go = Instantiate(projectileCandidate) as GameObject;
        Transform p = anim.GetBoneTransform((spellIsMirrored) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
        go.transform.position = p.position;

        if (lockonTransform && lockOn)
        {
            go.transform.LookAt(lockonTransform.position);
        }
        else {
            go.transform.rotation = transform.rotation;
        }

        Projectile proj = go.GetComponent<Projectile>();
        proj.Init();

        characterStats._stamina -= curStaminaCost;
        characterStats._focus -= curFocusCost;
    }

    bool CheckForParry(Action slot) {
        if (!slot.canParry)
            return false;

        EnemyStates parryTarget = null;

        Vector3 origin = transform.position;
        origin.y += 1;
        Vector3 rayDir = transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(origin, rayDir, out hit, 3, ignoreLayers)) {
            parryTarget = hit.transform.GetComponentInParent<EnemyStates>();
        }

        if (parryTarget == null)
            return false;
        if (parryTarget.parriedBy == null)
            return false;
        //float dis = Vector3.Distance(parryTarget.transform.position, transform.position);

        //if (dis > 3)
        //    return false;

        Vector3 dir = parryTarget.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0;
        float angle = Vector3.Angle(transform.forward, dir);

        if (angle < 60) {
            Vector3 targetPos = -dir * parryOffset;
            targetPos += parryTarget.transform.position;
            transform.position = targetPos;

            if (dir == Vector3.zero)
                dir = -parryTarget.transform.forward;

            Quaternion eRotation = Quaternion.LookRotation(-dir);
            Quaternion ourRotation = Quaternion.LookRotation(dir);

            parryTarget.transform.rotation = eRotation;
            transform.rotation = ourRotation;

            parryTarget.IsGettingParried(slot, inventoryManager.GetCurrentWeapon(slot.mirror));

            onEmpty = false;
            canAttack = false;
            canMove = false;
            inAction = true;
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(StaticStrings.parry_attack, 0.2f);
            return true;
        }   

        return false;
    }

    bool CheckForBackstab(Action slot) {
        if (!slot.canBackstab)
            return false;

        EnemyStates backstabTarget = null;

        Vector3 origin = transform.position;
        origin.y += 1;
        Vector3 rayDir = transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(origin, rayDir, out hit, 3, ignoreLayers))
        {
            backstabTarget = hit.transform.GetComponentInParent<EnemyStates>();
        }

        if (backstabTarget == null)
            return false;

        Vector3 dir = transform.position - backstabTarget.transform.position;
        dir.Normalize();
        dir.y = 0;
        float angle = Vector3.Angle(backstabTarget.transform.forward, dir);

        if (angle > 150)
        {
            Vector3 targetPos = dir * backstabOffset;
            targetPos += backstabTarget.transform.position;
            transform.position = targetPos;

            backstabTarget.transform.rotation = transform.rotation;
            backstabTarget.IsGettingBackstabbed(slot, inventoryManager.GetCurrentWeapon(slot.mirror));

            onEmpty = false;
            canAttack = false;
            canMove = false;
            inAction = true;
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(StaticStrings.parry_attack, 0.2f);
            lockonTarget = null;
            return true;
        }

        return false;
    }

    void BlockAction(Action slot) {
        isBlocking = true;
        enableIK = true;
        isLeftHand = slot.mirror;
        a_hook.currentHand = (slot.mirror) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;
        a_hook.InitIKForShield(slot.mirror);
        
        if (!blockAnim) {
            block_idle_anim = (!isTwoHanded) ? inventoryManager.GetCurrentWeapon(isLeftHand).oh_idle : inventoryManager.GetCurrentWeapon(isLeftHand).th_idle;
            block_idle_anim += (isLeftHand) ? "_l" : "_r";
            string targetAnim = slot.firstStep.targetAnim;
            targetAnim += (isLeftHand) ? "_l" : "_r";
            anim.CrossFade(targetAnim, 0.1f);
            blockAnim = true;
        }
    }

    void ParryAction(Action slot)
    {
        string targetAnim = null;
        targetAnim = slot.GetActionStep(ref actionManager.actionIndex).targetAnim;

        if (string.IsNullOrEmpty(targetAnim))
            return;

        canBeParried = slot.canBeParried;

        onEmpty = false;
        canAttack = false;
        canMove = false;
        inAction = true;
        float targetSpeed = 1;
        if (slot.changeSpeed)
        {
            targetSpeed = slot.animSpeed;
            if (targetSpeed <= 0)
                targetSpeed = 1;
        }
        anim.SetBool(StaticStrings.mirror, slot.mirror);
        anim.CrossFade(targetAnim, 0.2f);
    }

    void HandleRolls()
    {
        if (!rollInput || usingItem)
            return;

        float v = vertical;
        float h = horizontal;
        v = (moveAmount > 0.3f) ? 1 : 0;
        h = 0;
        /*if (!lockOn)
        {
            v = (moveAmount > 0.3f) ? 1 : 0;
            h = 0;
        }
        else {
            if (Mathf.Abs(v) < 0.3f)
                v = 0;
            if (Mathf.Abs(h) < 0.3f)
                h = 0;
        }*/

        if (v != 0)
        {
            if (moveDir == Vector3.zero)
                moveDir = transform.forward;
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = targetRot;
            a_hook.InitForRoll();
            a_hook.rootMotionMultiplier = rollSpeed;
        }
        else
        {
            a_hook.rootMotionMultiplier = 1.3f;
        }


        anim.SetFloat(StaticStrings.vertical, v);
        anim.SetFloat(StaticStrings.horizontal, h);

        onEmpty = false;
        canAttack = false;
        canMove = false;
        inAction = true;
        anim.CrossFade(StaticStrings.Rolls, 0.2f);

    }

    public void Tick(float d)
    {
        delta = d;
        onGround = OnGround();
        anim.SetBool(StaticStrings.onGround, onGround);

        if (!onGround)
            airTimer += delta;
        else
            airTimer = 0;
    }

    void HandleMovementAnimations()
    {
        anim.SetBool(StaticStrings.run, run);
        anim.SetFloat(StaticStrings.horizontal, moveAmount, 0.4f, delta);
    }

    void HandleLockonAnimations(Vector3 moveDir)
    {
        Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
        float h = relativeDir.x;
        float v = relativeDir.z;

        anim.SetFloat(StaticStrings.vertical, v, 0.2f, delta);
        anim.SetFloat(StaticStrings.horizontal, h, 0.2f, delta);
    }

    public bool OnGround()
    {
        bool r = false;

        Vector3 origin = transform.position + (Vector3.up * distanceToGround);
        Vector3 dir = -Vector3.up;
        float dis = distanceToGround + 0.3f;
        RaycastHit hit;
        Debug.DrawRay(origin, dir * dis);
        if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayers))
        {
            r = true;
            Vector3 targetPosition = hit.point;
            transform.position = targetPosition;
        }

        return r;
    }

    public void HandleTwoHanded() 
    {
        bool isRight = true;
        if (inventoryManager.rightHandWeapon == null)
            return;

        Weapon w = inventoryManager.rightHandWeapon.instance;
        if (w == null) {
            w = inventoryManager.leftHandWeapon.instance;
            isRight = false;
        }
        if (w == null)
            return;

        if (isTwoHanded){
            anim.CrossFade(w.th_idle, 0.2f);
            actionManager.UpdateActionsTwoHanded();
            if (isRight)
            {
                if (inventoryManager.leftHandWeapon)
                    inventoryManager.leftHandWeapon.weaponModel.SetActive(false);
            }
            else {
                if (inventoryManager.rightHandWeapon)
                    inventoryManager.rightHandWeapon.weaponModel.SetActive(false);
            }
        }
        else{
            string targetAnim = w.oh_idle;
            targetAnim += (isRight) ? StaticStrings._r : StaticStrings._l;
            //anim.CrossFade(targetAnim, 0.2f);
            anim.Play(StaticStrings.equip_weapon_oh);
            actionManager.UpdateActionsOneHanded();
            if (isRight)
            {
                if (inventoryManager.leftHandWeapon)
                    inventoryManager.leftHandWeapon.weaponModel.SetActive(true);
            }
            else
            {
                if (inventoryManager.rightHandWeapon)
                    inventoryManager.rightHandWeapon.weaponModel.SetActive(true);
            }
        }

    }

    public void IsGettingParried() { 
        
    }

    public void AddHealth() {
        characterStats.health++;
    }

    public void MonitorStats() {
        if (run && moveAmount > 00)
        {
            characterStats._stamina -= delta * 5;
        }
        else {
            characterStats._stamina += delta * 3;
        }

        if (characterStats._stamina > characterStats.focus)
            characterStats._stamina = characterStats.focus;
        //characterStats._stamina = Mathf.Clamp(characterStats._stamina, 0, characterStats.stamina);

        characterStats._health = Mathf.Clamp(characterStats._health, 0, characterStats.health);
        characterStats._focus = Mathf.Clamp(characterStats._focus, 0, characterStats.focus);
    }

    public void SubstractStaminaOverTime() {
        characterStats._stamina -= curStaminaCost;
    }

    public void SubstractFocusOverTime() {
        characterStats._focus -= curFocusCost;
    }

    public void AffectBlocking() {
        isBlocking = true;
    }

    public void StopAffectingBlocking() {
        isBlocking = false;
    }
}
