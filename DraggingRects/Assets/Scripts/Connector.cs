using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    private static int boxCounter = 0;

    public GameObject line;

    private static GameObject connectionStart;
    private Role _role;
    
    // TODO: Update to list
    public LineRenderer LineRenderer { get; set; }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            boxCounter = 0;
            Debug.Log($"UFF {boxCounter}");
        }
    }

    void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (boxCounter < 1)
            {
                _role = Role.Start;
                connectionStart = this.gameObject;
                boxCounter++;
                Debug.Log($"WOW {GetInstanceID()} {boxCounter}");
            }
//            else if (_role != Role.Start)
            else 
            {
                _role = Role.End;
                GameObject connection = Instantiate(line);
                LineRenderer = connection.GetComponent<LineRenderer>();
                connectionStart.GetComponent<Connector>().LineRenderer = connection.GetComponent<LineRenderer>();
                // Set start
                LineRenderer.SetPosition(0, connectionStart.transform.position);
                LineRenderer.startColor = connectionStart.GetComponent<SpriteRenderer>().color;
                // Set end
                LineRenderer.SetPosition(1, gameObject.transform.position);
                LineRenderer.endColor = GetComponent<SpriteRenderer>().color;
                boxCounter = 0;
            }
        }
    }

    void OnMouseDrag()
    {
        if (LineRenderer != null)
        {
            switch (_role)
            {
                case Role.Start:
                    LineRenderer.SetPosition(0, gameObject.transform.position);
                    break;
                case Role.End:
                    LineRenderer.SetPosition(1, gameObject.transform.position);
                    break;
            }
        }
    }

    void OnDestroy()
    {
        if(LineRenderer != null)
        {
            Destroy(LineRenderer.gameObject);
        }

    }

    public enum Role
    {
        Start,
        End
    }

//    private struct ConnectionAndRole
//    {
//        private LineRenderer line;
//        private Role role;
//    }
}