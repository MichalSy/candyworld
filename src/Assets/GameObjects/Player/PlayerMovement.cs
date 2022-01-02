using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed = 100;

    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private float _movementHorzValue;
    private bool _areLookingRight = true;

    [SerializeField] private bool _isGrounded = false;
    public Transform GroundTransform;
    public LayerMask GroundLayerMask;
    public FloatingJoystick Joystick;
    public ParticleSystem ParticleSystem;
    public AudioSource JumpAudioSource;
    public AudioSource LandAudioSource;

    private bool _isJumpingPressed = false;


    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (Joystick?.IsVisible ?? false)
        {
            _movementHorzValue = 0;
            if (Joystick.Horizontal >= 0.2)
            {
                _movementHorzValue = 1;
            }
            else if (Joystick.Horizontal <= -0.2)
            {
                _movementHorzValue = -1;
            }
        }
        else
        {
            _movementHorzValue = Input.GetAxisRaw("Horizontal");
        }



        if (_isGrounded && Input.GetButtonDown("Jump"))
        {
            _isJumpingPressed = true;
        }

        _animator.SetFloat("yVelocity", _rigidbody2D.velocity.y);
        if (_rigidbody2D.velocity.y == 0)
        {
            _animator.SetBool("isJump", false);
        }
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Move(_movementHorzValue);

    }

    private void GroundCheck()
    {
        var colliders = Physics2D.OverlapCircleAll(GroundTransform.position, 0.01f, GroundLayerMask);
        var oldValue = _isGrounded;
        _isGrounded = colliders.Length > 0;
        if (oldValue != _isGrounded && _isGrounded)
        {
            LandAudioSource.Play();
        }
    }

    private void Move(float direction)
    {
        if (_isJumpingPressed && _isGrounded)
        {
            ParticleSystem.Play();
            JumpAudioSource.Play();
            _animator.SetBool("isJump", true);
            _isJumpingPressed = false;
            _isGrounded = false;
            _rigidbody2D.AddForce(Vector2.up * 600);
        }

        var targetVelocity = new Vector2(direction * MovementSpeed * Time.fixedDeltaTime, _rigidbody2D.velocity.y);
        _rigidbody2D.velocity = targetVelocity;

        if (_areLookingRight && direction < 0)
        {
            _areLookingRight = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (!_areLookingRight && direction > 0)
        {
            _areLookingRight = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        _animator.SetFloat("xVelocity", _rigidbody2D.velocity.x != 0 ? 1 : 0);
    }
}
