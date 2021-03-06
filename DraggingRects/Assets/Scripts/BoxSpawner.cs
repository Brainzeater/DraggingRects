﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Spawns boxes on click within the screen.
 * Forbids spawning over other boxes and outside of the screen.
 */
[RequireComponent(typeof(BoxCollider2D))]
public class BoxSpawner : MonoBehaviour
{
    public static BoxSpawner Instance;

    [Header("Box settings")]
    public GameObject box;
    public BoxSize size;
    public LayerMask layer;

    // To keep the scene tidy:
    // boxParent stores prefab instances in a single game object
    public Transform boxParent;

    [Header("Spawner settings")]
    public float boundaryRadius;

    // Restrict boxes movement within the screen
    public GameObject topBoundary;
    public GameObject bottomBoundary;
    public GameObject leftBoundary;
    public GameObject rightBoundary;

    private Vector2 _screenBounds;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Set screen bounds
        _screenBounds = Camera.main.GetComponent<Camera>()
            .ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        // Set spawner's scale to screen bounds
        gameObject.transform.localScale = _screenBounds * 2;

        // Arrange boundary colliders around the screen
        SetupBoundaries();
    }

    void OnMouseDown()
    {
        CheckSpace();
    }
    
    void SetupBoundaries()
    {
        // Vertical Boundaries
        float yPosition = _screenBounds.y + boundaryRadius;
        float yRatio = boundaryRadius / _screenBounds.y;
        topBoundary.transform.position = new Vector2(0f, yPosition);
        topBoundary.transform.localScale = new Vector2(topBoundary.transform.localScale.x, yRatio);
        bottomBoundary.transform.position = new Vector2(0f, -yPosition);
        bottomBoundary.transform.localScale = new Vector2(bottomBoundary.transform.localScale.x, yRatio);

        // Horizontal Boundaries
        float xPosition = _screenBounds.x + boundaryRadius;
        float xRatio = boundaryRadius / _screenBounds.x;
        leftBoundary.transform.position = new Vector2(-xPosition, 0f);
        leftBoundary.transform.localScale = new Vector2(xRatio, leftBoundary.transform.localScale.y);
        rightBoundary.transform.position = new Vector2(xPosition, 0f);
        rightBoundary.transform.localScale = new Vector2(xRatio, rightBoundary.transform.localScale.y);
    }


    // Spawns a rect if there's enough space for it
    void CheckSpace()
    {
        Vector2 pointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Rectangular area coordinates of box around clicked point
        Vector2 pointA = new Vector2(pointerPosition.x - size.width / 2, pointerPosition.y - size.height / 2);
        Vector2 pointB = new Vector2(pointerPosition.x + size.width / 2, pointerPosition.y + size.height / 2);

        // Box can be spawned when area around click is not overlapping with other boxes
        // and is inside of the screen bounds
        if (!Physics2D.OverlapArea(pointA, pointB, layer) &&
            (pointA.x >= -_screenBounds.x && pointB.x <= _screenBounds.x &&
             pointA.y >= -_screenBounds.y && pointB.y <= _screenBounds.y))
        {
            SpawnBox(pointerPosition);
        }
        else
        {
            // Highlight overlapping boxes
            Collider2D[] otherBoxesColliders = Physics2D.OverlapAreaAll(pointA, pointB, layer);
            foreach (Collider2D boxCollider in otherBoxesColliders)
            {
                boxCollider.GetComponent<Animator>().SetTrigger("Red");
            }
        }
    }

    void SpawnBox(Vector2 position)
    {
        GameObject boxGameObject = Instantiate(box, position, Quaternion.identity, boxParent);

        // Set size of created box
        boxGameObject.transform.localScale = new Vector2(size.width, size.height);
    }
}