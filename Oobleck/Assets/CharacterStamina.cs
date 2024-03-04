using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStamina : MonoBehaviour
{
    public float stamina;

    public float loseStaminaRate;
    public float loseStaminaIdleRate;
    public float gainStaminaRate;
    private float maxStamina = 100f;

    public delegate void StaminaEvent(float currentStaminaNormalized);
    public static StaminaEvent staminaReset;
    public static StaminaEvent losingStamina;
    public static StaminaEvent gainingStamina;

    public ManagerCharacterState characterState;
    public delegate void HealthEvent();
    public static HealthEvent allStaminaLost;
    public static HealthEvent playerDrowned;

    private bool dead;

    private void Start()
    {
        stamina = maxStamina;
        allStaminaLost += CheckPlayer;
    }
    public void LoseStamina()
    {
        stamina -= Time.deltaTime * loseStaminaRate;
        ClampStamina();
        losingStamina?.Invoke(GetStaminaNormalized());

        if(stamina <= 0)
        {
            allStaminaLost?.Invoke();
        }
    }

    public void LoseStaminaIdle()
    {
        stamina -= Time.deltaTime * loseStaminaIdleRate;
        ClampStamina();
        losingStamina?.Invoke(GetStaminaNormalized());

        if (stamina <= 0)
        {
            allStaminaLost?.Invoke();
        }
    }
    public void LoseStamina(float loss)
    {
        stamina -= loss;
        ClampStamina();
        losingStamina?.Invoke(GetStaminaNormalized());

        if (stamina <= 0 )
        {
            allStaminaLost?.Invoke();
        }
    }
    public void GainStaminaBack()
    {
        stamina += Time.deltaTime * gainStaminaRate;
        ClampStamina();
        gainingStamina?.Invoke(GetStaminaNormalized());
    }

    public void ResetStamina()
    {
        stamina = maxStamina;
        dead = false;
        staminaReset?.Invoke(1f);
    }

    private void ClampStamina() => stamina = Mathf.Clamp(stamina, 0f, maxStamina);

    public float GetStaminaNormalized() => Mathf.InverseLerp(0f, maxStamina, stamina);

    public void CheckPlayer()
    {
        if (dead)
            return;

        if(characterState.HSM.currentState is Oobleck_Movement)
        {
            dead = true;
            playerDrowned?.Invoke();
        }
    }
}
