; Archivo: prueba.cpp
; Compilado: El 09/11/2022 a las 07:33:51
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
MOV AX, altura
PUSH AX
POP AX
MOV i, AX
for0:
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
POP AX
ADD j, AX
JMP while0
finWhile0:
PRINT "
"
POP AX
SUB i, AX
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
POP AX
ADD k, AX
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
PRINT "
"
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
JE if58
PRINT "Esto no se debe imprimir"
MOV AX, 2
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if59
PRINT "Esto tampoco"
if59:
if58:
MOV AX, 258
PUSH AX
POP AX
MOV a, AX
PRINT "Valor de variable int 'a' antes del casteo: "
MOV AX, a
PUSH AX
POP AX
CALL PRINT_NUM
MOV AX, a
PUSH AX
POP AX
MOV AH, 0
PUSH AX
POP AX
MOV y, AL
PRINT "
Valor de variable char 'y' despues del casteo de a: "
MOV AL, y
PUSH AX
POP AX
CALL PRINT_NUM
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
DEFINE_PRINT_NUM
DEFINE_SCAN_NUM_UNS
DEFINE_SCAN_NUM
