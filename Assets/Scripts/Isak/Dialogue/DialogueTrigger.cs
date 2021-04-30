using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour // improve to use reward system
{
    [SerializeField] NPCDialogue dialogues; // variable for all the dialogues (scriptable object)

    public static Events.DialogueEvent onNPCDialogue; // event used for printing out the dialogues sentence and name to screen
    NPCDialogue.Dialogue dialogueToCheck; 
    NPCDialogue.Dialogue dialogueToUse; 
    NPCDialogue.Dialogue lastDialogueUsed; 
    int timesSpokenWith = 0;
    string GetNextDialogue() 
    {
        string nextDialogue = dialogueToUse.Sentences[timesSpokenWith]; 
        if(timesSpokenWith < dialogueToUse.Sentences.Length - 1) // if not at final dialogue sentence, increase times spoken with
        {
            timesSpokenWith++;
        }
        else if(dialogueToUse.RepeatDialogue) // if dialogue is set to repeat, set times spoken with back to 0 after final dialogue sentence
        {
            timesSpokenWith = 0;
        }   
        return nextDialogue;
    }

    void CheckIfNewDialogue() 
    {        
        if(dialogueToUse != lastDialogueUsed && lastDialogueUsed != null) //if there is a difference between the dialogue to use and the last used dialogue
        {
            timesSpokenWith = 0;
        }
        lastDialogueUsed = dialogueToUse; // change which dialogue was last used
    }
    public void TriggerDialogue()
    {   
        dialogueToUse = ChooseDialogue();
        CheckIfNewDialogue();
        if(dialogueToUse == null)
        {
            Debug.LogError("requirements for dialogue is not met");
            return;
        }
        if(onNPCDialogue != null)
        {
            onNPCDialogue(GetNextDialogue(), dialogues.Name);
        }
    }
    NPCDialogue.Dialogue ChooseDialogue()
    {
            foreach(NPCDialogue.Dialogue dialogue in dialogues.Dialogues) //loop through all dialogues
            {
                dialogueToCheck = dialogue;
                if(CheckCurrentDialogue())
                {
                    return dialogue; // found a dialogue that fulfills requirement
                }
            }  
            return null;    // found no dialogues that fulfills requirement
    }

    bool CheckCurrentDialogue() // this will look like shit, improve over iterations
    {   
        bool meetsReqiurements = true;

        if(dialogueToCheck.DialogueRequirements.jetpack != Gamemanager.instance.UnlockedItems.jetpack) 
        {       
            meetsReqiurements = false;
        }
        if(dialogueToCheck.DialogueRequirements.pewpew != Gamemanager.instance.UnlockedItems.pewpew)
        {
            meetsReqiurements = false;
        }
        if(dialogueToCheck.DialogueRequirements.shovel != Gamemanager.instance.UnlockedItems.shovel)
        {
            meetsReqiurements = false;
        }
        
        return meetsReqiurements;
    }
}

