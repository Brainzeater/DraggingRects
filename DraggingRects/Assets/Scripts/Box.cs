using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : MonoBehaviour
{
    private const float DoubleClickInterval = 0.5f;
    private bool _doubleClickCounterStarted;

    private bool _selected;

    void Awake()
    {
        // Random color assignment
        GetComponent<SpriteRenderer>().color = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f));

        _doubleClickCounterStarted = false;
        Debug.Log($"Hello from {GetInstanceID()}");

        _selected = false;
    }

    void OnMouseDown()
    {
        if (!_doubleClickCounterStarted)
        {
            _selected = true;
            StartCoroutine(DoubleClickCounter());
        }
        else
        {
            // Double-click detected
            Destroy(this.gameObject);
        }
    }

    void OnMouseUp()
    {
        _selected = false;
    }

    void Update()
    {
        if (_selected)
        {
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = cursorPosition;
        }
    }

    // Double click interval timer
    private IEnumerator DoubleClickCounter()
    {
        _doubleClickCounterStarted = true;
        yield return new WaitForSeconds(DoubleClickInterval);
        _doubleClickCounterStarted = false;
    }

    void OnDestroy()
    {
        Debug.Log($"Goodbye from {GetInstanceID()}");
    }
}