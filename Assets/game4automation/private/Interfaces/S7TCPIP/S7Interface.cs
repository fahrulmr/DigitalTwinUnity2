// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

// This product bundles Sharp7,  Copyright (C) 2016 Davide Nardella. It was
// individually licensed from Davide Nardella under the Apache 2.0 license.
// For details on Snap7 see http://snap7.sourceforge.net/ 

using System;
using UnityEngine;
using Sharp7;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace game4automation
{
    //! S7 interface for connecting to Siemens S7 PLCs via TCPIP
    [HelpURL("https://game4automation.com/documentation/current/plcsimadvanced.html")]
    public class S7Interface : InterfaceThreadedBaseClass
    {
        public string Adress; //!< The ip adress of the PLC
        public int Rack; //!< the rack number of the plc
        public int Slot; //!< the slot number of the plc
        public bool ReverseHighLowBytes = true;
        private int connectionstatus;
        public string SymbolTable; //!< The path to the symbol table of the PLC (*.seq text file)
        public int ReduceMaxPduLenth = 60;

        [ReadOnly]
        public string ConnectionStatus; //!< The connection Status to the PLC - OK if everything is ok (ReadOnly)

        [ReadOnly] public string PLCStatus; //!< The Status of the PLC - running if PLC is running (ReadOnly)
        [ReadOnly] public int RequestedPduLength; //!< The requested PDU length (ReadOnly)
        [ReadOnly] public int NegotiatedPduLenght; //!< The negotiated PDU length (ReadOnly)
        [ReadOnly] public int NumberInputs; //!< The number of Inputs in the interface (ReadOnly)
        [ReadOnly] public int NumberOutputs; //!< The number of Outputs in the interface (ReadOnly)


        private S7Client Client;
        private S7MultiVar outputreader;
        private S7MultiVar inputwriter;
        private bool _isconnectedwrite;
        private bool _isconnectedread;
        private bool _isdisconnected;
        private float _lastreconnect;


        private int GetArea(string name)
        {
            var toupper = name.ToUpper();
            int area = 0;
            switch (toupper[0])
            {
                case 'A':
                    area = S7Consts.S7AreaPA;
                    break;
                case 'Q':
                    area = S7Consts.S7AreaPA;
                    break;
                case 'E':
                    area = S7Consts.S7AreaPE;
                    break;
                case 'I':
                    area = S7Consts.S7AreaPE;
                    break;
                case 'M':
                    area = S7Consts.S7AreaMK;
                    break;
                case 'D':
                    area = S7Consts.S7AreaDB;
                    break;
                default:
                    Debug.LogError("Area for this variable [" + name + "] is not allowed!");
                    break;
            }

            return area;
        }


        //! Gets the S7 Signal type with the absolute signal name
        private S7InterfaceSignal.S7TYPE GetS7Type(string s7type)
        {
            var res = S7InterfaceSignal.S7TYPE.UNDEFINED;
            S7InterfaceSignal.S7TYPE type;

            if (Enum.TryParse(s7type.ToUpper(), out type))
            {
                res = type;
            }
            else
            {
                Debug.LogError("Type  [" + s7type + "] is not allowed!");
            }

            return res;
        }

        //! Gets the standard signal type based on the S7 type
        private InterfaceSignal.TYPE GetType(S7InterfaceSignal.S7TYPE s7type)
        {
            switch (s7type)
            {
                case S7InterfaceSignal.S7TYPE.BOOL:
                    return InterfaceSignal.TYPE.BOOL;

                case S7InterfaceSignal.S7TYPE.BYTE:
                case S7InterfaceSignal.S7TYPE.WORD:
                case S7InterfaceSignal.S7TYPE.DWORD:
                case S7InterfaceSignal.S7TYPE.SINT:
                case S7InterfaceSignal.S7TYPE.INT:
                case S7InterfaceSignal.S7TYPE.DINT:
                case S7InterfaceSignal.S7TYPE.USINT:
                case S7InterfaceSignal.S7TYPE.UINT:
                case S7InterfaceSignal.S7TYPE.UDINT:
                case S7InterfaceSignal.S7TYPE.TIME:
                    return InterfaceSignal.TYPE.INT;

                case S7InterfaceSignal.S7TYPE.REAL:
                    return InterfaceSignal.TYPE.REAL;

                default:
                    return InterfaceSignal.TYPE.UNDEFINED;
            }
        }

        //! Gets the direction (Input or Output) based on the absolute S7 signal name
        private InterfaceSignal.DIRECTION GetDirection(string name)
        {
            InterfaceSignal.DIRECTION direction = InterfaceSignal.DIRECTION.NOTDEFINED;
            var toupper = name.ToUpper();
            var firsttwo = toupper.Substring(0, 2);
            if (firsttwo == "DB")
            {
                direction = InterfaceSignal.DIRECTION.INPUTOUTPUT;
            }
            else
            {
                switch (toupper[0])
                {
                    case 'A':
                        direction = InterfaceSignal.DIRECTION.OUTPUT;
                        break;
                    case 'Q':
                        direction = InterfaceSignal.DIRECTION.OUTPUT;
                        break;
                    case 'E':
                        direction = InterfaceSignal.DIRECTION.INPUT;
                        break;
                    case 'I':
                        direction = InterfaceSignal.DIRECTION.INPUT;
                        break;
                    case 'M':
                        direction = InterfaceSignal.DIRECTION.OUTPUT;
                        break;
                    case 'D':
                        direction = InterfaceSignal.DIRECTION.OUTPUT;
                        break;
                    default:
                        Debug.LogError("Type for this variable [" + name + "] is not allowed!");
                        break;
                }
            }


            return direction;
        }

        private static S7InterfaceSignal.S7TYPE GetDBType(string name)
        {
            // get pos of ".DB"
            S7InterfaceSignal.S7TYPE type = S7InterfaceSignal.S7TYPE.UNDEFINED;
            var pos = name.IndexOf(".DB");
            switch (name[pos + 3])
            {
                case 'W':
                    type = S7InterfaceSignal.S7TYPE.WORD;
                    break;
                case 'B':
                    type = S7InterfaceSignal.S7TYPE.BYTE;
                    break;
                case 'D':
                    type = S7InterfaceSignal.S7TYPE.DWORD;
                    break;
                default: // if no char
                    type = S7InterfaceSignal.S7TYPE.BOOL;
                    break;
            }

            return type;
        }

        private static int GetDBMemPos(string name)
        {
            var pos = name.IndexOf(".DB");
            return GetFirstNum(name, pos + 3);
        }

        //! Gets the S7 datatype from the absolute S7 signal name
        private static S7InterfaceSignal.S7TYPE GetTypeFromName(string name)
        {
            S7InterfaceSignal.S7TYPE type = S7InterfaceSignal.S7TYPE.UNDEFINED;
            var toupper = name.ToUpper();
            var firsttwo = toupper.Substring(0, 2);
            if (firsttwo == "DB")
            {
                type = GetDBType(toupper);
            }
            else
            {
                switch (toupper[1])
                {
                    case 'W':
                        type = S7InterfaceSignal.S7TYPE.WORD;
                        break;
                    case 'B':
                        type = S7InterfaceSignal.S7TYPE.BYTE;
                        break;
                    case 'D':
                        type = S7InterfaceSignal.S7TYPE.DWORD;
                        break;
                    default: // if no char
                        type = S7InterfaceSignal.S7TYPE.BOOL;
                        break;
                }
            }

            return type;
        }

        private static int GetFirstNum(string str, int start)
        {
            string res = "";
            for (int i = start; i < str.Length; i++) // loop over the complete input
            {
                if (Char.IsDigit(str[i])) //check if the current char is digit
                    res += str[i];
                else
                {
                    if (res.Length > 0)
                    {
                        break;
                    }
                }
            }

            try
            {
                return Convert.ToInt32(res);
            }
            catch (Exception e)
            {
                Debug.LogError("Error in Format" + str + " " + e.ToString());
                throw;
            }
        }

        private int GetNumberAfterPoint(string str)
        {
            // find first point
            var pospoint = str.IndexOf('.');
            if (pospoint != -1)
            {
                return GetFirstNum(str, pospoint);
            }
            else
            {
                return 0;
            }
        }


        private static int GetLenght(string name)
        {
            int len;
            var toupper = name.ToUpper();
            var mychar = toupper[1];
            var firsttwo = toupper.Substring(0, 2);
            if (firsttwo == "DB")
            {
                var pos = name.IndexOf(".DB");
                mychar = toupper[pos + 3];
            }

            switch (mychar)
            {
                case 'W':
                    len = 4;
                    // len = S7Consts.S7WLWord;
                    break;
                case 'B':
                    //len = 1;
                    len = S7Consts.S7WLByte;
                    break;
                case 'D':
                    //len = 4;
                    len = S7Consts.S7WLDWord;
                    break;
                default: // if no char
                    //len = 1;
                    len = S7Consts.S7WLBit;
                    break;
            }

            return len;
        }

        //! Gets a signal based on its name
        public override GameObject GetSignal(string name)
        {
            Transform[] children = transform.GetComponentsInChildren<Transform>();
            // First check names of signals
            foreach (var child in children)
            {
                var signal = child.GetComponent<Signal>();
                if (signal != null && child != gameObject.transform)
                {
                    if (signal.Name == name)
                    {
                        return child.gameObject;
                    }
                }
            }

            // Second check names of components
            foreach (var child in children)
            {
                if (child != gameObject.transform)
                {
                    if (child.name == name)
                    {
                        return child.gameObject;
                    }
                }
            }

            return null;
        }


        //! Imports all signal objects under the interface gameobject during simulation start
        private void ImportSignals(bool simstart)
        {
            NumberInputs = 0;
            NumberOutputs = 0;
            if (simstart)
            {
                Signal[] signals = GetComponentsInChildren<Signal>();
                DeleteSignals();
                foreach (var Signal in signals)
                {
                    var type = InterfaceSignal.TYPE.UNDEFINED;
                    var s7type = S7InterfaceSignal.S7TYPE.UNDEFINED;
                    if (Signal.OriginDataType == "")
                    {
                        s7type = GetTypeFromName(Signal.Name);
                        type = GetType(s7type);
                    }
                    else
                    {
                        s7type = GetS7Type(Signal.OriginDataType);
                        type = GetType(s7type);
                    }

                    var direction = GetDirection(Signal.Name);
                    if (direction == InterfaceSignal.DIRECTION.INPUT)
                    {
                        NumberInputs++;
                    }

                    if (direction == InterfaceSignal.DIRECTION.OUTPUT)
                    {
                        NumberOutputs++;
                    }

                    if (direction == InterfaceSignal.DIRECTION.INPUTOUTPUT)
                    {
                        if (Signal.IsInput())
                        {
                            direction = InterfaceSignal.DIRECTION.INPUT;
                            NumberInputs++;
                        }
                        else
                        {
                            direction = InterfaceSignal.DIRECTION.OUTPUT;
                            NumberOutputs++;
                        }
                    }

                    S7InterfaceSignal s7signal = new S7InterfaceSignal(Signal.Name, direction, type);
                    s7signal.Signal = Signal;
                    s7signal.S7Type = s7type;
                    s7signal.Area = GetArea(Signal.Name);
                    s7signal.Mempos = GetFirstNum(Signal.Name, 0);
                    s7signal.Bit = (byte) GetNumberAfterPoint(Signal.Name);
                    s7signal.DBNumber = 0;
                    if (s7signal.Area == S7Consts.S7AreaDB)
                    {
                        s7signal.IsDB = true;
                        s7signal.DBNumber = GetFirstNum(Signal.Name, 0);
                        s7signal.Mempos = GetDBMemPos(Signal.Name);
                    }

                    s7signal.Size = GetLenght(Signal.Name);
                    s7signal.Comment = Signal.Comment;
                    s7signal.OriginDataType = Signal.OriginDataType;
                    AddSignal(s7signal);
                }
            }
        }


        //! Imports the symbol table and creates the signal objects under this S7interface object
        public void ReadSignalFile()
        {
            List<string> iosymbol = new List<string>();
            List<string> ioadress = new List<string>();
            List<string> iotype = new List<string>();
            List<string> iocomment = new List<string>();
            try
            {
                using (StreamReader sr = new System.IO.StreamReader(SymbolTable))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var tmp = "\"";
                        var newline = line.Replace(tmp, string.Empty);
                        var values = newline.Split(',');
                        iosymbol.Add(values[0]);
                        var newvalues = values[1].Replace("%", string.Empty);
                        ioadress.Add(newvalues);
                        iotype.Add(values[2]);
                        iocomment.Add(values[6]);
                    }
                }
            }
            catch (Exception e)
            {
                Error("Error in reading PLC Signal table " + e.ToString());
            }

            for (int i = 0; i < iosymbol.Count; i++)
            {
                var direction = GetDirection(ioadress[i]);

                var s7type = GetS7Type(iotype[i]);
                var type = GetType(s7type);
                S7InterfaceSignal newsignal = new S7InterfaceSignal(ioadress[i], direction, type);
                newsignal.SymbolName = iosymbol[i];
                newsignal.S7Type = GetS7Type(iotype[i]);
                newsignal.Name = ioadress[i];
                newsignal.OriginDataType = s7type.ToString();
                newsignal.Comment = iocomment[i];
                AddSignal(newsignal);
            }
        }


        //!  Connects to the S7 and checks the connection
        public void CheckConnection()
        {
            ConnectS7();
            Client.Disconnect();
        }


        private void ConnectS7()
        {
            _lastreconnect = Time.time;
            Client = new S7Client();
            connectionstatus = Client.ConnectTo(Adress, Rack, Slot);
            int plcstatus = 0;
            if (connectionstatus == 0)
            {
                int ExecTime = Client.ExecTime();
                Log("S7 interface connected - time: " + ExecTime.ToString() + " ms");
            }
            else
            {
                Error(Client.ErrorText(connectionstatus), this);
                _isconnectedread = false;
                _isconnectedwrite = false;
                return;
            }

            RequestedPduLength = Client.RequestedPduLength();
            NegotiatedPduLenght = Client.NegotiatedPduLength();
            Client.PlcGetStatus(ref plcstatus);
            switch (plcstatus)
            {
                case S7Consts.S7CpuStatusRun:
                    PLCStatus = "Running";
                    break;
                case S7Consts.S7CpuStatusStop:
                    PLCStatus = "Stopped";
                    break;
                case S7Consts.S7CpuStatusUnknown:
                    PLCStatus = "Unknown";
                    break;
            }

            ConnectionStatus = Client.ErrorText(connectionstatus);
            _isconnectedread = true;
            _isconnectedwrite = true;
        }

        private void ReadS7()
        {
            /* S7MultiVar Reader = new S7MultiVar(Client); 
             
             byte[] DB_A = new byte[16];
             byte[] DB_B = new byte[16];
             int DBNumber_A = 1; // DB1  
             int DBNumber_B = 1; // DB2  
             int DBNumber_C = 1; // DB3  
  
 
             Reader.Add(S7Consts.S7AreaDB, S7Consts.S7WLByte, DBNumber_A, 0, 4, ref DB_A);
 
             var restdbread = Client.DBRead(1, 0, 4, DB_B);
             Log(Client.ErrorText(restdbread));
 
            int Result = Reader.Read(); 
            Log(Client.ErrorText(Result)); */
            var toread = NumberOutputs;
            var currsignal = 0;
            var pdufull = false;
            var pdu = 0;
            var numsignals = 0;
            var payload = 0;
            outputreader = new S7MultiVar(Client);
            while (toread > 0)
            {
                var signal = (S7InterfaceSignal) InterfaceSignals[currsignal];
                if (signal.Direction == InterfaceSignal.DIRECTION.OUTPUT)
                {
                    if (signal.Size % 2 != 0)
                        payload++;
                    payload = payload + signal.Size;
                    pdu = 14 + 4 * (numsignals + 1) + payload;

                    if (pdu < NegotiatedPduLenght - ReduceMaxPduLenth) // Add
                    {
                        if (signal.S7Type == S7InterfaceSignal.S7TYPE.BOOL)
                            outputreader.Add(signal.Area, signal.Size, signal.DBNumber, signal.Mempos * 8 + signal.Bit,
                                1, ref signal.TransferValue);
                        else
                            outputreader.Add(signal.Area, signal.Size, signal.DBNumber, signal.Mempos, 1,
                                ref signal.TransferValue);
                        //  outputreader.Add(signal.Area, S7Consts.S7WLByte, signal.DBNumber, signal.Mempos, 4, ref signal.TransferValue);
                        toread--;
                        numsignals++;
                    }
                    else
                    {
                        pdufull = true;
                    }
                }

                // send if pdu full or all signals are created
                if (pdufull || toread == 0 || numsignals >= 20)
                {
                    numsignals = 0;
                    payload = 0;
                    // Read PLC Outputs
                    var connected = outputreader.Read();
                    if (connected == 0)
                    {
                        _isconnectedread = true;
                    }
                    else
                    {
                        _isconnectedread = false;
                        ThreadStatus = "S7 interface plc output read error " + Client.ErrorText(connected);
                    }

                    if (toread > 0)
                        outputreader = new S7MultiVar(Client);
                }

                if (pdufull)
                    pdufull = false;
                else
                    currsignal++;
            }

            // lock now signals and transfer from temporary transfervalue for better multitasking performance
            toread = NumberOutputs;
            currsignal = 0;
            lock (InterfaceSignals)
            {
                while (toread > 0)
                {
                    var mysignal = (S7InterfaceSignal) InterfaceSignals[currsignal];
                    if (mysignal.Direction == InterfaceSignal.DIRECTION.OUTPUT)
                    {
                        byte[] newbyte = (byte[]) mysignal.TransferValue.Clone();
                        if (ReverseHighLowBytes)
                        {
                            if (mysignal.S7Type != S7InterfaceSignal.S7TYPE.BYTE)
                                Array.Reverse(newbyte);
                        }

                        mysignal.Value = newbyte;
                        toread--;
                    }

                    currsignal++;
                }
            }
        }

        private void WriteS7()
        {
            var toread = NumberInputs;
            var currsignal = 0;
            var pdufull = false;
            var pdu = 0;
            var numsignals = 0;
            var payload = 0;
            var request = 0;

            lock (InterfaceSignals)
            {
                // lock now signals and transfer from temporary transfervalue for better multitasking performance
                while (toread > 0)
                {
                    var mysignal = (S7InterfaceSignal) InterfaceSignals[currsignal];
                    if (mysignal.Direction == InterfaceSignal.DIRECTION.INPUT)
                    {
                        byte[] newbyte = (byte[]) mysignal.Value.Clone();
                        mysignal.TransferValue = newbyte;
                        toread--;
                    }

                    currsignal++;
                }
            }

            toread = NumberInputs;
            currsignal = 0;
            inputwriter = new S7MultiVar(Client);
            while (toread > 0)
            {
                var signal = (S7InterfaceSignal) InterfaceSignals[currsignal];
                if (signal.Direction == InterfaceSignal.DIRECTION.INPUT)
                {
                    if (signal.Size % 2 != 0)
                        payload++;
                    payload = payload + signal.Size;
                    pdu = 12 + 16 * (numsignals + 1) + payload;
                    request = 7 + 7 + 12 + (numsignals + 1) * 12;
                    if (pdu < NegotiatedPduLenght - ReduceMaxPduLenth &&
                        request < NegotiatedPduLenght - ReduceMaxPduLenth) // Add
                    {
                        if (signal.S7Type == S7InterfaceSignal.S7TYPE.BOOL)

                            inputwriter.Add(signal.Area, signal.Size, signal.DBNumber, signal.Mempos * 8 + signal.Bit,
                                1, ref signal.TransferValue);
                        else
                            inputwriter.Add(signal.Area, signal.Size, signal.DBNumber, signal.Mempos, 1,
                                ref signal.TransferValue);
                        toread--;
                        numsignals++;
                    }
                    else
                    {
                        pdufull = true;
                    }
                }

                // send if pdu full or all signals are created
                if (pdufull || toread == 0 || numsignals >= 20)
                {
                    numsignals = 0;
                    payload = 0;
                    // Read PLC Outputs
                    var connected = inputwriter.Write();
                    if (connected == 0)
                    {
                        _isconnectedwrite = true;
                    }
                    else
                    {
                        _isconnectedwrite = false;
                        ThreadStatus = "S7 interface plc input write error " + Client.ErrorText(connected);
                    }

                    if (toread > 0)
                        inputwriter = new S7MultiVar(Client);
                }

                if (pdufull)
                    pdufull = false;
                else
                    currsignal++;
            }
        }


        //! Updates all signals in the parallel communication thread
        protected override void CommunicationThreadUpdate()
        {
            if (!IsConnected)
                return;

            if (!Client.Connected)
                return;

            if (!_isconnectedread || !_isconnectedwrite)
                return;
            try
            {
                ReadS7();
                WriteS7();
            }
            catch (Exception e)
            {
                var error = e;
                _isconnectedread = false;
                _isconnectedwrite = false;
            }
        }

        //! Updates one signal in the communication thread
        void UpdateSignal(InterfaceSignal interfacesignal)
        {
            lock (InterfaceSignals)
            {
                PLCOutputInt plcoutputint = null;
                PLCOutputBool plcoutputbool = null;
                PLCOutputFloat plcoutputfloat = null;
                PLCInputInt plcinputint = null;
                PLCInputFloat plcinputfloat = null;
                PLCInputBool plcinputbool = null;
                if (Client == null)
                    return;
                var s7 = (S7InterfaceSignal) interfacesignal;

                var bytes = s7.Value;


                if (interfacesignal.Direction == InterfaceSignal.DIRECTION.OUTPUT)
                {
                    switch (s7.S7Type)
                    {
                        case S7InterfaceSignal.S7TYPE.BOOL:
                            plcoutputbool = (PLCOutputBool) s7.Signal;
                            if (bytes[3] == 1)
                                plcoutputbool.Value = true;
                            else
                            {
                                plcoutputbool.Value = false;
                            }

                            plcoutputbool.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.BYTE:
                            plcoutputint = (PLCOutputInt) s7.Signal;
                            if (s7.IsDB)
                                plcoutputint.Value = bytes[0];
                            else
                                plcoutputint.Value = bytes[3];
                            plcoutputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.WORD:
                            plcoutputint = (PLCOutputInt) s7.Signal;
                            plcoutputint.Value = BitConverter.ToInt16(bytes, 2);
                            plcoutputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.DWORD:
                            plcoutputint = (PLCOutputInt) s7.Signal;
                            plcoutputint.Value = (int) BitConverter.ToUInt32(bytes, 0);
                            plcoutputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.SINT:
                            plcoutputint = (PLCOutputInt) s7.Signal;
                            unchecked
                            {
                                sbyte s = (sbyte) bytes[3];
                                plcoutputint.Value = s;
                            }

                            plcoutputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.INT:
                            plcoutputint = (PLCOutputInt) s7.Signal;
                            plcoutputint.Value = BitConverter.ToInt16(bytes, 2);
                            plcoutputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.DINT:
                            plcoutputint = (PLCOutputInt) s7.Signal;
                            plcoutputint.Value = BitConverter.ToInt32(bytes, 0);
                            plcoutputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.USINT:
                            plcoutputint = (PLCOutputInt) s7.Signal;
                            plcoutputint.Value = bytes[3];
                            plcoutputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.UINT:
                            plcoutputint = (PLCOutputInt) s7.Signal;
                            plcoutputint.Value = BitConverter.ToUInt16(bytes, 2);
                            plcoutputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.UDINT:
                            plcoutputint = (PLCOutputInt) s7.Signal;
                            plcoutputint.Value = (int) BitConverter.ToUInt32(bytes, 0);
                            plcoutputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.REAL:
                            plcoutputfloat = (PLCOutputFloat) s7.Signal;
                            plcoutputfloat.Value = BitConverter.ToSingle(bytes, 0);
                            plcoutputfloat.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.TIME:
                            plcoutputfloat = (PLCOutputFloat) s7.Signal;
                            plcoutputfloat.Value = BitConverter.ToInt32(bytes, 0);
                            plcoutputfloat.Status.Connected = IsConnected;
                            break;
                    }
                }

                var notreverse = false;
                if (interfacesignal.Direction == InterfaceSignal.DIRECTION.INPUT)
                {
                    switch (s7.S7Type)
                    {
                        case S7InterfaceSignal.S7TYPE.BOOL:
                            plcinputbool = (PLCInputBool) s7.Signal;
                            if (plcinputbool.Value)
                                bytes[0] = 255;
                            else
                                bytes[0] = 0;
                            plcinputbool.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.BYTE:
                            plcinputint = (PLCInputInt) s7.Signal;
                            bytes[0] = (byte) plcinputint.Value;
                            notreverse = true;
                            plcinputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.WORD:
                            plcinputint = (PLCInputInt) s7.Signal;
                            bytes = BitConverter.GetBytes((UInt16) plcinputint.Value);
                            plcinputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.DWORD:
                            plcinputint = (PLCInputInt) s7.Signal;
                            bytes = BitConverter.GetBytes((UInt32) plcinputint.Value);
                            plcinputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.SINT:
                            plcinputint = (PLCInputInt) s7.Signal;
                            unchecked
                            {
                                sbyte s;
                                s = (sbyte) plcinputint.Value;
                                byte b = (byte) s;
                                bytes[0] = b;
                            }

                            notreverse = true;
                            plcinputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.INT:
                            plcinputint = (PLCInputInt) s7.Signal;
                            bytes = BitConverter.GetBytes((Int16) plcinputint.Value);
                            plcinputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.DINT:
                            plcinputint = (PLCInputInt) s7.Signal;
                            bytes = BitConverter.GetBytes(plcinputint.Value);
                            plcinputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.USINT:
                            plcinputint = (PLCInputInt) s7.Signal;
                            bytes = BitConverter.GetBytes((Byte) plcinputint.Value);
                            notreverse = true;
                            plcinputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.UINT:
                            plcinputint = (PLCInputInt) s7.Signal;
                            bytes = BitConverter.GetBytes((UInt16) plcinputint.Value);
                            plcinputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.UDINT:
                            plcinputint = (PLCInputInt) s7.Signal;
                            bytes = BitConverter.GetBytes((UInt32) plcinputint.Value);
                            plcinputint.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.REAL:
                            plcinputfloat = (PLCInputFloat) s7.Signal;
                            bytes = BitConverter.GetBytes((Single) plcinputfloat.Value);
                            plcinputfloat.Status.Connected = IsConnected;
                            break;
                        case S7InterfaceSignal.S7TYPE.TIME:
                            plcinputint = (PLCInputInt) s7.Signal;
                            bytes = BitConverter.GetBytes((Int32) plcinputint.Value);
                            plcinputint.Status.Connected = IsConnected;
                            break;
                    }

                    if (s7.S7Type != S7InterfaceSignal.S7TYPE.BOOL && ReverseHighLowBytes)
                    {
                        if (!notreverse)
                            bytes = bytes.Reverse().ToArray();
                    }

                    s7.Value = bytes;
                }
            }
        }

        public override void OpenInterface()
        {
            _isdisconnected = true;
            ImportSignals(true);
            ConnectS7();
            base.OpenInterface();
        }

        public override void CloseInterface()
        {
            if (Client != null)
            {
                Client.Disconnect();
            }

            base.CloseInterface();
        }


        private void Update()
        {
            if ((!_isconnectedread || !_isconnectedwrite) && !_isdisconnected)
            {
                OnDisconnected();
                CloseInterface();
                _isdisconnected = true;
            }

            if (_isdisconnected && (Time.time - _lastreconnect) > 1)
                OpenInterface();

            if (_isdisconnected && _isconnectedread && _isconnectedwrite)
            {
                OnConnected();
                _isdisconnected = false;
            }

            foreach (var interfacesignal in InterfaceSignals)
            {
                UpdateSignal(interfacesignal);
            }
        }
    }
}