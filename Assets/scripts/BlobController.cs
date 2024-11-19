using System.Collections.Generic;
using UnityEngine;

public class BlobController : MonoBehaviour
{
    private Rigidbody2D rb;
    public BoxCollider2D boundaryCollider;
    public BoxCollider2D mommyBlobCollider; 
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
            return;
        }

        if (boundaryCollider == null)
        {
            Debug.LogError("Boundary Collider is not assigned.");
        }

        if (mommyBlobCollider == null)
        {
            Debug.LogError("Mommy Blob Collider is not assigned.");
        }
    }

    void FixedUpdate()
    {
        if (boundaryCollider != null)
        {
            CheckBoundary();
        }

        if (mommyBlobCollider != null)
        {
            AvoidMommyBlob();
        }
    }

    void CheckBoundary()
    {
        Bounds boundaryBounds = boundaryCollider.bounds;
        Vector2 position = transform.position;

        if (position.x < boundaryBounds.min.x || position.x > boundaryBounds.max.x ||
            position.y < boundaryBounds.min.y || position.y > boundaryBounds.max.y)
        {
            Vector2 normal = Vector2.zero;
            if (position.x < boundaryBounds.min.x) normal = Vector2.right;
            if (position.x > boundaryBounds.max.x) normal = Vector2.left;
            if (position.y < boundaryBounds.min.y) normal = Vector2.up;
            if (position.y > boundaryBounds.max.y) normal = Vector2.down;

            rb.velocity = Vector2.Reflect(rb.velocity, normal);
            transform.position = new Vector2(Mathf.Clamp(position.x, boundaryBounds.min.x, boundaryBounds.max.x),
                                             Mathf.Clamp(position.y, boundaryBounds.min.y, boundaryBounds.max.y));
        }
    }

    void AvoidMommyBlob()
    {
        Bounds mommyBounds = mommyBlobCollider.bounds;
        Vector2 position = transform.position;

        if (mommyBounds.Contains(position))
        {
            Vector2 directionAway = (position - (Vector2)mommyBounds.center).normalized;
            rb.velocity = directionAway * rb.velocity.magnitude;
        }
    }
}
