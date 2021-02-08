﻿
using game4automation;
using UnityEngine;

namespace game4automation
{
    public class InterfaceSignal 
    {
        public enum TYPE
        {
            BOOL,            
            INT,
            REAL,
            TRANSFORM,
            UNDEFINED
        };

        public enum DIRECTION
        {
            NOTDEFINED,
            INPUT,
            OUTPUT,
            INPUTOUTPUT
        };
    
        public Signal Signal;
        public string Name;
        public TYPE Type;
        public DIRECTION Direction;
        public int Mempos;
        public byte Bit;
        public string SymbolName;
        public string Comment;
        public string OriginDataType;

        public InterfaceSignal()
        {
       
        }

  
        public InterfaceSignal(string name, DIRECTION direction, TYPE type)
        {
            Name = name;
            Direction = direction;
            Type = type;
        }

        public void UpdateSignal(Signal signal)
        {
        
        }
    }

}

