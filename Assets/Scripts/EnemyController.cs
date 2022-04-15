using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ViralVial;

public class EnemyController : MonoBehaviour
{
    public float speed = 0f;
    public int winAmount = 12;
    public List<GameObject> models;
    public SpawnerRoll spawner;
    public TextMeshProUGUI countText;
    public RunView view;

    private GameObject model;
    private Rigidbody rb;
    private int modelIndex, count;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        modelIndex = 0;

        SetModel();
        SetCountText();
    }

    private void SetModel()
    {
        if (model != null) Destroy(model);
        model = Instantiate(models[modelIndex], transform);
        ResizeModel(model);
        model.transform.localPosition += (Vector3.down * 0.25f);
        modelIndex = (modelIndex + 1) % models.Count;
    }

    private void ResizeModel(GameObject m)
    {
        var filter = m.GetComponentsInChildren<Renderer>();
        var size = filter[0].bounds.size;
        m.transform.localScale *= (1f / Mathf.Max(size.x, Mathf.Max(size.y, size.z)));
    }

    private void SetCountText()
    {
        // countText.text = "Enemy Count: " + count.ToString();
    }

    private void CheckWinCon()
    {
        if (count >= winAmount)
        {
            view.GoToGameLoss();
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = CalculateMovement();
        rb.AddForce(movement * speed);
    }

    private Vector3 CalculateMovement()
    {
        var target = spawner.GetFirstActiveSpawn();
        if (target is null) return Vector3.zero;

        var direction = target.transform.position - transform.position;
        direction.y = 0f;
        return direction.normalized;
    }

    private void OnTriggerEnter(Component other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            spawner.ReSpawn(other.gameObject);
            count++;
            SetCountText();
            SetModel();
            //check if win
            CheckWinCon();
        }
    }
}
