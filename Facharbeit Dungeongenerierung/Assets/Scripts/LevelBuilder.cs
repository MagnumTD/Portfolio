using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class LevelBuilder : MonoBehaviour
{
    public enum GenerationType { MultipleFloors, Crossways, BruteForce, NoSolution }

    // Prefabs
    public GameObject EntranceRoom;
    public GameObject GoalRoom;
    public GameObject EmptyRoom;
    public GameObject LeftCurve;
    public GameObject RightCurve;
    public GameObject LockedRoom;
    public GameObject KeyRoom;
    public GameObject Player; 
    /*_________________________*/

    private string Axiom = "S";
    private string DungeonString = "";
    public GenerationType genType;
    private int RoomSize = 10;
    public int MinimumAmountOfRooms = 10;
    GameObject[] Rooms;

    void Start()
    {
        CreateLevel(DungeonString);
    }

    public void CreateLevel(string generationString = "")
    {
        // Stopping the time it takes to generate the level
        DateTime dt = DateTime.Now;

        // Make sure there is currently no Level in the scene
        DestroyCurrentLevel();

        // if the player entered something, use his string, otherwise generate a new one
        if (!BuildDungeon(generationString))
        {
            // try to build a dungeon until it works; mostly the Brute Force method, that needs this
            do
            {
                {
                    // Make sure there is currently no Level in the scene
                    DestroyCurrentLevel();
                    do
                    {
                        generationString = StringInterpreter.IterateComplete(Axiom);
                    }
                    while (generationString.Length < MinimumAmountOfRooms);
                }
            }
            while (!BuildDungeon(generationString)) ;
        }

        // Add a FirstPersonController to the scene, so the user can testrun through the level
        Instantiate(Player, transform.GetChild(0).position + Vector3.up, Quaternion.identity);

        // Just some information telling how long it took to build the level and how long it is
        Debug.Log("Generation strings length: " + generationString.Length);
        Debug.Log("BuildingTime: " + (DateTime.Now -dt).TotalSeconds);


    }

    public void DestroyCurrentLevel()
    {
        //check for every instance of a prefab and if they exist, destroy them
        var p = GameObject.FindWithTag("Player");
        if (p != null) Destroy(p);
        if (Rooms == null) return;
        foreach (GameObject room in Rooms)
        {
            if (room == null) continue;
            Destroy(room);
        }
    }

    private Stack<Vector3> MultipleFloors(int i, Stack<Vector3> positions, Vector3 direction)
    {
        // Move the current position up, if the place is blocked
        if (CheckWetherPlaceIsBlocked(i, positions.Peek())) positions.Push(positions.Pop() + Vector3.up * (RoomSize + 0.5f));
        return positions;
    }

    private Stack<Vector3> Crossways(int i, Stack<Vector3> positions, Vector3 direction)
    {
        int blockingRoomID;
        // As long as there is a blocking room, move the position one step further and make a way by deactivating the walls in between
        while (CheckWetherPlaceIsBlocked(i, positions.Peek(), out blockingRoomID))
        {
            Quaternion q = Quaternion.FromToRotation(Vector3.right, direction);

            if ((Mathf.Abs(Rooms[blockingRoomID].transform.localRotation.eulerAngles.y) <= (q.eulerAngles.y + 1) % 360 &&
                Mathf.Abs(Rooms[blockingRoomID].transform.localRotation.eulerAngles.y) >= (q.eulerAngles.y - 1) % 360) ||
               (Mathf.Abs(Rooms[blockingRoomID].transform.localRotation.eulerAngles.y) <= (q.eulerAngles.y + 181) % 360 &&
                Mathf.Abs(Rooms[blockingRoomID].transform.localRotation.eulerAngles.y) >= (q.eulerAngles.y + 179) % 360))
            {
                if (Rooms[blockingRoomID].transform.childCount > 2) Rooms[blockingRoomID].transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                Rooms[blockingRoomID].transform.GetChild(0).gameObject.SetActive(false);
                Rooms[blockingRoomID].transform.GetChild(1).gameObject.SetActive(false);
            }
            positions.Push(positions.Pop() + direction * RoomSize);
        }
        return positions;
    }

    private Stack<Vector3> BruteForce(int i, Stack<Vector3> positions, Vector3 direction)
    {
        // If the place is blocked, restart
        if (CheckWetherPlaceIsBlocked(i, positions.Peek())) return null;
        return positions;
    }

    private bool BuildDungeon(string dungeonString)
    {
        // Create a delegate to deal with the different generation types
        Func<int, Stack<Vector3>, Vector3, Stack<Vector3>> SolutionType;

        switch (genType)
        {
            case GenerationType.Crossways:
                {
                    SolutionType = Crossways;
                    break;
                }
            case GenerationType.MultipleFloors:
                {
                    SolutionType = MultipleFloors;
                    break;
                }
            case GenerationType.BruteForce:
                {
                    SolutionType = BruteForce;
                    break;
                }
            default:
                {
                    SolutionType = (i, pos, direction) => pos;
                    break;
                }
        }
        // if an empty string enters, return false and restart
        if (dungeonString == "") return false;
        Rooms = new GameObject[dungeonString.Length];
        Debug.Log(dungeonString);

        Stack<Vector3> positions = new Stack<Vector3>();
        Stack<Vector3> directions = new Stack<Vector3>();
        Stack<GameObject> lockStack = new Stack<GameObject>();
        directions.Push(Vector3.right);
        positions.Push(Vector3.zero);

        #region ForLoop
        for (int i = 0; i < dungeonString.Length; i++)
        {
            // these are extracted from the switchstatement, because the are the only cases, where no room is instantiated, so they don't need to check for a blocking room.
            if (dungeonString[i] == '[')
            {
                // Save the current position
                var tmp = positions.Pop();
                positions.Push(tmp + directions.Peek() * RoomSize);
                positions.Push(tmp);
                directions.Push(directions.Peek());
                continue;
            }
            else if (dungeonString[i] == ']')
            {
                // Jump back to the last saved position
                positions.Pop();
                directions.Pop();
                continue;
            }

            // Check how to react to blocked places. 
            positions = SolutionType(i, positions, directions.Peek());
            if ( positions == null) return false;

            #region switchstatement
            // Check what room is supposed to be placed. For curves, also change the direction and the locks are stacked on a lockstack.
            switch (dungeonString[i])
            {
                case StringInterpreter.EmptyRoom:
                    {
                        Rooms[i] = Instantiate(EmptyRoom, positions.Peek(), Quaternion.FromToRotation(Vector3.right, directions.Peek()), transform) as GameObject;
                        break;
                    }
                case StringInterpreter.EntranceRoom:
                    {
                        Rooms[i] = Instantiate(EntranceRoom, positions.Peek(), Quaternion.FromToRotation(Vector3.right, directions.Peek()), transform) as GameObject;
                        break;
                    }
                case StringInterpreter.KeyRoom:
                    {
                        Rooms[i] = Instantiate(KeyRoom, positions.Peek(), Quaternion.FromToRotation(Vector3.right, directions.Peek()), transform) as GameObject;
                        break;
                    }
                case StringInterpreter.LockedRoom:
                    {
                        Rooms[i] = Instantiate(LockedRoom, positions.Peek(), Quaternion.FromToRotation(Vector3.right, directions.Peek()), transform) as GameObject;
                        lockStack.Push(Rooms[i]);
                        break;
                    }
                case StringInterpreter.GoalRoom:
                    {
                        Rooms[i] = Instantiate(GoalRoom, positions.Peek(), Quaternion.FromToRotation(Vector3.right, directions.Peek()), transform) as GameObject;
                        break;
                    }
                case StringInterpreter.LeftCurve:
                    {
                        Rooms[i] = Instantiate(LeftCurve, positions.Peek(), Quaternion.FromToRotation(Vector3.right, directions.Peek()), transform) as GameObject;
                        if (dungeonString[i - 1] == '[') Rooms[i].transform.GetChild(2).gameObject.SetActive(false);
                        directions.Push(GoLeft(directions.Pop()));
                        break;
                    }
                case StringInterpreter.RightCurve:
                    {
                        Rooms[i] = Instantiate(RightCurve, positions.Peek(), Quaternion.FromToRotation(Vector3.right, directions.Peek()), transform) as GameObject;
                        if (dungeonString[i - 1] == '[') Rooms[i].transform.GetChild(2).gameObject.SetActive(false);
                        directions.Push(GoRight(directions.Pop()));
                        break;
                    }
            }
            #endregion
            // Move the "turtle" when step forward
            positions.Push(positions.Pop() + directions.Peek() * RoomSize);
        }
        #endregion

        AssignKeysToLocks(lockStack);

        return true;
    }

    private Vector3 GoLeft(Vector3 currentDir)
    {
        // very simple method to change direction
        if (Vector3.right == currentDir) return Vector3.forward;
        if (Vector3.forward == currentDir) return Vector3.left;
        if (Vector3.left == currentDir) return Vector3.back;
        if (Vector3.back == currentDir) return Vector3.right;
        return Vector3.zero;
    }

    private Vector3 GoRight(Vector3 currentDir)
    {        
        // very simple method to change direction
        if (Vector3.right == currentDir) return Vector3.back;
        if (Vector3.forward == currentDir) return Vector3.right;
        if (Vector3.left == currentDir) return Vector3.forward;
        if (Vector3.back == currentDir) return Vector3.left;
        return Vector3.zero;
    }

    private bool CheckWetherPlaceIsBlocked(int i, Vector3 position)
    {
        // checks wether the current position is blocked already
        for (int j = 0; j < i; j++)
        {
            if (Rooms[j] == null) continue;
            if (position == Rooms[j].transform.position)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckWetherPlaceIsBlocked(int i, Vector3 position, out int blockingID)
    {
        // This version does not just check but also returns the id of the blocking room
        for (int j = 0; j < i; j++)
        {
            if (Rooms[j] == null) continue;
            if (position == Rooms[j].transform.position)
            {
                blockingID = j;
                return true;
            }
        }
        blockingID = -1;
        return false;
    }

    private void AssignKeysToLocks(Stack<GameObject> LockStack)
    {
        // each key is assigned a lock from the lockstack, if that lock is still locked. If the locked door is not active (crossway) then they both become normal empty rooms
        for (int i = Rooms.Length - 1; i > 0; i--)
        {
            if (Rooms[i] == null) continue;
            if (Rooms[i].GetComponent<Trigger>() == true && (Rooms[i].tag != "Respawn"))
            {
                if (!LockStack.Peek().transform.GetChild(2).gameObject.activeSelf)
                {
                    Trigger.Repaint(Rooms[i], EmptyRoom.GetComponent<Renderer>().sharedMaterial);
                    Destroy(Rooms[i].GetComponent<Trigger>());
                    Trigger.Repaint(LockStack.Pop(), EmptyRoom.GetComponent<Renderer>().sharedMaterial);
                }
                else Rooms[i].GetComponent<Trigger>().LockedDoor = LockStack.Pop().transform.GetChild(2).gameObject;
            }
        }
    }
}


