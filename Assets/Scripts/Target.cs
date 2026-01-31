using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;

    // This is the function the Raycast will trigger
    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // You can add death animations or particle effects here
        Debug.Log(gameObject.name + " was destroyed!");
        Destroy(gameObject);
    }
}