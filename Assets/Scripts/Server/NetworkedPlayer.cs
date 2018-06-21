using UnityEngine;

public class NetworkedPlayer : Photon.MonoBehaviour, IPunObservable
{
    public static GameObject LocalPlayerInstance;
    public GameObject BulletPrefab;

    private GameObject _mainCamera;
    private GameObject _player;
    private GameObject _head;
    private Vector3 _bodyPosition = Vector3.zero; // We lerp towards this
    private Quaternion _bodyRotation = Quaternion.identity; // We lerp towards this

    private Vector3 _headPosition = Vector3.zero;
    private Quaternion _headRotation = Quaternion.identity;
    private Rigidbody _parentRb;
    private Vector3 _remoteVelocity;
    private Vector3 _bulletOrigin;
    private float _lag;
    private NetworkedActions _networkActions;
    private PhotonView _view;


    void Awake()
    {
        _networkActions = GameObject.Find("MatchManager").GetComponent<NetworkedActions>();

        if (photonView.isMine)
            LocalPlayerInstance = gameObject;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _player = GameObject.Find("Player");
        _head = GameObject.Find("Head");
        _mainCamera = GameObject.Find("MainCamera");

        _view = GetComponent<PhotonView>();
        _networkActions = GetComponent<NetworkedActions>();

        transform.parent = _player.transform;
        _parentRb = transform.parent.GetComponent<Rigidbody>();

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        _head.transform.position = _mainCamera.transform.position;
        _head.transform.rotation = _mainCamera.transform.rotation;
    }

    void Update()
    {
        // Remote Updates
        if (!photonView.isMine)
        {
            UpdateRemotePlayers();
        }
        // Local Updates
        else
        {
            UpdateLocalPlayer();
            ProcessInputs();
        }
    }

    void UpdateLocalPlayer()
    {
        // If this is our photonView, then lock the _head to the camera rotation 
        //  and position without making the _head a child of the camera
        _head.transform.localRotation = _mainCamera.transform.localRotation;
        _head.transform.position = _mainCamera.transform.position;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    void UpdateRemotePlayers()
    {
        // Update other player's position and rotation to match last grabbed value
        transform.position = Vector3.Lerp(transform.position, this._bodyPosition, Time.deltaTime * 10);
        transform.rotation = Quaternion.Lerp(transform.rotation, this._bodyRotation, Time.deltaTime * 10);

        // Same for the player's _head
        _head.transform.localRotation = Quaternion.Lerp(_head.transform.localRotation, this._headRotation, Time.deltaTime * 10);
    }

    void ProcessInputs()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _bulletOrigin = new Vector3(_head.transform.position.x, _head.transform.position.y, _head.transform.position.z + 0.3f);
            _networkActions.FireBullet(BulletPrefab.name, _bulletOrigin, _head.transform.forward);
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // We own this player: send the others our data
        if (stream.isWriting)
        {
            // Send our position and rotation
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

            // Send our _head's rotation
            stream.SendNext(_head.transform.localRotation);
        }
        // Network player, receive data
        else
        {
            // Lag compensation
            float _lag = Mathf.Abs((float)(PhotonNetwork.time - info.timestamp));

            // Receive other players position and rotation
            this._bodyPosition = (Vector3)stream.ReceiveNext();
            this._bodyRotation = (Quaternion)stream.ReceiveNext();

            // Receive other player's head rotation
            this._headRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}