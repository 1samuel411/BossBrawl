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

    // State manager
    [System.Serializable]
    public class Attack
    {
        public Image icon;
        public float cooldown;
        public float curCooldown;
        public float moveSpeed;
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
        public bool airstriking;
        public float time;
        public float curTimer;
        public Text text;
    }
    [System.Serializable]
    public class FistAttack : Attack
    {

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

    void Start()
    {
        UpdatePosition();
    }

    void Update()
    {
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
        }
    }

    public void PerformFistAttack()
    {
        fistAttack.Invoke();
        Debug.Log("Performing Fist");
    }

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
            airstrikeAttack.curTimer = Time.time + airstrikeAttack.time;
            airstrikeAttack.airstriking = true;
        }
        if(InputManager.instance.player1.GetButtonDown("Cancel"))
        {
            airstrikeAttack.airstriking = false;
            airstrikeAttack.indicator.gameObject.SetActive(false);
            currentAttack = Attacks.None;
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

        if(timeLeft <= 0)
        {
            currentAttack = Attacks.None;
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

        if (inputX <= -0.5f)
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

        if (inputX >= 0.5f)
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

        if(inputY > 0.5f || swimAttack.targetDirection == curDirection)
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
            currentAttack = Attacks.None;
            swimAttack.directionIndicator.SetActive(false);
        }
    }

    public IEnumerator Swim()
    {
        swimAttack.swimming = true;
        swimAttack.directionIndicator.SetActive(false);
        // sink
        while (transform.position.y > swimAttack.swimYOffset)
        {
            transform.position += -Vector3.up * swimAttack.swimSpeed * Time.deltaTime;
            yield return null;
        }

        // wait half
        yield return new WaitForSeconds(swimAttack.swimTime / 2);

        // teleport
        curDirection = swimAttack.targetDirection;
        UpdatePosition();
        transform.position = new Vector3(transform.position.x, swimAttack.swimYOffset, transform.position.z);

        // wait other half
        yield return new WaitForSeconds(swimAttack.swimTime/2);


        // swim up
        while (transform.position.y < swimAttack.defaultYOffset)
        {
            transform.position += Vector3.up * swimAttack.swimSpeed * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        swimAttack.swimming = false;
        currentAttack = Attacks.None;
        swimAttack.Invoke();
    }
    #endregion

    public void UpdatePosition()
    {
        SceneManager.BossPosition bossPosition = SceneManager.instance.GetBossPosition(curDirection);
        transform.position = bossPosition.holder.position;
        transform.rotation = bossPosition.holder.rotation;
    }
}
