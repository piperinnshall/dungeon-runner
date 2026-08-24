using UnityEngine;

public interface PlayerState { }

public record Idle() : PlayerState;
public record Moving() : PlayerState;
public record Jumping() : PlayerState;
public record Falling() : PlayerState;


public class Movement : MonoBehaviour
{

    float inputHorizontal;
    float inputVertical;
    bool inputJump;

    float velocity = 5f;
    float verticalVelocity = 0f;

    float gravity = 9.8f;
    float jumpForce = 18f;
    float jumpElapsedTime = 0;
    float jumpTime = 0.85f;

    public Animator animator;
    public Transform playerCamera;
    CharacterController cc;

    public PlayerState currentState = new Idle();

    public PlayerState HandleIdle()
    {
        // Play idle animation
        Debug.Log("Player is idle");
        if (cc.isGrounded && inputJump)
        {
            return new Jumping();
        } else if (Mathf.Abs(inputHorizontal) > 0.01f || Mathf.Abs(inputVertical) > 0.01f)
        {
            return new Moving();
        } else if (!cc.isGrounded)
        {
            return new Falling();
        }
        return currentState;
    }

    public PlayerState HandleFall()
    {
        // Play falling animation
        
        Debug.Log("Player is falling");
        if (cc.isGrounded)
        {
            verticalVelocity = 0f;
            return new Idle();
        }
        return currentState;
    }

    public PlayerState HandleMove()
    {
        // Play moving animation
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
        Debug.Log("Player is moving");
        return currentState;
    }

    public PlayerState HandleJump()
    {
        Debug.Log("Player is jumping");
        jumpElapsedTime += Time.deltaTime;
        if (jumpElapsedTime >= jumpTime)
        {
            jumpElapsedTime = 0;
            return new Falling();
        }
        return currentState;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetAxis("Jump") == 1f;

        currentState = currentState switch
        {
            Idle => HandleIdle(),
            Moving => HandleMove(),
            Jumping => HandleJump(),
            Falling => HandleFall(),
            _ => currentState
        };

        // Get movement and then move the character controller
        Vector3 movement = GetMovement();
        cc.Move(movement);
    }

    // Calculate the movement vector based on input and current state
    Vector3 GetMovement()
    {

        float directionX = inputHorizontal * velocity * Time.deltaTime;
        float directionZ = inputVertical * velocity * Time.deltaTime;
        float directionY;
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
        return new Vector3(directionX, directionY, directionZ);
    }
}
