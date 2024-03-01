using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class KnightFSMController : MonoBehaviour
{

    public Tilemap tilemap;

    public float MaxDirDuration = 2f;

    public float currentDirDuration = 0f;

    public static KnightFSMController instance;

    public IState currentState;

    public List<IState> allStates = new List<IState>();

    public List<GameObject> enemies = new List<GameObject>();

    public List<GameObject> obstacles = new List<GameObject>();

    public GameObject key;

    public GameObject gate;

    public float speed = 5f;

    public float viewDistance = 50f;

    public float attackDistance = 2f;

    public float strength = 10f;

    public bool gameEnd = false;

    public bool hasKey = false;

    public int[,] AllPassableGrid;

    public int currentAstarNode = -1;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void Start()
    {
        allStates.Add(new StateSearching());
        allStates.Add(new StateAttacking());
        allStates.Add(new StateEscaping());
        currentState = allStates[0];
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        AllPassableGrid = new int[bounds.size.x, bounds.size.y];


        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    foreach(var obstacle in obstacles)
                    {
                        if(Vector3.Distance(tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0)), obstacle.transform.position) < 0.5f)
                        {
                            AllPassableGrid[x, y] = -1;
                        }
                        else
                        {
                            AllPassableGrid[x, y] = 1;

                        }
                    }
                }
            }
        }
    }

    public void Update()
    {
        Debug.Log("current state: " + currentState.GetType().Name);
        currentState.Update();
        foreach(var state in allStates)
        {
            if(state.CanEnter(currentState))
            {
                currentState.Exit();
                currentState = state;
                currentState.Enter();
                break;
            }
        }
    }

    public void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    public void Move(Vector3 direction)
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void Search()
    {
        if(!hasKey && IsKeyInRange())
        {
            Vector3Int cell = tilemap.WorldToCell(transform.position);
            Vector3Int cell1 = tilemap.WorldToCell(key.transform.position);
            List<AStar.Node> nodes = AStar.FindPath(AllPassableGrid, cell.x, cell.y, cell1.x, cell1.y);
            currentAstarNode++;
            HeadTo(nodes, currentAstarNode);
            //TakeTheKey();
        }
        else if (hasKey && IsGateInRange())
        {
            Vector3Int cell = tilemap.WorldToCell(transform.position);
            Vector3Int cell1 = tilemap.WorldToCell(gate.transform.position);
            List<AStar.Node> nodes = AStar.FindPath(AllPassableGrid, cell.x, cell.y, cell1.x, cell1.y);
            currentAstarNode++;
            HeadTo(nodes, currentAstarNode);
            //TakeTheKey();
        }
        else
        {
            int dir = Random.Range(0, 4);
            if (currentDirDuration <= 0)
            {
                switch (dir)
                {
                    case 0:
                        Move(Vector3.forward);
                        break;
                    case 1:
                        Move(Vector3.back);
                        break;
                    case 2:
                        Move(Vector3.left);
                        break;
                    case 3:
                        Move(Vector3.right);
                        break;
                }
                currentDirDuration = MaxDirDuration;
            }
            else
            {
                currentDirDuration -= Time.deltaTime;
            }

        }
    }

    public bool NoEnemyInRange()
    {
        foreach (var enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < viewDistance)
            {
                return false;
            }
        }
        return true;

    }

    public bool IsEnemyInRangeAndAttack()
    {
        foreach(var enemy in enemies)
        {
            if(Vector3.Distance(transform.position, enemy.transform.position) < viewDistance)
            {
                return enemy.transform.localScale.x < transform.localScale.x;
            }
        }
        return false;
    }

    public bool IsEnemyInRangeAndEscape()
    {
        foreach (var enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < viewDistance)
            {
                return enemy.transform.localScale.x >= transform.localScale.x;
            }
        }
        return false;
    }

    public bool IsKeyInRange()
    {
        if(key != null)
        {
            if(Vector3.Distance(transform.position, key.transform.position) < viewDistance)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsGateInRange()
    {
        if (gate != null)
        {
            if (Vector3.Distance(transform.position, gate.transform.position) < viewDistance)
            {
                return true;
            }
        }
        return false;
    }

    public void Attack()
    {
        foreach(var enemy in enemies)
        {
            if(Vector3.Distance(transform.position, enemy.transform.position) < attackDistance)
            {
                Destroy(enemy.gameObject);
            }
            else if (Vector3.Distance(transform.position, enemy.transform.position) < viewDistance)
            {
                Vector3 dir = enemy.transform.position - transform.position;
                Move(dir);
            }
        }
    }

    public void Escape()
    {
        foreach(var enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < viewDistance)
            {
                Vector3 dir = transform.position - enemy.transform.position;
                Move(dir);
            }
        }
        
    }

    public void HeadTo(List<AStar.Node> path, int currentNodeIndex)
    {
        if(path != null)
        {
            AStar.Node currentNode = path[currentNodeIndex];
            if(currentNodeIndex < path.Count)
            {
                Vector3 dir = new Vector3(currentNode.x, currentNode.y, 0) - transform.position;
                Move(dir);
            }
        }
    }

    public void TakeTheKey()
    {
        if(!hasKey)
        {
            hasKey = true;
            currentAstarNode = -1;
        }
        else
        {
            Debug.Log("You already have the key");
        }
    }
    
    public void OpenTheGate()
    {
        if(hasKey)
        {
            gameEnd = true;
            currentAstarNode = -1;
        }
        else
        {
            Debug.Log("You need the key to open the gate");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "Obstacle")
        {
            Vector2 dir = collision.contacts[0].point - new Vector2(transform.position.x, transform.position.y);
            Move(-dir);
        }
        else if(collision.gameObject.tag == "Key")
        {
            TakeTheKey();
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "Gate")
        {
            OpenTheGate();
        }
    }

}
