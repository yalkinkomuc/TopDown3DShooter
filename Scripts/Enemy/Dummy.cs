using UnityEngine;

public class Dummy : MonoBehaviour,IDamageble
{
    public int currentHealth;
    public int maxHealth;

    public MeshRenderer mesh;
    public Material whiteMat;
    public Material redMat;
    [Space]
    public float refreshCooldown;
    public float lastTimeDamaged;
    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (Time.time > refreshCooldown + lastTimeDamaged || Input.GetKeyDown(KeyCode.B))
            Refresh();
    }
    public void Refresh()
    {
        currentHealth = maxHealth;
        mesh.sharedMaterial = whiteMat;
    }


   
    public void TakeDamage(int damage)
    {
        lastTimeDamaged = Time.time;
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        mesh.sharedMaterial = redMat;
    }

}
