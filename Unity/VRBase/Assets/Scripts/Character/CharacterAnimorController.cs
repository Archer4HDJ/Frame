using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimorController : MonoBehaviour {
    private Animator animator;
    private float animatorSpeed = 1;
    public float AnimatorSpeed
    {
        get { return animatorSpeed; }
        set { animatorSpeed = value;
        animator.speed = animatorSpeed;
        }
    }
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
    /// <summary>
    /// animator状态强制切换，ForceCrossFade不能在StateMachineBehaviour里的Update中调用，否则会死循环，如果一定要调用需要加一个标记判断
    /// </summary>
    /// <param name="name"></param>
    /// <param name="transitionDuration"></param>
    /// <param name="layer"></param>
    /// <param name="normalizedTime"></param>
    public  void ForceCrossFade( string name, float transitionDuration, int layer = 0, float normalizedTime = float.NegativeInfinity)
    {
        animator.Update(0);

        if (animator.GetNextAnimatorStateInfo(layer).fullPathHash == 0)
        {
            animator.CrossFade(name, transitionDuration, layer, normalizedTime);
        }
        else
        {
            animator.Play(animator.GetNextAnimatorStateInfo(layer).fullPathHash, layer);
            animator.Update(0);
            animator.CrossFade(name, transitionDuration, layer, normalizedTime);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.CrossFade("idle_atk", 0.2f);
            animator.CrossFade("damage", 0.2f);
            
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ForceCrossFade("idle_atk", 0.2f);
            ForceCrossFade("damage", 0.2f);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
           // animator.CrossFade("idle_atk", 0.2f);
            animator.CrossFade("damage", 0.2f);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
          //  ForceCrossFade("idle_atk", 0.2f);
            ForceCrossFade("damage", 0.2f);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("damage"))
            {
              //  animator.Play("damage", 0, 0);
                animator.CrossFade("damage", 0f,0,0);
            }
            else
            {
                ForceCrossFade("damage", 0.2f);
            }
            // animator.CrossFade("idle_atk", 0.2f);
           // animator.CrossFade("damage", 0.2f);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("damage"))
            {
                  animator.Play("damage", 0, 0);
               // animator.CrossFade("damage", 0f, 0, 0);
            }
            else
            {
                ForceCrossFade("damage", 0.2f);
            }
            // animator.CrossFade("idle_atk", 0.2f);
            // animator.CrossFade("damage", 0.2f);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("damage"))
            {
                ForceCrossFade("damage", 0, 0,0);
                // animator.CrossFade("damage", 0f, 0, 0);
            }
            else
            {
                ForceCrossFade("damage", 0.2f);
            }
            // animator.CrossFade("idle_atk", 0.2f);
            // animator.CrossFade("damage", 0.2f);
        }
       
    }
}
