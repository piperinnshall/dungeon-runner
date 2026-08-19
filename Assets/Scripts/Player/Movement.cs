using UnityEngine;

public interface PlayerState { }

public record Idle() : PlayerState;
public record Moving(float speed) : PlayerState;
public record Jumping(float height) : PlayerState;
public record Falling(float gravity) : PlayerState;


public class Movement : MonoBehaviour
{

    float inputHorizontal;
    float inputVertical;

    float velocity = 5f;

    float gravity = 9.8f;
    float jumpForce = 18f;
    float jumpElapsedTime = 0;
    public float jumpTime = 0.85f;

    public Animator animator;
    CharacterController cc;

    public PlayerState currentState = new Idle();

    public PlayerState HandleIdle()
    {
        // Play idle animation
        Debug.Log("Player is idle");
        if (cc.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            return new Jumping(jumpForce);
        } else if (inputHorizontal > 0 || inputVertical > 0)
        {
            return new Moving(velocity);
        } else if (!cc.isGrounded)
        {
            return new Falling(gravity);
        }
        return currentState;
    }

    public PlayerState HandleFall(float gravity)
    {
        // Play falling animation
        // Apply falling physics
        Debug.Log("Player is falling");
        if (cc.isGrounded)
        {
            return new Idle();
        }
        return currentState;
    }

    public PlayerState HandleMove()
    {
        float directionX = inputHorizontal * velocity * Time.deltaTime;
        float directionY = inputVertical * velocity * Time.deltaTime;
        if (!cc.isGrounded)
        {
            return new Falling(gravity);
        }
        Debug.Log("Player is moving");
        return currentState;
    }

    public PlayerState HandleJump(float jumpForce)
    {
        Debug.Log("Player is jumping");
        float directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;
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

        currentState = currentState switch
        {
            Idle => HandleIdle(),
            Moving moving => HandleMove(),
            Jumping jumping => HandleJump(jumpForce),
            Falling falling => HandleFall(gravity),
            _ => currentState
        };
    }
}
