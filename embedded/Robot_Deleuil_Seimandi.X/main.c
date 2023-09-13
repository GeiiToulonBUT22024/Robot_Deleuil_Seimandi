#include <stdio.h>
#include <stdlib.h>
#include <xc.h>
#include "ChipConfig.h"
#include "IO.h"
#include "timer.h"

int main(void) {
    /***************************************************************************************************/
    //Initialisation de l'oscillateur
    /****************************************************************************************************/
    
    InitOscillator();
    void InitTimer23(void);
    void InitTimer1(void);
    
    /****************************************************************************************************/
    // Configuration des entr�es/sorties
    /****************************************************************************************************/
    InitIO();

    LED_BLANCHE = 1;
    LED_BLEUE = 1;
    LED_ORANGE = 1;
    
    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/
    while (1) {
//        LED_BLANCHE = !LED_BLANCHE;
//        LED_BLEUE = !LED_BLEUE;
//        LED_ORANGE = !LED_ORANGE;  
    } // fin main

}
