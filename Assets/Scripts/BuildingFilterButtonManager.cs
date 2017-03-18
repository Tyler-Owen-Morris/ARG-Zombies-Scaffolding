using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildingFilterButtonManager : MonoBehaviour {

    public Sprite[] myButtonSprites; //length 3 for bldg, wall, both 
    public GameObject[] myBldgs, myWalls;
    public Image myImage;

    public enum currentFilterType
    {
        BUILDINGS,
        WALLS,
        BOTH
    }

    public currentFilterType current_filter;

    public void ChangeFilterType ()
    {
        if (current_filter == currentFilterType.BUILDINGS)
        {
            //change to walls
            GameObject[] bldgs = GameObject.FindGameObjectsWithTag("building");
            
            if (bldgs.Length > 0)
            {
                foreach(GameObject bldg in bldgs)
                {
                    bldg.SetActive(false); //all buildings off
                }
            }

            GameObject[] walls = GameObject.FindGameObjectsWithTag("walllocation");
            if (walls.Length > 0)
            {
                foreach(GameObject wall in walls)
                {
                    wall.SetActive(true); //turn all walls on
                }
            }
            current_filter = currentFilterType.WALLS;//set the enum
            myImage.sprite = myButtonSprites[1];
            Debug.Log("filter changer found " + bldgs.Length + " buildings to turn off, " + walls.Length + " walls to turn on");

        }else if (current_filter== currentFilterType.WALLS)
        {
            //change to both
            GameObject[] bldgs = GameObject.FindGameObjectsWithTag("building");
            if (bldgs.Length > 0)
            {
                foreach (GameObject bldg in bldgs)
                {
                    bldg.SetActive(true); //all buildings on
                }
            }

            GameObject[] walls = GameObject.FindGameObjectsWithTag("walllocation");
            if (walls.Length > 0)
            {
                foreach (GameObject wall in walls)
                {
                    wall.SetActive(true); //turn all walls on
                }
            }
            current_filter = currentFilterType.BOTH;//set the enum
            myImage.sprite = myButtonSprites[2];
            Debug.Log("filter changer found " + bldgs.Length + " buildings to turn on, " + walls.Length + " walls to turn on");

        }
        else if (current_filter == currentFilterType.BOTH)
        {
            //change to buildings
            GameObject[] bldgs = GameObject.FindGameObjectsWithTag("building");
            if (bldgs.Length > 0)
            {
                foreach (GameObject bldg in bldgs)
                {
                    bldg.SetActive(true); //all buildings on
                }
            }

            GameObject[] walls = GameObject.FindGameObjectsWithTag("walllocation");
            if (walls.Length > 0)
            {
                foreach (GameObject wall in walls)
                {
                    wall.SetActive(false); //turn all walls off
                }
            }
            current_filter = currentFilterType.BUILDINGS;//set the enum
            myImage.sprite = myButtonSprites[0];
            Debug.Log("filter changer found " + bldgs.Length + " buildings to turn on, " + walls.Length + " walls to turn off");
        }
    }
}
