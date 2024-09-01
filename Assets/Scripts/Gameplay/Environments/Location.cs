using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{

    // type
    // ContainsPlayer
    // visited
    [SerializeField]
    LocationGenerator locationGenerator;

    public struct LocationAttributes
    {
        public bool containsPlayer;
        public bool visited;
        public ConnectedLocations connectedLocations;

        public LocationAttributes(bool containsPlayer, bool visited, ConnectedLocations connectedLocations)
        {
            this.containsPlayer = containsPlayer;
            this.visited = visited;
            this.connectedLocations = connectedLocations;
        }
    }

    public struct ConnectedLocations
    {
        Dictionary<LocationGenerator.LocationDirection, Location> locationDictionary;

        public ConnectedLocations(Location left, Location right, Location top, Location bottom)
        {
            locationDictionary = new Dictionary<LocationGenerator.LocationDirection, Location>();
            locationDictionary.Add(LocationGenerator.LocationDirection.Left, left);
            locationDictionary.Add(LocationGenerator.LocationDirection.Right, right);
            locationDictionary.Add(LocationGenerator.LocationDirection.Top, top);
            locationDictionary.Add(LocationGenerator.LocationDirection.Bottom, bottom);
        }

        public Location getLocation(LocationGenerator.LocationDirection direction)
        {
            return locationDictionary[direction];
        }

        public void setLocation(Location locationToConnect, LocationGenerator.LocationDirection directionToConnect) 
        {
            locationDictionary[directionToConnect] = locationToConnect;
        }

    }

    public LocationAttributes attributes;

    //public Location(Location parentLocation, LocationGenerator.LocationDirection parentDirection) 
    //{
    //    connectLocation(parentLocation, parentDirection);
    //}

    private void Awake()
    {
        locationGenerator = GameObject.Find("GameManager").GetComponent<LocationGenerator>();
        setAttributes();
    }

    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            Debug.Log("Entered by player");
            enterLocation();
        }
    }

    void setAttributes()
    {
        attributes = new LocationAttributes(false, false, new ConnectedLocations(null, null, null, null));
    }

    public Location getLocation(LocationGenerator.LocationDirection direction) 
    {
        return attributes.connectedLocations.getLocation(direction);
    }

    public void connectLocation(Location locationToConnect, LocationGenerator.LocationDirection connectingLocationDirection)
    {
        switch (connectingLocationDirection)
        {
            case LocationGenerator.LocationDirection.Left:
                attributes.connectedLocations.setLocation(locationToConnect, LocationGenerator.LocationDirection.Right);
                if (locationToConnect.getLocation(connectingLocationDirection) == null)
                {
                    //locationToConnect.connectLocation(this, LocationGenerator.LocationDirection.Right);
                    locationToConnect.attributes.connectedLocations.setLocation(this, LocationGenerator.LocationDirection.Left);
                }
                break;
            case LocationGenerator.LocationDirection.Right:
                attributes.connectedLocations.setLocation(locationToConnect, LocationGenerator.LocationDirection.Left);
                if (locationToConnect.getLocation(connectingLocationDirection) == null)
                {
                    //locationToConnect.connectLocation(this, LocationGenerator.LocationDirection.Left);
                    locationToConnect.attributes.connectedLocations.setLocation(this, LocationGenerator.LocationDirection.Right);
                }
                break;
            case LocationGenerator.LocationDirection.Top:
                attributes.connectedLocations.setLocation(locationToConnect, LocationGenerator.LocationDirection.Bottom);
                if (locationToConnect.getLocation(connectingLocationDirection) == null)
                {
                    //locationToConnect.connectLocation(this, LocationGenerator.LocationDirection.Bottom);
                    locationToConnect.attributes.connectedLocations.setLocation(this, LocationGenerator.LocationDirection.Top);
                }
                break;
            case LocationGenerator.LocationDirection.Bottom:
                attributes.connectedLocations.setLocation(locationToConnect, LocationGenerator.LocationDirection.Top);
                if (locationToConnect.getLocation(connectingLocationDirection) == null)
                {
                    //locationToConnect.connectLocation(this, LocationGenerator.LocationDirection.Top);
                    locationToConnect.attributes.connectedLocations.setLocation(this, LocationGenerator.LocationDirection.Bottom);
                }
                break;
        }
    }

    //public void connectLocation(Location locationToConnect, LocationGenerator.LocationDirection connectingLocationDirection)
    //{
    //    attributes.connectedLocations.setLocation(locationToConnect, connectingLocationDirection);
    //}

    public void enterLocation()
    {
        attributes.containsPlayer = true;
        if (!attributes.visited)
        {
            attributes.visited = true;
            locationGenerator.visitLocation(this);
        }
        locationGenerator.setNewCurrentLocation(this);
    }

    public void setLocationGenerator(LocationGenerator locationGenerator) 
    {
        this.locationGenerator = locationGenerator;
    }

}
