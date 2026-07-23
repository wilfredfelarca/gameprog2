using UnityEngine;

public class PickupController : MonoBehaviour
{
    [Header("Settings")]
    public float pickupRange = 1.5f;          // How close you need to be
    public Transform carryPosition;           // Where held object sits

    [Header("State")]
    private GameObject carriedObject = null;  // Currently held item
    

    void Update()
    {
        // Press Space to pick up or drop
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (carriedObject == null)
                TryPickup();
            else
                DropObject();
        }
    }

    // pickup
    void TryPickup()
    {
        // Find all colliders within range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRange);

        // Pick the closest object with the Pickup tag
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Pickup"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = hit.gameObject;
                }
            }
        }

        if (closest != null)
        {
            carriedObject = closest;

            // Disable physics while carrying
            Rigidbody2D rb = carriedObject.GetComponent<Rigidbody2D>();
            if (rb != null) rb.isKinematic = true;

            Collider2D col = carriedObject.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // Parent it to the carry position
            carriedObject.transform.SetParent(carryPosition);
            carriedObject.transform.localPosition = Vector3.zero;

            Debug.Log("Picked up: " + carriedObject.name);
        }
        else
        {
            Debug.Log("No pickupable object nearby.");
        }
    }

    // drop
    void DropObject()
    {
        if (carriedObject == null) return;

        // Unparent it
        carriedObject.transform.SetParent(null);

        // Re-enable physics
        Rigidbody2D rb = carriedObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Collider2D col = carriedObject.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        carriedObject = null;
    }

    // Draw pickup range in the Scene view for easy tweaking
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}