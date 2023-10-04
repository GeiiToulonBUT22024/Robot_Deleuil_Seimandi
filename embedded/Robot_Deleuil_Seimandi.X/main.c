

#include <stdio.h>
#include <stdlib.h>
#include <xc.h>
#include "ChipConfig.h"
#include "IO.h"
#include "timer.h"
#include "PWM.h"
#include "Robot.h"
#include "ADC.h"

int main(void) {
    /***************************************************************************************************/
    //Initialisation de l?oscillateur
    /****************************************************************************************************/
    InitOscillator();

    /****************************************************************************************************/
    // Configuration des entrées sorties
    /****************************************************************************************************/
    InitIO();

    InitTimer23();
    InitTimer1();
    InitTimer4();

    InitPWM();

    InitADC1();
    // PWMSetSpeed(0, MOTEUR_DROIT);

    robotState.acceleration = 2;
    unsigned int ADCValue0 = 0;
    unsigned int ADCValue1 = 0;
    unsigned int ADCValue2 = 0;
    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/
    while (1) {
        /*if (ADCIsConversionFinished()) {
            unsigned int *ADCResult = ADCGetResult();


            ADCClearConversionFinishedFlag();
        }

        if (ADCIsConversionFinished() == 1) {
            ADCClearConversionFinishedFlag();
            unsigned int * result = ADCGetResult();
            
            float volts = ((float) result [2])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreDroit = 34 / volts - 5;
            
            volts = ((float) result [1])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreCentre = 34 / volts - 5;
            
            volts = ((float) result [0])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreGauche = 34 / volts - 5;
            
            // Update LED
            if (robotState.distanceTelemetreGauche >= 30.0f) {
                LED_ORANGE = 1;
            }else {
                LED_ORANGE = 0;
            }


            if (robotState.distanceTelemetreCentre >= 30.0f) {
                LED_BLEUE = 1;
            }else {
                LED_BLEUE = 0;
            }

            if (robotState.distanceTelemetreDroit >= 30.0f) {
                LED_BLANCHE = 1;
            }else { 
                LED_BLANCHE = 0;
            }
        }
        */


    } // fin main

}
