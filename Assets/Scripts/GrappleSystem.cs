using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject grappleAnchorPoint;
    [SerializeField]
    private GameObject grappleShootingPoint;
    [SerializeField]
    private PolygonCollider2D grappleCollider;
    [SerializeField]
    private LineRenderer grappleRenderer;
    [SerializeField]
    private LayerMask grappleLayerMask;
    [SerializeField]
    ContactFilter2D grappleContactFilter;
    [SerializeField]
    private float retractSpeedMultiplier;
    [SerializeField]
    private Material grappleRendererMaterial;
    [SerializeField]
    private float grappleMaxCastDistance;
    [SerializeField]
    BarrierSystem barrierSystem;

    private DistanceJoint2D grappleJoint;
    private Rigidbody2D playerRB;
    private PlayerMovement playerMovement;
    private bool grapple;
    private bool stopGrapple;
    private bool grappleAttached;
    private Rigidbody2D grappleAnchorPointRB;
    private Collider2D[] overlapColliders;
    private Vector2 grappleShootingPointPosition;
    private GameObject grappledObject;
    private bool animatingGrapple;
    private bool shootingGrappleStraight; // true while player is shooting grapple with no target

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerRB = GetComponent<Rigidbody2D>();
        grappleJoint = GetComponent<DistanceJoint2D>();
        grapple = false;
        grappleJoint.enabled = false;
        grappleCollider.enabled = false;
        grappleAttached = false;
        grappleAnchorPointRB = grappleAnchorPoint.GetComponent<Rigidbody2D>();
        grappleShootingPointPosition = grappleShootingPoint.transform.position;
        overlapColliders = new Collider2D[10];
        grappleRenderer.positionCount = 2;
        grappledObject = null;
        animatingGrapple = false;
        shootingGrappleStraight = false;
    }

    void Update()
    {
        grappleShootingPointPosition = grappleShootingPoint.transform.position;
        if (grapple && !grappleAttached) // if player left clicked to shoot grapple
        {
            ShootGrapple();
        }

        if (stopGrapple) // if player right clicks
        {
            ResetGrapple();
        }

        UpdateGrapplePositions();
        ResetValues();
    }

    private void ShootGrapple()
    {
        // Find closest grapple point
        grappleCollider.enabled = true;
        int numColliders = grappleCollider.OverlapCollider(grappleContactFilter, overlapColliders);
        grappleCollider.enabled = false;
        int closestContact = -1;
        for (int i = 0; i < numColliders; i++) // find closest contact point to player in the grapple hitbox 
        {
            if (closestContact == -1 || Vector2.Distance(grappleShootingPointPosition, overlapColliders[closestContact].transform.position) >
                                        Vector2.Distance(grappleShootingPointPosition, overlapColliders[i].transform.position))
            {
                closestContact = i;
            }
        }

        // Calculate aim direction of grapple
        Vector3 aimDirectionVector;
        if (closestContact != -1) // if the grapple found something to hook onto
        {
            aimDirectionVector = ((Vector2)overlapColliders[closestContact].transform.position - grappleShootingPointPosition).normalized;
        }
        else
        {
            if (playerMovement.IsFacingLeft())
            {
                aimDirectionVector = Vector2.left;
            }
            else
            {
                aimDirectionVector = Vector2.right;
            }
        }
        float aimAngle = Mathf.Atan2(aimDirectionVector.y, aimDirectionVector.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        // Check line of sight to closestTarget
        var hit = Physics2D.Raycast(grappleShootingPointPosition, aimDirectionVector, grappleMaxCastDistance, grappleLayerMask);

        // If the hit object is same as closestContact, nothing is in the way so grapple onto it
        if (hit.collider != null && closestContact != -1 && overlapColliders[closestContact].gameObject.Equals(hit.collider.gameObject))
        {
            grappleToObject(overlapColliders[closestContact].gameObject, false);
        }
        else // shoot the grapple straight forward
        {
            grappleAnchorPoint.transform.position = GetIdealForwardAnchorPosition();
            grappleRenderer.enabled = true;
            shootingGrappleStraight = true;
            StartCoroutine("AnimateGrappleForward");
        }
    }

    private void grappleToObject(GameObject objectToGrapple, bool fromStraightGrapple)
    {
        barrierSystem.ChangeColor(objectToGrapple.tag);
        grappledObject = objectToGrapple;
        grappleAttached = true;
        grappleJoint.distance = Vector2.Distance(grappleShootingPointPosition, grappledObject.transform.position);
        grappleJoint.enabled = true;
        grappleRenderer.SetPosition(0, grappleShootingPointPosition);
        grappleRenderer.enabled = true;
        if (!fromStraightGrapple)
        {
            StartCoroutine("AnimateGrappleToAnchor");
        }

        // Make sure the joint starts on correct side of player based on player's rotation
        if (!playerMovement.IsFacingLeft())
        {
            grappleJoint.anchor = new Vector2(Mathf.Abs(grappleJoint.anchor.x), grappleJoint.anchor.y);
        }
        else
        {
            grappleJoint.anchor = new Vector2(-Mathf.Abs(grappleJoint.anchor.x), grappleJoint.anchor.y);
        }
    }

    IEnumerator AnimateGrappleToAnchor() // makes grapple appear as if it is extending from player's hand to a target
    {
        animatingGrapple = true;
        float lerpAmount = 0.0f;
        float grappleAnimationSpeed = 4.0f;
        while (lerpAmount < 1)
        {
            grappleRenderer.SetPosition(1, Vector2.Lerp(grappleShootingPointPosition, grappleAnchorPoint.transform.position, lerpAmount));
            yield return null;
            lerpAmount += grappleAnimationSpeed * Time.deltaTime;
        }
        grappleRenderer.SetPosition(1, grappleAnchorPoint.transform.position);
        animatingGrapple = false;
    }

    private void UpdateGrapplePositions()
    {
        if ((grappleAttached && grappledObject != null))
        {
            grappleAnchorPoint.transform.position = grappledObject.transform.position;
            grappleRenderer.SetPosition(0, grappleShootingPointPosition);
            if (!animatingGrapple) // after done animating grapple
            {
                grappleRenderer.SetPosition(1, grappleAnchorPoint.transform.position);
            }

            // update tiling for line renderer
            grappleRendererMaterial.mainTextureScale = new Vector2(Vector2.Distance(grappleAnchorPoint.transform.position, grappleShootingPointPosition), 1);
        }
        else if (shootingGrappleStraight)
        {
            grappleAnchorPoint.transform.position = GetIdealForwardAnchorPosition();
            grappleRenderer.SetPosition(0, grappleShootingPointPosition);
            if(!animatingGrapple) // reset grapple after done animating (if it hasn't found a target)
            {
                grappleRenderer.SetPosition(1, grappleAnchorPoint.transform.position);
            }

            // update tiling for line renderer
            grappleRendererMaterial.mainTextureScale = new Vector2(Vector2.Distance(grappleAnchorPoint.transform.position, grappleShootingPointPosition), 1);
        }
    }

    IEnumerator AnimateGrappleForward() // animates grapple straight ahead and raycasts to see if it hits a potential target
    {
        animatingGrapple = true;
        float lerpAmount = 0.0f;
        float grappleAnimationSpeed = 4.0f;
        float numSecondsExtended = 0.5f;
        float numSecondsExtendedSoFar = 0.0f;
        while (numSecondsExtendedSoFar < numSecondsExtended) // only end once grapple is done extending
        {
            Vector2 newPosition = Vector2.Lerp(grappleShootingPointPosition, grappleAnchorPoint.transform.position, lerpAmount);
            float maxDistance = Vector2.Distance(grappleShootingPointPosition, newPosition);
            Vector2 aimDirection = ((Vector2)grappleAnchorPoint.transform.position - grappleShootingPointPosition).normalized;
            var hit = Physics2D.Raycast(grappleShootingPointPosition, aimDirection, maxDistance, grappleLayerMask);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grapple")) // if it hit a grappling point, grapple onto it
                {
                    shootingGrappleStraight = false;
                    animatingGrapple = false;
                    grappleToObject(hit.collider.gameObject, true);
                    yield break;
                }
                else // kill the grapple because it hit a non-grapplable object
                {
                    ResetGrapple();
                    yield break;
                }
            }
            grappleRenderer.SetPosition(1, newPosition);

            yield return null;
            if (animatingGrapple)
            {
                lerpAmount += grappleAnimationSpeed * Time.deltaTime;
                if (lerpAmount >= 1.0f)
                {
                    lerpAmount = 1.0f;
                    animatingGrapple = false;
                }
            }
            else
            {
                numSecondsExtendedSoFar += Time.deltaTime;
            }
        }
        // Reset once grapple finishes being out after not colliding with anything
        ResetGrapple();
    }

    private Vector2 GetIdealForwardAnchorPosition() // gets the position in front of player to place the anchor when there's no target
    {
        if(playerMovement.IsFacingLeft())
        {
            return grappleShootingPointPosition - new Vector2(grappleMaxCastDistance, 0);
        }
        else
        {
            return grappleShootingPointPosition + new Vector2(grappleMaxCastDistance, 0);
        }
    }

    public void RetractGrapple() // this should make player get shot out in direction of grapple point (when they left click while tethered)
    {
        grappleJoint.enabled = false;
        StartCoroutine("PropelPlayerToAnchor");
    }

    IEnumerator PropelPlayerToAnchor()
    {
        Vector2 startPosition = grappleShootingPointPosition;
        Vector2 grappleAnchorPointPosition = grappleAnchorPoint.transform.position;
        float distanceToAnchorFromStart = Vector2.Distance(startPosition, grappleAnchorPointPosition);
        Vector2 directionOfTravel = (grappleAnchorPointPosition - startPosition).normalized;
        while (distanceToAnchorFromStart > Vector2.Distance(grappleShootingPointPosition, startPosition) && grappleAttached)
        {
            playerRB.velocity = new Vector2(retractSpeedMultiplier * directionOfTravel.x, retractSpeedMultiplier * directionOfTravel.y);
            yield return null;
        }
        ResetGrapple();
    }

    private void ResetValues()
    {
        grapple = false;
        stopGrapple = false;
    }

    private void ResetGrapple() // retract grapple if currently out
    {
        grappleJoint.enabled = false;
        grappleAttached = false;
        grappleRenderer.SetPosition(0, grappleShootingPointPosition);
        grappleRenderer.SetPosition(1, grappleShootingPointPosition);
        grappleRenderer.enabled = false;
        grappledObject = null;
        shootingGrappleStraight = false;
        animatingGrapple = false;
        animatingGrapple = false;
        playerMovement.OnEndGrapple();
    }

    public void DoGrapple()
    {
        grapple = true;
    }

    public void StopGrapple()
    {
        stopGrapple = true;
    }

    public bool IsSwinging()
    {
        return grappleAttached;
    }
}
