using UnityEngine;
using System.Collections;

// VelocityRange Utility Class++++++++
[System.Serializable]
public class VelocityRange
{
    // PUBLIC INSTANCE VARIABLES++++++++
    public float minimum;
    public float maximum;

    // CONSTRUCTOR+++++++++++++++++++++++++++
    public VelocityRange(float minimum, float maximum)
    {
        this.minimum = minimum;
        this.maximum = maximum;
    }
}

public class HeroController : MonoBehaviour {
    //PUBLIC INSTANCE VARIABLE
    public VelocityRange velocityRange;
    public float moveForce;
    public float jumpForce;
    public Transform groundCheck;
    public Transform camera;
    public GameController gameController;

    //PRIVATE INSTANCE VARIABLE
    private Animator _animator;
    private float _move;
    private float _jump;
    private bool _facingRight;
    private Transform _transform;
    private Rigidbody2D _rigidBody2d;
    private bool _isGrounded;

	// Use this for initialization
	void Start () {

        //INTIALIZE PUBLIC INSTANCE VARIABLE
        this.velocityRange = new VelocityRange(300f, 1000f);
        

        //INTIALIZE PRIVATE INSTANCE VARIABLE
        this._transform = gameObject.GetComponent<Transform>();
        this._animator = gameObject.GetComponent<Animator>();
        this._rigidBody2d = gameObject.GetComponent<Rigidbody2D>();
        this._move = 0f;
        this._jump = 0f;
        this._facingRight = true;
        // set default animation state
        this._animator.SetInteger("AnimState", 0);

        // place a hero in a starting position
        this._spawn();

    }
	
	// Update is called once per frame
	void FixedUpdate () {

        Vector3 currentPosition = new Vector3(this._transform.position.x, this._transform.position.y,-10f);
        this.camera.position = currentPosition;

        this._isGrounded = Physics2D.Linecast(this._transform.position,
            this.groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        Debug.DrawLine(this._transform.position, this.groundCheck.position);

        float forceX = 0f;
        float forceY = 0f;
        // get absolute value of velocity for our game object
        float absValueX = Mathf.Abs(this._rigidBody2d.velocity.x);
        float absValueY = Mathf.Abs(this._rigidBody2d.velocity.y);

      
        // Ensure Player is Grounded before any movement checked
        if (this._isGrounded)
        {
            // get a number between -1 to 1 for both horizontal and vertical Axis
            this._move = Input.GetAxis("Horizontal");
            this._jump = Input.GetAxis("Vertical");
            if (this._move != 0)
            {
                if (this._move > 0)
                {
                    // movement force
                    if(absValueX < this.velocityRange.maximum)
                    {
                        forceX = this.moveForce;
                    }
                    this._facingRight = true;
                    this._flip();
                }
                if (this._move < 0)
                {
                    // movement force
                    if (absValueX < this.velocityRange.maximum)
                    {
                        forceX = -this.moveForce;
                    }
                    this._facingRight = false;
                    this._flip();
                }

                //call walk animation
                this._animator.SetInteger("AnimState", 1);
            }
            else
            {

                // set default animation state
                this._animator.SetInteger("AnimState", 0);
            }
            if (this._jump > 0)
            {
                // jump force
                if (absValueY < this.velocityRange.maximum)
                {
                    forceY = this.jumpForce;
                }

            }
        }
        else
        {
            // call the "jump" clip
            this._animator.SetInteger("AnimState", 2);
        }
        Debug.Log(forceX);
        // Apply force to the player
        this._rigidBody2d.AddForce(new Vector2(forceX, forceY));
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Death"))
        {
            this._spawn();
        }
    }

    //PRIVATE METHODS
    private void _flip()
    {
        if (this._facingRight)
        {
            this._transform.localScale = new Vector2(4, 4);
        }
        else
        {
            this._transform.localScale = new Vector2(-4, 4);
        }
    }
    private void _spawn()
    {
        this._transform.position = new Vector3(-580f,125f,0);
    }
}
