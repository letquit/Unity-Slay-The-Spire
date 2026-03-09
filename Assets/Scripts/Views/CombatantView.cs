using TMPro;
using UnityEngine;

public class CombatantView : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    public int MAXHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    protected void SetupBase(int health, Sprite image)
    {
        MAXHealth = CurrentHealth = health;
        spriteRenderer.sprite = image;
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        healthText.text = "HP: " + CurrentHealth;
    }
}
