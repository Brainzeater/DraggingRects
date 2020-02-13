using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Box : MonoBehaviour
{
    private const float DoubleClickInterval = 0.5f;
    private bool _doubleClickCounterStarted;

    private const float DragSpeed = 30f;
    private const float DragMaxSpeed = 80f;

    // Mouse cursor position relative to the box center at the start of dragging
    // This variable enables dragging the box by any point of the box
    private Vector3 _cursorCenterOffsetPosition;

    private Rigidbody2D _rigidbody;

    void Start()
    {
        // Random color assignment
        GetComponent<SpriteRenderer>().color = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f));

        _doubleClickCounterStarted = false;
        Debug.Log($"Hello from {GetInstanceID()}");

        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnMouseDown()
    {
        if (!_doubleClickCounterStarted)
        {
            // Save relative position of mouse click point to the box center
            _cursorCenterOffsetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            StartCoroutine(DoubleClickCounter());
        }
        else
        {
            // Double-click detected
            Destroy(this.gameObject);
        }
    }


    void OnMouseDrag()
    {
        // Enable box to be movable by velocity
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        
        // Keep the cursor offset
        Vector3 boxWithCursorOffsetPosition = transform.position + _cursorCenterOffsetPosition;

        // The following code is inspired by:
        // http://answers.unity.com/answers/149435/view.html

        // Cast a ray from the mouse cursor
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Get distance from the mouse pointer
        Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(cursorPosition, Vector2.zero);
        float length = hit.distance;

        // Calculate velocity necessary to follow the mouse cursor
        Vector2 velocity = (ray.GetPoint(length) - boxWithCursorOffsetPosition) * DragSpeed;

        // Limit max velocity to avoid pass through other boxes
        if (velocity.magnitude > DragMaxSpeed) velocity *= DragMaxSpeed / velocity.magnitude;

        // Set box velocity
        _rigidbody.velocity = velocity;
    }

    void OnMouseUp()
    {
        // Reset rigid body to  
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.bodyType = RigidbodyType2D.Static;
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