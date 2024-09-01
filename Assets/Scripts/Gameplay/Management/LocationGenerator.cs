using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationGenerator : MonoBehaviour
{

    public List<Location> locations = new List<Location>();

    [SerializeField]
    public Location startLocation;

    Location currentLocation;

    [SerializeField]
    GameManager gameManager;
    Player player;

    [SerializeField]
    GameObject startLocationPrefab;

    public enum LocationDirection: int
    {
        Left,
        Right,
        Top,
        Bottom
    }

    private void Awake()
    {
        player = gameManager.getPlayer();
        startLocation.setLocationGenerator(this);
        setNewCurrentLocation(startLocation);
        //currentLocation = startLocation;
        locations.Add(startLocation);
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //generateNewLocation(currentLocation, LocationDirection.Right);

        }
    }

    public LocationGenerator(Location startLocation) 
    {
        this.startLocation = startLocation;
    }
       
    /* Generate a new location in the surrounding location if no locations currently exist
     *
     */
    void generateNewLocation(Location fromLocation, LocationDirection directionFromLocation) 
    {
        //TODO: Instatiate new prefab based on generation rules (TBD)

        Vector3 newLocationPosition = new Vector3(fromLocation.transform.position.x, fromLocation.transform.position.y);

        switch (directionFromLocation)
        {
            case LocationDirection.Left:
                newLocationPosition.x = fromLocation.transform.position.x - fromLocation.transform.localScale.x;
                break;
            case LocationDirection.Right:
                newLocationPosition.x = fromLocation.transform.position.x + fromLocation.transform.localScale.x;
                break;
            case LocationDirection.Top:
                newLocationPosition.y = fromLocation.transform.position.y + fromLocation.transform.localScale.y;
                break;
            case LocationDirection.Bottom:
                newLocationPosition.y = fromLocation.transform.position.y - fromLocation.transform.localScale.y;
                break;
        }

        GameObject newLocationObject = Instantiate(startLocationPrefab, newLocationPosition, fromLocation.transform.rotation);
        Location newLocation = newLocationObject.GetComponent<Location>();
        newLocation.setLocationGenerator(this);
        newLocation.connectLocation(fromLocation, directionFromLocation); 
        locations.Add(newLocation);
        connectToSurroundingLocations(newLocation);

    }

    public void visitLocation(Location location)
    {
        // Link to previous location
        LocationDirection entryDirection = determineLoactionEntryEdge(location, currentLocation);
        setNewCurrentLocation(location);
        //currentLocation = location;

        for (int i = 0; i < 4; i++)
        {
            if (location.attributes.connectedLocations.getLocation((LocationDirection) i) == null)
            {
                generateNewLocation(currentLocation,(LocationDirection) i);
            }

            if (i < 2 /*Left or Right*/)
            {
                Location sideLocation = currentLocation.attributes.connectedLocations.getLocation((LocationDirection)i);
                if (sideLocation.attributes.connectedLocations.getLocation(LocationDirection.Bottom) == null)
                {
                    generateNewLocation(sideLocation, LocationDirection.Bottom);
                }

                if (sideLocation.attributes.connectedLocations.getLocation(LocationDirection.Top) == null)
                {
                    generateNewLocation(sideLocation, LocationDirection.Top);
                }
            }
        }

        attemptConnectLocation();

    }

    void attemptConnectLocation()
    {
        foreach (Location location in locations)
        {
            connectToSurroundingLocations(location);
        }
    }

    void connectToSurroundingLocations(Location location) 
    {
        List<LocationDirection> unconnectedLocationDirections = new List<LocationDirection>();
        for (int i = 0; i < 4; i++)
        {
            if (location.getLocation((LocationDirection)i) == null)
            {
                unconnectedLocationDirections.Add((LocationDirection)i);
            }
        }

        foreach (LocationDirection dir in unconnectedLocationDirections)
        {
            bool connected = false;
            switch (dir)
            {
                case LocationDirection.Left:
                case LocationDirection.Right:
                    if (!unconnectedLocationDirections.Contains(LocationDirection.Top))
                    {
                        Location topLocation = location.getLocation(LocationDirection.Top);
                        if (topLocation.getLocation(dir) != null && (topLocation.getLocation(dir).getLocation(LocationDirection.Bottom) != null))
                        {
                            topLocation.getLocation(dir).getLocation(LocationDirection.Bottom).connectLocation(location, dir);
                            connected = true;                            
                        }
                    }
                    if (!connected)
                    {
                        if (!unconnectedLocationDirections.Contains(LocationDirection.Bottom))
                        {
                            Location bottomLocation = location.getLocation(LocationDirection.Bottom);
                            if (bottomLocation.getLocation(dir) != null && (bottomLocation.getLocation(dir).getLocation(LocationDirection.Top) != null))
                            {
                                bottomLocation.getLocation(dir).getLocation(LocationDirection.Top).connectLocation(location, dir);
                                connected = true;
                            }
                        }
                    }
                    break;
                case LocationDirection.Top:
                case LocationDirection.Bottom:
                    if (!unconnectedLocationDirections.Contains(LocationDirection.Left))
                    {
                        Location leftLocation = location.getLocation(LocationDirection.Left);
                        if (leftLocation.getLocation(dir) != null && (leftLocation.getLocation(dir).getLocation(LocationDirection.Right) != null))
                        {
                            leftLocation.getLocation(dir).getLocation(LocationDirection.Right).connectLocation(location, dir);
                            connected = true;
                        }
                    }
                    if (!connected)
                    {
                        if (!unconnectedLocationDirections.Contains(LocationDirection.Right))
                        {
                            Location rightLocation = location.getLocation(LocationDirection.Right);
                            if (rightLocation.getLocation(dir) != null && (rightLocation.getLocation(dir).getLocation(LocationDirection.Left) != null))
                            {
                                rightLocation.getLocation(dir).getLocation(LocationDirection.Left).connectLocation(location, dir);
                                connected = true;
                            }
                        }
                    }
                    break;
            }
        }
    }

    void calculatePlayerLocation(Player player)
    {
        Vector3 playersPosition = player.getPlayerPosition();
    }
     
    LocationDirection determineLoactionEntryEdge(Location locationEntered, Location currentLocation)
    {
        Vector3 newLocationPosition = locationEntered.transform.position;
        Vector3 currentLocationPosition = currentLocation.transform.position;

        // Left -ve X, Down -ve Y, Right +ve X, Up +ve Y
        // From current to New
        Vector3 direction = newLocationPosition - currentLocationPosition.normalized;

        if (Mathf.Abs(direction.x) > 0)
        {
            return (direction.x > 0) ? LocationDirection.Left : LocationDirection.Right;
        }
        else if (Mathf.Abs(direction.y) > 0)
        {
            return (direction.y > 0) ? LocationDirection.Bottom : LocationDirection.Top;
        }
        else
        {
            return LocationDirection.Right; // TEMP DEFAULT
        }
    }

    public void setNewCurrentLocation(Location location)
    {
        currentLocation = location;
        gameManager.setCurrentLocationGameObject(currentLocation);
    }

}
