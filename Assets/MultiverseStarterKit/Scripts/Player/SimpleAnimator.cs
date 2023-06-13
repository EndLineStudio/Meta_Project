using UnityEngine;
using Fusion.KCC;
using Fusion;

[OrderAfter(typeof(KCC))]
public class SimpleAnimator : NetworkBehaviour
{
    private NetworkMecanimAnimator netAnim;
    private KCC kcc;

    #region KCC State
    private Vector3 inputDirection;
    private bool isGrounded;
    private bool isJump;
    private bool isMoving;
    private bool isSprint;
    #endregion

    #region Animator Float Parameter
    private static readonly float IDLE = 0f;
    private static readonly float WALK = 0.5f;
    private static readonly float RUN = 1f;
    #endregion

    #region Animator Hash
    private static readonly int IS_GROUNDED_PARAM_HASH = Animator.StringToHash(RPMConstant.GROUNDED);
    private static readonly int SPEED_PARAM_HASH = Animator.StringToHash(RPMConstant.MOVESPEED);
    private static readonly int JUMP_PARAM_HASH = Animator.StringToHash(RPMConstant.JUMP);
    private static readonly int FALL_PARAM_HASH = Animator.StringToHash(RPMConstant.FALL);
    #endregion

    private void Awake()
    {
        if (TryGetComponent(out NetworkMecanimAnimator anim))
        {
            netAnim = anim;
        }
        else
        {
            Debug.LogError("Could not find NetworkMecanimAnimator component on object " + gameObject.name);
        }

        if (TryGetComponent(out KCC kccComponent))
        {
            kcc = kccComponent;
        }
        else
        {
            Debug.LogError("Could not find KCC component on object " + gameObject.name);
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsForward || IsProxy)
            return;

        isGrounded = kcc.FixedData.IsGrounded; // check grounded state from KCC
        isJump = kcc.FixedData.HasJumped; // check jump state from KCC
        isSprint = kcc.FixedData.Sprint; // chechk sprint state KCC

        CheckKCC();
        AnimatorBridge();
    }
    private void CheckKCC()
    {
        KCCData fixedData = kcc.FixedData; // using KCC Fixed data

        inputDirection = fixedData.InputDirection; // Get input direction from KCC
        isMoving = !inputDirection.IsZero();

        isGrounded = fixedData.IsGrounded; // check grounded state from KCC
        isJump = fixedData.HasJumped; // check jump state from KCC
        isSprint = fixedData.Sprint; // chechk sprint state KCC
    }
    private void AnimatorBridge()
    {
        Animator animator = netAnim.Animator; // Bridge Animator to Network Mecanim Animator

        animator.SetBool(IS_GROUNDED_PARAM_HASH, isGrounded); // set grounded bool state based on kcc.FixedData.IsGrounded

        if (isJump)
        {
            animator.SetBool(JUMP_PARAM_HASH, true); // set jump if kcc.FixedData.HasJumped
        }

        if (isGrounded)
        {
            // set false to jump and fall parameter when grounded
            animator.SetBool(JUMP_PARAM_HASH, false);
            animator.SetBool(FALL_PARAM_HASH, false);

            if (!isMoving)
            {
                animator.SetFloat(SPEED_PARAM_HASH, IDLE); // Is not moving, set to Idle
            }
            else if (isSprint)
            {
                animator.SetFloat(SPEED_PARAM_HASH, RUN); // Is moving and Sprint button pressed, set to Run
            }
            else
            {
                animator.SetFloat(SPEED_PARAM_HASH, WALK); // Is moving, set to Walk
            }
        }
        else if (!isJump)
        {
            animator.SetBool(FALL_PARAM_HASH, true); // if not grounded and not jump the it is Fall
        }
    }
}
