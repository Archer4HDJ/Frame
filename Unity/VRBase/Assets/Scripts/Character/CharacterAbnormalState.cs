using HDJ.Framework.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbnormalState : MonoBehaviour {
    private CharacterMovementController move;
    private CharacterAnimorController anim;
	// Use this for initialization
	void Start () {
        move = GetComponent<CharacterMovementController>();
        anim = GetComponent<CharacterAnimorController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 僵直状态
    /// </summary>
    /// <param name="time"></param>
    public void RigidState(float time)
    {
        move.IsCanControlMove = false;
        anim.AnimatorSpeed = 0;
         TimerManager.SetTimerRunOnce (time,"", (name) =>
        {
            anim.AnimatorSpeed = 1;
            move.IsCanControlMove = true;
        });
    }
}
