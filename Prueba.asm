; Archivo: prueba.cpp
; Compilado: El 06/11/2022 a las 22:39:26
#make_COM#
include emu8086.inc
ORG 1000h
for0:
MOV AX, 250
PUSH AX
POP AX
MOV y, AX
MOV AX, 256
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE endfor0
JMP for0
endfor0:
