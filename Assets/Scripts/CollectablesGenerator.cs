using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesGenerator : MonoBehaviour
{
    #region Constants

    /* max food constant */
    private const int FOOD_MAX = 5;
    /* seconds constants */
    private const float SECOND_THIRTY = 30f;

    #endregion


    #region Global attributes

    /* is food eaten or not */
    public bool isEaten;
    /* is regen taken or not */
    public bool isRegen;

    /* food prefab for spawning food */
    public GameObject foodPrefab, regenPrefab;
    /* reference to MovementSnake.cs script */
    private MovementSnake script_movementSnake;

    /* maximum distance for collectable to spawn */
    private float borderArea;
    /* maximum food to be spawned */
    private int maxFood;
    /* timer for regeneration object countdown */
    private float timerRegen;

    #endregion


    #region Unity engine methods

    private void Awake() {
        // get MovementSnake.cs script
        script_movementSnake = GameObject.Find("/Snake/HeadPref").GetComponent<MovementSnake>();
    }

    void Start() {
        // init base variables
        maxFood = FOOD_MAX;
        timerRegen = 0f;
        isEaten = false;
        isRegen = true;

        // get maximum distance of food from beginning
        // = length of one component of vector of movement sphere
        borderArea = (script_movementSnake.borderArea - 1f) / Mathf.Sqrt(3f);
        // generate initial food
        for (int i = 0; i < maxFood; i++) {
            addCollectable(foodPrefab);
        }
    }

    void Update() {
        // if food have been eaten, generate new food
        if (isEaten) {
            // add new food
            addCollectable(foodPrefab);
            // set state to not-eaten-food
            isEaten = false;
        }

        // if regen have been taken, generate new regen after 1 minute
        if (isRegen) {
            // countdown for next regeneration object
            if (timerRegen < SECOND_THIRTY) {
                timerRegen += Time.fixedDeltaTime;
            }
            else {
                // reset timer
                timerRegen = 0f;
                // add new regeneration object
                addCollectable(regenPrefab);
                // set state to not-taken-regen
                isRegen = false;
            }
        }
    }

    #endregion


    #region Collectables methods

    private void addCollectable(GameObject collectPref) {
        // vector for random position of collectable
        Vector3 randomPos = getRandomPos();

        // instantiate new collectable on random position
        Instantiate(collectPref, randomPos, Quaternion.identity)
            // set parent to it = arrange new collectable in project hierarchy
            .transform.SetParent(transform);
    }

    private Vector3 getRandomPos() {
        // make random 3D position
        Vector3 randomPos = new Vector3(
            Random.Range(-borderArea, borderArea),
            Random.Range(-borderArea, borderArea),
            Random.Range(-borderArea, borderArea)
        );

        return randomPos;
    }

    #endregion
}
