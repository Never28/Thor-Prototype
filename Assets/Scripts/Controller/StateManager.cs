using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{

    [Header("Init")]
    public GameObject activeModel;

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

    [Header("States")]
    public bool onGround;
    public bool run;
    public bool lockOn;
    public bool inAction;
    public bool canMove;
    public bool isTwoHanded;
    public bool usingItem;

    [Header("Other")]
    public EnemyTarget lockonTarget;
    public Transform lockonTransform;
    public AnimationCurve rollCurve;

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
    public LayerMask ignoreLayer;

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
        ignoreLayer = ~(1 << 9);

        anim.SetBool("onGround", true);
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

        usingItem = anim.GetBool("interacting");

        DetectItemAction();
        DetectAction();
        
        inventoryManager.rightHandWeapon.weaponModel.SetActive(!usingItem);

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

        canMove = anim.GetBool("canMove");

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

        anim.SetBool("lockon", lockOn);

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
        if (!canMove || usingItem)
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

        string targetAnim = null;

        Action slot = actionManager.GetActionSlot(this);
        if (slot == null)
            return;
        targetAnim = slot.targetAnim;

        if (string.IsNullOrEmpty(targetAnim))
            return;

        canMove = false;
        inAction = true;
        anim.SetBool("mirror", slot.mirror);
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


        anim.SetFloat("vertical", v);
        anim.SetFloat("horizontal", h);

        canMove = false;
        inAction = true;
        anim.CrossFade("Rolls", 0.2f);

    }

    public void Tick(float d)
    {
        delta = d;
        onGround = OnGround();
        anim.SetBool("onGround", onGround);
    }

    void HandleMovementAnimations()
    {
        anim.SetBool("run", run);
        anim.SetFloat("horizontal", moveAmount, 0.4f, delta);
    }

    void HandleLockonAnimations(Vector3 moveDir)
    {
        Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
        float h = relativeDir.x;
        float v = relativeDir.z;

        anim.SetFloat("vertical", v, 0.2f, delta);
        anim.SetFloat("horizontal", h, 0.2f, delta);
    }

    public bool OnGround()
    {
        bool r = false;

        Vector3 origin = transform.position + (Vector3.up * distanceToGround);
        Vector3 dir = -Vector3.up;
        float dis = distanceToGround + 0.3f;
        RaycastHit hit;
        Debug.DrawRay(origin, dir * dis);
        if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayer))
        {
            r = true;
            Vector3 targetPosition = hit.point;
            transform.position = targetPosition;
        }

        return r;
    }

    public void HandleTwoHanded()
    {
        anim.SetBool("two_handed", isTwoHanded);
        if (isTwoHanded)
            actionManager.UpdateActionsTwoHanded();
        else
            actionManager.UpdateActionsOneHanded(); 
    }
}
