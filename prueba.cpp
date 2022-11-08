//Morales Almeida David
#include <iostream>
#include <stdio.h>
#include <conio.h>
float area, radio, pi, resultado;
int a, d, altura;
float x;
char y;
int i, j, k, l, p;
// Este programa calcula el volumen de un cilindro.
void main()
{
    altura = 10;
    i = altura;
    do
    {
        j = 0;
        do
        {
            if (j == 2)
            {
                printf(j);
            }
            else
            {
                printf("*");
            }
            j++;
        } while (j <= altura - i);
        printf("\n");
        i--;
    } while (i > 0);
}