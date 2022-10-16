using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    public NavMeshAgent agent;

    public Vector3 target;
    public Vector3 roamPoint;

    public EnemyMode mode;

    private GameObject player => GameManager.Instance.player;

    /// <summary>
    /// roaming variables
    /// </summary>
    public float roamingRadius => GameManager.Instance.roamingRadius;
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
    public bool playerCanBeHeard = false;

    public Vector3 sightOffest = new Vector3(0, 1.5f, 0);

    public bool canSeePlayer => (playerInFOV || playerCanBeHeard) && playerWithinReach && playerInSight && player.GetComponent<FirstPersonController>().isGrounded;

    public float fov = 90f;
    public float sightDistance = 20f;
    public float earDistance = 4f;

    public LayerMask lookForPlayerMask; //include player and walls and trees and leaves (+triggercolliders) but no ghost

    public Vector3 lastPlayerPos; //last pos where the player was seen
    public Vector3 lastPlayerDir; //last dir where the player was seen heading

    public Vector2 huntingCooldownRange = new Vector2(1f, 5f); //how much time between new roaming target assignment

    public bool waiting = false;
    public float waitingTime = 7f;
    public float huntingSpeed = 4f;
    public float targetReachedThreshold = .5f;

    public Material freezeMaterial;

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
        if(mode == EnemyMode.Freeze)
        {
            return;
        }
        //update status here
        UpdateView();

        if (canSeePlayer && mode != EnemyMode.Hunt)
        {
            SetMode(EnemyMode.Hunt);
        }
    }

    public void Die()
    {
        //do stuff here
        mode = EnemyMode.Freeze;
        GetComponentInChildren<MeshRenderer>().material = freezeMaterial;
        StartCoroutine(Shrink());
    }

    public void Attack()
    {
        GetComponentInChildren<MeshRenderer>().enabled = false;

        //vanishing particles here
        GameManager.Instance.huntingGhosts.Remove(this);
        GameManager.Instance.AlterHuntingGhostCount();
        Player.Instance.Attacked(this);
    }

    public float ghostShrinkDuration = 0.35f;

    private IEnumerator Shrink()
    {
        float timeStep = ghostShrinkDuration / 100f;
        float volumeStep = transform.GetChild(0).localScale.x / 100f;//terrible way to do it but time's running out and so is my brain

        int popInt = Random.Range(69, 100);

        for (int i = 0; i < 100; i++)
        {
            if (i == popInt)
            {
                //add particles here !!!!!!!!!!!!!!!!
                Player.Instance.PlayAudio($"pop 0{Random.Range(1, 9)}", transform.position);
            }
            transform.GetChild(0).localScale -= Vector3.one * volumeStep;
            yield return new WaitForSeconds(timeStep);
        }

        GameManager.Instance.huntingGhosts.Remove(this);
        GameManager.Instance.AlterHuntingGhostCount();
        GameManager.Instance.SpawnGhost();
        Destroy(this.gameObject);
    }

    private void UpdateView()
    {
        float angle = Vector3.Angle(PlaneVector(transform.forward), PlaneVector(player.transform.position-transform.position));
        playerInFOV = angle <= fov / 2f;

        float dist = PlaneDist(player.transform.position, transform.position);
        playerWithinReach = dist <= sightDistance;
        playerCanBeHeard = dist <= sightDistance;

        RaycastHit hit;
        if (Physics.Raycast(transform.position+ sightOffest, (player.transform.position-(transform.position+ sightOffest)), out hit, sightDistance, lookForPlayerMask, QueryTriggerInteraction.Collide))
        {
            Debug.DrawLine(transform.position + sightOffest, hit.point, Color.red);
            playerInSight = hit.transform.tag == "Player";
            if (canSeePlayer)
            {
                lastPlayerDir = player.transform.position - lastPlayerPos; //we look at player movement and not facing direction
                lastPlayerPos = player.transform.position;
            }
        }
        else
        {
            playerInSight = false;
        }
    }

    private void Hunt()
    {
        if (mode != EnemyMode.Hunt)
        {
            return;
        }

        target = lastPlayerPos;

        if (canSeePlayer)
        {
            StartCoroutine(Hunting());
        }

        if (!canSeePlayer && !waiting) //we don't see the player and aren't waiting yet
        {
            waiting = true;
            StartCoroutine(WaitBeforeEndingTheHunt());
        }
    }

    private IEnumerator Hunting()
    {
        agent.SetDestination(target);

        float cooldown = huntingCooldownRange.x;
        float startCooldownTime = Time.time;

        yield return new WaitUntil(() => PlaneDist(transform.position, agent.destination) < targetReachedThreshold || Time.time > startCooldownTime+cooldown);

        if (mode == EnemyMode.Hunt)
        {
            Hunt();
        }
    }

    private IEnumerator RoamWhileWaiting()
    {
        while (waiting && mode == EnemyMode.Hunt)
        {
            Vector3 newRoamingTarget = Random.Range(randomStepRange.x, randomStepRange.y) * weightedTargetDirWhileHunting() + roamPoint;
            roamPoint = newRoamingTarget;

            agent.SetDestination(roamPoint);

            float cooldown = randomCooldownRange.x; //ghosts roam quickly cuz they're on alert
            yield return new WaitForSeconds(cooldown);
        }
        yield return null;
    }

    private IEnumerator WaitBeforeEndingTheHunt()
    {
        float startWaitingTime = Time.time;

        target = lastPlayerPos;

        //try to roam meanwhile ?
        roamPoint = lastPlayerPos;
        StartCoroutine(RoamWhileWaiting());

        yield return new WaitUntil(() => canSeePlayer || Time.time >= startWaitingTime + waitingTime);

        StopCoroutine(RoamWhileWaiting());

        if (canSeePlayer) //resume hunt
        {
            Hunt();
        }
        else //stop hunting and go back to roam
        {
            SetMode(EnemyMode.Roam);
        }

        waiting = false;
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
            GameManager.Instance.roamingGhosts.Remove(this);

            StopCoroutine(SetRoamingDist());
        }
        if (this.mode == EnemyMode.Hunt) //if we stop hunting
        {
            GameManager.Instance.huntingGhosts.Remove(this);
            GameManager.Instance.AlterHuntingGhostCount();

            StopCoroutine(Hunting());
            StopCoroutine(WaitBeforeEndingTheHunt());
            StopCoroutine(RoamWhileWaiting());
            waiting = false;
        }


        //update mode
        this.mode = mode;


        if (mode == EnemyMode.Roam) //if we start roaming
        {
            GameManager.Instance.roamingGhosts.Add(this);

            agent.speed = roamingWalkSpeed;

            Roam();
        }
        if (mode == EnemyMode.Hunt) //if we start hunting
        {
            GameManager.Instance.huntingGhosts.Add(this);
            GameManager.Instance.AlterHuntingGhostCount();

            agent.speed = huntingSpeed;

            Hunt();
        }
        if (mode == EnemyMode.Freeze) //if we start freezing
        {
            agent.enabled = false;
        }
    }

    public Vector3 GetAwayFromRoamersDir()
    {
        List<Ghost> roamingGhosts = new List<Ghost>(GameManager.Instance.roamingGhosts); //make a copy of the list to make sure it doesn't change while we iterate on the list

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

    public Vector3 weightedTargetDirWhileHunting()
    {
        Vector3 hateDir = GetAwayFromRoamersDir();
        Vector3 roamDir = GetNextRoamingPointDir();

        Vector3 playerDir = PlaneVector(lastPlayerDir).normalized; //ghost will tend to look toward player heading direction

        Vector3 weightedTargetDir = hateDir * roamersHatredWeight + roamDir * roamingPointWeight + (roamersHatredWeight + roamingPointWeight) * playerDir;
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
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lastPlayerPos, 1);
    }

    public static Vector3 PlaneVector(Vector3 u)
    {
        return new Vector3(u.x, 0, u.z);
    }

    public float PlaneDist(Vector3 u, Vector3 v) => GameManager.PlaneDist(u, v);
}
