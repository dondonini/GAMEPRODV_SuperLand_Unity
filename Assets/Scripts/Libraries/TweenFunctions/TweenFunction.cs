using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenFunction : MonoBehaviour{

    public static TweenFunction instance;

    // Activates the task pool
    private bool isActive;

    public class TweenTask
    {
        public TweenData tweenData;
        public float elaspedTime;
    }

    // Task pool
    private List<TweenTask> taskPool;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public TweenFunction()
    {
        // Setting default settings
        isActive = false;
        taskPool = new List<TweenTask>();
    }
	
	// Update is called once per frame
	void Update () 
    {
        Debug.Log(isActive);

		if (isActive)
        {
            Debug.Log("eh");
            UpdateTasks();

            // Stops the "engine" when there are no more tasks to do.
            if (taskPool.Count == 0)
            {
                isActive = false;
            }
        }
	}

    void UpdateTasks()
    {
        /****************************************************************
         * Task Loop                                                    *
         ****************************************************************/

        for (int t = 0; t < taskPool.Count; t++)
        {
            TweenTask task = taskPool[t];

            // Calculate progress
            float progress = Mathf.Clamp01(task.elaspedTime / task.tweenData.duration);
            Debug.Log(progress);

            // Calculate eased value from 0.0f to 1.0f
            float easedValue = EasingFunction.GetEasingFunction(task.tweenData.func)
                /*EasingFunction*/(0.0f, 1.0f, progress);

            // Clamps eased value if asked for (IDK why you would need this, but it's there for people to enjoy!)
            if (task.tweenData.isClamped)
            {
                easedValue = Mathf.Clamp01(easedValue);
            }

            /****************************************************************
             * Modification Section                                         *
             ****************************************************************/

            // Modding position
            if (task.tweenData.modifyPosition)
            { 
                // Calculate actor lerped position via. ease
                Vector3 newPosition = Vector3.LerpUnclamped(
                    task.tweenData.startPosition, 
                    task.tweenData.endPosition, 
                    easedValue
                    );

                // Apply new position to actor
                task.tweenData.actor.position = newPosition;
            }

            // Modding angle
            if (task.tweenData.modifyAngle)
            {
                // Calculate actor lerped angle via. ease
                Quaternion newAngle = Quaternion.LerpUnclamped(task.tweenData.startRotation, 
                    task.tweenData.endRotation, 
                    easedValue
                    );

                // Apply new angle to actor
                task.tweenData.actor.rotation = newAngle;
            }

            // Modding Scale
            if (task.tweenData.modifyAngle)
            {
                // Calculate actor lerped scale via. ease
                Vector3 newScale = Vector3.LerpUnclamped(
                    task.tweenData.startScale, 
                    task.tweenData.endScale, 
                    easedValue
                    );

                // Apply new scale to actor
                task.tweenData.actor.localScale = newScale;
            }

            task.elaspedTime += Time.deltaTime;

            // Removes task when it's finished
            if (task.elaspedTime >= task.tweenData.duration)
            {
                taskPool.Remove(task);
            }
        }
    }

    /// <summary>
    /// Literally starting the engine of the task pool
    /// </summary>
    public void StartTasks()
    {
        isActive = true;
    }

    /************************************************************************/
    /* Task Builder                                                         */
    /************************************************************************/

    /// <summary>
    /// Tweens actor position from one position to another.
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="newPos"></param>
    /// <param name="easeFunc"></param>
    /// <param name="duration"></param>
    /// <param name="notOverrideExistingTask"></param>
    public void TweenPosition(Transform newActor, Vector3 newPos, EasingFunction.Ease easeFunc, float newDuration, bool notOverrideExistingTask = default(bool))
    {
        // Merging parameters into TweenData set
        TweenData newTweenData = new TweenData
        {
            actor = newActor,
            modifyPosition = true,
            func = easeFunc,
            duration = newDuration
        };

        newTweenData.startPosition = newActor.position;
        newTweenData.endPosition = newPos;

        // Passing TweenData into main AddTask function
        AddTask(newTweenData, notOverrideExistingTask);
    }

    /// <summary>
    /// Tweens actor angle from one angle to another.
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="newAngle"></param>
    /// <param name="easeFunc"></param>
    /// <param name="duration"></param>
    /// <param name="notOverrideExistingTask"></param>
    public void TweenAngle(Transform newActor, Quaternion newAngle, EasingFunction.Ease easeFunc, float newDuration, bool notOverrideExistingTask = default(bool))
    {
        // Merging parameters into TweenData set
        TweenData newTweenData = new TweenData
        {
            actor = newActor,
            modifyAngle = true,
            func = easeFunc,
            duration = newDuration
        };

        

        Transform startT = newActor;
        Transform endT = startT;
        endT.rotation = newAngle;

        newTweenData.startRotation = newActor.rotation;
        newTweenData.endRotation = newAngle;

        // Passing TweenData into main AddTask function
        AddTask(newTweenData, notOverrideExistingTask);
    }

    /// <summary>
    /// Tweens actor scale from one size to another.
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="newScale"></param>
    /// <param name="easeFunc"></param>
    /// <param name="duration"></param>
    /// <param name="notOverrideExistingTask"></param>
    public void TweenScale(Transform newActor, Vector3 newScale, EasingFunction.Ease easeFunc, float newDuration, bool notOverrideExistingTask = default(bool))
    {
        // Merging parameters into TweenData set
        TweenData newTweenData = new TweenData
        {
            actor = newActor,
            modifyScale = true,
            func = easeFunc,
            duration = newDuration
        };

        Transform startT = newActor;
        Transform endT = startT;
        endT.localScale = newScale;

        newTweenData.startScale = newActor.localScale;
        newTweenData.endScale = newScale;

        // Passing TweenData into main AddTask function
        AddTask(newTweenData, notOverrideExistingTask);
    }

    /// <summary>
    /// Adds a task to the task pool
    /// </summary>
    /// <param name="data"></param>
    public void AddTask(TweenData data, bool notOverrideExistingTask = default(bool))
    {
        // Remove existing data if applicable
        if (!notOverrideExistingTask)
        {
            RemoveExisitingData(data);
        }

        // Fixing duration
        if (data.duration < 0.0f)
        {
            data.duration = 0.0f;
        }

        // Fixing mod
        if (!data.modifyPosition && !data.modifyAngle && !data.modifyScale)
        {
            Debug.LogWarning("Task on: " + data.actor + " | No modification was tasked. Defaulting to modifying position.");
            data.modifyPosition = true;
        }

        // Build task
        TweenTask newTask = new TweenTask
        {
            tweenData = data,
            elaspedTime = 0.0f,
        };

        taskPool.Add(newTask);
        Debug.Log("New tasked added for " + data.actor);
        StartTasks();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    void RemoveExisitingData(TweenData data)
    {
        for (int t = 0; t < taskPool.Count; t++)
        {
            TweenTask task = taskPool[t];

            if (data.actor == task.tweenData.actor)
            {
                taskPool.Remove(task);
            }
        }
    }
}
