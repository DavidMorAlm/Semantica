; Archivo: prueba.cpp
; Compilado: El 29/10/2022 a las 17:36:24
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
MOV AX, 300000
PUSH AX
POP AX
MOV x, AX
RET
DEFINE_SCAN_NUM
