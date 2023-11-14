

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


    // PWMSetSpeedConsigne(25, MOTEUR_DROIT);
    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/
    while (1) {
        if (ADCIsConversionFinished()) {
            ADCClearConversionFinishedFlag();
        }

        if (ADCIsConversionFinished() == 1) {
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



            // Update LED
            if (robotState.distanceTelemetreMelanchon >= 30.0f) {
                LED_ORANGE = 1;
            } else {
                LED_ORANGE = 0;
            }


            if (robotState.distanceTelemetreCentre >= 30.0f) {
                LED_BLEUE = 1;
            } else {
                LED_BLEUE = 0;
            }

            if (robotState.distanceTelemetreLePen >= 30.0f) {
                LED_BLANCHE = 1;
            } else {
                LED_BLANCHE = 0;
            }
            float baseGauche = VITESSE;
            float baseDroite = VITESSE;
            if(robotState.distanceTelemetreMelanchon <= 40) {
                if(robotState.distanceTelemetreMelanchon <= 25) {
                    baseGauche += VITESSE / 3.0f;
                    baseDroite -= VITESSE / 3.0f; 
                }
                else {
                    baseGauche += VITESSE / 5.0f;
                    baseDroite -= VITESSE / 5.0f; 
                }
            }
            
            if(robotState.distanceTelemetreGauche <= 40) {
                if(robotState.distanceTelemetreGauche <= 25) {
                    baseGauche += VITESSE / 3.0f;
                    baseDroite -= VITESSE / 3.0f; 
                }
                else {
                    baseGauche += VITESSE / 5.0f;
                    baseDroite -= VITESSE / 5.0f; 
                }
            }
            
            if(robotState.distanceTelemetreCentre <= 30) {
                if(robotState.distanceTelemetreCentre <= 18) {
                    baseGauche -= VITESSE * 1.2f;
                    baseDroite -= VITESSE * 1.2; 
                }
                else {
                    baseGauche -= VITESSE * 0.5f;
                    baseDroite -= VITESSE * 0.5f; 
                }
            }
            
            if(robotState.distanceTelemetreDroit <= 40) {
                if(robotState.distanceTelemetreDroit <= 25) {
                    baseGauche -= VITESSE / 3.0f;
                    baseDroite += VITESSE / 3.0f; 
                }
                else {
                    baseGauche -= VITESSE / 5.0f;
                    baseDroite += VITESSE / 5.0f; 
                }
            }
            
            if(robotState.distanceTelemetreLePen <= 40) {
                if(robotState.distanceTelemetreLePen <= 25) {
                    baseGauche -= VITESSE / 3.0f;
                    baseDroite += VITESSE / 3.0f; 
                }
                else {
                    baseGauche += VITESSE / 5.0f;
                    baseDroite -= VITESSE / 5.0f; 
                }
            }
            
            PWMSetSpeedConsigne(baseGauche, MOTEUR_GAUCHE);
            PWMSetSpeedConsigne(baseDroite, MOTEUR_DROIT);
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