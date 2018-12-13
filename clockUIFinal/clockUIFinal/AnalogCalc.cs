using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clockUIFinal
{
    
     class AnalogCalc
    {
        //Field
        private static int ResistorValue;
        private static double Vref;

        //Constructor that takes no arguements
        public AnalogCalc()
        {
            ResistorValue = 100;
            Vref = 4.73; //Voltage Reference 
            //ohms law
            //5.03V/100 ohm = 50.3 mA
        }


        public string GetBuzzer(int an0)
        {
            double dAn0 = an0 * Vref / 1024.0;  // Vref = 5.03
            return dAn0.ToString("0.0000");
        }


        public string GetRed(int an1)
        {
            /* Encoding Analog Values to RGB scale
             * Without division applied 707 max value was displayed in GUI
               Max(707)/Range(255{rgb encoding})
             */

            double dAn1 = an1 / 4.015686;  //without division applied max value found was around 707
            return dAn1.ToString("0.0000");

        }

        public string GetGreen(int an2)
        {

            double dAn2 = an2 / 4.015686; ;// / 2.772549;  //
            return dAn2.ToString("0.0000");

        }

        public string GetBlue(int an3)
        {
          
            double dAn3 = an3 / 4.015686;
            return dAn3.ToString("0.0000");

        }

        public string LightDetect(int an4)
        {
            double dAn4 = an4;// * Vref / 1024.0;  // Vref = 5.03
            return dAn4.ToString("0.0000");
        }

        public string BuzzerToggle(int an5)
        {
            double dAn5 = an5 * Vref / 1024.0;  // Vref = 5.03
            return dAn5.ToString("0.0000");
        }
        /*
                public string PacketsRx(int an5)
                {
                    double dAn5 = an5 * Vref / 1024.0;  // Vref = 5.03
                    return dAn5.ToString("0.0000");
                    int packetrx = txtPacketNum.ToInt;
                    int packetlost;

                    txtnumPacketsReceived.Text =
                }

                public string PacketsLost(int an5)
                {
                    double dAn5 = an5 * Vref / 1024.0;  // Vref = 5.03
                    return dAn5.ToString("0.0000");
                    txtnumPacketslost.Text =
                }

                public string rateError(int an5)
                {
                    double dAn5 = an5 * Vref / 1024.0;  // Vref = 5.03
                    return dAn5.ToString("0.0000");
                    txtnumErrorRate.Text = 
                }
                */
    }
}
