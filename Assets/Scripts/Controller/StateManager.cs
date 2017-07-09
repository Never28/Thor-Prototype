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
    public bool canMove;
    public bool isTwoHanded;
    public bool usingItem;
    public bool canBeParried;
    public bool parryIsOn;
    public bool isBlocking;
    public bool isLeftHand;

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

        DetectAction();
        DetectItemAction();

        inventoryManager.rightHandWeapon.weaponModel.SetActive(!usingItem);

        anim.SetBool(StaticStrings.blocking, isBlocking);
        anim.SetBool(StaticStrings.isLeft, isLeftHand);

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

        canMove = anim.GetBool(StaticStrings.canMove);

        if (!canMove)
        {
            return;
        }

        //a_hook.rootMotionMultiplier = 1;
        a_hook.CloseRoll();
        HandleRolls();

        anim.applyRootMotion = false;

        rigid.drag = (moveAmount > 0 || !onGround) ? 0 : 4;

        float targetSpeed = moveSpeed;

        if (usingItem) {
            run = false;
            moveAmount = Mathf.Clamp(moveAmount, 0, 0.5f);
        }

        if (run)
            targetSpeed = runSpeed;

        if (onGround)
            rigid.velocity = moveDir * (targetSpeed * moveAmount);

        if (run)
            lockOn = false;


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

        anim.SetBool(StaticStrings.lockon, lockOn);

        if (lockOn)
        {
            HandleLockonAnimations(moveDir);
        }
        else
        {
            HandleMovementAnimations();
        }
    }

    public void DetectItemAction() {
        if (!canMove || usingItem || isBlocking)
            return;
        if (!itemInput)
            return;

        ItemAction slot = actionManager.consumableItem;
        string targetAnim = slot.targetAnim;

        if (string.IsNullOrEmpty(targetAnim))
            return;

        usingItem = true;
        anim.Play(targetAnim);
    }

    public void DetectAction()
    {
        if (!canMove || usingItem)
            return;

        if (!rb && !rt && !lt && !!!lb)
            return;

        Action slot = actionManager.GetActionSlot(this);
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
                break;
            case ActionType.parry:
                ParryAction(slot);
                break;
            default:
                break;
        }
    }

    void AttackAction(Action slot) {

        if (CheckForParry(slot))
            return;
        if (CheckForBackstab(slot))
            return;

        string targetAnim = null;
        targetAnim = slot.targetAnim;

        if (string.IsNullOrEmpty(targetAnim))
            return;

        currentAction = slot;

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

            parryTarget.IsGettingParried(slot);

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
            backstabTarget.IsGettingBackstabbed(slot);

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
        isLeftHand = slot.mirror;
    }

    void ParryAction(Action slot)
    {
        string targetAnim = null;
        targetAnim = slot.targetAnim;

        if (string.IsNullOrEmpty(targetAnim))
            return;

        canBeParried = slot.canBeParried;
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

        canMove = false;
        inAction = true;
        anim.CrossFade(StaticStrings.Rolls, 0.2f);

    }

    public void Tick(float d)
    {
        delta = d;
        onGround = OnGround();
        anim.SetBool(StaticStrings.onGround, onGround);
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
}
