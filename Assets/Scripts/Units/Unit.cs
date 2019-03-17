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

        public bool move;
        public bool onGround;
        int airFrame;
        public int airDeathFrames = 80;

        public int digForwardFrames = 35;
        int df_counter;
        public int digDownFrames = 35;
        int dd_counter;
        bool prevGround;

        public bool isUmbrella;
        public bool isDigForward;

        bool movingLeft;
        public Ability curAbility;
        public float lerpSpeed = .3f;
        public float fallSpeed = 5;
        public float umbrellaSpeed = 0.4f;
        public float dig_down = .2f;
        

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

        public void Init(GameManager gm)
        {
            gameManager = gm;
            PlaceOnNode();
            isInit = true;
            curAbility = Ability.walker;
        }

        void PlaceOnNode()
        {
            curNode = gameManager.spawnNode;
            transform.position = gameManager.spawnPosition;
        }

        public void Tick(float delta)
        {
            if (!isInit)
                return;
            if (!move)
                return;          

            ren.flipX = movingLeft;
            anim.SetBool("isUmbrella", isUmbrella);

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
                case Ability.dead:
                    break;
                default:
                    break;
            }
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

        void Stopper()
        {

        }

        public bool ChangeAbility(Ability a)
        {
            isUmbrella = false;


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
                    isDigForward = true;
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
                case Ability.dead:
                    curAbility = a;
                    break;
                default:
                    break;
            }
            return true;
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

                gameManager.AddCanidateNodes(canidates);
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

                if (canidates.Count == 0 || df_counter > digForwardFrames)
                {
                    ChangeAbility(Ability.walker);
                    isDigForward = false;
                    return;
                }
                df_counter++;

                gameManager.AddCanidateNodes(canidates);

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
                        anim.Play("land");
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
                                movingLeft = !movingLeft;
                                t_x = (movingLeft) ? curNode.x - 1 : curNode.x + 1;
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
            return n.isEmpty;
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
    }
}
