using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] public bool isDead = false;
    [SerializeField] public GameManager gameManager;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 5f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform feetPos;
    [SerializeField] private float groundDistance;

    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isJumping = false;

    [SerializeField] private Animator animator;

    //attack fields
    [SerializeField] private Collider2D attackCollider;
    private float attackDuration = 0.1f;
    [SerializeField] private Animator attackAnimator;
    [SerializeField] private bool isAttacking = false;

    public bool canAttack;

    private bool doubleJumped;

    // Start is called before the first frame update
    void Start()
    {
        canAttack = true;
        attackCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead)
        {
            // --Jump Mechanics start--

            isGrounded = Physics2D.OverlapCircle(feetPos.position, groundDistance, groundLayer);

            animator.SetFloat("yVelocity", rb.velocity.y);

            //reset jump
            if(isGrounded)
            {
                isJumping = false;
                doubleJumped = false;
                animator.SetBool("isJumping", isJumping);
            }

            else
            {
                // so that the fall anim plays whenever not grounded
                isJumping = true;
                animator.SetBool("isJumping", isJumping);
            }

            // jump
            if(isGrounded && Input.GetButtonDown("Jump"))
            {
                isJumping = true;
                rb.velocity = Vector2.up * jumpForce;
                animator.SetBool("isJumping", isJumping);
            }

            if(!isGrounded && Input.GetButtonDown("Jump") && !doubleJumped)
            {
                doubleJumped = true;
                rb.velocity = Vector2.up * jumpForce;
                animator.SetBool("isJumping", isJumping);
            }
            
            // --Jump mechanics end--

            // -- Attack Start --
            if(Input.GetButtonDown("Fire1") && !isAttacking && canAttack)
            {
                StartCoroutine(Attack());
            }

            //pause
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.PauseGame();
            }
        }
    }

    private IEnumerator Attack()
    {
        // Debug.Log("Attack start");

        attackCollider.enabled = true;
        isAttacking = true;

        // Debug.Log(attackCollider.isActiveAndEnabled);

        attackAnimator.SetInteger("attackIndex", Random.Range(0,2));
        attackAnimator.SetBool("isAttacking",true);        

        yield return new WaitForSeconds(attackDuration);

        attackCollider.enabled = false;
        isAttacking = false;
        attackAnimator.SetBool("isAttacking",false);
        // Debug.Log("Attack end");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            isDead = true;
            gameManager.GameOver();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Collectible"))
        {
            if(collision.gameObject.tag == "GoldenBone")
            {
                gameManager.AddPoint(50);
            }

            else
            {
                gameManager.AddPoint(1);
            }

            Debug.Log("Collected!");
            Destroy(collision.gameObject);
        }
    }

}
