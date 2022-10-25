; Archivo: prueba.cpp
; Compilado: El 24/10/2022 a las 09:55:58
#make_COM#
include emu8086.inc
ORG 1000h

; Variables:

	area dw ?
	radio dw ?
	pi dw ?
	resultado dw ?
	a dw ?
	d dw ?
	altura dw ?
	x dw ?
	y dw ?
	i dw ?
	j dw ?
MOV AX, 71
PUSH AX
POP AX
MOV y, AX
MOV AX, 71
PUSH AX
POP AX
POP BX
MOV AX, 0
PUSH AX
POP AX
MOV x, AX
if1:
MOV AX, 1
PUSH AX
MOV AX, 5
PUSH AX
POP AX
POP BX
MOV AX, 0
PUSH AX
POP AX
if2:
RET
