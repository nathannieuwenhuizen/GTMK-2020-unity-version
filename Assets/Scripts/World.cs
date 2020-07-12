using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public List<LayerObject> layers;

    public Transform layerParent;
    public Transform maskParent;
    public AnimationCurve growCurve = AnimationCurve.EaseInOut(0,0,1,1);

    public GameObject layerPrefab;
    public GameObject maskPrefab;

    public Outliner outliner;

    private bool perfectItteration = true;
    public int sizeOfWorld = 0;

    public float precentageMarge = .4f;

    public UI ui;


    public static World instance;
    private void Awake()
    {
        instance = this;
    }

    public void NewItteration()
    {
        if (perfectItteration) 
        {
            AddLayerAt(0);
        }
        perfectItteration = true;

        outliner.gameObject.SetActive(true);
        outliner.ResetScale();
        outliner.maxScale = TotalSize + .4f;
        UpdateSizes();

    }
    void Start()
    {
        layers = new List<LayerObject>();
    }
    public void AddLayerAt(int index = -1)
    {
        Layer newLayer = GameObject.Instantiate(layerPrefab, layerParent).GetComponent<Layer>();
        SpriteMask newMask = GameObject.Instantiate(maskPrefab, maskParent).GetComponent<SpriteMask>();
        newLayer.Setup();
        LayerObject newPiece = new LayerObject()
        {
            layer = newLayer,
            mask = newMask
        };
        if (layers.Count == 0 || index >= layers.Count || index <= 0)
        {
            layers.Add(newPiece);
        } else
        {
            layers.Insert(index, newPiece);
            newPiece.setSize(newPiece.layer.transform, layers[index - 1].layer.transform.localScale.x);
            newPiece.setSize(newPiece.mask.transform, layers[index - 1].mask.transform.localScale.x);
        }
        SizeOfWorld = layers.Count;
    }
    public void RemoveLayerAt(int index)
    {
        CameraShake.instance.Shake(.5f);

        perfectItteration = false;
        if (index < layers.Count)
        {
            LayerObject removeObject = layers[index];
            Destroy(removeObject.layer.gameObject);
            Destroy(removeObject.mask.gameObject);
            layers.Remove(removeObject);
        }
        SizeOfWorld = layers.Count;

        if (layers.Count == 0)
        {
            outliner.gameObject.SetActive(false);
        }
    }

    public int SizeOfWorld
    {
        get
        {
            return sizeOfWorld;
        }
        set
        {
            sizeOfWorld = value;
            ui.UpdateScoreText(value);

            if (value > Settings.HighScore)
            {
                Settings.HighScore = value;
                ui.UpdateHighScoreText(value);
            }
        }
    }

    public void Update()
    {
        CheckMissingBeat();
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckBeat();
        }
    }
    private void CheckMissingBeat()
    {
        if (outliner.resetting || !outliner.gameObject.active) return;

        bool allGood = true;
        foreach (LayerObject layer in layers)
        {
            if (layer.active == false)
            {
                allGood = false;
            }
            if (layer != null)
            {
                if (!layer.active && outliner.Scale < layer.layer.transform.localScale.x)
                {
                    float distance = layer.layer.transform.localScale.x - outliner.Scale;
                    float precentage = distance / (TotalSize / (float)layers.Count);
                    if (precentage > precentageMarge)
                    {
                        RemoveLayerAt(layers.IndexOf(layer));
                    }
                }
            }
        }
        if (allGood)
        {
            NewItteration();
        }
    }
    private void CheckBeat()
    {
        if (layers.Count == 0 && !outliner.resetting  && !outliner.gameObject.active)
        {
            ui.Play();
            Debug.Log("Checking new itteration");
            NewItteration();
            return;
        }

        LayerObject closestObject = layers[0];
        float distance = Mathf.Infinity;
        int index = 0;
        int closestIndex = 0;
        foreach(LayerObject layer in layers)
        {
            Debug.Log("Distance: " + Mathf.Abs(outliner.Scale - layer.layer.transform.localScale.x));
            if (Mathf.Abs(outliner.Scale - layer.layer.transform.localScale.x) < distance)
            {
                closestIndex = index;
                closestObject = layer;
                distance = Mathf.Abs(outliner.Scale - layer.layer.transform.localScale.x);
            }
            index++;

        }


        float precentage = distance / (1f / (float)layers.Count); // between 0 and 50
        if (precentage > precentageMarge || closestObject.active)
        {
            RemoveLayerAt(closestIndex);
        } else
        {
            closestObject.layer.Dance();
            closestObject.active = true;
        }
        outliner.Pulse();
        
    }

    public float TotalSize { get {
            return 1 - (Mathf.Pow(.7f, layers.Count));
        }
    }
    private void UpdateSizes()
    {
         
        float totalSize = TotalSize;
        float sizePerPiece = TotalSize / layers.Count;
        outliner.maskWidth = sizePerPiece * .8f;
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].active = false;
            StartCoroutine(layers[i].ChangeSize(totalSize, totalSize - sizePerPiece * .8f, growCurve, 2f));
            totalSize -= sizePerPiece;

            layers[i].SetLayerIndex(i * 3);
        }
    }

}

public class LayerObject
{
    public Layer layer;
    public SpriteMask mask;
    public bool active = false;
    public IEnumerator ChangeSize(float layerEnd, float maskEnd, AnimationCurve curve, float size)
    {
        float layerStart = layer.transform.localScale.x;
        float maskStart = mask.transform.localScale.x;
        float index = 0;
        float duration = .5f;
        while (index < duration)
        {
            yield return new WaitForFixedUpdate();
            index += Time.deltaTime;
            setSize(layer.transform, Mathf.Lerp(layerStart, layerEnd, curve.Evaluate(index / duration)));
            setSize(mask.transform, Mathf.Lerp(maskStart, maskEnd, curve.Evaluate(index / duration)));
            layer.updateScale(size);
        }
    }
    public void SetLayerIndex( int val)
    {
        layer.GetComponent<SpriteRenderer>().sortingOrder = val;
        mask.frontSortingOrder = val +1;
        mask.backSortingOrder = val -1;
    }
    public void setSize(Transform tr, float value)
    {
        tr.localScale = new Vector3(value, value, value);
    }
}