using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class Unit : MonoBehaviour
    {
        bool isInit;
        Node curNode;
        Node targetNode;                
        int airFrame;        
        int df_counter;
        public int digDownFrames = 35;
        int dd_counter;
        int ddiag_counter;
        bool prevGround;

        [Header("states")]
        public bool move;
        public bool onGround;
        public bool isUmbrella;
        public bool isDigForward;
        bool movingLeft;
        public Ability curAbility;

        [Header("Ability stats")]
        public float lerpSpeed = .3f;
        public float fallSpeed = 5;
        public float umbrellaSpeed = 0.4f;
        public float dig_down = .2f;
        public int airDeathFrames = 80;
        public int digForwardFrames = 35;
        public float build_Time;
        public float build_Speed;
        public int buildAmount = 25;
        public int pixelsOut;
        public int maxPixels = 25;
        public float fillStart = 0.6f;
        public float explodeTimer;
        public float explodeRadius;
        public bool doesMakeLight;

        float e_t;
        bool startFilling;
        float fs_t;
        float f_t;
        float p_t;
        int b_amount;
        float b_t;
        float archerCmpt;
        [SerializeField] bool isclimb = false;

        [Header("References")]
        public SpriteRenderer ren;
        public Animator anim;
        float baseSpeed;
        bool initLerp;
        Vector3 targetPos;
        Vector3 startPos;
        float t;
        GameManager gameManager;
        List<Node> stoppedNodes = new List<Node>();
        int t_x;
        int t_y;

        public bool isDead;

        public GameObject archerView;
        public GameObject combattantView;

        public UIManager uIManager;

        [Header("IA Monstre")]
        public bool isMonster;
        public GameManager gameManagerStart;
        public UnitManager unitManager;
        public bool isAttacking;

        public GameObject spriteMask;

        public void Start()
        {
            if(isMonster)
            {
                StartCoroutine(InitMonster());
            }
        }

        IEnumerator InitMonster()
        {
            yield return new WaitForSeconds(0.5f);
            gameManager = gameManagerStart;
            PlaceOnNode();
            isInit = true;
            curAbility = Ability.walker;
        }

        /*private void Update()
        {
            if(isMonster)
            {
                Tick(unitManager.delta);
            }
        }*/

        public void Init(GameManager gm)
        {
            gameManager = gm;
            PlaceOnNode();
            isInit = true;
            curAbility = Ability.walker;
        }

        void PlaceOnNode()
        {
            if (!isMonster)
            {
                curNode = gameManager.spawnNode;
                transform.position = gameManager.spawnPosition;
            }
            else if (isMonster)
            {
                curNode = gameManager.GetNodeFromWorldPos(transform.position);
            }
        }

        public void Tick(float delta)
        {
            //Debug.Log(isMonster);
            //Debug.Log(curAbility);
            if (!isInit)
                return;
            if (!move)
                return;          

            ren.flipX = movingLeft;
            anim.SetBool("isUmbrella", isUmbrella);

            if (!isAttacking)
            {
                switch (curAbility)
                {
                    case Ability.walker:
                    case Ability.umbrella:
                        Walker(delta);
                        break;
                    case Ability.stopper:
                        Stopper();
                        break;
                    case Ability.dig_down:
                        DiggingDown(delta);
                        break;
                    case Ability.dig_forward:
                        DiggingForward(delta);
                        break;
                    case Ability.dig_diagonale:
                        DiggindDiagonal(delta);
                        break;
                    case Ability.dead:
                        break;
                    case Ability.builder:
                        Builder(delta);
                        break;
                    case Ability.filler:
                        Filler(delta);
                        //CheckCurrentNode();
                        //CheckNodeBelow();
                        break;
                    case Ability.explode:
                        Exploder(delta);
                        break;
                    case Ability.archer:
                        archerCmpt += Time.deltaTime;
                        if (archerCmpt >= 5f)
                        {
                            archerCmpt = 0;
                            archerView.SetActive(false);
                            move = true;
                            curAbility = Ability.walker;
                        }
                        break;
                    case Ability.combattant:
                        Debug.Log("Test");
                        break;
                    case Ability.climber:
                        break;
                    case Ability.lighter:
                        Lighter();
                        break;
                    default:
                        break;
                }
            }
            //Debug.Log(curAbility);
        }

        public bool ChangeAbility(Ability a)
        {
            isUmbrella = false;

            if (!isMonster)
            {
                Debug.Log("Test ChangementAbility : " + a);
                switch (a)
                {
                    case Ability.walker:
                        curAbility = a;
                        anim.Play("walk");
                        break;
                    case Ability.stopper:
                        if (onGround)
                        {
                            FindStoppedNodes();
                            anim.Play("stop");
                            curAbility = a;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Ability.umbrella:
                        isUmbrella = true;
                        break;
                    case Ability.dig_forward:
                        anim.Play("dig_forward");
                        curAbility = a;
                        //isDigForward = true;
                        df_counter = 0;
                        break;
                    case Ability.dig_down:
                        if (onGround)
                        {
                            anim.Play("dig_down");
                            curAbility = a;
                            dd_counter = 0;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Ability.dig_diagonale:
                        if (onGround)
                        {
                            //anim.Play("dig_diag");
                            curAbility = a;
                            ddiag_counter = 0;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Ability.archer:
                        curAbility = a;
                        Debug.Log(curAbility);
                        if (!archerView.activeSelf)
                        {
                            archerView.SetActive(true);
                        }
                        break;
                    case Ability.combattant:
                        curAbility = a;
                        if(!combattantView.activeSelf)
                        {
                            combattantView.SetActive(true);
                            curAbility = Ability.walker;
                            Debug.Log("TestCombattant " + curAbility);
                        }
                        break;
                    case Ability.dead:
                        curAbility = a;
                        break;
                    case Ability.builder:
                        if (onGround)
                        {
                            anim.Play("build");
                            curAbility = a;
                            b_amount = 0;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Ability.filler:
                        if (onGround)
                        {
                            curAbility = a;
                            anim.Play("filler");
                            startFilling = false;
                            fs_t = 0;
                            f_t = 0;
                            pixelsOut = 0;
                            p_t = 0;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Ability.explode:
                        curAbility = a;
                        anim.Play("dead");
                        e_t = 0;
                        break;
                    case Ability.climber:
                        isclimb = true;
                        break;
                    case Ability.lighter:
                        curAbility = a;
                        break;
                    default:
                        break;
                }

            }
            return true;
        }

        void Walker(float delta)
        {
            if (!initLerp)
            {
                initLerp = true;
                startPos = transform.position;
                t = 0;
                bool hasPath = PathFind();
                if (hasPath)
                {
                    Vector3 tp = gameManager.GetWorldPosFromNode(targetNode);
                    targetPos = tp;
                }
                else
                {
                    
                }

                float d = Vector3.Distance(targetPos, startPos);
                if (onGround)
                {
                    baseSpeed = lerpSpeed / d;
                }
                else
                {
                    if (isUmbrella)
                    {
                        baseSpeed = umbrellaSpeed / d;
                    }
                    else
                    {
                        baseSpeed = fallSpeed / d;
                    }
                }
            }
            else
            {
                LerpIntoPosition(delta);   
            }
        }

        void LerpIntoPosition(float delta)
        {
            t += delta * baseSpeed;
            if (t > 1)
            {
                t = 1;
                initLerp = false;
                curNode = targetNode;
            }

            Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
            transform.position = tp;
        }

        bool CheckNodeBelow()
        {
            Node b = gameManager.GetNode(curNode.x, curNode.y - 1);

            if (b != null)
            {
                if (b.isEmpty)
                {
                    ChangeAbility(Ability.walker);
                    return true;
                }
            }
            else
            {
                ChangeAbility(Ability.walker);
                return true; 
            }

            return false;
        }

        bool CheckCurrentNode()
        {
            /*Node b = gameManager.GetNode(curNode.x, curNode.y - 1);
            
            if(b != null)
            {
                if (b.isEmpty)
                {
                    ChangeAbility(Ability.walker);
                    return;
                }
            }
            else
            {
                ChangeAbility(Ability.walker);
                return;
            }*/

            if (curNode.isEmpty == false)
            {
                ChangeAbility(Ability.walker);
                return true;
            }

            return false;
        }

        void Stopper()
        {
            /*Node b = gameManager.GetNode(curNode.x, curNode.y - 1);

            if (b != null)
            {
                if (b.isEmpty)
                {
                    ChangeAbility(Ability.walker);
                    ClearStopNodes();
                    return;
                }
            }
            else
            {
                ChangeAbility(Ability.walker);
                ClearStopNodes();
                return;
            }*/

            if (CheckNodeBelow() || CheckCurrentNode())
            {
                ClearStopNodes();

            }
        }

        void Builder(float delta)
        {
            if (!initLerp)
            {
                b_t += delta;
                if (b_t > build_Time)
                {
                    b_t = 0;
                    initLerp = true;
                    bool interrupt = false;
                    b_amount++;

                    if(b_amount > buildAmount)
                    {
                        interrupt = true;
                    }

                    int _tx = curNode.x;
                    int _ty = curNode.y;
                    _tx = (movingLeft) ? _tx - 1 : _tx + 1;
                    _ty++;

                    startPos = transform.position;
                    targetNode = gameManager.GetNode(_tx, _ty);

                    if(targetNode.isEmpty == false || interrupt)
                    {
                        ChangeAbility(Ability.walker);
                        return;
                    }

                    targetPos = gameManager.GetWorldPosFromNode(targetNode.x, targetNode.y);
                    float d = Vector3.Distance(startPos, targetPos);
                    baseSpeed = build_Speed / d;

                    List<Node> canidates = new List<Node>();
                    for (int i = 0; i < 5; i++)
                    {
                        int xx = _tx + i;
                        Node n = gameManager.GetNode(xx, curNode.y);
                        if (n.isEmpty == true)
                        {
                            canidates.Add(n);
                        }
                    }

                    gameManager.AddCanidatesNodesToBuild(canidates);
                }
            }
            else
            {
                LerpIntoPosition(delta);
            }
        }
               
        void DiggingDown(float delta)
        {
            if (!initLerp)
            {
                initLerp = true;
                startPos = transform.position;
                t = 0;

                int t_x = (movingLeft) ? curNode.x + 1 : curNode.x - 1;
                Node originNode = gameManager.GetNode(t_x, curNode.y + 1);
                List<Node> canidates = CheckNode(originNode, 4);

                if (canidates.Count == 0 || dd_counter > digDownFrames)
                {
                    ChangeAbility(Ability.walker);
                    return;
                }
                dd_counter++;

                gameManager.AddCanidateNodesToClear(canidates);
                Node n = gameManager.GetNode(curNode.x, curNode.y - 1);
                if(n == null)
                {
                    ChangeAbility(Ability.walker);
                    return;
                }
                targetNode = n;
                targetPos = gameManager.GetWorldPosFromNode(targetNode);

                float d = Vector3.Distance(targetPos, startPos);
                baseSpeed = dig_down / d;
            }
            else
            {
                LerpIntoPosition(delta);
            }
        }

        void DiggingForward(float delta)
        {
            if (!initLerp)
            {
                initLerp = true;
                startPos = transform.position;
                t = 0;

                int t_x = (movingLeft) ? curNode.x - 2 : curNode.x + 2;
                Node originNode = gameManager.GetNode(t_x, curNode.y + 4);
                List<Node> canidates = CheckNode(originNode, 5);

                if (df_counter > 0 && (canidates.Count < 2 || df_counter > digForwardFrames))
                {
                    ChangeAbility(Ability.walker);
                    isDigForward = false;
                    return;
                }
                df_counter++;

                gameManager.AddCanidateNodesToClear(canidates);

                Node n = gameManager.GetNode(t_x, curNode.y);
                if (n == null)
                {
                    ChangeAbility(Ability.walker);
                    return;
                }
                targetNode = n;
                targetPos = gameManager.GetWorldPosFromNode(targetNode);

                float d = Vector3.Distance(targetPos, startPos);
                baseSpeed = dig_down / d;
            }
            else
            {
                LerpIntoPosition(delta);
            }
        }

        void DiggindDiagonal(float delta)
        {
            Debug.Log("Test");
            if (!initLerp)
            {
                initLerp = true;
                startPos = transform.position;
                t = 0;

                int t_x = (movingLeft) ? curNode.x - 2 : curNode.x + 2;
                Node originNode = gameManager.GetNode(t_x, curNode.y + 1);
                List<Node> canidates = CheckNode(originNode, 5);

                if (df_counter > 0 && (canidates.Count < 2 || df_counter > digForwardFrames) || canidates.Count == 0 || dd_counter > digDownFrames)
                {
                    ChangeAbility(Ability.walker);
                    isDigForward = false;
                    return;
                }
                df_counter++;
                dd_counter++;

                gameManager.AddCanidateNodesToClear(canidates);

                Node n = gameManager.GetNode(t_x, curNode.y - 1);
                if (n == null)
                {
                    ChangeAbility(Ability.walker);
                    return;
                }
                targetNode = n;
                targetPos = gameManager.GetWorldPosFromNode(targetNode);

                float d = Vector3.Distance(targetPos, startPos);
                baseSpeed = dig_down / d;

            }
            else
            {
                LerpIntoPosition(delta);
            }
        }

        void Filler(float delta)
        {
            if (!startFilling)
            {
                fs_t += delta;
                if (fs_t < fillStart)
                {
                    startFilling = true;
                }
            }

            if (pixelsOut > maxPixels)
            {
                ChangeAbility(Ability.walker);
                return;
            }

            p_t += delta;

            if (p_t > 0.05f)
            {
                pixelsOut++;
                p_t = 0;
            }
            else
            {
                return;
            }

            int _x = (movingLeft) ? curNode.x - 3 : curNode.x + 3;
            int _y = curNode.y + 3;

            Node n = gameManager.GetNode(_x, _y);
            FillNode f = new FillNode();
            f.x = n.x;
            f.y = n.y;
            gameManager.AddFillNodes(f);
        }

        public void Exploder(float delta)
        {
            e_t += delta;
            if(e_t > explodeTimer)
            {
                ChangeAbility(Ability.dead);

                float radius = explodeRadius * 0.01f;
                int steps = Mathf.RoundToInt(explodeRadius);
                Vector3 center = transform.position;
                List<Node> canidates = new List<Node>();

                for(int x = -steps; x < steps; x++)
                {
                    for (int y = -steps; y < steps; y++)
                    {
                        int t_x = x + curNode.x;
                        int t_y = y + curNode.y;

                        float d = Vector3.Distance(center, gameManager.GetWorldPosFromNode(t_x, t_y));
                        if (d > radius)
                            continue;

                        Node n = gameManager.GetNode(t_x, t_y);
                        if (n == null)
                            continue;

                        canidates.Add(n);
                    }
                }

                gameManager.AddCanidateNodesToClear(canidates);
            }
        }

        List<Node> CheckNode(Node o, float rad)
        {
            List<Node> r = new List<Node>();
            Vector3 center = gameManager.GetWorldPosFromNode(o);
            float radius = rad * 0.01f;

            for (int x = -10; x < 10; x++)
            {
                for (int y = -10; y < 10; y++)
                {
                    int t_x = x + curNode.x;
                    int t_y = y + curNode.y;

                    float d = Vector3.Distance(center, gameManager.GetWorldPosFromNode(t_x, t_y));
                    if (d > radius)
                        continue;

                    Node n = gameManager.GetNode(t_x, t_y);
                    if (n == null)
                        continue;

                    if(!n.isEmpty)
                        r.Add(n);
                }
            }

            return r;
        }

        /*List<Node> CheckNodesDown()
        {
            List<Node> r = new List<Node>();

            for (int x = -3; x < 3; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int t_x = curNode.x - x;
                    if (Mathf.Abs(x) == 3)
                    {
                        if (y == 1)
                            continue;
                    }
                    int t_y = curNode.y - y;
                    Node n = gameManager.GetNode(t_x, t_y);
                    if (n == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (!n.isEmpty)
                            r.Add(n);
                    }
                }
            }

            return r;
        }

        List<Node> CheckNodesForward()
        {
            List<Node> r = new List<Node>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (x == 2)
                    {
                        if (y == 0 || y == 8)
                            continue;
                    }

                    int t_x = curNode.x;
                    t_x = (movingLeft) ? t_x - x : t_x + x;
                    int t_y = curNode.y + y;

                    Node n = gameManager.GetNode(t_x, t_y);
                    if (n == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (!n.isEmpty)
                            r.Add(n);
                    }
                }
            }

            return r;
        }*/        

        bool PathFind()
        {
            if (curNode == null)
            {
                targetPos = transform.position;
                targetPos.y = -50;
                prevGround = onGround;
                return false;
            }

            t_x = curNode.x;
            t_y = curNode.y;

            bool downIsAir = IsAir(t_x, t_y - 1);
            bool forwardIsAir = IsAir(t_x, t_y);

            if (downIsAir) //we are falling 
            {
                t_x = curNode.x;
                t_y -= 1;
                airFrame++;

                if (onGround)
                {
                    if (airFrame > 4)
                    {
                        onGround = false;
                        anim.Play("fall");
                    }
                }

            }
            else //we are grounded
            {
                onGround = true;
                if (onGround && !prevGround) // where you land
                {
                    if (airFrame > airDeathFrames && !isUmbrella)
                    {
                        targetNode = curNode;
                        ChangeAbility(Ability.dead);
                        anim.Play("death_land");
                        prevGround = onGround;
                        return true;
                    }
                    else
                    {
                        anim.Play("walk");
                        targetNode = curNode;
                        prevGround = onGround;
                        airFrame = 0;
                        return true;
                    }
                }
                airFrame = 0;

                int s_x = (movingLeft) ? t_x - 1 : t_x + 1;
                bool stopped = IsStopped(s_x, t_y);

                if (stopped)
                {
                    movingLeft = !movingLeft;
                    t_x = (movingLeft) ? curNode.x - 1 : curNode.x + 1;
                    t_y = curNode.y;
                }
                else
                {
                    if (forwardIsAir)
                    {
                        t_x = (movingLeft) ? t_x - 1 : t_x + 1;
                        t_y = curNode.y;
                    }
                    else
                    {
                        int s = 0;
                        bool isValid = false;
                        bool startDig = false;
                        while (s < 4)
                        {                         
                            bool f_isAir = IsAir(t_x, t_y + s);

                            if (isDigForward)
                            {
                                if (s > 0)
                                {
                                    if (!f_isAir)
                                    {
                                        startDig = true;
                                        break;
                                    }
                                }                                
                            }

                            if (f_isAir)
                            {
                                isValid = true;
                                break;
                            }

                            s++;
                        }

                        if (isValid && !startDig)
                        {
                            t_y += s;
                        }
                        else
                        {
                            if (startDig)
                            {
                                curAbility = Ability.dig_forward;
                                anim.Play("dig_forward");
                                return false;
                            }
                            else
                            {
                                if (isclimb)
                                {                    
                                    t_x = curNode.x;
                                    t_y += 1;

                                    Node c = gameManager.GetNode(t_x + 1, t_y + 3);
                                    if(c.isEmpty)
                                    {
                                        isclimb = false;
                                    }
                                }
                                else
                                {
                                    movingLeft = !movingLeft;
                                    t_x = (movingLeft) ? curNode.x - 1 : curNode.x + 1;
                                }                                
                            }
                        }
                    }
                }
            }

            targetNode = gameManager.GetNode(t_x, t_y);
            prevGround = onGround;
            return true;
        }

        bool IsAir(int x, int y)
        {
            Node n = gameManager.GetNode(x, y);
            if (n == null)
                return true;
            bool isAir = n.isEmpty;
            if (n.isFiller)
                isAir = true;

            return isAir;
        }

        bool IsStopped(int x, int y)
        {
            Node n = gameManager.GetNode(x, y);
            if (n == null)
                return false;
            return n.isStoped;
        }

        void FindStoppedNodes()
        {
            for (int x = -2; x < 2; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    Node n = gameManager.GetNode(curNode.x + x, curNode.y + y);
                    if (n == null)
                        continue;

                    n.isStoped = true;
                    stoppedNodes.Add(n);
                }
            }
        }

        void ClearStopNodes()
        {
            for (int i = 0; i < stoppedNodes.Count; i++)
            {
                stoppedNodes[i].isStoped = false;
            }
            stoppedNodes.Clear();
        }

        void Lighter()
        {
            doesMakeLight = true;
            spriteMask.SetActive(true);
            curAbility = Ability.walker;
        }

        public void Die()
        {
            if (isDead == false)
            {
                unitManager.aliveUnits--;
                ChangeAbility(Ability.dead);
                anim.Play("dead");
                isDead = true;
                if(isMonster)
                {
                    foreach (Transform child in gameObject.transform)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "EnemyDamage" && !isMonster)
            {
                Die();
            }
            else if(collision.tag == "PlayerAttack" && isMonster)
            {
                Die();
            }
        }
    }
}
