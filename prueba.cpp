/*
    Morales Almeida David
*/
#include <iostream>
#include <stdio.h>
#include <conio.h>
float area, radio, pi, resultado;
int x;
int y;
int z;
// Este programa calcula el volumen de un cilindro.
void main()
{
    printf("Introduce el numero de renglones para la piramide: ");
    scanf("%d", &z);

    for (x = 1; x <= z; x++)
    {
        for (y = 0; y < x; y++)
        {
            printf(x);
        }
        printf("\n");
    }
}