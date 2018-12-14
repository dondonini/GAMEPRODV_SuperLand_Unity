using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Renderer))]
public class LevelButton : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField]
    int minStars = 1;
    [SerializeField]
    string targetScene;

    [Header("Animation Settings")]
    [SerializeField]
    float animationDuration = 1.0f;
    [SerializeField]
    EasingFunction.Ease easeFunction = EasingFunction.Ease.EaseInOutQuad;

    /************************************************************************/
    /* References                                                           */
    /************************************************************************/

    public Material normalMaterial;
    public Material highlightedMaterial;
    public Material pressedMaterial;
    public Material disabledMaterial;
    Renderer render;

    /************************************************************************/
    /* Runtime Variables                                                    */
    /************************************************************************/

    Coroutine currentTransition;
    Material currentMaterial;
    Material previousMaterial;
    bool isActive = false;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint cp in collision.contacts)
        {
            Collider other = cp.otherCollider;

            if (other.CompareTag("Player"))
            {
                GoToLevel();
                return;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get renderer
        render = GetComponent<Renderer>();

        // Set starter material
        currentMaterial = disabledMaterial;
        previousMaterial = currentMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMaterial != previousMaterial)
        {
            StopCoroutine(currentTransition);
            currentTransition = null;
            currentTransition = StartCoroutine(AnimateMaterials(currentMaterial));
        }
            
    }

    void GoToLevel()
    {
        if (isActive) return;

        isActive = true;

        SceneManager.LoadScene(targetScene);
    }

    IEnumerator AnimateMaterials(Material _newMaterial)
    {
        currentMaterial = _newMaterial;

        for (float elaspedTime = 0.0f; elaspedTime < animationDuration; elaspedTime += Time.deltaTime)
        {
            float progress = elaspedTime / animationDuration;

            float easeExpression = EasingFunction.GetEasingFunction(easeFunction)(0.0f, 1.0f, progress);

            render.material.Lerp(previousMaterial, currentMaterial, easeExpression);

            yield return null;
        }

        // Set material to final
        render.material = _newMaterial;
        previousMaterial = currentMaterial;
    }
}
