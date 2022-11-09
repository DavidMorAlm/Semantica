; Archivo: prueba.cpp
; Compilado: El 08/11/2022 a las 21:09:52
#make_COM#
include emu8086.inc
ORG 100h
PRINT "Introduce la altura de la piramide: "
CALL SCAN_NUM
MOV altura, CX
MOV AX, altura
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE if1
for0:
MOV AX, altura
PUSH AX
POP AX
MOV i, AX
MOV AX, i
PUSH AX
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE endfor0
MOV AX, 1
PUSH AX
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
while0:
MOV AX, j
PUSH AX
MOV AX, altura
PUSH AX
MOV AX, i
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finWhile0
MOV AX, j
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV BX
PUSH DX
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if2
PRINT "*"
JMP else2
if2:
PRINT "-"
else2:
MOV AX, 1
PUSH AX
JMP while0
finWhile0:
PRINTN ""
POP AX
SUB AX, i
JMP for0
endfor0:
MOV AX, 0
PUSH AX
POP AX
MOV k, AX
do0:
PRINT "-"
MOV AX, 2
PUSH AX
MOV AX, k
PUSH AX
MOV AX, altura
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finDo0
JMP do0
finDo0:
PRINTN ""
JMP else1
if1:
PRINT "
Error: la altura debe de ser mayor que 2
"
else1:
MOV AX, 1
PUSH AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JE if31
PRINT "Esto no se debe imprimir"
MOV AX, 2
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if32
PRINT "Esto tampoco"
if32:
if31:
MOV AX, 258
PUSH AX
POP AX
MOV a, AX
PRINT "Valor de variable int a antes del casteo: "
MOV AX, a
PUSH AX
POP AX
PRINT AX
MOV AX, a
PUSH AX
POP AX
PUSH 2
POP AX
MOV y, AX
PRINT "
Valor de variable char y despues del casteo de a: "
MOV AX, y
PUSH AX
POP AX
PRINT AX
PRINT "
A continuacion se intenta asignar un int a un char sin usar casteo: 
"
RET

; Variables:
	area dd ?
	radio dd ?
	pi dd ?
	resultado dd ?
	a dw ?
	d dw ?
	altura dw ?
	cinco dw ?
	x dd ?
	y db ?
	i dw ?
	j dw ?
	k dw ?
DEFINE_SCAN_NUM
