using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace robotInterface
{
    public class SerialProtocolManager
    {
        public enum CommandID
        {
            TEXT = 0x0080,
            LED = 0x0020,
            TELEMETRE_IR = 0x0030,
            CONSIGNE_VITESSE = 0x0040
        }


        private enum StateReception
        {
            Waiting,
            FunctionMSB, 
            FunctionLSB,
            PayloadLengthMSB,
            PayloadLengthLSB,
            Payload,
            CheckSum
        }

        StateReception rcvState = StateReception.Waiting;
        private int msgDecodedFunction = 0;
        private int msgDecodedPayloadLength = 0;
        private byte[] msgDecodedPayload;
        private int msgDecodedPayloadIndex = 0;

        private Robot robot;


        public SerialProtocolManager() {}

        public void setRobot(Robot robot) { this.robot = robot;}

        public void ProcessDecodedMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            switch (msgFunction)
            {
                case (int)CommandID.TEXT:
                    this.robot.stringListReceived.Enqueue(Encoding.Default.GetString(msgPayload));
                    break;

                case (int)CommandID.LED:
                    break;

                case (int)CommandID.TELEMETRE_IR:
                    break;

                case (int)CommandID.CONSIGNE_VITESSE:
                    break;

            }
        }

        public void DecodeMessage(byte c)
        {
            switch (rcvState)
            {
                case StateReception.Waiting:
                    if (c == 0xFE) rcvState = StateReception.FunctionMSB;
                    break;

                case StateReception.FunctionMSB:
                    msgDecodedFunction = c << 8;
                    rcvState = StateReception.FunctionLSB; 
                    break;

                case StateReception.FunctionLSB:
                    msgDecodedFunction |= c;
                    rcvState = StateReception.PayloadLengthMSB;
                    break;

                case StateReception.PayloadLengthMSB:
                    msgDecodedPayloadLength = c << 8;
                    rcvState = StateReception.PayloadLengthLSB;
                    break;

                case StateReception.PayloadLengthLSB:
                    msgDecodedPayloadLength |= c;

                    msgDecodedPayload = new byte[msgDecodedPayloadLength];
                    rcvState = StateReception.Payload;
                    break;

                case StateReception.Payload:
                    msgDecodedPayload[msgDecodedPayloadIndex++] = c;
                    if (msgDecodedPayloadIndex == msgDecodedPayloadLength) 
                        rcvState = StateReception.CheckSum;
                    break;

                case StateReception.CheckSum:
                
                    if (CalculateChecksum(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload) == c) {
                        MessageBox.Show("Success ID : " + msgDecodedFunction.ToString("X2") + " : " + Encoding.Default.GetString(msgDecodedPayload));
                        ProcessDecodedMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                    }
                    rcvState = StateReception.Waiting;
                    break;

                default:
                    rcvState = StateReception.Waiting;
                    break;
            }
        }

        public byte CalculateChecksum(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte checksum = 0xFE;

            checksum ^= (byte)(msgFunction >> 8);
            checksum ^= (byte)msgFunction;

            checksum ^= (byte)(msgPayloadLength >> 8);
            checksum ^= (byte) msgPayloadLength;

            for (int i = 0; i < msgPayloadLength; i++) {
                checksum ^= msgPayload[i];
            }

            return checksum;
        }



        public byte[] UartEncode(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            List<byte> payload = new List<byte>();
            payload.Add((byte)0xFE);
            payload.Add((byte)(msgFunction >> 8));
            payload.Add((byte)msgFunction);

            payload.Add((byte)(msgPayloadLength >> 8));
            payload.Add((byte)msgPayloadLength);

            for(int i = 0; i < msgPayloadLength;i++)
            {
                payload.Add(msgPayload[i]);
            }

            payload.Add(CalculateChecksum(msgFunction, msgPayloadLength, msgPayload));
            return payload.ToArray();
            
        }
    }
}

