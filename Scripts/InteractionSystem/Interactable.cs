using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected PlayerWeaponController weaponController;
    [SerializeField] protected MeshRenderer m_Renderer;

    [SerializeField] private Material highlightedMaterial;
    [SerializeField] protected Material defaultMaterial;



    private void Start()
    {
        if (m_Renderer == null)
            m_Renderer = GetComponentInChildren<MeshRenderer>();

        defaultMaterial = m_Renderer.sharedMaterial;
    }

    protected void UpdateMeshMaterial(MeshRenderer newMesh)
    {
        m_Renderer = newMesh;
        defaultMaterial = newMesh.sharedMaterial;
    }

    public virtual void Interaction()
    {
        Debug.Log("i interacted with a "+ gameObject.name);
    }

    public void HighlightActive(bool active)
    {
        if (active)
            m_Renderer.material = highlightedMaterial;
        else
            m_Renderer.material = defaultMaterial;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (weaponController == null)
            weaponController = other.GetComponent<PlayerWeaponController>();


        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
            return;

        playerInteraction.GetInteractables().Add(this);
        playerInteraction.UpdateClosestInteractable();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
            return;

        playerInteraction.GetInteractables().Remove(this);
        playerInteraction.UpdateClosestInteractable();
    }
}
