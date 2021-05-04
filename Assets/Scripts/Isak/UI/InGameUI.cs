using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{

    [SerializeField] Text health;
    [SerializeField] Text npcText;
    [SerializeField] Text npcName;
    [SerializeField] Image npcTextBackground;
    [SerializeField] Image npcImage;
    [SerializeField] Slider fuelBar;
    private void Awake()
    {
        ClearScreen();
    }

    private void ClearScreen()
    {
        npcImage.gameObject.SetActive(false);
        npcTextBackground.gameObject.SetActive(false);
        npcName.text = string.Empty;
        npcText.text = string.Empty;
        health.text = string.Empty;
    }

    private void Update() { // change to event later
        if(Gamemanager.instance.UnlockedItems.jetpack)
        {
            fuelBar.gameObject.SetActive(true);
        }
        else
        {
            fuelBar.gameObject.SetActive(false);
        }
    }

    void PrintNPCText(string dialogue, string name, Sprite npcSprite)
    {
        npcTextBackground.gameObject.SetActive(true);
        npcImage.gameObject.SetActive(true);
        npcImage.sprite = npcSprite;
        npcText.text = dialogue;
        npcName.text = name;
    }

    void UpdateHealthText(int amount)
    {
        health.text = amount.ToString();
    }

    void ClearConversation()
    {
        npcText.text = string.Empty;
        npcName.text = string.Empty;
        npcTextBackground.gameObject.SetActive(false);
        npcImage.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void UpdateFuelText(float amount)
    {
        fuelBar.value = amount;
    }

    void SetupFuelSlider(float maxValue)
    {
        fuelBar.maxValue = maxValue;
        fuelBar.value = maxValue;
    }
    private void OnEnable() {
        UIManager.onNPCDialogue += PrintNPCText;
        UIManager.onPlayerHealthChange += UpdateHealthText;
        UIManager.onFuelUse += UpdateFuelText;
        UIManager.onJetpackAwake += SetupFuelSlider;
        UIManager.onPlayerLeavingConversation += ClearConversation;
    }

    private void OnDisable() {
        UIManager.onNPCDialogue -= PrintNPCText;
        UIManager.onPlayerHealthChange -= UpdateHealthText;
        UIManager.onFuelUse -= UpdateFuelText;
        UIManager.onJetpackAwake -= SetupFuelSlider;
        UIManager.onPlayerLeavingConversation -= ClearConversation;
    }
}
