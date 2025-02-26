﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JetPack : JetpackBase
{   

    #region variables

    public bool Invulnerable
    {
        get{return invulnerable;}
    }
    public bool GameOver
    {
        get{return gameOver;}
    }
    public float AutoMoveSpeed
    {
        get{return autoMoveSpeed;} 
    }
    new Camera camera;
    bool isAutoBoosting = false;
     bool gameOver = false;
    Pewpew pewpew;
    public float autoMoveSpeed;
    bool pewpewUnlocked = false;
    float countedTime = 0;
    float lastZPosition = 0;
    Transform parent;
    
    [Header("movement settings")]
    [SerializeField] float moveSpeed = 1;   
    [SerializeField]float flightBoost = 1;
    [SerializeField] float startingAutoMoveSpeed = 1;
    [SerializeField] float autoMoveSpeedIncreaseOverTime = 1;
    [SerializeField] float maxAutoMoveSpeed = 1;  

    [Header("Camera settings")]
    [SerializeField] Vector3 cameraRotation = new Vector3(0,0,0);
    [SerializeField] Vector3 cameraOffsetFromPlayer = new Vector3(0,0,0);

    //Input values

    private float horizontalSteerInput;
    private float verticalSteerInput;

    #endregion

    protected override void Awake()
    {       
        base.Awake();
        parent = transform.root;
        lastZPosition = parent.position.z;
        autoMoveSpeed = startingAutoMoveSpeed;
        camera = Camera.main;
        pewpew = GetComponentInChildren<Pewpew>();   
        
             
    }
    protected override void Start()
    {
        base.Start();
        foreach(ParticleSystem stream in fireStreams)
        {
            stream.Play();
        }
        AudioManager.instance.Play("JetpackSound");
    }

    protected override void Update() 
    {
        if (gameOver) return;
        base.Update();

        CheckIfPlayerIsStuck();
        TogglePewpew();
        Move();
        SetCameraPosition();
        float moveSpeedIncreasePerFrame = Time.deltaTime * autoMoveSpeedIncreaseOverTime;
        IncreaseAutoMoveSpeed(moveSpeedIncreasePerFrame);
    }

    private void TogglePewpew()
    {
        pewpewUnlocked = Gamemanager.instance.UnlockedItems.pewpew;
        if (pewpew != null && pewpewUnlocked)
        {
            pewpew.gameObject.SetActive(pewpewUnlocked);
            pewpew.JetpackSpeed = autoMoveSpeed;
        }
    }

    private void CheckIfPlayerIsStuck()
    {
        countedTime += Time.deltaTime;
        if (countedTime > 2 && parent != null)
        {
            countedTime = 0;
            if (parent.position.z >= lastZPosition - 1 && parent.position.z <= lastZPosition + 1)
            {
                Collider[] colliders = Physics.OverlapSphere(parent.position, 2);
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject == parent.gameObject) continue;
                    collider.enabled = false;
                }
            }
            lastZPosition = parent.position.z;
        }
    }

    void IncreaseAutoMoveSpeed(float amount)
    {
        autoMoveSpeed += amount;
        if(autoMoveSpeed >= maxAutoMoveSpeed)
        {
            autoMoveSpeed = maxAutoMoveSpeed;
        }
    }

    protected override void GetDashInput()
    {   
        if(overCharged) return;
        if(dashOnCooldown) return;
        if(!dashUnlocked) return;
        base.GetDashInput();

        if(Input.GetButtonDown("Horizontal"))
        {
            
            if(Input.GetAxis("Horizontal") < 0)
            {
                        
                if(!leftAxisPushed)
                {
                    lastKeyPressTime = Time.time;
                    leftAxisPushed = true;                       
                }   
                else
                {
                    dashDirections = DashDirections.Left;
                    dashOnCooldown = true; 
                    ResetAxisBools();
                    StartCoroutine(DashInDirection(dashDirections));
                }                                      
            }
            else if(Input.GetAxis("Horizontal") > 0)
            {
                if(!rightAxisPushed)
                {
                    lastKeyPressTime = Time.time;
                    rightAxisPushed = true;                      
                }   
                else
                {
                    dashDirections = DashDirections.Right;                   
                    dashOnCooldown = true; 
                    ResetAxisBools();
                    StartCoroutine(DashInDirection(dashDirections));
                }            
            }       
        }                    
    }
    protected override IEnumerator DashInDirection(DashDirections directions)
    {
        yield return base.DashInDirection(directions);   
        if(directions != DashDirections.Forward)
        {
            ToggleDashAnimation(true);
        } 
        while(dashTimeLeft > 0)
        {           
        switch(directions)
        {
            case DashDirections.Left:
            movement = Vector3.left * dashSpeed;
            movement.z = body.velocity.z;
            body.velocity = movement;
            if(dashEffect != null) dashEffect.transform.localRotation = Quaternion.Euler(0, 90, 0);                
            break;

            case DashDirections.Right:
            movement = Vector3.right * dashSpeed;
            movement.z = body.velocity.z;
            body.velocity = movement;
            if(dashEffect != null) dashEffect.transform.localRotation = Quaternion.Euler(0, -90, 0);
            break;

            case DashDirections.Forward:
                
            movement = Vector3.forward * dashSpeed;
            movement.z += body.velocity.z;
            body.velocity = movement;
            invulnerable = true;
            break;

            default:
            break;
        }
        dashTimeLeft -= Time.deltaTime;
        yield return new WaitForEndOfFrame();
        invulnerable = false;
            
    }
    ToggleDashAnimation(false);
}

    void Move()
    {

        movement.z = autoMoveSpeed;
        movement.x = horizontalSteerInput * moveSpeed; 
     
        body.velocity = movement;
        
    }

    void SetCameraPosition()
    {
        camera.transform.rotation = Quaternion.Euler(cameraRotation);
        camera.transform.position = transform.position + cameraOffsetFromPlayer;
    }

    void OnPlayerDeath()
    {
        gameOver = true;
        body.useGravity = true;
        body.freezeRotation = false;
        StopAllCoroutines();
    }

    private void OnEnable() {
        PlayerHealth.onPlayerDeath += OnPlayerDeath;
    }

    private void OnDisable() {
        StopAllCoroutines();
        PlayerHealth.onPlayerDeath -= OnPlayerDeath;
    }

    #region Inputs

    public void OnSteerInput(float horizontal, float vertical)
    {
        horizontalSteerInput = horizontal;
        verticalSteerInput = vertical;
    }

    public void OnDashInput()
    {
        if(overCharged) return;
        if(dashOnCooldown) return;
        if(!dashUnlocked) return;
        
        var direction = horizontalSteerInput > 0 ? DashDirections.Right : DashDirections.Left;

        dashOnCooldown = true;
        StartCoroutine(DashInDirection(direction));
    }
    
    public void OnActionInput()
    {
        Debug.Log("Jetpack ACTION");
    }
    #endregion
}
