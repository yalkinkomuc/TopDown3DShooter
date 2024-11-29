using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>();
    private Interactable closestInteractable;

    private void Start()
    {
        Player player = GetComponent<Player>();

        player.controls.Character.Interaction.performed += context => InteractWithClosest();
    }

    public void UpdateClosestInteractable()
    {
        closestInteractable?.HighlightActive(false);

        closestInteractable = null; //resetlemek için
        float closestDistance = float.MaxValue;

        foreach (Interactable interactible in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactible.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactible;
            }
        }
        closestInteractable?.HighlightActive(true);

    }

    public void InteractWithClosest()
    {
        closestInteractable.Interaction();
        interactables.Remove(closestInteractable);

        UpdateClosestInteractable();
    }

    public List<Interactable> GetInteractables() => interactables;

}
