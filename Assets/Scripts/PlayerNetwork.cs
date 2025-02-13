using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor.VersionControl;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;

    //private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<MyCustomData> randomCustomData = new NetworkVariable<MyCustomData>(new MyCustomData
    {
        _int = 56,
        _bool = true,
        message = " "
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        /*randomNumber.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log(OwnerClientId + "; randomnumber : " + randomNumber.Value);
        };*/

        randomCustomData.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; randomCustomData : " + newValue._int + " ;" + newValue._bool + " ; " + newValue.message);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            spawnedObjectTransform =  Instantiate(spawnedObjectPrefab);
            //spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);

            //TestServerRpc(new ServerRpcParams());
            /*TestClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { 1 }
                }
            });*/

            /*randomCustomData.Value = new MyCustomData
            {
                _int = 10,
                _bool = true,
                message = "All your value"
            };*/
        }

        Vector3 moveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) moveDir.z = 1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = 1f;

        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    [ServerRpc]
    private void TestServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log("Test Server RPC : " + OwnerClientId + " ; " + serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]    
    private void TestClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log("TEST CLIENT RPC");
    }    

    
}
