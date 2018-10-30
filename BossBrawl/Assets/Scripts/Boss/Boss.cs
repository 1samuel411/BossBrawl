using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{

    public enum Attacks
    {
        Airstrike, Fist, Swim, None
    };
    public Attacks currentAttack = Attacks.None;
    public SceneManager.Direction curDirection;

    public bool toggled = true;

    public Animator animator;
    public OnTriggerForward triggerForward;

    // State manager
    [System.Serializable]
    public class Attack
    {
        public Image icon;
        public float cooldown;
        public float curCooldown;
        public string inputControl;
        public bool canAttack
        {
            get
            {
                return Time.time >= curCooldown;
            }
        }
        public Attacks attack;

        public void Invoke()
        {
            curCooldown = Time.time + cooldown;
        }
    }
    [System.Serializable]
    public class AirstrikeAttack : Attack
    {
        public GameObject missile;
        public float missileYSpawnPos;
        public Transform indicator;
        public float moveSpeed;
        public bool airstriking;
        public float time;
        public float curTimer;
        public Text text;
    }
    [System.Serializable]
    public class FistAttack : Attack
    {
        public bool happening;
    }
    [System.Serializable]
    public class SwimAttack : Attack
    {
        public SceneManager.Direction targetDirection;
        public GameObject directionIndicator;
        public float swimTime;
        public float swimSpeed;
        public float swimYOffset;
        public float defaultYOffset;
        public bool swimming;
    }

    public FistAttack fistAttack;
    public AirstrikeAttack airstrikeAttack;
    public SwimAttack swimAttack;
    public Health health;
    private bool dead = false;

    void Start()
    {
        UpdatePosition();
        triggerForward.eventForwardDelegate += OnHexagonTriggerEnter;
        health = GetComponent<Health>();
        health.onDeath += OnDeath;
    }

    public void OnHexagonTriggerEnter(Collider collider)
    {
        hexagonInFront = collider.gameObject;
    }

    void Update()
    {
        if (Time.time == 0)
            return;
        if (!doUpdate)
            return;

        if (dead)
            return;

        DoAttack();

        UpdateUI();

        if (currentAttack != Attacks.None)
            return;

        // Check fist attack
        if (InputManager.instance.player1.GetButtonDown(fistAttack.inputControl) && fistAttack.canAttack)
        {
            PerformFistAttack();
        }

        // Check air attack
        if (InputManager.instance.player1.GetButtonDown(airstrikeAttack.inputControl) && airstrikeAttack.canAttack)
        {
            PerformAirstrike();
        }

        // Check swim attack
        if (InputManager.instance.player1.GetButtonDown(swimAttack.inputControl) && swimAttack.canAttack)
        {
            PerformSwim();
        }
    }

    void UpdateUI()
    {
        fistAttack.icon.fillAmount = ((Time.time - fistAttack.curCooldown) + fistAttack.cooldown) / fistAttack.cooldown;
        airstrikeAttack.icon.fillAmount = ((Time.time - airstrikeAttack.curCooldown) + airstrikeAttack.cooldown) / airstrikeAttack.cooldown;
        swimAttack.icon.fillAmount = ((Time.time - swimAttack.curCooldown) + swimAttack.cooldown) / swimAttack.cooldown;

        if (airstrikeAttack.airstriking)
            airstrikeAttack.icon.fillAmount = 0;
        if (swimAttack.swimming)
            swimAttack.icon.fillAmount = 0;
    }

    public void DoAttack()
    {
        switch(currentAttack)
        {
            case Attacks.Swim:
                SwimUpdate();
                break;
            case Attacks.Airstrike:
                AirstrikeUpdate();
                break;
            case Attacks.Fist:
                FistUpdate();
                break;
        }
    }

    void OnDeath()
    {
        GameOver.instance.Death();
        dead = true;
        animator.SetBool("Dead", true);
    }

    #region Fist attack
    public void PerformFistAttack()
    {
        currentAttack = Attacks.Fist;
        Debug.Log("Performing Fist");
        animator.SetTrigger("Attack");
        fistAttack.happening = true;
    }

    void FistUpdate()
    {
        fistAttack.Invoke();
    }

    GameObject hexagonInFront;
    public void Impact()
    {
        if (hexagonInFront == null)
            return;

        PillarManager.instance.Impact(hexagonInFront);

        // Grounded
        if (SceneManager.instance.playerObj.GetComponent<PlayerController>().isGrounded)
        {
            // Damage
            SceneManager.instance.playerObj.GetComponent<Health>().TakeDamage(7);
            SceneManager.instance.playerObj.GetComponent<PlayerController>().jump = true;
        }
        if ((transform.position - SceneManager.instance.playerObj.transform.position).sqrMagnitude <= 13)
        {
            // Damage
            SceneManager.instance.playerObj.GetComponent<Health>().TakeDamage(4);
        }
    }

    public void CompleteFist()
    {
        fistAttack.Invoke();
        fistAttack.happening = false;
        StartCoroutine(WaitFrame());
    }
    #endregion

    #region Airstrike
    public void PerformAirstrike()
    {
        currentAttack = Attacks.Airstrike;
        airstrikeAttack.indicator.transform.position = SceneManager.instance.center.position;
    }

    void AirstrikeUpdate()
    {
        float inputX = (InputManager.instance.player1.GetAxis("XLook"));
        float inputY = (InputManager.instance.player1.GetAxis("YLook"));
        Vector3 movement = new Vector3(inputX, 0, inputY);
        movement = transform.TransformDirection(movement);
        airstrikeAttack.indicator.transform.position += movement * Time.deltaTime * airstrikeAttack.moveSpeed;

        if (airstrikeAttack.airstriking)
        {
            AirstrikeHappening();
            return;
        }

        airstrikeAttack.indicator.gameObject.SetActive(true);

        if (InputManager.instance.player1.GetButtonDown("Select"))
        {
            animator.SetTrigger("Striking");
            animator.SetBool("Strike", true);
            airstrikeAttack.curTimer = Time.time + airstrikeAttack.time;
            airstrikeAttack.airstriking = true;
        }
        if(InputManager.instance.player1.GetButtonDown("Cancel"))
        {
            airstrikeAttack.airstriking = false;
            airstrikeAttack.indicator.gameObject.SetActive(false);
            StartCoroutine(WaitFrame());
        }
    }

    private float strikeTimer;
    void AirstrikeHappening()
    {
        float timeLeft = airstrikeAttack.curTimer - Time.time;
        airstrikeAttack.text.text = ((int)timeLeft).ToString();

        if(Time.time >= strikeTimer)
        {
            SpawnMissile();
            strikeTimer = Time.time + 0.5f + Random.Range(-0.4f, 0.1f);
        }

        if (timeLeft <= 0)
        {
            animator.SetBool("Strike", false);
            StartCoroutine(WaitFrame());
            airstrikeAttack.airstriking = false;
            airstrikeAttack.indicator.gameObject.SetActive(false);
            airstrikeAttack.text.text = "";
            airstrikeAttack.Invoke();
        }
    }

    void SpawnMissile()
    {
        GameObject missile = Instantiate(airstrikeAttack.missile);
        missile.transform.position = new Vector3(airstrikeAttack.indicator.transform.position.x + Random.Range(-6, 6), airstrikeAttack.missileYSpawnPos, airstrikeAttack.indicator.transform.position.z + Random.Range(-6, 6));
        missile.transform.LookAt(airstrikeAttack.indicator);
    }
    #endregion

    #region Swimming
    public void PerformSwim()
    {
        currentAttack = Attacks.Swim;
        swimAttack.targetDirection = curDirection;
    }

    void SwimUpdate()
    {
        if (swimAttack.swimming)
            return;

        if(swimAttack.targetDirection == curDirection)
            swimAttack.directionIndicator.SetActive(false);
        else
            swimAttack.directionIndicator.SetActive(true);
        swimAttack.directionIndicator.transform.position = SceneManager.instance.GetBossPosition(swimAttack.targetDirection).holder.position;

        float inputX = (InputManager.instance.player1.GetAxis("XLook"));
        float inputY = (InputManager.instance.player1.GetAxis("YLook"));

        if (inputX <= -0.1f)
        {
            // left
            switch (curDirection)
            {
                case SceneManager.Direction.North:
                    // if we're north left is east
                    swimAttack.targetDirection = SceneManager.Direction.East;
                    break;
                case SceneManager.Direction.East:
                    // if we're east left is south
                    swimAttack.targetDirection = SceneManager.Direction.South;
                    break;
                case SceneManager.Direction.South:
                    // if we're south left is west
                    swimAttack.targetDirection = SceneManager.Direction.West;
                    break;
                case SceneManager.Direction.West:
                    // if we're west left is north
                    swimAttack.targetDirection = SceneManager.Direction.North;
                    break;
            }
        }

        if (inputX >= 0.1f)
        {
            // right
            switch (curDirection)
            {
                case SceneManager.Direction.North:
                    // if we're north right is west
                    swimAttack.targetDirection = SceneManager.Direction.West;
                    break;
                case SceneManager.Direction.East:
                    // if we're east right is north
                    swimAttack.targetDirection = SceneManager.Direction.North;
                    break;
                case SceneManager.Direction.South:
                    // if we're south right is east
                    swimAttack.targetDirection = SceneManager.Direction.East;
                    break;
                case SceneManager.Direction.West:
                    // if we're west right is south
                    swimAttack.targetDirection = SceneManager.Direction.South;
                    break;
            }
        }

        if(inputY > 0.1f || swimAttack.targetDirection == curDirection)
        {
            // straight
            switch (curDirection)
            {
                case SceneManager.Direction.North:
                    // if we're north straight is south
                    swimAttack.targetDirection = SceneManager.Direction.South;
                    break;
                case SceneManager.Direction.East:
                    // if we're east straight is west
                    swimAttack.targetDirection = SceneManager.Direction.West;
                    break;
                case SceneManager.Direction.South:
                    // if we're south straight is north
                    swimAttack.targetDirection = SceneManager.Direction.North;
                    break;
                case SceneManager.Direction.West:
                    // if we're west straight is east
                    swimAttack.targetDirection = SceneManager.Direction.East;
                    break;
            }
        }

        if (InputManager.instance.player1.GetButtonDown("Select"))
        {
            currentAttack = Attacks.Swim;
            StartCoroutine(Swim());
        }
        if(InputManager.instance.player1.GetButtonDown("Cancel"))
        {
            swimAttack.swimming = false;
            StartCoroutine(WaitFrame());
            swimAttack.directionIndicator.SetActive(false);
        }
    }

    public IEnumerator Swim()
    {
        swimAttack.swimming = true;
        swimAttack.directionIndicator.SetActive(false);
        // sink
        /*
        while (transform.position.y > swimAttack.swimYOffset)
        {
            transform.position += -Vector3.up * swimAttack.swimSpeed * Time.deltaTime;
            yield return null;
        }*/
        animator.SetTrigger("Despawn");

        // wait half
        yield return new WaitForSeconds(swimAttack.swimTime / 2);

        // teleport
        curDirection = swimAttack.targetDirection;
        UpdatePosition();
        transform.position = new Vector3(transform.position.x, swimAttack.swimYOffset, transform.position.z);

        // wait other half
        yield return new WaitForSeconds(swimAttack.swimTime/2);


        animator.SetTrigger("Spawn");
        // swim up
        /*
        while (transform.position.y < swimAttack.defaultYOffset)
        {
            transform.position += Vector3.up * swimAttack.swimSpeed * Time.deltaTime;
            yield return null;
        }*/

        yield return new WaitForSeconds(0.2f);
        swimAttack.swimming = false;
        StartCoroutine(WaitFrame());
        swimAttack.Invoke();
    }
    #endregion

    public void UpdatePosition()
    {
        SceneManager.BossPosition bossPosition = SceneManager.instance.GetBossPosition(curDirection);
        transform.position = bossPosition.holder.position;
        transform.position = new Vector3(transform.position.x, swimAttack.swimYOffset, transform.position.z);
        transform.rotation = bossPosition.holder.rotation;
    }

    private bool doUpdate = true;
    IEnumerator WaitFrame()
    {
        doUpdate = false;
        yield return null;
        doUpdate = true;
        currentAttack = Attacks.None;
    }
}
