using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float maxSpeed;
    public float turnSpeed;
    public float attackPushForce;
    public float dodgeSpeed;
    public float dodgeMaxSpeed;

    public float decelRate;
    float activeMaxSpeed;

    Rigidbody rb;
    Vector3 moveDir;

    public Animator anim;
    public float baseAnimSpeed;

    [Header("Ground")]
    public LayerMask groundLayer;
    public Transform groundPoint;
    RaycastHit screenHit;
    bool rayHit;

    [Header("Actions")]
    public InputActionReference attackAction;
    [HideInInspector]
    public bool attacked;
    bool attackPressed;
    public bool canCancel = false;
    public float hitStopDuration;

    public InputActionReference dodgeAction;
    bool dodgePressed;
    bool dodge;
    bool dodgeCanceled;

    bool attackCancelled;

    public InputActionReference chargeAttackAction;
    bool charging;
    bool chargingCanceled;
    public float chargeTimer;
    public float chargeMaxDuration;

    [Header("Spirit Meter/Mode")]
    public float maxSpirit = 100;
    float currentSpirit;
    [HideInInspector]
    public bool spiritMode;
    public Slider spiritSlider;
    public float spiritDeprecRate = 3f;
    public float speedMultiplier = 2;
    public GameObject doll;
    GameObject currentDoll;
    bool movingToDoll;
    public float returnSpeed = 10f;
    public float healAmount = 5f;

    [Header("Health")]
    public float maxHP = 100;
    float currentHP;
    bool isDead;
    public Slider healthBar;
    public float invulnDuration = 1f;
    float invulnTimer;
    bool isInvuln;

    [Header("Intro/Outro")]
    public bool isControllable;
    public Animator hudAnim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        activeMaxSpeed = maxSpeed;

        spiritSlider.maxValue = maxSpirit;
        currentSpirit = 0;
        spiritSlider.value = currentSpirit;
        currentHP = maxHP;
        healthBar.maxValue = maxHP;
        healthBar.value = currentHP;

        anim.speed = baseAnimSpeed;
    }

    void Update()
    {
        if (!isControllable) return;
        if (movingToDoll) return;
        if (GameManager.Instance.isPaused) return;
        if (isDead) return;

        if (invulnTimer > 0 && isInvuln)
        {
            invulnTimer -= Time.deltaTime;
        } else
        {
            isInvuln = false;
        }

        attackAction.action.performed += _ => { attackPressed = true; };
        attackAction.action.canceled += _ => { attackCancelled = true; };

        charging = chargeAttackAction.action.IsPressed();
        chargeAttackAction.action.canceled += _ => { chargingCanceled = true; };

        if (charging)
        {
            anim.SetBool("Charge", true);
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Heavy_Charge")) chargeTimer += Time.deltaTime;
        }

        if (chargingCanceled)
        {
            chargingCanceled = false;
            if (chargeTimer >= chargeMaxDuration || (chargeTimer >= chargeMaxDuration/2 && spiritMode))
            {
                anim.SetTrigger("Release");
            } else
            {
                anim.SetTrigger("FailRelease");
            }
            anim.SetBool("Charge", false);
            anim.SetBool("CancelableCharge", false);
            chargeTimer = 0;
        }

        if (attackCancelled)
        {
            attackCancelled = false;
            anim.SetBool("Attack2", false);
            anim.SetBool("Attack3", false);
        }

        dodgeAction.action.performed += _ => { dodgePressed = true; };
        dodgeAction.action.canceled += _ => { dodgeCanceled = true; };

        if (dodgeCanceled)
        {
            dodgeCanceled = false;
            anim.SetBool("DodgeBool", false);
        }

        if (!attacked && attackPressed && !anim.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
        {
            attackPressed = false;
            anim.SetTrigger("Attack");
            attacked = true;
        } else if (attackPressed && anim.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
        {
            attackPressed = false;
        }

        if (!attacked && dodgePressed && !anim.IsInTransition(0) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
        {
            dodgePressed = false;
            anim.SetTrigger("Dodge");
        } else if (dodgePressed && anim.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
        {
            dodgePressed = false;
        }
        
        if (canCancel)
        {
            StartCoroutine(CheckMoveCancel());
        }

        if (spiritMode)
        {
            currentSpirit -= spiritDeprecRate * Time.deltaTime;
            if (currentSpirit <= 0)
            {
                spiritMode = false;
                anim.SetBool("SpiritMode", false);
                currentSpirit = 0;
                movingToDoll = true;
                anim.SetBool("ReturningToDoll", true);
                AudioManager.Instance.Play("Rewind");
                ResetMaxSpeed();
            }
            spiritSlider.value = currentSpirit;
        }
    }

    void FixedUpdate()
    {
        if (!isControllable) return;
        if (isDead) return;
        if (GameManager.Instance.isPaused) return;
        if (movingToDoll)
        {
            var pos = Vector3.Lerp(transform.position, currentDoll.transform.position, returnSpeed * Time.fixedDeltaTime);
            rb.MovePosition(pos);

            if (Vector3.Distance(transform.position, currentDoll.transform.position) <= 1f)
            {
                movingToDoll = false;
                anim.SetBool("ReturningToDoll", false);
                Destroy(currentDoll);
            }
            return;
        }

        if (!attacked && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_Slash_3") && !charging)
        {
            rb.velocity += moveDir.normalized * moveSpeed;
        } else if (anim.IsInTransition(0) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_Slash_3") && !charging)
        {
            rb.velocity += moveDir.normalized * (moveSpeed/2);
        }


        if (rb.velocity.magnitude > activeMaxSpeed && (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_Slash_3") || anim.IsInTransition(0)))
        {
            var localVelocity = rb.velocity;
            localVelocity.x = Mathf.Clamp(localVelocity.x, -activeMaxSpeed, activeMaxSpeed);
            localVelocity.z = Mathf.Clamp(localVelocity.z, -activeMaxSpeed, activeMaxSpeed);
            rb.velocity = localVelocity;
        }

        if (moveDir.x == 0 && (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_Slash_3") || anim.IsInTransition(0)))
        {
            var localVelocity = rb.velocity;
            localVelocity.x /= decelRate;
            rb.velocity = localVelocity;
        }

        if (moveDir.z == 0 && (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_Slash_3") || anim.IsInTransition(0)))
        {
            var localVelocity = rb.velocity;
            localVelocity.z /= decelRate;
            rb.velocity = localVelocity;
        }


        float dotForward = 0;
        float dotSide = 0;
        if (rb.velocity.magnitude > 0.5)
        {
            dotForward = Vector3.Dot(rb.velocity.normalized, transform.forward.normalized);
            dotSide = Vector3.Dot(rb.velocity.normalized, -transform.right.normalized);
        }
        
        anim.SetFloat("xVel", dotSide);
        anim.SetFloat("zVel", dotForward);
    }

    private void LateUpdate()
    {
        if (!isControllable) return;
        if (isDead) return;
        if (GameManager.Instance.isPaused) return;
        if (movingToDoll)
        {
            transform.forward = Vector3.Lerp(transform.forward, currentDoll.transform.forward, turnSpeed / 3 * Time.deltaTime);
            return;
        }

        if (rayHit && (!attacked || anim.IsInTransition(0)))
        {
            var dir = screenHit.point - groundPoint.position;

            if (dodge)
            {
                transform.forward = Vector3.Lerp(transform.forward, moveDir, 50 * Time.deltaTime);
            } else transform.forward = Vector3.Lerp(transform.forward, dir, turnSpeed * Time.deltaTime);

        }
    }

    void OnMove(InputValue input)
    {
        if (!isControllable) return;
        if (GameManager.Instance.isPaused) return;
        var temp = input.Get<Vector2>();
        moveDir = new Vector3(temp.x, 0, temp.y);

        
    }

    void OnLook(InputValue input)
    {
        if (!isControllable) return;
        if (GameManager.Instance.isPaused) return;
        var temp = input.Get<Vector2>();
        var ray = Camera.main.ScreenPointToRay(new Vector3(temp.x, temp.y, 0));

        if (Physics.Raycast(ray, out screenHit, Mathf.Infinity, groundLayer))
        {
            rayHit = true;
        } else
        {
            rayHit = false;
        }
    }
    
    IEnumerator CheckMoveCancel()
    {
        if (!isControllable) yield break;
        if (dodgePressed)
        {
            dodgePressed = false;
            anim.SetBool("DodgeBool", true);
            dodge = true;
            yield break;
        }
        yield return new WaitForSeconds(0.02f);
        if (attackPressed)
        {
            attackPressed = false;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack_Slash_2"))
            {
                anim.SetBool("Attack3", true);
            } else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
            {
                canCancel = false;
                anim.SetTrigger("Attack");
            }
            else anim.SetBool("Attack2", true);
            anim.SetBool("Cancel", false);
            yield break;
        }

        if (charging)
        {
            anim.SetBool("CancelableCharge", true);

            yield break;
        }
    }
    
    public void CallHitStop()
    {
        StartCoroutine(HitStop());
    }

    public IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(FindObjectOfType<PlayerController>().hitStopDuration);
        Time.timeScale = 1f;
    }

    public void PushPlayer()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * attackPushForce, ForceMode.Impulse);
    }

    public void Dodge()
    {
        if (!isControllable) return;
        if (spiritMode)
        {
            activeMaxSpeed = dodgeMaxSpeed * speedMultiplier;
        }
        else activeMaxSpeed = dodgeMaxSpeed;

        anim.speed = baseAnimSpeed;
        dodge = true;

        if (moveDir.magnitude == 0)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(transform.forward * dodgeSpeed, ForceMode.Impulse);
        } else
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(moveDir.normalized * dodgeSpeed, ForceMode.Impulse);
        }
        
    }

    public void ResetMaxSpeed()
    {
        if (spiritMode)
        {
            activeMaxSpeed = maxSpeed * speedMultiplier;
            anim.speed = speedMultiplier*2;
        }
        else
        {
            activeMaxSpeed = maxSpeed;
            anim.speed = baseAnimSpeed;
        }
        dodge = false;
    }

    public void IncreaseSpirit(int i)
    {
        if (!spiritMode)
        {
            currentSpirit += i;
            spiritSlider.value = currentSpirit;

            if (currentSpirit >= maxSpirit)
            {
                spiritMode = true;
                anim.SetBool("SpiritMode", true);
                AudioManager.Instance.Play("Power");
                activeMaxSpeed = maxSpeed * speedMultiplier;
                anim.speed = speedMultiplier * 2;
                currentDoll = Instantiate(doll, transform.position, transform.rotation);
            }
        }
        
    }

    public void TakeDamage(int i)
    {
        if (!isControllable) return;
        if (isDead) return;
        if (isInvuln) return;


        currentHP -= i;
        healthBar.value = currentHP;
        AudioManager.Instance.Play("Punch0");
        anim.SetTrigger("Hit");
        isInvuln = true;

        if (currentHP <= 0)
        {
            isDead = true;
            GameManager.Instance.RestartCurrentLevel();
            anim.SetBool("Dead", true);
        }
    }

    public void Heal()
    {
        currentHP += healAmount;
        healthBar.value = currentHP;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void MakeControllable()
    {
        isControllable = true;
        hudAnim.SetTrigger("HudIn");
        FindObjectOfType<EnemySpawner>().allowSpawn = true;
    }

    public void StartNextLevel()
    {
        isControllable = false;
        rb.velocity = Vector3.zero;
        hudAnim.SetTrigger("HudOut");
        anim.SetTrigger("Win");
    }
}
