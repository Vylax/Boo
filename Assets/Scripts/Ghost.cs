using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    public NavMeshAgent agent;

    public Vector3 target;
    public Vector3 roamPoint;

    public EnemyMode mode;

    private GameManager gameManager = GameManager.Instance;
    private GameObject player => gameManager.player;

    /// <summary>
    /// roaming variables
    /// </summary>
    public float roamingRadius => gameManager.roamingRadius;
    public float roamingPointWeight = 2f; //weight of the roaming random vector
    public float roamersHatredWeight = 1f; //weight of the "get away from other roamers" vector
    public Vector2 randomStepRange = new Vector2(1f, 5f); //how much units should be attributed to weightedTargetDir
    public Vector2 randomCooldownRange = new Vector2(1f, 5f); //how much time between new roaming target assignment

    public float roamingWalkSpeed = 2f;//how fast we should set agent speed while roaming

    /// <summary>
    /// hunting variables
    /// </summary>

    public bool playerInFOV = false;
    public bool playerWithinReach = false;
    public bool playerInSight = false; //if there is no obstacle between the player and a ghost (make sure u take trees leaves in ur calculations!!!!)

    public bool canSeePlayer => playerInFOV && playerWithinReach && playerInSight;

    public float fov = 90f;
    public float sightDistance = 20f;

    public LayerMask lookForPlayerMask; //include player and walls and trees and leaves (+triggercolliders) but no ghost

    public enum EnemyMode
    {
        Idle,//don't move but can attack if in range
        Roam,
        Hunt,
        Freeze//don't move don't attack
    }

    private void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        //update status here
        UpdateView();
    }

    private void UpdateView()
    {
        float angle = Vector3.Angle(PlaneVector(transform.forward), PlaneVector(player.transform.position));
        playerInFOV = angle <= fov / 2f;

        float dist = PlaneDist(player.transform.position, transform.position);
        playerWithinReach = dist <= sightDistance;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, (player.transform.position-transform.position), out hit, sightDistance, lookForPlayerMask, QueryTriggerInteraction.Collide))
        {
            playerInSight = hit.transform.tag == "Player";
        }
        else
        {
            playerInSight = false;
        }
    }

    private void Roam()
    {
        if (mode != EnemyMode.Roam)
        {
            return;
        }

        Vector3 newRoamingTarget = Random.Range(randomStepRange.x, randomStepRange.y) * weightedTargetDir() + roamPoint;
        roamPoint = newRoamingTarget;

        StartCoroutine(SetRoamingDist());
    }

    private IEnumerator SetRoamingDist()
    {
        agent.SetDestination(roamPoint);

        float cooldown = Random.Range(randomCooldownRange.x, randomCooldownRange.y);
        yield return new WaitForSeconds(cooldown);

        if (mode == EnemyMode.Roam)
        {
            Roam();
        }
    }

    public void SetMode(EnemyMode mode)
    {
        if(mode == this.mode) //make sure we don't do anything is there is no change in mode
        {
            Debug.LogWarning($"Ghost {gameObject.name} was already set to EnemyMode {this.mode}");
            return;
        }

        //do things if we have to change some parameter like stop hunting etc...
        
        if(this.mode == EnemyMode.Roam) //if we're no longer roaming
        {
            gameManager.roamingGhosts.Remove(this);

            StopCoroutine(SetRoamingDist());
        }

        //update mode
        this.mode = mode;


        if (mode == EnemyMode.Roam) //if we start roaming
        {
            gameManager.roamingGhosts.Add(this);

            agent.speed = roamingWalkSpeed;

            Roam();
        }
    }

    public Vector3 GetAwayFromRoamersDir()
    {
        List<Ghost> roamingGhosts = new List<Ghost>(gameManager.roamingGhosts); //make a copy of the list to make sure it doesn't change while we iterate on the list

        float minDist = Mathf.Infinity;

        Vector3 dir = Vector3.zero;
        foreach (Ghost ghost in roamingGhosts)
        {
            if(ghost != this) //don't try to get away from youself ;)
            {
                dir += (ghost.transform.position - roamPoint);
                minDist = Mathf.Min(minDist, PlaneDist(ghost.transform.position, roamPoint));
            }
        }
        dir *= -1; //get the opposite vector to the sum
        dir.y = 0;

        return dir.normalized * (minDist < roamingRadius/2f ? roamersHatredWeight : 0);
    }

    public Vector3 GetNextRoamingPointDir()
    {
        Vector2 temp = Random.insideUnitCircle;

        return new Vector3(temp.x, 0, temp.y);
    }

    public Vector3 GetStayInRoamAreaDir()
    {
        Vector3 temp = target - roamPoint;
        temp.y = 0;

        return temp; //we don't normalize it to avoid calculations of the plane dist right after
    }

    public Vector3 weightedTargetDir()
    {
        Vector3 hateDir = GetAwayFromRoamersDir();
        Vector3 roamDir = GetNextRoamingPointDir();
        Vector3 stayInRoamingAreaDir = GetStayInRoamAreaDir();

        Vector3 weightedTargetDir = hateDir * roamersHatredWeight + roamDir * roamingPointWeight + (stayInRoamingAreaDir.magnitude>roamingRadius ? (roamersHatredWeight+ roamingPointWeight) / roamingRadius * stayInRoamingAreaDir : Vector3.zero);
        return weightedTargetDir.normalized;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(target, 1);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target, roamingRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(agent.destination, 1);
    }

    public static Vector3 PlaneVector(Vector3 u)
    {
        return new Vector3(u.x, 0, u.z);
    }

    public float PlaneDist(Vector3 u, Vector3 v) => GameManager.PlaneDist(u, v);
}
