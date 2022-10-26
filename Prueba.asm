; Archivo: prueba.cpp
; Compilado: El 25/10/2022 a las 19:21:39
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
MOV AX, 61
PUSH AX
POP AX
MOV y, AX
MOV AX, 60
PUSH AX
MOV AX, 61
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if1
MOV AX, 10
PUSH AX
POP AX
MOV x, AX
if1:
RET
