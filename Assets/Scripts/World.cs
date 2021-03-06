﻿using System.Collections;
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

    public float precentageMarge = .5f;

    public UI ui;
    public ParticleSystem particles;

    private AudioSource failAudio;

    public static World instance;
    private void Awake()
    { 
        instance = this;
    }
    void Start()
    {
        failAudio = GetComponent<AudioSource>();
        layers = new List<LayerObject>();
    }

    public void NewItteration()
    {
        if (perfectItteration) 
        {
            AddLayerAt(0);
            StopAllCoroutines();
            StartCoroutine(SuperParty());
            particles.Emit(50);
        }
        perfectItteration = true;

        outliner.gameObject.SetActive(true);
        outliner.ResetScale();
        outliner.maxScale = TotalSize + .4f;
        UpdateWorld();

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
        #if UNITY_ANDROID
        if (Settings.Vibration)
        {
            Handheld.Vibrate();
        }
        #endif
        failAudio.Play();
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
            if (value == 0)
            {
                particles.Stop();
            } else if (value == 1)
            {
                particles.Play();
            }
            particles.emissionRate = Mathf.Min(100, 3 +value * 5);
            particles.startSpeed  = 5 + value;
        }
    }

    public void Update()
    {
        CheckMissingBeat();

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CheckBeat(ColorType.white);
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            CheckBeat(ColorType.blue);
        }
    }
    private void CheckMissingBeat()
    {
        if (outliner.resetting || !outliner.gameObject.active) return;

        bool allGood = true;
        foreach (LayerObject layer in layers)
        {
            if (layer.inPulse == false)
            {
                allGood = false;
            }
            if (layer != null)
            {
                if (!layer.inPulse && outliner.Scale < layer.layer.transform.localScale.x)
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

    public void CheckWhite()
    {
        CheckBeat(ColorType.white);
    }
    public void CheckBlue()
    {
        CheckBeat(ColorType.blue);
    }
    public void CheckBeat(ColorType type)
    {
        if (layers.Count == 0 && !outliner.resetting  && !outliner.gameObject.active)
        {
            particles.Play();
            ui.FirstMove();
            perfectItteration = true;
            failAudio.Play();
            NewItteration();
            return;
        }

        LayerObject closestObject = layers[0];
        float distance = Mathf.Infinity;
        int index = 0;
        int closestIndex = 0;
        foreach(LayerObject layer in layers)
        {
            if (Mathf.Abs(outliner.Scale - layer.layer.transform.localScale.x) < distance)
            {
                closestIndex = index;
                closestObject = layer;
                distance = Mathf.Abs(outliner.Scale - layer.layer.transform.localScale.x);
            }
            index++;

        }


        float precentage = distance / (1f / (float)layers.Count); // between 0 and 50
        if (precentage > precentageMarge || closestObject.inPulse || type != closestObject.layer.Type)
        {
            RemoveLayerAt(closestIndex);
        } else
        {
            if (layers.IndexOf(closestObject) != layers.Count - 1 || !perfectItteration)
            {
                closestObject.layer.Dance();
            }
            closestObject.inPulse = true;
        }
        outliner.Pulse();
        
    }

    public float TotalSize { get {
            return 1 - (Mathf.Pow(.7f, layers.Count));
        }
    }
    private void UpdateWorld()
    {

        bool extraColors = sizeOfWorld >= 10;
        if (extraColors)
        {
            outliner.shrinkSpeed = 0.0035f;
            ui.ShowPairButtons();
        } else
        {
            outliner.shrinkSpeed = 0.0045f;
            ui.ShowSingleButton();
        }

        float totalSize = TotalSize;
        float sizePerPiece = TotalSize / layers.Count;
        outliner.maskWidth = sizePerPiece * .8f;
        for (int i = 0; i < layers.Count; i++)
        {
            if (extraColors)
            {
                if (Random.value < 0.5f)
                {
                    layers[i].layer.Type = ColorType.blue;
                } else
                {
                    layers[i].layer.Type = ColorType.white;
                }
            } else
            {
                layers[i].layer.Type = ColorType.white;
            }
            layers[i].layer.ChangePitch(.3f + ((i/ TotalSize) * .7f));
            layers[i].inPulse = false;
            StartCoroutine(layers[i].ChangeSize(totalSize, totalSize - sizePerPiece * .8f, growCurve, 2f));
            totalSize -= sizePerPiece;

            layers[i].SetLayerIndex(i * 3);
        }
    }
    public IEnumerator SuperParty()
    {
        for(int i = 0; i < layers.Count - 1; i++)
        {
            layers[i].layer.Dance();
            yield return new WaitForSeconds(.4f / layers.Count);
        }
    }
}

public class LayerObject
{
    public Layer layer;
    public SpriteMask mask;
    public bool inPulse = false;
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