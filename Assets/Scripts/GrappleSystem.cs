using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject grappleAnchorPoint;
    [SerializeField]
    private DistanceJoint2D grappleJoint;
    [SerializeField]
    private PolygonCollider2D grappleCollider;
    [SerializeField]
    private LineRenderer grappleRenderer;
    [SerializeField]
    private LayerMask grappleLayerMask;

    private PlayerMovement playerMovement;
    private bool grapple;
    private bool grappleAttached;
    private Rigidbody2D grappleAnchorPointRB;
    private SpriteRenderer grappleAnchorPointSprite;
    private Collider2D[] contactPoints;
    Vector2 grappleAnchorPosition;
    private float grappleMaxCastDistance = 20f;


    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        grapple = false;
        grappleJoint.enabled = false;
        grappleCollider.enabled = false;
        grappleAttached = false;
        grappleAnchorPointRB = grappleAnchorPoint.GetComponent<Rigidbody2D>();
        grappleAnchorPointSprite = grappleAnchorPoint.GetComponent<SpriteRenderer>();
        grappleAnchorPosition = grappleAnchorPoint.transform.position;
        contactPoints = new Collider2D[10];
    }

    void Update()
    {
        grappleAnchorPosition = grappleAnchorPoint.transform.position;
        if (grapple && !grappleAttached)
        {
            int numContacts = grappleCollider.GetContacts(contactPoints);
            int closestContact = -1;
            for (int i = 0; i < numContacts; i++) // find closest contact point to player in the grapple hitbox 
            {
                if (closestContact == -1 || (grappleAnchorPosition - (Vector2)contactPoints[closestContact].transform.position).magnitude >
                                            (grappleAnchorPosition - (Vector2)contactPoints[i].transform.position).magnitude)
                {
                    closestContact = i;
                }
            }

            // calculate aim direction of grapple
            Vector3 aimDirectionVector;
            if (closestContact != -1) // if the grapple found something to hook onto
            {
                aimDirectionVector = grappleAnchorPosition - (Vector2)contactPoints[closestContact].transform.position;
            }
            else
            {
                aimDirectionVector = grappleAnchorPosition - (Vector2)transform.position;
            }

            float aimAngle = Mathf.Atan2(aimDirectionVector.y, aimDirectionVector.x);
            if (aimAngle < 0f)
            {
                aimAngle = Mathf.PI * 2 + aimAngle;
            }
            Vector3 aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

            // Shoot the grapple
            grappleRenderer.enabled = true;
            // CONTINUE HERE
        }

        resetValues();
    }

    public void doGrapple()
    {
        grapple = true;
    }

    void resetValues()
    {
        grapple = false;
    }
}
