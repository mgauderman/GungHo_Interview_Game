﻿using System.Collections;
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
    float retractSpeedMultiplier;

    private DistanceJoint2D grappleJoint;
    private Rigidbody2D playerRB;
    private PlayerMovement playerMovement;
    private bool grapple;
    private bool stopGrapple;
    private bool grappleAttached;
    private Rigidbody2D grappleAnchorPointRB;
    private Collider2D[] overlapColliders;
    private Vector2 grappleShootingPointPosition;
    private float grappleMaxCastDistance;
    private GameObject grappledObject;

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
        grappleMaxCastDistance = 9f;
    }

    void Update()
    {
        grappleShootingPointPosition = grappleShootingPoint.transform.position;
        if (grapple && !grappleAttached) // if player left clicked to shoot grapple
        {
            ShootGrapple();
        }

        if(stopGrapple) // if player right clicks
        {
            ResetGrapple();
        }

        if(grappleAttached) // if player is currently tethered
        {
            UpdateGrapplePositions(); 
        }
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
            aimDirectionVector = (Vector2)overlapColliders[closestContact].transform.position - grappleShootingPointPosition;
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
        aimDirectionVector.Normalize();
        float aimAngle = Mathf.Atan2(aimDirectionVector.y, aimDirectionVector.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        // Shoot the grapple
        grappleRenderer.enabled = true;
        var hit = Physics2D.Raycast(grappleShootingPointPosition, aimDirectionVector, grappleMaxCastDistance, grappleLayerMask);

        if (hit.collider != null && closestContact != -1) // TODO: look at this section more
        {
            grappleAttached = true;
            grappledObject = overlapColliders[closestContact].gameObject;
            if (grappledObject.Equals(hit.collider.gameObject)) // if nothing blocks way to grapple point
            {
                grappleJoint.distance = Vector2.Distance(grappleShootingPointPosition, grappledObject.transform.position);
                grappleJoint.enabled = true;
                if (!playerMovement.IsFacingLeft()) // make sure the joint starts on the fist in direction player is facing
                {
                    grappleJoint.anchor = new Vector2(Mathf.Abs(grappleJoint.anchor.x), grappleJoint.anchor.y);
                }
                else
                {
                    grappleJoint.anchor = new Vector2(-Mathf.Abs(grappleJoint.anchor.x), grappleJoint.anchor.y);
                }
            }
        }
        else
        {
            grappleRenderer.enabled = false;
            grappleAttached = false;
            grappleJoint.enabled = false;
        }
    }

    private void UpdateGrapplePositions()
    {
        if (grappledObject != null)
        {
            grappleRenderer.SetPosition(0, grappleShootingPointPosition);
            grappleRenderer.SetPosition(1, grappledObject.transform.position);
            grappleAnchorPoint.transform.position = grappledObject.transform.position;
        }
    }

    public void RetractGrapple() // this should make player get shot out in direction of grapple point (when they left click while tethered)
    {
        grappleJoint.enabled = false;
        StartCoroutine("PropelPlayer");
    }

    IEnumerator PropelPlayer()
    {
        Vector2 startPosition = grappleShootingPointPosition;
        Vector2 grappleAnchorPointPosition = grappleAnchorPoint.transform.position;
        float distanceToAnchorFromStart = Vector2.Distance(grappleShootingPointPosition, grappleAnchorPointPosition);
        Vector2 directionOfTravel = (grappleAnchorPointPosition - startPosition).normalized;
        //playerRB.velocity = new Vector2(0, 0);
       while (distanceToAnchorFromStart > Vector2.Distance(transform.position, startPosition) + 2) {
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
        grappledObject = null;
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
