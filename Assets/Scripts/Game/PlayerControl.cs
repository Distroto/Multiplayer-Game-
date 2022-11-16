using Unity.Netcode;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    [SerializeField]
    private float speed = 3.5f;

    [SerializeField]
    private Vector2 defaultPositionRange = new Vector2(-4,4);

    [SerializeField]
    private NetworkVariable<float> forwardBackPosition = new NetworkVariable<float>();  

    [SerializeField]
    private NetworkVariable<float> leftRightPosition = new NetworkVariable<float>();    

    //Client Caching

    private float oldForwardBackPosition;
    private float oldLeftRightPosition;

    private void Start() 
    {
        transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 1, Random.Range(defaultPositionRange.x, defaultPositionRange.y)); 
    }

    private void Upadte()
    {
        if(IsServer)
        {
            UpdateServer();
        }

        if(IsClient && IsOwner)
        {
            UpdateClient();            
        }
    }

    private void UpdateServer()
    {
        transform.position = new Vector3(transform.position.x + leftRightPosition.Value, 0,transform.position.z + forwardBackPosition.Value);
    }

    private void UpdateClient()
    {
        float forwardBackward = 0;
        float leftRight = 0;

        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            forwardBackward += speed;
        }
        else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            forwardBackward -= speed;
        }
        else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftRight -= speed;
        }
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            leftRight += speed;
        }

        if(oldForwardBackPosition != forwardBackward || oldLeftRightPosition != leftRight)
        {
            oldForwardBackPosition = forwardBackward;
            oldLeftRightPosition = leftRight;

            //Update the Server
            UpdateClientPositionServerRpc(forwardBackward, leftRight);
        }
    }

    [ServerRpc]
    public void UpdateClientPositionServerRpc(float forwardBackward, float leftRight)
    {
        forwardBackPosition.Value = forwardBackward;
        leftRightPosition.Value = leftRight;
    }

    
}
