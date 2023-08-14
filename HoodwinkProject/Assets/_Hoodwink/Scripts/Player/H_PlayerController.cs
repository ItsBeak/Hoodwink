using UnityEngine;
using Mirror;
using UnityEngine.Rendering;

public class H_PlayerController : NetworkBehaviour
{

    [Header("Movement Variables")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    public float gravityForce;

    [Header("Run Variables")]
    public float maxStaminaTime;
    float stamina;
    public float staminaRechargeOffset;
    public float staminaRechargeSpeed;
    float staminaRechargeTimer;

    [Header("Camera Variables")]
    public float lookSpeed;
    public float lookXLimit;
    public GameObject playerCamera;

    [Header("Components")]
    [HideInInspector] public CharacterController characterController;
    H_PlayerBrain brain;
    H_PlayerHealth health;

    [HideInInspector] public Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    [HideInInspector] public bool isRunning = false;
    

    void Start()
    {
        brain = GetComponent<H_PlayerBrain>();
        health = GetComponent<H_PlayerHealth>(); 
        characterController = GetComponent<CharacterController>();

        if (!isLocalPlayer) return;

        stamina = maxStaminaTime;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update()
    {
        characterController.enabled = !health.isDead;

        if (!isLocalPlayer) return;


        if (!brain.isPaused && !health.isDead)
        {
            Inputs();
            Direction();
            Turning();
        }
        else
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
        }

        CalculateRunning();

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravityForce * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

    }

    void Inputs()
    {
        moveDirection.x = brain.canMove ? Input.GetAxis("Horizontal") : 0;
        moveDirection.z = brain.canMove ? Input.GetAxis("Vertical") : 0;
    }

    void Direction()
    {    
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float movementDirectionY = moveDirection.y;
        moveDirection.y = 0;

        moveDirection.Normalize();

        moveDirection.x *= isRunning ? runSpeed : walkSpeed;
        moveDirection.z *= isRunning ? runSpeed : walkSpeed;

        moveDirection = (forward * (moveDirection.z * brain.speedMultiplier)) + (right * (moveDirection.x * brain.speedMultiplier));

        if (Input.GetButtonDown("Jump") && brain.canMove && characterController.isGrounded && brain.canJump)
        {
            moveDirection.y = jumpForce;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

    }

    void Turning()
    {
        if (brain.canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void CalculateRunning()
    {
        if (brain.canSprint && Input.GetAxis("Vertical") > 0)
        {
            if (stamina > 0)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    isRunning = true;
                    stamina -= 1 * Time.deltaTime;
                    staminaRechargeTimer = staminaRechargeOffset;
                }
                else
                {
                    isRunning = false;
                }
            }
            else
            {
                isRunning = false;
            }

        }
        else
        {
            isRunning = false;
        }

        if (isRunning == false)
        {
            staminaRechargeTimer -= 1 * Time.deltaTime;

            if (staminaRechargeTimer <= 0)
            {
                if (stamina < maxStaminaTime)
                {
                    stamina += 1 * staminaRechargeSpeed * Time.deltaTime;
                }
            }
        }

        brain.playerUI.staminaBarImage.fillAmount = Mathf.Clamp01(stamina / maxStaminaTime);

    }
}
