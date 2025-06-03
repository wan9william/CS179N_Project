using UnityEngine;
using UnityEngine.UI; // This lets us use UI elements like Image

public class PlayerHealth : MonoBehaviour
{
    // This is the red fill part of the health bar
    public Image fillImage;

    // This keeps track of how much health the player has
    private UnitHealth unitHealth;

    public Dmg_flash flashEffect;

    // This runs once when the game starts
    void Start()
    {
        // Create a new health object with 100 max health and 100 current health
        unitHealth = new UnitHealth(100, 100);

        // Show the starting health on the health bar
        UpdateHealthBar();
    }

    // This runs every frame
    void Update()
    {
        // If the player presses the space key, take 10 damage
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
            flashEffect.Flash();
            
        }
        */

        // Optional: heal by pressing H
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(5);
        }
    }

    // This method reduces the player's health
    public void TakeDamage(int amount)
    {
        Debug.Log($"[PlayerHealth] Took {amount} damage.");
        unitHealth.Dmgunit(amount); // or: health -= amount;
        UpdateHealthBar();  

    if (flashEffect != null)
    {
        flashEffect.Flash();
    }
    }

    public void SetHealth(int health) {
        unitHealth.Health = health;
        UpdateHealthBar();
    }

    // This method increases the player's health (up to max)
    public void Heal(int amount)
    {
        unitHealth.Healunit(amount); // add health
        UpdateHealthBar(); // update the UI
    }

    // This method updates the fill amount of the red bar
    void UpdateHealthBar()
    {
        // Calculate how full the bar should be (between 0 and 1)
        float fillAmount = (float)unitHealth.Health / unitHealth.MaxHealth;

        // Apply it to the fill image
        fillImage.fillAmount = fillAmount;
    }
}
