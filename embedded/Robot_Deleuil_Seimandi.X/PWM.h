/* 
 * File:   PWM.h
 * Author: GEII Robot
 *
 * Created on 27 septembre 2023, 08:48
 */

#ifndef PWM_H
#define	PWM_H
#define MOTEUR_DROIT 1
#define MOTEUR_GAUCHE 0

        
void InitPWM(void);
void PWMSetSpeed(float vitesseEnPourcents, uint8_t moteur);

#endif	/* PWM_H */

