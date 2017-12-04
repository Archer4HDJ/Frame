using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
public class CharacterMovementController : MonoBehaviour {
    public float runSpeed = 7.0f;
    public float walkSpeed = 2.0f;
     [Range(0f, 1f)]
    [SerializeField]
    float m_crouchSpeedMultiplier = 0.4f;

    private Rigidbody m_Rigidbody;
    private Vector3 velocity;
    private Animator m_Animator;

    [SerializeField]
    float m_MovingTurnSpeed = 360;
    [SerializeField]
    float m_StationaryTurnSpeed = 180;
    [SerializeField]
    float m_JumpPower = 12f;
    [Range(1f, 4f)]
    [SerializeField]
    float m_GravityMultiplier = 2f;
    [SerializeField]
    float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    [SerializeField]
    float m_MoveSpeedMultiplier = 1f;
    [SerializeField]
    float m_AnimSpeedMultiplier = 1f;
    [SerializeField]
    float m_GroundCheckDistance = 0.3f;

    static int locoState = Animator.StringToHash("MovementLayer.Locomotion");
    private int layerNum = 0;

    bool m_IsGrounded;
    float m_OrigGroundCheckDistance;
    const float k_Half = 0.5f;
    float m_TurnAmount;
    float m_ForwardAmount;
    Vector3 m_GroundNormal;
    float m_CapsuleHeight;
    Vector3 m_CapsuleCenter;
    CapsuleCollider m_Capsule;
    bool m_Crouching;

    private bool isCanControlMove = true;
    public bool IsCanControlMove
    {
        get { return isCanControlMove; }
        set { isCanControlMove = value;

        if (!isCanControlMove)
        {
            m_Animator.SetFloat("Turn", 0);
            m_Animator.SetFloat("Forward", 0);
            m_Animator.SetBool("Crouch", false);
        }
        }
    }

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CapsuleHeight = m_Capsule.height;
        m_CapsuleCenter = m_Capsule.center;
      //  m_Rigidbody.useGravity = true;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        m_OrigGroundCheckDistance = m_GroundCheckDistance;

    }
    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
          //  m_Animator.applyRootMotion = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
           // m_Animator.applyRootMotion = false;
        }
    }
    public void Move(Vector3 move,bool isRun, bool crouch, bool jump)
    {

        if (!isCanControlMove) return;
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;

        ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (m_IsGrounded)
        {
            HandleGroundedMovement(crouch, jump);
        }
        else
        {
            HandleAirborneMovement();
        }

        ScaleCapsuleForCrouching(crouch);
        PreventStandingInLowHeadroom();
        MoveFunction(crouch,move,isRun);
        UpdateAnimator(move,isRun);
    }
    void ScaleCapsuleForCrouching(bool crouch)
    {
        if (m_IsGrounded && crouch)
        {
            if (m_Crouching) return;
            m_Capsule.height = m_Capsule.height / 2f;
            m_Capsule.center = m_Capsule.center / 2f;
            m_Crouching = true;
        }
        else
        {
            Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_Crouching = true;
                return;
            }
            m_Capsule.height = m_CapsuleHeight;
            m_Capsule.center = m_CapsuleCenter;
            m_Crouching = false;
        }
    }

    void PreventStandingInLowHeadroom()
    {
        // prevent standing up in crouch-only zones
        if (!m_Crouching)
        {
            Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_Crouching = true;
            }
        }
    }


    void UpdateAnimator(Vector3 move,bool isRun)
    {
       // Debug.Log(moveData);
        float Forward;
        if (move.magnitude >0.1f)
            Forward =  isRun ? 1f : 0.5f;
        else
            Forward = 0;
        m_Animator.SetFloat("Turn", m_TurnAmount);
        m_Animator.SetFloat("Forward", Forward);
        m_Animator.SetBool("Crouch", m_Crouching);
        m_Animator.SetBool("OnGround", m_IsGrounded);
        if (!m_IsGrounded)
        {
            m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
        }

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                m_Animator.GetCurrentAnimatorStateInfo(layerNum).normalizedTime + m_RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
        if (m_IsGrounded)
        {
            m_Animator.SetFloat("JumpLeg", jumpLeg);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (m_IsGrounded && move.magnitude > 0)
        {
            m_Animator.speed = m_AnimSpeedMultiplier;
        }
        else
        {
            // don't use that while airborne
            m_Animator.speed = 1;
        }
    }


    void HandleAirborneMovement()
    {
        // apply extra gravity from multiplier:
        Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
        m_Rigidbody.AddForce(extraGravityForce);

        m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
    }


    void HandleGroundedMovement(bool crouch, bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(layerNum).fullPathHash == locoState)
        {
            // jump!
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
            m_IsGrounded = false;
            m_Animator.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }
    }
 
    void MoveFunction(bool crouch,Vector3 move,bool isRun)
    {
        if (m_IsGrounded)
        {
            velocity = move;
            velocity = transform.TransformDirection(velocity);

            if (crouch)
            {
                velocity = velocity * m_crouchSpeedMultiplier;
            }
            else
            {

                if (isRun)
                {
                    velocity *= runSpeed;
                }
                else
                {
                    velocity *= walkSpeed;
                }

            }
            velocity.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = velocity;
        }
    }
    void ApplyExtraTurnRotation()
    {
       
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        float turn =  m_TurnAmount * turnSpeed * Time.deltaTime;
      //  Debug.Log("m_TurnAmount:" + m_TurnAmount);
        transform.Rotate(0,turn, 0);
    }
    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 260, 10, 250, 150), "Interaction");
        GUI.Label(new Rect(Screen.width - 245, 30, 250, 30), "Up/Down Arrow : Go Forwald/Go Back");
        GUI.Label(new Rect(Screen.width - 245, 50, 250, 30), "Left/Right Arrow : Turn Left/Turn Right");
        GUI.Label(new Rect(Screen.width - 245, 70, 250, 30), "Hit Space key while Running : Jump");
        GUI.Label(new Rect(Screen.width - 245, 90, 250, 30), "Hit C key crouch");

    }

	}

