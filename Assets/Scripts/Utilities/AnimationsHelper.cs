using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsHelper : MonoBehaviour {

    [Range(0, 1)]
    public float vertical;
    [Range(0, 1)]
    public float horizontal;

    public bool twoHanded;
    public bool useItem;
    public bool interacting;

    public bool enableRootMotion;

    public string[] oh_attacks;
    public string[] th_attacks;
    public bool playAnim;

    Animator anim;
    
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        enableRootMotion = !anim.GetBool("canMove");
        anim.applyRootMotion = enableRootMotion;

        interacting = anim.GetBool("interacting");
        
        if (enableRootMotion) {
            return;     
        }
        
        if (useItem)
        {
            anim.Play("use_item");
            useItem = false;
        }

        if (interacting) {
            playAnim = false;
            horizontal = Mathf.Clamp(horizontal, 0, 0.5f);
        }

        anim.SetFloat("vertical", vertical);
        anim.SetFloat("horizontal", horizontal);
        anim.SetBool("twoHanded", twoHanded);

        if (playAnim) {
            string targetAnim;
            if (!twoHanded)
            {
                int r = Random.Range(0, oh_attacks.Length);
                targetAnim = oh_attacks[r];

                if (horizontal > 0)
                {
                    targetAnim = "oh_attack_3";
                }
            }
            else {
                int r = Random.Range(0, th_attacks.Length);
                targetAnim = th_attacks[r];

                if (horizontal > 0)
                {
                    targetAnim = "oh_attack_3";
                }
            }

            vertical = 0;
            anim.CrossFade(targetAnim, 0.2f);
            //anim.SetBool("caMove", false);
            //enableRootMotion = true;
            playAnim = false;
        }

        useItem = false;

	}
}
