using UnityEngine;

public class ExpOnDestroy : MonoBehaviour
{
    public int expAmount = 10;  
    private bool hasGivenExp = false;

    public void GiveExpAndDestroy()
    {
        GiveExp();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (hasGivenExp) return;

        if (!gameObject.scene.isLoaded)
            return;

        GiveExp();
    }

    private void GiveExp()
    {
        if (hasGivenExp) return;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddExperience(expAmount);
            Debug.Log($"[ExpOnDestroy] {gameObject.name} 파괴 → Player에게 EXP {expAmount} 지급");
        }

        hasGivenExp = true;
    }
}