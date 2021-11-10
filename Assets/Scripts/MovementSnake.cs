using System;
using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class MovementSnake : MonoBehaviour
{
    #region Constants

    /* snake size constants */
    private const int SNAKE_SIZE = 5;
    private const int SNAKE_GROW = 5;
    /* amout of heart when snake is dead */
    private const int SNAKE_DEAD = -1;
    /* snake movement constants */
    private const float MOVEMENT_SLOW = 0.1f;
    private const float MOVEMENT_FAST = 2f;
    private const float MOVEMENT_NORMALIZER = 40f;
    /* snake rotation constants */
    private const float ROTATION_SLOW = 30f;
    private const float ROTATION_FAST = 60f;
    /* area for snake to move in */
    private const float AREA_SIZE = 25f;
    /* base score amount */
    private const int SCORE_AMOUNT = 100;
    /* amount of particles to emit */
    private const int PARTICLE_AMOUT = 5;
    /* mindwave value constants */
    private const int MINIMAL_ATTENTION = 30;
    private const int MINIMAL_MEDITATION = 250;
    /* seconds constants */
    private const float SECOND_TWO_TENTHS = 0.2f;
    private const float SECOND_HALF = 0.5f;
    private const float SECOND_ONE = 1f;
    private const float SECOND_FIVE = 5f;
    /* gameObject tags */
    private const string TAG_FOOD = "Food";
    private const string TAG_REGENERATION = "Regeneration";
    private const string TAG_BODY = "Body";

    #endregion


    #region Global attributes

    /* bodypart to instantiate */
    public GameObject bodyPrefab;
    /* list of snake bodyparts */
    public List<Transform> bodyParts = new List<Transform>();
    /* transform used for faster accessing head */
    private Transform bodyPartHead;

    /* movement speed */
    private float speedMovement;
    /* rotation speed */
    private float speedRotation;

    /* timer for HP remove*/
    private float timerHPremove;
    /* timer for countdown of growing */
    private float timerGrow;
    /* timer for meditation */
    private float timerMed;

    /* can heart be removed or not */
    private bool isHPtoRemove;
    /* is snake alive or not */
    private bool isAlive;
    /* is snake growing or not */
    private bool isGrowing;
    /* is snake meditating or not */
    private bool isMeditating;
    /* value of achieved meditation */
    private int meditationValue;
    /* is game is puased or not */
    public bool isPaused;

    /* minimal snake size */
    private int sizeSnake;
    /* counting how many bodyparts remain to grow */
    private int counterSizeGrown;

    /* border of area for snake to move in */
    public float borderArea;

    /* actual width and height of window */
    private int resolutionHeight, resolutionWidth;
    /* borders for eyetracking   */
    private int borderUp, borderDown, borderLeft, borderRight;

    /* particle system used for particles to emit while on border */
    private ParticleSystem particle;
    /* particle system main used for change color of particles */
    private ParticleSystem.MainModule particleMain;

    /* reference to FoodGenerator.cs script */
    private CollectablesGenerator script_collectablesGenerator;
    /* reference to GameSceneScript.cs script */
    private GameSceneScript script_gameSceneScript;

    /* if Mindwave will be used or not */
    public bool useMindwave;

    #endregion


    #region Unity engine methods

    private void Awake() {
        // get FoodGenerator.cs script
        script_collectablesGenerator = GameObject.Find("/Collectables").GetComponent<CollectablesGenerator>();
        // get GameSceneScript.cs script
        script_gameSceneScript = GameObject.Find("/Canvas").gameObject.GetComponent<GameSceneScript>();

        // get particle system
        particle = GetComponent<ParticleSystem>();
        // get particle system main
        particleMain = particle.main;

        // get Mindwave usage bool value
        if(LoaderManager.instance.getUseMindwave() != null)
        {
            useMindwave = (bool) LoaderManager.instance.getUseMindwave();
        }
        else //not loaded - default
        {
            useMindwave = true;
        }

        if (useMindwave) {
            // if Mindwave will be used, add OnUpdateMindwaveData method to Mindwave Controller,
            // in order to be able to recived Mindwave data values
            MindwaveManager.Instance.Controller.OnUpdateMindwaveData += OnUpdateMindwaveData;
            MindwaveManager.Instance.Controller.OnConnectMindwave    += OnConnectMindwave;
            MindwaveManager.Instance.Controller.OnDisconnectMindwave += OnDisconnectMindwave;
            MindwaveManager.Instance.Controller.OnConnectionTimeout  += OnConnectionTimeout;
        }
    }

    private void Start() {
        // default values
        initDefaults();

        // add initial parts of body
        for (int i = 0; i < sizeSnake; i++) {
            SnakeAddBodyPart();
        }

        // set borders for eyertracker according to resolution
        SetBorders();

        // if Mindwave will be used and is not connected -- connect
        if (useMindwave && !MindwaveManager.Instance.Controller.IsConnected) {
            speedMovement = 0f;
            script_gameSceneScript.ShowReconnectWindow(true);
            MindwaveManager.Instance.Controller.Connect();
        }
    }

    private void Update() {
        // if window size has been changed
        if (!(resolutionWidth.Equals(Screen.height) && resolutionHeight.Equals(Screen.width))) {
            SetBorders();
        }

        // pause game by key
        if (Input.GetKeyDown(KeyCode.Escape)) {
            script_gameSceneScript.PauseBtnPressed();
        }
    }

    private void FixedUpdate() {
        // if game is paused
        if (isPaused) {
            return;
        }
        // is snake is alive play
        else if (isAlive) {
            SnakeControll();
        }
        // else game over
        else {
            script_gameSceneScript.ShowGameOverWindow();
        }
    }

    #endregion


    #region Initialization

    private void initDefaults() {
        // save head for better access
        bodyPartHead = bodyParts[0];

        // init base variables
        speedMovement = MOVEMENT_FAST;
        speedRotation = ROTATION_FAST;
        timerHPremove = 0f;
        timerGrow = 0f;
        timerMed = 0f;
        isHPtoRemove = true;
        isAlive = true;
        isGrowing = false;
        isMeditating = false;
        isPaused = false;
        meditationValue = 0;
        sizeSnake = SNAKE_SIZE;
        counterSizeGrown = SNAKE_GROW;
        borderArea = AREA_SIZE;
    }

    private void SetBorders() {
        // get resolution of game window
        resolutionHeight = Screen.width;
        resolutionWidth = Screen.height;

        // borders on screen
        borderUp    = (int)(resolutionWidth * (3f / 5f));
        borderDown  = (int)(resolutionWidth * (1f / 5f));
        borderLeft  = (int)(resolutionHeight * (2f / 5f));
        borderRight = (int)(resolutionHeight * (3f / 5f));
    }

    #endregion


    #region Snake methods

    private void SnakeControll() {
        float speedCurrent = speedMovement;

        // if heart was removed, start countdown for next heart removal
        if (!isHPtoRemove) {
            // handle time for bodyparts destroying
            if (timerHPremove > SECOND_ONE) {
                isHPtoRemove = true;
                timerHPremove = 0f;
            }
            else {
                timerHPremove += Time.fixedDeltaTime;
            }
        }

        // snake cant go furter than border
        if (bodyPartHead.position.magnitude > borderArea) {
            // set speed to 0, in order not to get stuck
            speedCurrent = 0f;
            SnakeRestrict();
        }

        // snake is meditating for HP
        if (isMeditating) {
            // slow snake while meditating
            speedCurrent = MOVEMENT_SLOW;
            SnakeMeditate();
        }

        // snake ate food and is growing
        if (isGrowing) {
            SnakeGrow();
        }

        // used in DEVELOPMENT vvv
        // increase speed of snake
        //if (Input.GetKey(KeyCode.X)) speedCurrent *= 5;
        // add body part on keyboard signal
        //if (Input.GetKey(KeyCode.C)) SnakeAddBodyPart();
        // used in DEVELOPMENT ^^^

        // rotate snake with eyetracker
        if (TobiiAPI.IsConnected)
            SnakeRotateTobii();
        // rotate snake with keyboard
        SnakeRotateKeyboard();

        // move snake head forward
        bodyPartHead.Translate(Vector3.forward * speedCurrent * Time.fixedDeltaTime);
        // move whole snake forward
        SnakeMoveWholeBody(ref speedCurrent);
    }

    private void SnakeRestrict() {
        // emit stop particle
        particleMain.startColor = Color.yellow;
        particle.Emit(1);

        // rotate head and push snake
        bodyPartHead.Rotate(Vector3.up * speedRotation * Time.fixedDeltaTime);
        bodyPartHead.GetComponent<Rigidbody>().AddForce(
            -bodyPartHead.position.normalized, ForceMode.Impulse);
    }

    private void SnakeMeditate() {
        // is meditation successful of not
        // meditation value has to be over 'MINIMAL_MEDITATION' in 'SECOND_FIVE' seconds
        bool meditationSuccess = meditationValue > MINIMAL_MEDITATION;

        // emit meditating particle (color set in CollideWithRegen(...) method)
        particle.Emit(1);

        // if using Mindwave cumulate meditation value for 'SECOND_FIVE' seconds
        if (useMindwave) {
            // player has 'SECOND_FIVE' seconds to achieve meditation value of 'MINIMAL_MEDITATION'
            if (timerMed < SECOND_FIVE) {
                timerMed += Time.fixedDeltaTime;
            }
            // else state is changed back to normal playing without adding HP
            else {
                isMeditating = false;
                meditationValue = 0;
                timerMed = 0f;
            }
        }
        // else set variables to pick regeneration instantly
        else {
            meditationSuccess = true;
            isMeditating = false;
        }

        // if meditation was successful
        if (meditationSuccess) {
            // add heart
            script_gameSceneScript.addHeart();
            // increase score
            script_gameSceneScript.addToScore(SCORE_AMOUNT / 2);

            // set state to normal playing
            isMeditating = false;
            meditationValue = 0;
            timerMed = 0f;
        }
    }

    private void SnakeGrow() {
        // grow 'sizeGrown' bodyparts after 0.2 second
        if (timerGrow > SECOND_TWO_TENTHS) {
            SnakeAddBodyPart();
            counterSizeGrown -= 1;
            timerGrow = 0f;
            
            // 'sizeGrown' bodyparts have grown - stop growing
            if (counterSizeGrown == 0) {
                counterSizeGrown = sizeSnake;
                isGrowing = false;
            }
        }
        else {
            timerGrow += Time.fixedDeltaTime;
        }
    }

    private void SnakeRotateTobii() {
        // get where player is looking
        Vector2 gazePoint = TobiiAPI.GetGazePoint().Screen;
        float gazeX = gazePoint.x;
        float gazeY = gazePoint.y;

        // up
        if (gazeY < borderDown) {
            bodyPartHead.Rotate(Vector3.right * speedRotation * Time.fixedDeltaTime);
        }
        // down
        else if (gazeY > borderUp) {
            bodyPartHead.Rotate(Vector3.left * speedRotation * Time.fixedDeltaTime);
        }

        // left
        if (gazeX < borderLeft) {
            bodyPartHead.Rotate(Vector3.down * speedRotation * Time.fixedDeltaTime);
        }
        // right
        else if (gazeX > borderRight) {
            bodyPartHead.Rotate(Vector3.up * speedRotation * Time.fixedDeltaTime);
        }
    }

    private void SnakeRotateKeyboard() {
        // go left or right with snake
        if (Input.GetAxis("Horizontal") != 0)
            bodyPartHead.Rotate(Vector3.up * speedRotation * Input.GetAxis("Horizontal") * Time.fixedDeltaTime);
        
        // go up or down with snake
        if (Input.GetAxis("Vertical") != 0)
            bodyPartHead.Rotate(Vector3.right * speedRotation * Input.GetAxis("Vertical") * Time.fixedDeltaTime);
    }

    private void SnakeMoveWholeBody(ref float speedCurrent) {
        // current and previous bodypart while moving them
        Transform bodyPartCurrent, bodyPartPrevious;
        // rigidbody component for better access
        Rigidbody rigidbody;
        // distance of bodyparts and delta for interpolation
        float distance, deltaT;

        // set head velocity to zero in order to keep snake in moving way we want
        bodyPartHead.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // move whole snake
        for (int i = 1; i < bodyParts.Count; i++) {
            bodyPartCurrent = bodyParts[i];
            bodyPartPrevious = bodyParts[i - 1];
            rigidbody = bodyPartCurrent.GetComponent<Rigidbody>();

            // distance between two actual bodyparts
            distance = Vector3.Distance(bodyPartPrevious.position, bodyPartCurrent.position);
            
            // time required to move bodypart to new position
            deltaT = distance * speedCurrent * Time.deltaTime;
            
            // handle time difference for correct snake movement
            if (deltaT > SECOND_TWO_TENTHS)
                deltaT = SECOND_TWO_TENTHS;

            // move actual bodypart according to one before it
            rigidbody.MovePosition(Vector3.Slerp(bodyPartCurrent.position, bodyPartPrevious.position, deltaT));
            rigidbody.MoveRotation(Quaternion.Slerp(bodyPartCurrent.rotation, bodyPartPrevious.rotation, deltaT));
            
            // set actual bodypart velocity to zero
            rigidbody.velocity = Vector3.zero;
        }
    }

    private void SnakeAddBodyPart() {
        // last bodypart of snake
        Transform lastPiece = bodyParts[bodyParts.Count - 1];

        // instantiate new bodypart with same position and rotation as previous one
        Transform newPart = (Instantiate(
            bodyPrefab,
            //actualPrefab, // easter egg
            lastPiece.position + lastPiece.TransformDirection(Vector3.back),
            lastPiece.rotation)
            as GameObject).transform;

        // set parent of new bodypart
        newPart.SetParent(transform.parent);

        // add new bodypart to list
        bodyParts.Add(newPart);

        // join last piece with new part -- new tail
        lastPiece.GetComponent<ConfigurableJoint>().connectedBody = newPart.GetComponent<Rigidbody>();
    }

    public void SnakeReset() {
        // delete grown snake parts
        for (int i = bodyParts.Count - 1; i > sizeSnake; i--) {
            // destroy bodypart
            Destroy(bodyParts[i].gameObject);
            bodyParts.Remove(bodyParts[i]);
        }
        
        // move snake head to center
        bodyPartHead.position = Vector3.zero;
        bodyPartHead.rotation = Quaternion.identity;

        // revive snake
        isAlive = true;
    }

    #endregion


    #region Collision methods

    private void OnCollisionEnter(Collision coll) {
        switch (coll.gameObject.tag) {
            case TAG_FOOD:
                CollideWithFood(ref coll);
                break;
            case TAG_REGENERATION:
                CollideWithRegen(ref coll);
                break;
            case TAG_BODY:
                CollideWithBody(ref coll);
                break;
        }
    }

    private void CollideWithFood(ref Collision coll) {
        // emit food particle
        particleMain.startColor = Color.red;
        particle.Emit(PARTICLE_AMOUT);

        // eat collided food
        Destroy(coll.gameObject);
        script_collectablesGenerator.isEaten = true;
        isGrowing = true;

        // increase score
        script_gameSceneScript.addToScore(SCORE_AMOUNT);
    }

    private void CollideWithRegen(ref Collision coll) {
        // emit regeneration particle
        particleMain.startColor = Color.blue;
        particle.Emit(PARTICLE_AMOUT);

        // consume regeneration object
        Destroy(coll.gameObject);
        script_collectablesGenerator.isRegen = true;
        isMeditating = true;
    }

    private void CollideWithBody(ref Collision coll) {
        // used to find collided bodypart
        bool toDestroy = false;
        // set color of particle
        particleMain.startColor = Color.green;

        // check with which bodypart snake collided (from 'sizeSnake' bodypart included to tail)
        for (int i = sizeSnake + 1; i < bodyParts.Count; i++) {
            // check from which bodypart destroy snake
            if (coll.transform.Equals(bodyParts[i])) {
                toDestroy = true;
            }

            // collided bodypart found
            if (toDestroy) {
                // emit particle
                particle.Emit(1);
                // destroy bodypart
                Destroy(bodyParts[i].gameObject);
                bodyParts.Remove(bodyParts[i]);
            }
        }

        // if really collided with body and HP was removed at least 1 second before
        if (toDestroy && isHPtoRemove) {
            // remove HP and score
            script_gameSceneScript.removeHeart();
            
            // set state to not-remove-HP in order to wait 1 second
            isHPtoRemove = false;

            // if snake does not have HP, dies
            if (script_gameSceneScript.getAvailableHearts() == SNAKE_DEAD) {
                isAlive = false;
            }
        }
    }

    #endregion


    #region Mindwave methods

    public void OnUpdateMindwaveData(MindwaveDataModel _Data) {
        int att = _Data.eSense.attention;
        int med = _Data.eSense.meditation;

        // update sliders in GUI
        script_gameSceneScript.setBrainActivitySliderValue(att);
        script_gameSceneScript.setMeditationActivitySliderValue(med);

        // snake movement
        if (att > MINIMAL_ATTENTION) {
            // move snake according to level of attention
            speedMovement = att / MOVEMENT_NORMALIZER;
            speedRotation = ROTATION_FAST * speedMovement / 2f;
        }
        else {
            // if attention is too low, almost stop the snake
            speedMovement = MOVEMENT_SLOW;
            speedRotation = ROTATION_SLOW;
        }

        // snake meditation
        if (isMeditating) {
            meditationValue += med;
        }
    }

    public void OnConnectMindwave() {
        //Debug.Log("> Connect");

        // hide reconnecting window on Mindwave connect
        if (script_gameSceneScript != null) {
            script_gameSceneScript.ShowReconnectWindow(false);
        }
    }

    public void OnDisconnectMindwave() {
        //Debug.Log("> Disconnect");

        // show reconnecting window on Mindwave disconnect
        if (script_gameSceneScript != null) {
            script_gameSceneScript.ShowReconnectWindow(true);
        }

        // stop snake
        speedMovement = 0f;
    }

    public void OnConnectionTimeout() {
        //Debug.Log("> Timeout");

        if (script_gameSceneScript != null) {
            // on timeout -- reset sliders, reset useMindwave bool, speed of snake and disconnect
            // (works only on first connect attempt)
            script_gameSceneScript.setBrainActivitySliderValue(0);
            script_gameSceneScript.setMeditationActivitySliderValue(0);
            script_gameSceneScript.ShowReconnectWindow(false);
        }
        useMindwave = false;
        speedMovement = MOVEMENT_FAST;
        MindwaveManager.Instance.Controller.Disconnect();
    }

    #endregion

    // debug method, which draw area in scene editor where snake can move
    //private void OnDrawGizmos() { Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 2f * (borderArea / Mathf.Sqrt(3))); }
}
