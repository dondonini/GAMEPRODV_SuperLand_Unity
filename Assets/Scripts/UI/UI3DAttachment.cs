using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class UI3DAttachment : MonoBehaviour
{
    [SerializeField][Tooltip("Adjust this if the element is getting clipped off the screen")]
    float depthPosition = 0.0f;
    
    [SerializeField]
    Transform model;

    [SerializeField]
    Transform ui3DSpace;

    [HideInInspector]
    public RectTransform rectTransform;

    Rect screenRect;

    // Start is called before the first frame update
    void Start()
    {
        screenRect = new Rect(0,0, Screen.width, Screen.height);

        // Refer RectTransform
        rectTransform = GetComponent<RectTransform>();

        // Remove placeholder image
        if (Application.isPlaying)
            Destroy(GetComponent<Image>());
    }

    private void OnDrawGizmosSelected()
    {
        // Rect on screen
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(rectTransform.position, new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.1f));

    }

    public void Attach3DUIObject()
    {
        // Checks if model exists
        if (!model)
        {
            // Grab the first child and use it as the model
            if (transform.childCount > 0)
            {
                // Set model
                model = transform.GetChild(0);
            }
        }

        // Move model into 3D UI space
        if (model.parent != ui3DSpace)
            model.SetParent(ui3DSpace);
    }

    // Update is called once per frame
    void Update()
    {
        // Allocate screen rect
        screenRect = new Rect(0,0, Screen.width, Screen.height);

        if (rectTransform.rect.Overlaps(screenRect))
        {
            model.GetComponent<MeshRenderer>().enabled = true;

            Set3DUIPosition(rectTransform.position);

            float xScale = rectTransform.rect.width / 100.0f;
            float yScale = rectTransform.rect.height / 100.0f;
            float scaleAmount = Mathf.Min(xScale, yScale);

            Set3DUIScale(scaleAmount);
        }
        else
        {
            model.GetComponent<MeshRenderer>().enabled = false;
        }

        
    }

    public void Set3DUIPosition(Vector2 _pos)
    {
        model.localPosition = ScreenPointTo3DScreenPosition(_pos);
    }

    public void Set3DUIScale(float _scale)
    {
        model.localScale = model.localScale.normalized * _scale;
    }

    Vector3 ScreenPointTo3DScreenPosition(Vector2 _screenPoint)
    {
        // Calculate X position percentage relative to the left of the screen
        float xPer = _screenPoint.x / screenRect.width;

        // Calculate Y position percentage relative to the top of the screen
        float yPer = _screenPoint.y / screenRect.height;

        float xPos = Mathf.LerpUnclamped(0.0f, Get3DUISize().x, xPer) - (Get3DUISize().x * 0.5f);
        float yPos = Mathf.LerpUnclamped(0.0f, Get3DUISize().y, yPer) - (Get3DUISize().y * 0.5f);

        return new Vector3(xPos, yPos, depthPosition);
    }

    Vector2 Get3DUISize()
    {
        BoxCollider area = ui3DSpace.GetComponent<BoxCollider>();

        return new Vector2(area.size.x, area.size.y);
    }
}
