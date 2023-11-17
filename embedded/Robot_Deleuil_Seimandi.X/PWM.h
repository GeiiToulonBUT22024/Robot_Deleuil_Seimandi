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
#define COEF_D 1.1

        
void InitPWM(void);
// void PWMSetSpeed(float vitesseEnPourcents, uint8_t moteur); (decrepated)
void PWMUpdateSpeed();
void PWMSetSpeedConsigne(float vitesse, char moteur);


#endif	/* PWM_H */

