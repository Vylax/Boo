using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GameManager : MonoBehaviour
{
    public float minDistToPlayer;
    public float skyYCoordinate;
    public float roamingRadius = 20f;

    public int[] maxItemInstances;

    public LayerMask groundMask;
    public LayerMask wallMask;

    public GameObject ghostPrefab;

    public GameObject player;
    private Collider[] walls;

    public List<Ghost> roamingGhosts;

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        walls = GameObject.FindGameObjectWithTag("Walls").GetComponents<Collider>();

        roamingGhosts = new List<Ghost>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SpawnGhost();
        }
    }

    public void SpawnItem(int itemID)
    {

    }

    private Vector3 GhostSpawnPos()
    {
        Vector3 closest = Vector3.zero;
        float minDist = Mathf.Infinity;
        for (int i = 0; i < walls.Length; i++)
        {
            Vector3 point = walls[i].ClosestPoint(player.transform.position);
            float dist = Vector3.Distance(point, player.transform.position);
            if(dist < minDist)
            {
                minDist = dist;
                closest = point;
            }
        }

        Vector3 dir = player.transform.position - closest;
        dir.y = 0;
        dir.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, dir, out hit, Mathf.Infinity, wallMask, QueryTriggerInteraction.Collide))
        {
            float maxDist = PlaneDist(player.transform.position, hit.point) - roamingRadius;// -roamingRadius allows us to make sure ghost wont stick to walls for ever
            float dist = Random.Range(minDistToPlayer, maxDist);

            //Debug.Log($"dir={dir} |dist={dist} |hit.point={hit.point} | maxDist={maxDist}");
            Vector3 spawnPos = player.transform.position + dist * dir;
            spawnPos.y = skyYCoordinate;
            spawnPos = GetGroundPos(spawnPos); //project spawnPos on the ground mesh

            return spawnPos;
        }
        else
        {
            Debug.LogError($"No point found on the other side. closest = {closest} | dir={dir}");
            return Vector3.zero;
        }
    }

    private void SpawnGhost()
    {
        Vector3 spawnPos = GhostSpawnPos();

        Ghost ghost = Instantiate(ghostPrefab, spawnPos, Quaternion.identity).GetComponent<Ghost>();

        ghost.roamPoint = spawnPos;
        ghost.target = spawnPos;
        ghost.SetMode(Ghost.EnemyMode.Roam);
    }

    private Vector3 GetGroundPos(Vector3 pos)
    {
        RaycastHit hit;
        if(Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, groundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        Debug.LogError($"Couldn't find a ground point under position {pos} in GetGroundPos() call");
        return pos;
    }

    public static float PlaneDist(Vector3 u, Vector3 v)
    {
        return Mathf.Sqrt(Mathf.Pow(u.x - v.x, 2) + Mathf.Pow(u.z - v.z, 2));
    }
}
