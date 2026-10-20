using NUnit.Framework.Interfaces;
using System;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using Unity.AI.Navigation.LowLevel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public interface PlayerState { }

public record Idle() : PlayerState;
public record Moving() : PlayerState;
public record Jumping() : PlayerState;
public record Falling() : PlayerState;
public record Attacking() : PlayerState;
public record Blocking() : PlayerState;
public record Throwing() : PlayerState;
public record Aiming() : PlayerState;


public class Movement : MonoBehaviour
{
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputAttack;
    public bool inputBlock;

    // The horizontal velocity of the player, used for moving
    float velocity = 5.5f;
    // The vertical velocity of the player, used for jumping and falling
    float verticalVelocity = 0f;

    float gravity = 2.7f;
    // The force applied to the player when jumping, the higher the value, the higher the player will jump
    float jumpForce = 2f;
    // The time elapsed since the player started jumping, used to determine when to start falling
    float jumpElapsedTime = 0;
    // Max time the player can stay in the air while jumping, after this time the player will start falling
    float jumpTime = 0.5f;

    public bool canMove = true;

    // Time Player has to wait until they can attack again
    float attackCoolldown = 0.5f;
    // Variable for storing attack time
    float nextAttackTime = 0f;
    // Amount of damage player does to enemies with melee attack
    int damage = 1;
    // How far the player's attack reaches
    float attackRange = 1.5f;
    // Time when the player started attacking, used to determine when to stop attacking
    float attackStartTime = 0f;
    // Duration of the attack animation in seconds
    float attackDuration; 
    // Ensure Attack() is only called once per attack window
    bool hasAttacked;
    // Is the player blocking?
    public bool isBlocking = false;

    [SerializeField] Bomb bomb;
    private GameObject currentBomb;

    private Equipment equipmentScript;
    [SerializeField] BoomerangProjectile boomerang;
    // Has the player thrown the boomerang? Used to prevent multiple throws
    public bool thrownBoomerang = false;
    public Animator _animator;
    public Transform playerCamera;
    public Transform attackPoint;
    CharacterController cc;

    public PlayerState currentState = new Idle();

    public PlayerState HandleIdle()
    {
        canMove = true;
        //Debug.Log("Player is idle");
        if (cc.isGrounded && inputJump)
        {
            return new Jumping();
        }
        else if (Mathf.Abs(inputHorizontal) > 0.01f || Mathf.Abs(inputVertical) > 0.01f)
        {
            return new Moving();
        }
        else if (!cc.isGrounded)
        {
            return new Falling();
        }
        else if (cc.isGrounded && Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && !equipmentScript.hasBoomerang)
        {
            attackStartTime = Time.time;
            hasAttacked = false;
            return new Attacking();
        } else if (cc.isGrounded && Input.GetMouseButtonDown(0) && equipmentScript.hasBoomerang && thrownBoomerang == false)
        {
            return new Throwing();
        }
        else if (cc.isGrounded && Input.GetMouseButton(1) && !equipmentScript.hasBoomerang && equipmentScript.hasSwordAndShield)
        {
            return new Blocking();
        }
        else if (cc.isGrounded && Input.GetMouseButton(1) && equipmentScript.hasBoomerang)
        {
            return new Aiming();
        }
        return currentState;
    }

    public PlayerState HandleFall()
    {
        // Play falling animation
        canMove = true;
        //Debug.Log("Player is falling");
        if (cc.isGrounded)
        {
            verticalVelocity = 0f;
            return new Idle();
        }
        return currentState;
    }

    public PlayerState HandleMove()
    {
        canMove = true;

        //animator.Play("Move");

        if (!cc.isGrounded)
        {
            return new Falling();
        } else if (inputJump)
        {
            return new Jumping();
        } else if (Mathf.Abs(inputHorizontal) < 0.01f && Mathf.Abs(inputVertical) < 0.01f)
        {
            return new Idle();
        }
        else if (cc.isGrounded && Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && equipmentScript.hasSwordAndShield)
        {
            attackStartTime = Time.time;
            hasAttacked = false;
            return new Attacking();
        }
        else if (cc.isGrounded && Input.GetMouseButton(1) && equipmentScript.hasSwordAndShield)
        {
            return new Blocking();
        }
        //Debug.Log("Player is moving");
        return currentState;
    }

    public PlayerState HandleJump()
    {
        canMove = true;
        _animator.SetTrigger("JumpTrigger");

        //Debug.Log("Player is jumping");
        jumpElapsedTime += Time.deltaTime;
        if (jumpElapsedTime >= jumpTime)
        {
            jumpElapsedTime = 0;
            return new Falling();
        }
        return currentState;
    }

    public PlayerState HandleAttack()
    {
        _animator.SetTrigger("AttackTrigger");
        attackDuration = 0.8f;
        _animator.SetBool("Attacked", hasAttacked);
        if (!hasAttacked)
        {
            canMove = false;
            Invoke(nameof(Attack), 0.4f);
            hasAttacked = true;
            //Debug.Log("Player is attacking");
            // Play attack animation
            nextAttackTime = Time.time + attackCoolldown;
        }

        if(Time.time - attackStartTime >= attackDuration)
        {
            return new Idle();
        }

        return currentState;
    }

    public PlayerState HandleBlock()
    {
        canMove = false;
        // Play blocking animation
        //Debug.Log("Player is blocking");
        StartBlocking();
        if (Input.GetMouseButtonUp(1))
        {
            StopBlocking();
            return new Idle();
        }
        return currentState;
    }

    public PlayerState HandleThrow()
    {
        canMove = false;
        // Play throwing animation
        //Debug.Log("Player is throwing");
        _animator.SetTrigger("ThrowTrigger");
        Invoke(nameof(ThrowBoomerang), 0.3f);

        return new Idle();
    }

    public PlayerState HandleAim()
    {
        canMove = false;
        playerCamera.Translate(Vector3.forward * 0.2f);
        if(Input.GetMouseButtonUp(1))
        {
            playerCamera.Translate(Vector3.back * 0.2f);
            return new Idle();
        }
        else if (Input.GetMouseButtonDown(0) && thrownBoomerang == false)
        {
            playerCamera.Translate(Vector3.back * 0.2f);
            return new Throwing();
        }
        return currentState;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        equipmentScript = GetComponent<Equipment>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from player
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetAxis("Jump") == 1f;

        // State machine
        currentState = currentState switch
        {
            Idle => HandleIdle(),
            Moving => HandleMove(),
            Attacking => HandleAttack(),
            Blocking => HandleBlock(),
            Jumping => HandleJump(),
            Falling => HandleFall(),
            Throwing => HandleThrow(),
            Aiming => HandleAim(),
            _ => currentState
        };

        // Bomb controls
        if(Input.GetKeyDown(KeyCode.G) && bomb != null)
        {
            currentBomb = Instantiate(bomb.gameObject, transform.position + transform.forward * 1.5f, Quaternion.identity);
            currentBomb.GetComponent<Bomb>().Ignite();
        }

        bool isGrounded = cc.isGrounded;
        _animator.SetBool("IsGrounded", isGrounded);

        // Get movement direction and then move the character controller 
        Vector3 movement = GetMovement();
        if (canMove)
        {
            cc.Move(movement);
        }

        // Calculate forward direction value for animation
        float forwardValue = Vector3.Dot(transform.forward, movement.normalized);
        _animator.SetFloat("Forward", forwardValue);

        // Calculate upward direction value for animation
        float upwardValue = Vector3.Dot(transform.up, movement.normalized);
        _animator.SetFloat("Upward", upwardValue);
    }

    // Calculate the movement vector based on input and current state, horizontal movement based on camera direction
    Vector3 GetMovement()
    {
        float directionX = inputHorizontal * velocity * Time.deltaTime;
        float directionZ = inputVertical * velocity * Time.deltaTime;
        float directionY;

        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        // Flatten the forward and right vectors to ignore vertical movement
        forward.y = 0f;
        right.y = 0f;

        // Normalize the vectors to ensure consistent movement speed
        forward.Normalize();
        right.Normalize();

        // Scale the forward and right vectors by the input direction
        forward = forward * directionZ;
        right = right * directionX;

        if(directionX != 0f || directionZ != 0f)
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        if (currentState is Jumping)
        {
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;
        }
        else
        {
            // Apply gravity when not jumping
            if (cc.isGrounded)
            {
                // Ensures player remains grounded when on ground and not falling
                verticalVelocity = -2f;
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }
            directionY = verticalVelocity * Time.deltaTime;
        }
        // Combine horizontal and vertical movement
        Vector3 horizontalDirection = forward + right;
        Vector3 verticalDirection = Vector3.up * directionY;

        return horizontalDirection + verticalDirection;
    }

    void Attack()
    {
        // Detect enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange);

        // For each enemy hit, apply damage
        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, transform.position);
                //Debug.Log("Enemy Health: " + enemyHealth.health);
            }
        }
    }

    void StartBlocking()
    {
        isBlocking = true;
        _animator.SetBool("IsBlocking", true);
    }

    void StopBlocking()
    {
        isBlocking = false;
        _animator.SetBool("IsBlocking", false);
    }

    void ThrowBoomerang()
    {
        thrownBoomerang = true;
        // Spawn the boomerang in front of the player
        Vector3 spawnPosition = transform.position + transform.forward * 1.5f;
        Quaternion boomerangRotation = transform.rotation;
        Instantiate(boomerang, spawnPosition, boomerangRotation);
        BoomerangProjectile boomerangScript = boomerang.GetComponent<BoomerangProjectile>();
        boomerangScript.Initialize(transform.forward);
    }
}
