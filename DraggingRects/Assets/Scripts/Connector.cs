using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Connector : MonoBehaviour
{
    // Stores the number of boxes selected for the connection
    private static int boxCounter = 0;

    // Stores the first box selected for the connection
    private static GameObject connectionStart;

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
        if (Input.GetKeyUp(KeyCode.Space))
        {
            boxCounter = 0;
        }
    }

    void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (boxCounter < 1)
            {
                connectionStart = this.gameObject;
                boxCounter++;
            }

            else if (this.gameObject.GetInstanceID() != connectionStart.gameObject.GetInstanceID())
            {
                CreateNewConnection();
                boxCounter = 0;
            }
        }

        if (LineCollection.Any())
        {
            LineCollection.RemoveAll(item => item.line == null);
        }
    }

    void CreateNewConnection()
    {
        bool alreadyConnected = false;

        foreach (ConnectionAndRole connection in LineCollection)
        {
            if (connection.otherSideId == connectionStart.gameObject.GetInstanceID())
            {
                alreadyConnected = true;
                break;
            }
        }

        if (!alreadyConnected)
        {
            GameObject connectionLine = Instantiate(line);

            LineRenderer renderer = connectionLine.GetComponent<LineRenderer>();

            // Set start
            renderer.SetPosition(0, connectionStart.transform.position);
            renderer.startColor = connectionStart.GetComponent<SpriteRenderer>().color;
            connectionStart.GetComponent<Connector>().LineCollection
                .Add(new ConnectionAndRole(renderer, Role.Start, this.gameObject.GetInstanceID()));

            // Set end
            renderer.SetPosition(1, gameObject.transform.position);
            renderer.endColor = GetComponent<SpriteRenderer>().color;
            LineCollection.Add(
                new ConnectionAndRole(renderer, Role.End, connectionStart.gameObject.GetInstanceID()));
        }
    }

    void OnMouseDrag()
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
        if (LineCollection.Any())
        {
            LineCollection.RemoveAll(item => item.line == null);
            foreach (ConnectionAndRole connection in LineCollection)
            {
                Destroy(connection.line.gameObject);
            }
        }

        boxCounter = 0;
    }

    public enum Role
    {
        Start,
        End
    }

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