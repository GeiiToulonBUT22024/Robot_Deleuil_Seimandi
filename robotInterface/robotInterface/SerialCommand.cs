using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace robotInterface
{
    
    abstract class SerialCommand
    {
        // -----------------------------------
        public enum CommandType
        {
            TEXT = 0x0080,
            CONSIGNE_VITESSE = 0x0040,
            TELEMETRE_IR = 0x0030,
            LED = 0x0020
        }
        // -----------------------------------

        public abstract void Process(Robot robot);
        
        protected CommandType type;
        protected byte[]? payload;

        public new CommandType GetType() {
            return type;
        }

        // -----------------------------------
        public static SerialCommand? CreateCommand(int commandCode, byte[] payload)
        {
            switch (commandCode)
            {
                case (int)CommandType.TEXT:
                    return new SerialCommandText(payload);

                case (int)CommandType.LED:
                    return new SerialCommandLED(payload);

                case (int)CommandType.TELEMETRE_IR:
                    return new SerialCommandTelemetreIR(payload);

                case (int)CommandType.CONSIGNE_VITESSE:
                    return new SerialCommandConsigneVitesse(payload);

            }
            return null;
        }
    }
    // ---------------------------------------------------------
    internal class SerialCommandText : SerialCommand
    {
        private string text;

        public SerialCommandText(byte[] payload) 
        {
            this.type = CommandType.TEXT;
            this.payload = payload;
            this.text = Encoding.Default.GetString(payload);
        }
        public override void Process(Robot robot)
        {
            robot.stringListReceived.Enqueue(text);
        }
    }
    // ---------------------------------------------------------
    internal class SerialCommandConsigneVitesse : SerialCommand
    {
        private int vitesseGauche;
        private int vitesseDroite;

        public SerialCommandConsigneVitesse(byte[] payload)
        {
            this.type = CommandType.CONSIGNE_VITESSE;
            this.payload = payload;
            this.vitesseGauche = (payload[0] > 127 ? payload[0] - 256 : payload[0]);
            this.vitesseDroite = (payload[1] > 127 ? payload[1] - 256 : payload[1]);
        }
        public override void Process(Robot robot)
        {
            robot.consigneGauche = (float) this.vitesseGauche;
            robot.consigneDroite = (float) this.vitesseDroite;
        }
    }
    // ---------------------------------------------------------
    internal class SerialCommandTelemetreIR : SerialCommand
    {
        private int telemetreMelenchon;
        private int telemetreGauche;
        private int telemetreCentre;
        private int telemetreDroit;
        private int telemetreLePen;

        public SerialCommandTelemetreIR(byte[] payload)
        {
            this.type = CommandType.TELEMETRE_IR;
            this.payload = payload;
            this.telemetreMelenchon = payload[0];
            this.telemetreGauche = payload[1];
            this.telemetreCentre = payload[2];
            this.telemetreDroit = payload[3];
            this.telemetreLePen = payload[4];            
        }
        public override void Process(Robot robot)
        {
            robot.distanceTelemetreMelenchon = this.telemetreMelenchon;
            robot.distanceTelemetreGauche = this.telemetreGauche;
            robot.distanceTelemetreCentre = this.telemetreCentre;
            robot.distanceTelemetreDroit = this.telemetreDroit;
            robot.distanceTelemetreLePen = this.telemetreLePen;
        }
    }
    // ---------------------------------------------------------
    internal class SerialCommandLED : SerialCommand
    {
        private int numero;
        private byte state;

        public SerialCommandLED(byte[] payload)
        {
            this.type = CommandType.CONSIGNE_VITESSE;
            this.payload = payload;
            this.numero = payload[0];
            this.state = payload[1];
        }
        public override void Process(Robot robot)
        {
            switch (this.numero) 
            {
                case 0x00:
                    robot.ledBlanche = this.state;
                    break;
                case 0x01:
                    robot.ledBlanche = this.state;
                    break;
                case 0x10:
                    robot.ledBlanche = this.state;
                    break;
            }
        }
    }
}
