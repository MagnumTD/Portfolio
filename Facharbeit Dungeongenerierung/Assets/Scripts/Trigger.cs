using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Trigger : MonoBehaviour
{
    public GameObject LockedDoor;
    public Material TriggeredKeyMaterial;
    public Material OpenDoorMaterial;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Done. The player reached the goal and a new level is generated
            if (this.tag == "Respawn")
            {
                GameObject.FindWithTag("LevelCreator").GetComponent<LevelBuilder>().CreateLevel();
            }
            // otherwise the player deactivated a lock. The rooms are repainted, so it is more visible which lock is opened now
            else
            {
                LockedDoor.SetActive(false);
                GameObject lockedRoom = LockedDoor.transform.parent.gameObject;
                Repaint(lockedRoom, OpenDoorMaterial);
                Repaint(gameObject, TriggeredKeyMaterial);
            }
        }
    }

    static public void Repaint(GameObject g, Material newMat)
    {
        // repaints the current object and all it's children to the given material
        for (int i = 0; i < g.transform.childCount; i++)
        {
            var childRenderer = g.transform.GetChild(i).GetComponent<MeshRenderer>();
            childRenderer.material = newMat;
        }
        var renderer = g.GetComponent<MeshRenderer>();
        renderer.material = newMat;
    }
}
