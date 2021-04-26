using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureJetpack : JetpackBase //make script check if jetpack is unlocked
{
    
    PlayerMovement playerMovement;
    CharacterController controller;
    Renderer[] jetpack;  
    public int currentJumpCount;
    bool jetpackUnlocked = false;
    ThirdPersonController player;

    [Header("Jetpack boost settings")]
    [SerializeField] float jumpHeight = 1;
    [SerializeField] int maxJumpsInAir;
    [SerializeField] float coyoteTime = 1;

    // Start is called before the first frame update
    protected override void Awake() {
        base.Awake();
        player = GetComponentInParent<ThirdPersonController>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        controller = GetComponentInParent<CharacterController>();
        jetpack = GetComponentsInChildren<Renderer>();
        currentJumpCount = maxJumpsInAir;
    }

    // Update is called once per frame
    protected override void Update()
    {         
        base.Update();
        foreach(Renderer renderer in jetpack) 
        {
            renderer.enabled = Gamemanager.instance.UnlockedItems.jetpack;
        }   

        jetpackUnlocked = Gamemanager.instance.UnlockedItems.jetpack;
        if(!jetpackUnlocked) return;
        
        if (playerMovement.isGrounded)
        {
            currentJumpCount = maxJumpsInAir;
        }
    }

    protected override void GetDashInput()
    {
        if(overCharged) return;
        if(dashOnCooldown) return;
        if(!dashUnlocked) return;
        base.GetDashInput();
    }

    protected override IEnumerator DashInDirection(DashDirections directions)
    {
        bool groundedStart = playerMovement.isGrounded;
        print(groundedStart);
        player.disablePlayerMovement = true;
        yield return base.DashInDirection(directions);
        while(dashTimeLeft > 0)
        {           
            switch(directions)
            {
                case DashDirections.Forward:
                movement = gameObject.transform.parent.transform.forward * dashSpeed;
                controller.Move(movement * Time.deltaTime);
                invulnerable = true;
                break;

                default:
                break;
            }
            dashTimeLeft -= Time.deltaTime;
            cooldown -= Time.deltaTime;
            yield return new WaitForEndOfFrame();           
        }
        invulnerable = false;
        player.disablePlayerMovement = false;
        print(playerMovement.isGrounded);
        if(!playerMovement.isGrounded && groundedStart)
        {
            player.disablePlayerMovement = true;
            print("test");
            float coyoteTimeLeft = coyoteTime;
            while(coyoteTimeLeft > 0)
            {
                coyoteTimeLeft -= Time.deltaTime;
                cooldown -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        
        }
        player.disablePlayerMovement = false;
        
        //-------------------------------------------count the remaining cooldown after dashing
        while(cooldown > 0)
        {
            yield return new WaitForEndOfFrame();
            cooldown -= Time.deltaTime;
        }
        dashOnCooldown = false;
    }


    public void OnJump()
    {
        if (!playerMovement.isGrounded && !overCharged && Gamemanager.instance.UnlockedItems.jetpack && currentJumpCount > 0 && !player.disablePlayerMovement)
        {
            currentJumpCount--;
            playerMovement.velocity.y = Mathf.Sqrt(jumpHeight * -2f * playerMovement.gravity);
            UseFuel(fuelUsage);
        }
    }
}
