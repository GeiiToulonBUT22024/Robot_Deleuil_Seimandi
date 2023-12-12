#include <stdio.h>
#include <math.h>
#include <stdlib.h>
#include <xc.h>
#include "ChipConfig.h"
#include "IO.h"
#include "timer.h"
#include "PWM.h"
#include "Robot.h"
#include "ADC.h"
#include "PWM.h"
#include "main.h"
#include "Toolbox.h"
#include "UART.h"
#include "CB_TX1.h"
#include "CB_RX1.h"
#include <libpic30.h>



extern unsigned long timestamp;

unsigned char stateRobot;

int main(void) {
    /***************************************************************************************************/
    //Initialisation de l'oscillateur
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
    robotState.acceleration = 2;

    InitUART();


    // PWMSetSpeedConsigne(25, MOTEUR_DROIT);
    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/
    while (1) {
        /* UART */
        /*
        // SendMessageDirect((unsigned char*) "Bonjour", 7);
        SendMessage("Bonjour", 7);
        __delay32(40000000);
         */

        int i;
        for (i = 0; i < CB_RX1_GetDataSize(); i++) {
            unsigned char c = CB_RX1_Get();
            SendMessage(&c, 1);
        }
        __delay32(1000);




        /* -------------------- IMPLEMENTATION STRATEGIE --------------------*/
        if (0) {
            //if (ADCIsConversionFinished() == 1) {
            ADCClearConversionFinishedFlag();
            unsigned int * result = ADCGetResult();

            float volts = ((float) result [4])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreDroit = 34 / volts - 5;

            volts = ((float) result [2])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreCentre = 34 / volts - 5;

            volts = ((float) result [1])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreGauche = 34 / volts - 5;

            volts = ((float) result [3])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreLePen = 34 / volts - 5;

            volts = ((float) result [0])* 3.3 / 4096 * 3.2;
            robotState.distanceTelemetreMelanchon = 34 / volts - 5;


            float baseGauche = VITESSE;
            float baseDroite = VITESSE;
            int isViteVite = 1;
            if (robotState.distanceTelemetreMelanchon <= 45) {
                isViteVite = 0;

                if (robotState.distanceTelemetreMelanchon <= 10) {
                    baseGauche += (-0.4481) * robotState.distanceTelemetreMelanchon + 9.7403;
                    baseDroite -= (-0.4481) * robotState.distanceTelemetreMelanchon + 9.7403;
                } else {
                    baseGauche += (-0.055) * robotState.distanceTelemetreMelanchon + 5;
                    baseDroite -= (-0.055) * robotState.distanceTelemetreMelanchon + 5;
                }
            }

            if (robotState.distanceTelemetreGauche <= 45) {
                isViteVite = 0;

                baseGauche += (-0.075) * robotState.distanceTelemetreGauche + 5;
                baseDroite -= (-0.075) * robotState.distanceTelemetreGauche + 5;
            }

            if (robotState.distanceTelemetreCentre <= 40) {
                isViteVite = 0;

                baseGauche -= (-1.25) * robotState.distanceTelemetreCentre + 42.5 + ((robotState.distanceTelemetreMelanchon + robotState.distanceTelemetreGauche) > (robotState.distanceTelemetreLePen + robotState.distanceTelemetreDroit) ? 10 : -10);
                baseDroite -= (-1.25) * robotState.distanceTelemetreCentre + 42.5 + ((robotState.distanceTelemetreMelanchon + robotState.distanceTelemetreGauche) > (robotState.distanceTelemetreLePen + robotState.distanceTelemetreDroit) ? -10 : 10);

            }

            if (robotState.distanceTelemetreDroit <= 45) {
                isViteVite = 0;

                baseGauche -= (-0.075) * robotState.distanceTelemetreDroit + 5;
                baseDroite += (-0.075) * robotState.distanceTelemetreDroit + 5;
            }

            if (robotState.distanceTelemetreLePen <= 45) {
                isViteVite = 0;

                if (robotState.distanceTelemetreLePen <= 10) {
                    baseGauche -= (-0.4481) * robotState.distanceTelemetreLePen + 9.7403;
                    baseDroite += (-0.4481) * robotState.distanceTelemetreLePen + 9.7403;
                } else {
                    baseGauche -= (-0.055) * robotState.distanceTelemetreLePen + 5;
                    baseDroite += (-0.055) * robotState.distanceTelemetreLePen + 5;
                }
            }
            if (isViteVite) {
                //PWMSetSpeedConsigne(VITE_VITE, MOTEUR_GAUCHE);
                //PWMSetSpeedConsigne(VITE_VITE, MOTEUR_DROIT);

                LED_ORANGE = 1;
                LED_BLEUE = 1;
                LED_BLANCHE = 1;

            } else {
                //PWMSetSpeedConsigne(baseGauche, MOTEUR_GAUCHE);
                //PWMSetSpeedConsigne(baseDroite, MOTEUR_DROIT);

                LED_ORANGE = 0;
                LED_BLEUE = 0;
                LED_BLANCHE = 0;
            }
            /*float uVect[3] = {robotState.distanceTelemetreMelanchon * 0.5f + robotState.distanceTelemetreGauche * 0.866f,
                robotState.distanceTelemetreGauche * 0.866f + robotState.distanceTelemetreCentre + robotState.distanceTelemetreDroit * 0.866f,
                robotState.distanceTelemetreDroit * 0.866f + robotState.distanceTelemetreLePen * 0.5f};

            float vVect[3] = {robotState.distanceTelemetreMelanchon * -0.866f + robotState.distanceTelemetreGauche *-0.5f,
                robotState.distanceTelemetreGauche * -0.5f + robotState.distanceTelemetreDroit * 0.5f,
                robotState.distanceTelemetreDroit * 0.5f + robotState.distanceTelemetreLePen * 0.866f};

            // calcul de la norme
            float normVect[3] = {sqrt(uVect[0] * uVect[0] + vVect[0] * vVect[0]),
                sqrt(uVect[1] * uVect[1] + vVect[1] * vVect[1]),
                sqrt(uVect[2] * uVect[2] + vVect[2] * vVect[2])};

            // Choix du vecteur
            uint8_t finalVect;
            if (normVect[0] > normVect[1]) {
                if (normVect[0] > normVect[2]) {
                    finalVect = 0;
                } else {
                    finalVect = 2;
                }
            } else {
                if (normVect[1] > normVect[2]) {
                    finalVect = 1;
                } else {
                    finalVect = 2;
                }
            }

            // Application à la commande

            int dir = (int) (180 / PI) * atan(vVect[finalVect] / uVect[finalVect]);
            PWMSetSpeedConsigne(((dir * -VITESSE / 2.0f) / -60.0f) + VITESSE, MOTEUR_GAUCHE);
            PWMSetSpeedConsigne(((dir * VITESSE / 2.0f) / -60.0f) + VITESSE, MOTEUR_DROIT);*/



        }
    } // fin main

}