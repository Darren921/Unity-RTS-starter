using UnityEngine;

public class Commander : MonoBehaviour
{

    [SerializeField] private Monster[] controlledMonsters;
    
    private float _rotationDirection;
    private Vector3 _moveDirection;
    // Start is called before the first frame update
    void Start()
    {
        InputManager.Initialize(this);
    }

    private void Update()
    {
        Transform myTransCached  = transform;
        myTransCached.Rotate(Vector3.up,Time.deltaTime *   Settings.Instance.MouseRotateSens * _rotationDirection );
        myTransCached.position += transform.rotation * (Time.deltaTime * Settings.Instance.MouseMoveSense * _moveDirection);
    }
     public void Marked(Ray camToWorldRay)
    {
        LayerMask enemies = LayerMask.GetMask("Enemy");
        RaycastHit2D raycastHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, enemies);
        Debug.DrawRay(camToWorldRay.origin, camToWorldRay.direction * 100, Color.black, 10);
        if (Physics.Raycast(camToWorldRay, out RaycastHit hit, 100, enemies))
        {
            Debug.Log("Enemy Marked " + hit.transform.name);
        }
        else
        {
            Debug.Log("No target located" + hit.transform.name);
        }
    }
    public void Attack(Ray camToWorldRay)
    {
        Debug.DrawRay(camToWorldRay.origin, camToWorldRay.direction * 100, Color.red,2);

        if (!Physics.Raycast(camToWorldRay, out RaycastHit hit, 100, StaticUtilities.AttackLayerMask))
            return; 

        foreach (Monster monster in controlledMonsters)
        {
            monster.TryAttack(hit);
        }
    }

    public void MoveTo(Ray camToWorldRay)
    {
        Debug.DrawRay(camToWorldRay.origin, camToWorldRay.direction* 100, Color.blue,2);

        if (!Physics.Raycast(camToWorldRay, out RaycastHit hit, 100, StaticUtilities.MoveLayerMask))
            return;

        foreach (Monster monster in controlledMonsters)
        {
            monster.MoveToTarget(hit.point);
        }
        
    }
    
    
    public void SetRotationDirection(float direction)
    {
        _rotationDirection = direction;
    }

    public void SetMoveDirection(Vector2 direction)
    {
        _moveDirection.x = direction.x;
        _moveDirection.z = direction.y;
    }
}
