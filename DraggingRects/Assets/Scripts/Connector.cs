using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Enables the connection of the boxes when the key is pressed (space by default).
 * When the two boxes are selected, the new connection is created between them.
 * Connections are translated with the boxes.
 * Repeat of the connection destroys the existing one.
 * Connections are destroyed with the boxes they belong to.
 */
[RequireComponent(typeof(Box))]
public class Connector : MonoBehaviour
{
    // Stores the number of boxes selected for the connection
    private static int selectedBoxCounter = 0;

    // Stores the first box selected for the connection
    private static GameObject connectionStart;

    // Key to activate connection mode
    public KeyCode key;

    // Line prefab
    public GameObject line;

    // All lines with other boxes
    public List<ConnectionAndRole> LineCollection { get; set; }


    void Awake()
    {
        LineCollection = new List<ConnectionAndRole>();
    }

    void Update()
    {
        if (Input.GetKeyUp(key))
        {
            selectedBoxCounter = 0;
        }
    }

    void OnMouseDown()
    {
        if (Input.GetKey(key))
        {
            if (selectedBoxCounter < 1)
            {
                connectionStart = this.gameObject;
                selectedBoxCounter++;
            }

            // The box cannot be connected with itself
            else if (this.gameObject.GetInstanceID() != connectionStart.gameObject.GetInstanceID())
            {
                CheckConnection();
                selectedBoxCounter = 0;
            }
        }

        // Clear the list from null lines (when they were destroyed by the other box)
        if (LineCollection.Any())
        {
            LineCollection.RemoveAll(item => item.line == null);
        }
    }

    void CheckConnection()
    {
        bool alreadyConnected = false;
        int connectionID = -1;
        foreach (ConnectionAndRole connection in LineCollection)
        {
            if (connection.otherSideId == connectionStart.gameObject.GetInstanceID())
            {
                alreadyConnected = true;
                connectionID = connectionStart.gameObject.GetInstanceID();
                break;
            }
        }

        if (!alreadyConnected)
        {
            CreateNewConnection();
        }
        else
        {
            // Destroy the connection and remove it from the list
            Destroy(LineCollection.Find(item => item.otherSideId == connectionID).line.gameObject);
            LineCollection.RemoveAll(item => item.otherSideId == connectionID);
        }
    }

    void CreateNewConnection()
    {
        GameObject connectionLine = Instantiate(line);
        LineRenderer lineRenderer = connectionLine.GetComponent<LineRenderer>();

        // Set start of the line and update the list of connections
        lineRenderer.SetPosition(0, connectionStart.transform.position);
        lineRenderer.startColor = connectionStart.GetComponent<SpriteRenderer>().color;
        connectionStart.GetComponent<Connector>().LineCollection
            .Add(new ConnectionAndRole(lineRenderer, Role.Start, this.gameObject.GetInstanceID()));

        // Set end of the line and update the list of connections
        lineRenderer.SetPosition(1, gameObject.transform.position);
        lineRenderer.endColor = GetComponent<SpriteRenderer>().color;
        LineCollection.Add(
            new ConnectionAndRole(lineRenderer, Role.End, connectionStart.gameObject.GetInstanceID()));
        
        // To disable destruction on dragging right after the connection
        GetComponent<Box>().ResetDoubleClickTimer();
    }

    void OnMouseDrag()
    {
        DragLines();
    }

    // Drags the lines depending on the role of the box
    void DragLines()
    {
        if (LineCollection.Any())
        {
            foreach (ConnectionAndRole connection in LineCollection)
            {
                switch (connection.role)
                {
                    case Role.Start:
                        connection.line.SetPosition(0, gameObject.transform.position);
                        break;
                    case Role.End:
                        connection.line.SetPosition(1, gameObject.transform.position);
                        break;
                }
            }
        }
    }

    void OnDestroy()
    {
        // Destroy the connections and clear the list from null lines
        if (LineCollection.Any())
        {
            LineCollection.RemoveAll(item => item.line == null);
            foreach (ConnectionAndRole connection in LineCollection)
            {
                Destroy(connection.line.gameObject);
            }
        }

        selectedBoxCounter = 0;
    }

    public enum Role
    {
        Start,
        End
    }

    /*
     * Stores the info about the connection.
     * line - to update one of the line's positions
     * role - to figure out which line's position to update
     * otherSideId - to check if the connection already exists
     */
    public struct ConnectionAndRole
    {
        public LineRenderer line;
        public Role role;
        public int otherSideId;

        public ConnectionAndRole(LineRenderer line, Role role, int otherSideId)
        {
            this.line = line;
            this.role = role;
            this.otherSideId = otherSideId;
        }
    }
}