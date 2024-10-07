using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class BubbleSort : MonoBehaviour
{
    public float[] arrayBubble;
    public float[] arraySelection;
    public List<GameObject> bubbleObjs;
    public List<GameObject> selectionObjs;
    public GameObject prefab;
    public GameObject prefabSelection;

    void Start()
    {
        bubbleObjs = new List<GameObject>();
        selectionObjs = new List<GameObject>();
        arrayBubble = new float[30000];
        arraySelection = new float[30000];
        for (int i = 0; i < 30000; i++)
        {
            arrayBubble[i] = (float)Random.Range(0, 1000) / 100;
            arraySelection[i] = (float)Random.Range(0, 1000) / 100;
        }

        //TO DO 4
        //Call the three previous functions in order to set up the exercise
        logArray();

        spawnBubbleObjs();
        

        updateHeights();
        updateSelectionHeights();

        //TO DO 5
        //Create a new thread using the function "bubbleSort" and start it.
        Thread sortingThread = new Thread(bubbleSort);
        sortingThread.Start();

        Thread selectionThread = new Thread(selectionSort);
        selectionThread.Start();


    }

    void Update()
    {
        //TO DO 6
        //Call ChangeHeights() in order to update our object list.
        //Since we'll be calling UnityEngine functions to retrieve and change some data,
        //we can't call this function inside a Thread
        bool updating = false;

        if (!updating)
        {
            updating = updateHeights();
        }


        bool updatingSelection = false;
        if (!updatingSelection)
        {
            updatingSelection = updateSelectionHeights();
        }

    }

    //TO DO 5
    //Create a new thread using the function "bubbleSort" and start it.
    void bubbleSort()
    {
        int i, j;
        int n = arrayBubble.Length;
        bool swapped;
        for (i = 0; i < n - 1; i++)
        {
            swapped = false;
            for (j = 0; j < n - i - 1; j++)
            {
                if (arrayBubble[j] > arrayBubble[j + 1])
                {
                    (arrayBubble[j], arrayBubble[j + 1]) = (arrayBubble[j + 1], arrayBubble[j]);
                    swapped = true;
                }
            }
            if (swapped == false)
                break;
        }
        Debug.Log("Finished sorting");
        //You may debug log your Array here in case you want to. It will only be called one the bubble algorithm has finished sorting the array
    }


    void selectionSort()
    {
        int insertions = 0;
        for (int j = 1; j < arraySelection.Length; j++)
        {

            float key = arraySelection[j];
            int i = j - 1;
            while (i >= 0 && arraySelection[i] > key)
            {
                arraySelection[i + 1] = arraySelection[i];
                i -= 1;
                this.arraySelection = arraySelection;

                insertions++;
            }

            arraySelection[i + 1] = key;
            this.arraySelection = arraySelection;
        }
    }

    void logArray()
    {
        string text = "";

        for (int i = 0; i < arrayBubble.Length; i++)
        {
            text += arrayBubble[i];
        }
        //TO DO 1
        //Simply show in the console what's inside our array.

        Debug.Log(text);
    }

    void spawnBubbleObjs()
    {
        //TO DO 2
        //We should be storing our objects in a list so we can access them later on.

        for (int i = 0; i < arrayBubble.Length; i++)
        {
            //We have to separate the objs accordingly to their width, in which case we divide their position by 1000.
            //If you decide to make your objs wider, don't forget to up this value

            bubbleObjs.Add(Instantiate(prefab, new Vector3((float)i / 1000,
                this.gameObject.GetComponent<Transform>().position.y, 0), Quaternion.identity));

            selectionObjs.Add(Instantiate(prefabSelection, new Vector3((float)i / 1000,
                this.gameObject.GetComponent<Transform>().position.y -20, 0), Quaternion.identity));
        }
    }





    //TO DO 3
    //We'll just change the height of every obj in our list to match the values of the array.
    //To avoid calling this function once everything is sorted, keep track of new changes to the list.
    //If there weren't, you might as well stop calling this function

    bool updateHeights()
    {

        bool changed = false;
        for (int i = 0; i < arrayBubble.Length; i++)
        {
            if (bubbleObjs[i].transform.localScale != Vector3.up * arrayBubble[i])
            {
                bubbleObjs[i].transform.localScale = new Vector3(0.001f, arrayBubble[i], 1);
                changed = true;
            }
        }
        return changed;
    }

    bool updateSelectionHeights()
    {
        bool changed = false;
        for (int i = 0; i < arraySelection.Length; i++)
        {
            if (selectionObjs[i].transform.localScale != Vector3.up * arraySelection[i])
            {
                selectionObjs[i].transform.localScale = new Vector3(0.001f, arraySelection[i], 1);
                changed = true;
            }
        }
        return changed;
    }


}
