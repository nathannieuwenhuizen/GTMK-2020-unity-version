using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    private List<Human> humans;

    public Transform humanParent;
    public GameObject humanPrefab;

    public Transform propParent;
    private List<GameObject> props;
    public GameObject[] propPrefabs;

    private AudioSource audioS;

    public void Setup()
    {
        audioS = GetComponent<AudioSource>();
        humans = new List<Human>();
        props = new List<GameObject>();
        spawnHumans(5);
        spawnProps(3);
    }

    public void ChangePitch(float val)
    {
        audioS.pitch = val;
        audioS.volume = Mathf.Max(.2f, 1 - (val / 2));
    }
    public void spawnHumans(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newHuman = GameObject.Instantiate(humanPrefab, humanParent);
            SetPositionToProp(newHuman.transform);
            humans.Add(newHuman.GetComponent<Human>());
        }
    }
    public void spawnProps(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newProp = GameObject.Instantiate(propPrefabs[ Mathf.FloorToInt(Random.Range(0, propPrefabs.Length))], propParent);
            SetPositionToProp(newProp.transform);
            props.Add(newProp);
        }
    }
    public void SetPositionToProp(Transform tr)
    {
        Vector2 pos = new Vector2(0, 0);
        float angle = 0;
        float size = 4.2f;
        float randomPosSize = size * .8f;

        float random = Random.value;
        if (random <= .25f) //left
        {
            pos.x = -size;
            angle = 90f;
            pos.y = Random.Range(-randomPosSize, randomPosSize);
        }
        else if (random <= .50) //right
        {
            angle = -90;
            pos.x = size;
            pos.y = Random.Range(-randomPosSize, randomPosSize);

        }
        else if (random <= .75) //down
        {
            pos.x = Random.Range(-randomPosSize, randomPosSize);
            pos.y = size;
            angle = 0;
        }
        else //up
        {
            angle = 180;
            pos.y = -size;
            pos.x = Random.Range(-randomPosSize, randomPosSize);
        }
        tr.localPosition = pos;
        tr.Rotate(0, 0, angle);
    }
    public void updateScale(float size)
    {
        float aspect = 1 / (size * transform.localScale.x);
        Vector3 scale = new Vector3(aspect, aspect, aspect);
        foreach (Human human in humans)
        {
            human.transform.localScale = scale;
        }
        foreach (GameObject prop in props)
        {
            prop.transform.localScale = scale;
        }
    }
    public void Dance()
    {
        audioS.Play();

        foreach (Human human in humans)
        {
            human.Dance();
        }
        StartCoroutine(AfterDance());
    }
    IEnumerator AfterDance()
    {
        yield return new WaitForSeconds(.5f);
        foreach (Human human in humans)
        {
            human.Idle();
        }
    }
}
