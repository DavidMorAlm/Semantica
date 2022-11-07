; Archivo: prueba.cpp
; Compilado: El 07/11/2022 a las 01:35:45
#make_COM#
include emu8086.inc
ORG 1000h
MOV AX, 10
PUSH AX
POP AX
MOV altura, AX
POP AX
MOV i, AX
do0:
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
do1:
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if1
PRINTN "-"
JMP else1
if1:
PRINTN "*"
else1:
POP BX
POP AX
SUB AX, BX
PUSH AX
POP BX
POP AX
CMP AX, BX
JG finDo1
JMP do1
finDo1:
PRINTN "
"
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE finDo0
JMP do0
finDo0:
RET

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
	p dw ?
DEFINE_SCAN_NUM
