﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public static GameManager Instance => _instance;

    public GameObject box;
    public BoxSize size;
    public LayerMask layer;
    
    private GameObject _camera;
    private Vector2 _screenBounds;

    void Awake()
    {
        // Singleton pattern
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != null)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Set screen bounds
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _screenBounds = _camera.GetComponent<Camera>()
            .ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _camera.transform.position.z));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckSpace();
        }
    }

    /*
     * Spawns a rect if there's enough space for it
     */
    void CheckSpace()
    {
        Vector2 pointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!Physics2D.Raycast(pointerPosition, Vector2.zero))
        {
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
        }
    }

    void SpawnBox(Vector2 position)
    {
        GameObject boxGameObject = Instantiate(box, position, Quaternion.identity);

        // Set size of created box
        boxGameObject.transform.localScale = new Vector3(size.width, size.height, 1f);
    }
}