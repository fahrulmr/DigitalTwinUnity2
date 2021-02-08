using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sharp7;
using System;

namespace game4automation
{

    public class S7InterfaceSignal : InterfaceSignal
    {
        public int Area;
        public int DBNumber;
        public bool IsDB;
        public int Size;
        public S7TYPE S7Type;
        public byte[] Value = new byte[4];
        public byte[] TransferValue = new byte[4];

    public enum S7TYPE
        {
            UNDEFINED,
            BOOL,
            BYTE,
            WORD,
            DWORD,
            SINT,
            INT,
            DINT,
            USINT,
            UINT,
            UDINT,
            REAL,
            TIME
         
        };
    
        public S7InterfaceSignal(string name, DIRECTION direction, TYPE type)
        {
            Name = name;
            Direction = direction;
            Type = type;
        }
           
    }
}

