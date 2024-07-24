using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Audios_Personatge))]

public class Player : MonoBehaviour
{

    // PUBLIC VARS
    public float mouseSensivity = 400f;
    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float jumpForce = 10;
    public float dashFForce = 5;
    public float dashUpForce = 8;
    public float moveInAttackPercentage = 0.6f;

    // Weapon
    [SerializeField]CapsuleCollider espaDOTe;

    float dashAttackMultiplier = 1.3f;

    // GROUND CHECK
    public LayerMask groundedMask;
    bool grounded;

    // JUMP VARS
    bool jumpInput= false;
    float jumpBufferTime = 0.2f;
    float jumpBuffer = 0f;
    bool airDash = false;
    // jump smoothing
    float smallJumpReduction = 1f;
    float bigJump;

    // PLAYER DIRECTION
    float mouseX;
    float maxMouseSpeed = 15f;

    // MOVEMENT VARS
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    bool running;

    // ATTACK VARIABLES
    int attackType = 0;
    int strongAttackType = 0;
    // Next attack timer
    float nextAttack = 0;
    // Combo timer
    float resetAttacks = 0;
    // Last attack type
    bool strong;
    // Air Attack
    bool airAttack = false;

    Rigidbody rb;
    Animator animator;

    // MOVEMENT REFERENCE
    Transform planet;
    Planeta script_planeta;

    // PLAYER SOUNDS
    AudioSource audioSource;
    Audios_Personatge audios;

    public bool Strong { get => strong; set => strong = value; }

    void Awake()
    {
        bigJump = smallJumpReduction / 4;
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<Transform>();
        script_planeta = GameObject.FindGameObjectWithTag("Planet").GetComponent<Planeta>();

        audioSource = gameObject.GetComponent<AudioSource>();
        audios = gameObject.GetComponent<Audios_Personatge>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        animator = GetComponent<Animator>();

        // Set initial position
        transform.position = planet.position + planet.up * (script_planeta.shapeSettings.planetRadius + 10);

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (Controls_generals.game_paused)
        {
            return;
        }
        mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime; // Rotar personatge amb ratolí
        mouseX = Mathf.Clamp(mouseX, -maxMouseSpeed, maxMouseSpeed);

        float inputX = Input.GetAxisRaw("Horizontal"); // Moviment X
        float inputY = Input.GetAxisRaw("Vertical"); // Moviment Z

        running = false;
        if (inputY != -1)
        {
            //Movement
            running = Input.GetKey(KeyCode.LeftShift);
        }

        float targetSpeed = ((running) ? runSpeed : walkSpeed);

        Vector3 targetMoveAmount = new Vector3(inputX, 0, inputY).normalized * targetSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

        // Grounded check with sphere for better refference
        grounded = Physics.CheckSphere(transform.position, 0.2f, groundedMask);
        if (grounded)
        {
            airDash = true;
            animator.ResetTrigger("AirDash");
            airAttack = true;
        }

        // Jump
        jumpBuffer -= Time.deltaTime;
        jumpInput = Input.GetButtonDown("Jump");
        if (jumpInput) jumpBuffer = jumpBufferTime;
        // Atack check
        Attack();

        // Movement animation
        animator.SetBool("grounded", grounded);
        float currentSpeed = moveAmount.magnitude;
        float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
        animator.SetFloat("vSpeed", inputY * animationSpeedPercent, 0.1f, Time.deltaTime);
        animator.SetFloat("hSpeed", inputX * animationSpeedPercent, 0.2f, Time.deltaTime);

        // Rotation animation
        animator.SetFloat("turnSpeed", mouseX, 0.01f, Time.deltaTime);
    }

    void FixedUpdate()
    {
        // Allign bodies up axis with the centre of planet and apply down force
        Vector3 gravityUp = (transform.position - planet.position).normalized;
        rb.AddForce(gravityUp * (-script_planeta.shapeSettings.planetGravity));
        Quaternion targetTurn = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.rotation = targetTurn;

        // Rotar personatge amb ratolí
        transform.Rotate(Vector3.up * mouseX);

        // Moure
        rb.MovePosition(rb.position + transform.TransformVector(moveAmount) * Time.fixedDeltaTime);

        // Jump
        StartCoroutine(Jump());
    }

    void Attack()
    {
        nextAttack -= Time.deltaTime;
        resetAttacks -= Time.deltaTime;
        if (resetAttacks <= 0)
        {
            attackType = 0;
            strongAttackType = 0;
        }
        if (Input.GetMouseButtonDown(0) && nextAttack <= 0)
        {
            Strong = false;

            // Time between atacks
            nextAttack = .5f;

            // Reset combos
            resetAttacks = 1.3f;

            string lightAttack;
            float percentage = moveAmount.magnitude / runSpeed;

            // Dash Attack
            if (percentage >= 0.85f && grounded)
            {
                lightAttack = "Attack" + 2;
                rb.AddRelativeForce(0, 0, dashAttackMultiplier * percentage * runSpeed, ForceMode.Impulse);
                // Play matching sound and attack animation
                PlayDashAttackSound();
                animator.SetTrigger(lightAttack);

                // Reset attacks on dash
                attackType = 0;
            }
            else
            {
                attackType++;
                // Chose attack based on last attack and combo
                if (attackType > 3) attackType = 1;
                lightAttack = "Attack" + attackType;

                // Play matching sound and attack animation
                PlayAttackSound(attackType);
                animator.SetTrigger(lightAttack);
            }
            if (!grounded && airAttack && airDash)
            {
                rb.AddRelativeForce(0, dashUpForce, dashFForce, ForceMode.Impulse);
                airAttack = false;
            }
        }

        if (Input.GetMouseButtonDown(1) && nextAttack <= 0)
        {
            Strong = true;
            //  Time between strong atacks
            nextAttack = .8f;

            // Reset combos
            resetAttacks = 1.3f;
            strongAttackType++;

            // Chose attack based on last attack and combo
            if (strongAttackType > 2) strongAttackType = 1;
            string strongAttack = "StrongAttack" + strongAttackType;
            float percentage = moveAmount.magnitude / runSpeed;

            // Dash Attack
            if (percentage >= 0.85f && grounded)
            {
                strongAttackType = 1;
                strongAttack = "StrongAttack" + strongAttackType;
                rb.AddRelativeForce(0, 0, dashAttackMultiplier * runSpeed, ForceMode.Impulse);
            }
            // Play matching sound and attack animation
            PlayStrongAttackSound(strongAttackType);
            animator.SetTrigger(strongAttack);
            if (!grounded && airAttack && airDash)
            {
                rb.AddRelativeForce(0, dashUpForce, dashFForce, ForceMode.Impulse);
                airAttack = false;
            }
        }
        espaDOTe.enabled = false;
        // Uncheck third attack to avoid forming a queue of attacks
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") || animator.GetCurrentAnimatorStateInfo(0).IsName("StrongAttack2"))
        {
            // Enable sword
            espaDOTe.enabled = true;
            if (grounded)
                moveAmount = moveAmount * moveInAttackPercentage;
            else
                moveAmount = Vector3.zero;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            // Enable sword with custom delayed hitbox
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5)
                espaDOTe.enabled = true;
            if (grounded)
                moveAmount = moveAmount * moveInAttackPercentage;
            else
                moveAmount = Vector3.zero;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") || animator.GetCurrentAnimatorStateInfo(0).IsName("StrongAttack1"))
        {
            // Enable sword with custom delayed hitbox
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3)
                espaDOTe.enabled = true;
            if (grounded)
                moveAmount = moveAmount * moveInAttackPercentage;
            else
                moveAmount = Vector3.zero;
        }
    }

    IEnumerator Jump()
    {
        if (!grounded && jumpBuffer>0 && airDash && airAttack)
        {
            airDash = false;
            jumpBuffer = 0;
            jumpInput = false;
            animator.SetTrigger("AirDash");
            rb.AddRelativeForce(0, dashUpForce, dashFForce, ForceMode.Impulse);
            PlayJumpSound(2);

        }
        if (!grounded && !Input.GetButton("Jump"))
        {
            Gravity(smallJumpReduction);
        }
        else if (!grounded)
        {
            Gravity(bigJump);
        }

        if (jumpBuffer>0)
        {
            if (grounded && (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !animator.GetCurrentAnimatorStateInfo(0).IsName("AirDash") && !animator.IsInTransition(0)))
            {
                jumpInput = false;
                jumpBuffer = 0;
                animator.SetTrigger("TriggerJump");
                yield return new WaitForSeconds(0.2f);

                rb.velocity = Vector3.zero;
                rb.AddRelativeForce(0,jumpForce,0, ForceMode.Impulse);
                PlayJumpSound(1);
            }
        }
    }

    // Apply extra gravity to smooth jump
    public void Gravity(float times)
    {
        // Gravity
        Vector3 gravityUp = (transform.position - planet.position).normalized;
        rb.AddForce(gravityUp * (-script_planeta.shapeSettings.planetGravity * times));
    }

    void PlayAttackSound(int whichSound)
    {
        audioSource.PlayOneShot(audios.GetAttackClip(whichSound));
    }

    void PlayDashAttackSound()
    {
        audioSource.PlayOneShot(audios.GetDashAttackClip());
    }

    void PlayStrongAttackSound(int whichSound)
    {
        audioSource.PlayOneShot(audios.GetStrongAttackClip(whichSound));
    }

    void PlayJumpSound(int j)
    {
        audioSource.PlayOneShot(audios.GetJumpClip(j));
    }

    void PlayDieSound()
    {
        audioSource.PlayOneShot(audios.GetDieClip());
    }

    public bool getLastAttackType()
    {
        return Strong;
    }
}