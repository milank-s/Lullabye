using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using Random = System.Random;

public class GameState : MonoBehaviour
{

    public List<Room> _rooms;
    public List<Prop> _props;
    
    private int date;

    private float testTimer;

    public TextMesh dateReadout;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void ChangeDate(int date)
    {
        string binary = Convert.ToString(date, 2);
        for (int i = 0; i < binary.Length; i++)
        {
            bool active;
            
            int isActive = int.Parse(Char.ToString(binary[i]));
            
            if (isActive == 0)
            {
                active = true;
            }
            else
            {
                active = false;    
            }

            _props[i].SetState(active);
        }

        dateReadout.text = binary;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (testTimer < 0)
        {
            date++;
            ChangeDate(date);
            testTimer = 0.1f;
        }
        testTimer -= Time.deltaTime;
    }
}
