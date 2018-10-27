using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public Sprite icon;
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
        public Transform indicator;
        public bool airstriking;
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

    public void PerformAirstrike()
    {
        airstrikeAttack.Invoke();
        currentAttack = Attacks.Airstrike;
        airstrikeAttack.indicator.transform.position = SceneManager.instance.center.position;
    }

    void AirstrikeUpdate()
    {
        float inputX = (InputManager.instance.player1.GetAxis("XLook"));
        float inputY = (InputManager.instance.player1.GetAxis("YLook"));
        airstrikeAttack.indicator.transform.position += new Vector3(inputX, 0, inputY) * Time.deltaTime * airstrikeAttack.moveSpeed;

        if (airstrikeAttack.airstriking)
        {
            AirstrikeHappening();
            return;
        }

        airstrikeAttack.indicator.gameObject.SetActive(true);

        if (InputManager.instance.player1.GetButtonDown("Select"))
        {
            airstrikeAttack.airstriking = true;
        }
        if(InputManager.instance.player1.GetButtonDown("Cancel"))
        {
            airstrikeAttack.airstriking = false;
            airstrikeAttack.indicator.gameObject.SetActive(false);
            currentAttack = Attacks.None;
        }
    }

    void AirstrikeHappening()
    {

    }

    #region Swimming
    public void PerformSwim()
    {
        swimAttack.Invoke();
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

        if (inputX <= 0.5f)
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

        if(inputX == 0)
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
    }
    #endregion

    public void UpdatePosition()
    {
        SceneManager.BossPosition bossPosition = SceneManager.instance.GetBossPosition(curDirection);
        transform.position = bossPosition.holder.position;
        transform.rotation = bossPosition.holder.rotation;
    }
}
