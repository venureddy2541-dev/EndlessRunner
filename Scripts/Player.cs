using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float speedFac = 6f,xClampVal,zClampVal;
    Vector2 movePos;
    Rigidbody rb;

    [SerializeField] LayerMask layers;
    [SerializeField] float radius;
    [SerializeField] float offset;
    float gravity = -9.81f;
    bool slide = false;
    bool jump = false;
    bool isGorunded;
    [SerializeField] float jumpHeight;
    float initialVelocity = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnDrawGizmos()
    {
        if(isGorunded)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawWireSphere((transform.position + Vector3.up*offset),radius);
    }

    void FixedUpdate()
    {
        float speed = speedFac * Time.fixedDeltaTime;
        
        Vector3 startPos = rb.position;
        Vector3 movementPos = new Vector3(movePos.x * speed , 0f , movePos.y * speed);

        Vector3 ClmapPos = startPos + movementPos;

        float XClampValue = Mathf.Clamp(ClmapPos.x,-xClampVal,xClampVal);
        float ZClampValue = Mathf.Clamp(ClmapPos.z,-zClampVal,zClampVal);

        Vector3 finalPos = new Vector3(XClampValue,this.transform.position.y,ZClampValue);
        rb.MovePosition(finalPos + new Vector3(0f,initialVelocity,0f)*Time.fixedDeltaTime); 

        isGorunded = Physics.CheckSphere(transform.position + Vector3.up*offset,radius,layers);

        initialVelocity += gravity*Time.fixedDeltaTime;

        if(isGorunded)
        {
            if(initialVelocity < 0.01f)
            {
                initialVelocity = -2f;
            }

            if(jump)
            {
                initialVelocity = Mathf.Sqrt(-2f*gravity*jumpHeight);
            }
        }
        else
        {
            jump = false;
        }
    }

    public void ResetSlideAnimations()
    {
        animator.SetBool("slide",false);
    }

    public void ResetHitAnimations()
    {
        animator.SetBool("gotHit",false);
    }

    public void Move(InputAction.CallbackContext context)
    {
        movePos = context.ReadValue<Vector2>();
    }
    public void Slide(InputAction.CallbackContext context)
    {
        if(!isGorunded) return;

        slide = context.performed;
        if(slide)
        {
            animator.SetBool("slide",true);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(animator.GetBool("slide")) return;

        jump = context.performed;
        if(jump)
        {
            animator.SetBool("jump",true);
        }
    }

    public void ResetJumpAnimations()
    {
        animator.SetBool("jump",false);
    }
}
