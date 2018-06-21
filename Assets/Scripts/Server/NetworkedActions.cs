using UnityEngine;

public class NetworkedActions : Photon.MonoBehaviour
{
    static public NetworkedActions Instance;

    void Awake()
    {
        Instance = this;
    }

    [PunRPC]
    public void DestructionEffect(string effectName, Vector3 location, int meshOutlineId, float duration)
    {
        GameObject dieEffect = PhotonNetwork.Instantiate(effectName, location, Quaternion.identity, 0);
        dieEffect.gameObject.transform.parent = null;

        // Modify particle systems shape module
        ParticleSystem.ShapeModule shapeModule = dieEffect.GetComponent<ParticleSystem>().shape;
        shapeModule.shapeType = ParticleSystemShapeType.MeshRenderer;
        shapeModule.meshRenderer = PhotonView.Find(meshOutlineId).GetComponent<MeshRenderer>();

        // Start and destroy after 1 second
        dieEffect.GetComponent<ParticleSystem>().Play();
        Destroy(dieEffect, duration);

        if (this.photonView.isMine)
            this.photonView.RPC("DestructionEffect", PhotonTargets.OthersBuffered, effectName, location, meshOutlineId, duration);
    }

    [PunRPC]
    public void ParticleEffect(string effectName, Vector3 location, Vector3 direction, float delay)
    {
        GameObject obj = PhotonNetwork.Instantiate(effectName, location, Quaternion.LookRotation(direction), 0);

        obj.transform.SetParent(gameObject.transform);
        obj.GetComponent<ParticleSystem>().Play();

        if (delay != -1)
            Destroy(obj, delay);

        if (this.photonView.isMine)
            this.photonView.RPC("ParticleEffect", PhotonTargets.OthersBuffered, effectName, location, direction, delay);
    }

    [PunRPC]
    public void ImpactEffect(string effectName, Vector3 location, Vector3 direction, float delay)
    {
        GameObject obj = PhotonNetwork.Instantiate(effectName, location, Quaternion.LookRotation(direction), 0);
        obj.GetComponent<ParticleSystem>().Play();

        Destroy(obj, delay);

        if (this.photonView.isMine)
            this.photonView.RPC("ImpactEffect", PhotonTargets.OthersBuffered, effectName, location, direction, delay);
    }

    [PunRPC]
    public void UpdateHealth(int id, float health)
    {
        Interactable interact = PhotonView.Find(id).GetComponent<Interactable>();
        interact.SetHealth(health);

        if (this.photonView.isMine)
            this.photonView.RPC("UpdateHealth", PhotonTargets.OthersBuffered, id, health);
    }

    [PunRPC]
    public void UpdateIgnoredCollisions(int first, int second)
    {
        Physics.IgnoreCollision(PhotonView.Find(first).GetComponent<Collider>(), PhotonView.Find(second).GetComponent<Collider>());

        if (this.photonView.isMine)
            this.photonView.RPC("UpdateIgnoredCollisions", PhotonTargets.OthersBuffered, first, second);
    }

    [PunRPC]
    public void SetGameObjectState(int obj, bool isActive)
    {
        PhotonView.Find(obj).gameObject.SetActive(isActive);

        if (this.photonView.isMine)
            this.photonView.RPC("SetGameObjectState", PhotonTargets.OthersBuffered, obj, isActive);
    }

    [PunRPC]
    public void UpdateCollider(int id, bool isActive)
    {
        Collider beforeCollider = PhotonView.Find(id).GetComponent<Collider>();
        beforeCollider.enabled = isActive;

        if (this.photonView.isMine)
            this.photonView.RPC("UpdateCollider", PhotonTargets.OthersBuffered, id, isActive);
    }

    [PunRPC]
    public void UpdateTransform(int before, int parent, Vector3 locPosition, Vector3 locRotation)
    {
        PhotonView toChange = PhotonView.Find(before);
        PhotonView toParent = PhotonView.Find(parent);

        if (toParent == null)
            toChange.transform.parent = null;
        else if (toParent.transform.parent != null)
            toChange.transform.parent = toParent.transform;

        if (locPosition != null)
            toChange.transform.localPosition = locPosition;

        if (locRotation != null)
            toChange.transform.localRotation = Quaternion.Euler(locRotation);

        if (photonView.isMine)
            photonView.RPC("UpdateTransform", PhotonTargets.OthersBuffered, before, parent, locPosition, locRotation);
    }

    [PunRPC]
    public void DestroyGameObject(int toDestroy, float delay)
    {
        GameObject go = PhotonView.Find(toDestroy).gameObject;
        Debug.Log(string.Format("Destroyed {0} over the network.", go.name), this);

        Destroy(go, delay);

        if (this.photonView.isMine)
            this.photonView.RPC("DestroyGameObject", PhotonTargets.OthersBuffered, toDestroy, delay);
    }

    [PunRPC]
    public void DestroyDropEffect(int toDestroy, string effectName)
    {
        GameObject go = PhotonView.Find(toDestroy).gameObject;

        Transform to = go.transform.Find(effectName + "(Clone)");
        Destroy(to.gameObject);

        if (this.photonView.isMine)
            this.photonView.RPC("DestroyDropEffect", PhotonTargets.OthersBuffered, toDestroy, effectName);
    }

    [PunRPC]
    public void FireBullet(string PrefabName, Vector3 pos, Vector3 dir)
    {
        Debug.Log("Firing bullet over network.", this);

        GameObject bullet = PhotonNetwork.Instantiate(PrefabName, pos, Quaternion.LookRotation(dir), 0);
        bullet.GetComponent<Rigidbody>().AddForce(dir * 100f, ForceMode.Force);

        Destroy(bullet, 3f);

        if (this.photonView.isMine)
            this.photonView.RPC("FireBullet", PhotonTargets.OthersBuffered, PrefabName, pos, dir);
    }
}