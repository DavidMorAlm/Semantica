; Archivo: prueba.cpp
; Compilado: El 31/10/2022 a las 16:45:30
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
	k dw ?
	l dw ?
MOV AX, 100
PUSH AX
POP AX
MOV y, AX
POP AX
RET
DEFINE_SCAN_NUM
