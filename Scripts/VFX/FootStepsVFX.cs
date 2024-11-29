using UnityEngine;

public class FootStepsVFX : MonoBehaviour
{
    [SerializeField] LayerMask whatIsGround;

    [SerializeField] TrailRenderer leftFoot;
    [SerializeField] TrailRenderer rightFoot;

    [Range(0.001f, .3f)]
    [SerializeField] private float checkRadius = 0.05f;
    [Range(-.15f, .15f)]
    [SerializeField] private float rayDistance = -0.05f;

    private void Update()
    {
        CheckFootsteps(leftFoot);
        CheckFootsteps(rightFoot);
    }

    private void CheckFootsteps(TrailRenderer foot)
    {
        Vector3 checkPosition = foot.transform.position + Vector3.down * rayDistance;

        bool touchingGround = Physics.CheckSphere(checkPosition,checkRadius,whatIsGround);

        foot.emitting = touchingGround;
    }

    private void OnDrawGizmos()
    {
        DrawFootGizmos(leftFoot.transform);
        DrawFootGizmos(rightFoot.transform);
    }

    private void DrawFootGizmos(Transform foot)
    {
        if(foot == null) 
            return;

        Gizmos.color = Color.blue;
        Vector3 checkPosition = foot.transform.position + Vector3.down * rayDistance;

        Gizmos.DrawWireSphere(checkPosition,checkRadius);
    }

}
