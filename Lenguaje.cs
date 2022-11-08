/*
    Morales Almeida David
*/
//Requerimiento 1: Actualización:
//                 a) Agregar el residuo de la division en el PorFactor
//                 b) Agregar en instruccion los incrementos de término y los incrementos de Factor
//                   a++, a--, a += 1, a -= 1, a *= 1, a /= 1, a %= 1
//                   en donde el 1 puede ser una Expresion
//                 c) Programar el destructor para ejecutar el metodo close()
//
//Requerimiento 2: Actualización:
//                 a) Marcar errores semánticos cuando los incrementos de término o incrementos de factor superen el rango de la variable.
//                 b) Considerar el inciso b) y c) para el for.
//                 c) Hacer funcionar el do y el While.
//
//Requerimiento 3: Actualización:
//                 a) Considerar las variables y los casteos de las expresiones matematicas en ensamblador.
//                 b) Considerar el residuo de la division en ensamblador.
//                 c) Programar el Print y el Scanf en ensamblador.
//
//Requerimiento 4: Actualización:
//                 a) Programar el else en ensamblador.
//                 b) Programar el for en ensamblador.
//
//Requerimiento 5: Actualización:
//                 a) Programar el while en ensamblador.
//                 b) Programar el do en ensamblador.

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Semantica
{
    public class Lenguaje : Sintaxis, IDisposable
    {
        List<Variable> listaVariables = new List<Variable>();
        Stack<float> stackOperandos = new Stack<float>();
        Variable.TipoDato dominante;
        int cIf, cElse, cFor, cWhile, cDo;
        public Lenguaje()
        {
            cIf = cElse = cFor = cWhile = cDo = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = cElse = cFor = cWhile = cDo = 0;
        }
        public void Dispose()
        {
            Console.WriteLine("\nDestructor");
            close();
            GC.SuppressFinalize(this);
        }
        ~Lenguaje()
        {
            Dispose();
        }
        private void addVariable(string name, Variable.TipoDato type)
        {
            listaVariables.Add(new Variable(name, type));
        }
        private void displayVariables()
        {
            Log.WriteLine("\n\nVariables: ");
            foreach (Variable v in listaVariables)
            {
                Log.WriteLine(v.getNombre() + " " + v.getTipo() + " " + v.getValue());
            }
        }
        private void variablesAsm()
        {
            asm.WriteLine("\n; Variables:");
            foreach (Variable v in listaVariables)
            {
                asm.Write("\t" + v.getNombre());
                switch (v.getTipo())
                {
                    case Variable.TipoDato.Char:
                        asm.WriteLine(" db ?");
                        break;
                    case Variable.TipoDato.Int:
                        asm.WriteLine(" dw ?");
                        break;
                    default:
                        asm.WriteLine(" dd ?");
                        break;
                }
            }
        }
        private bool existeVariable(string name)
        {
            foreach (Variable v in listaVariables)
            {
                if (v.getNombre().Equals(name))
                    return true;
            }
            return false;
        }
        private void modValor(string name, float newValue)
        {
            foreach (Variable v in listaVariables)
            {
                if (v.getNombre().Equals(name))
                {
                    v.setValor(newValue);
                    break;
                }
            }
        }
        private float getValor(string nameVariable)
        {
            float value = 0;
            foreach (Variable v in listaVariables)
            {
                if (v.getNombre().Equals(nameVariable))
                {
                    value = v.getValue();
                    break;
                }
            }
            return value;
        }
        private Variable.TipoDato getType(string nameVariable)
        {
            foreach (Variable v in listaVariables)
            {
                if (v.getNombre().Equals(nameVariable))
                {
                    return v.getTipo();
                }
            }
            return Variable.TipoDato.Char;
        }
        private float convert(float value, Variable.TipoDato cast)
        {
            if (evaluaNumero(value) == Variable.TipoDato.Float && cast != Variable.TipoDato.Float)
                value = value - (value % 1);
            switch (cast)
            {
                case Variable.TipoDato.Char:
                    value = value % 256;
                    break;
                case Variable.TipoDato.Int:
                    value = value % 65536;
                    break;
            }
            return value;
        }
        // Programa -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("#make_COM#");
            asm.WriteLine("include emu8086.inc");
            asm.WriteLine("ORG 100h");
            Librerias();
            Variables();
            Main();
            displayVariables();
            asm.WriteLine("RET");
            variablesAsm();
            asm.WriteLine("DEFINE_SCAN_NUM");
            // asm.WriteLine("END");
        }
        // Librerias -> #include<identificador(.h)?> Librerias?
        private void Librerias()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Librerias();
            }
        }
        // Variables -> tipoDato Lista_identificadores ; Variables?
        private void Variables()
        {
            if (getClasificacion() == tipos.TipoDato)
            {
                Variable.TipoDato type = Variable.TipoDato.Char;
                switch (getContenido())
                {
                    case "int": type = Variable.TipoDato.Int; break;
                    case "float": type = Variable.TipoDato.Float; break;
                }
                match(tipos.TipoDato);
                Lista_identificadores(type);
                match(tipos.FinSentencia);
                Variables();
            }
        }
        // Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato type)
        {
            if (getClasificacion() == Token.tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                    addVariable(getContenido(), type);
                else
                    throw new Error("Error de sintáxis. Variable duplicada \"" + getContenido() + "\" en la línea " + linea + ".", Log);
            }
            match(tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(type);
            }
        }
        // Main -> void main() Bloque_Instrucciones 
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            Bloque_Instrucciones(true, false);
        }
        // Bloque_Instrucciones -> {Lista_Instrucciones?}
        private void Bloque_Instrucciones(bool evaluacion, bool ejecutado)
        {
            match("{");
            if (getContenido() != "}")
            {
                Lista_Instrucciones(evaluacion, ejecutado);
            }
            match("}");
        }
        // Lista_Instrucciones -> Instruccion Lista_Instrucciones?
        private void Lista_Instrucciones(bool evaluacion, bool ejecutado)
        {
            Instruccion(evaluacion, ejecutado);
            if (getContenido() != "}")
            {
                Lista_Instrucciones(evaluacion, ejecutado);
            }
        }
        // Instruccion -> Printf | Scanf | If | While | Do | For | Switch | Asignacion
        private void Instruccion(bool evaluacion, bool ejecutado)
        {
            if (getContenido() == "printf")
                Printf(evaluacion, ejecutado);
            else if (getContenido() == "scanf")
                Scanf(evaluacion, ejecutado);
            else if (getContenido() == "if")
                If(evaluacion, ejecutado);
            else if (getContenido() == "while")
                While(evaluacion, ejecutado);
            else if (getContenido() == "do")
                Do(evaluacion, ejecutado);
            else if (getContenido() == "for")
                For(evaluacion, ejecutado);
            else if (getContenido() == "switch")
                Switch(evaluacion, ejecutado);
            else
            {
                Asignacion(evaluacion, ejecutado);
                //Console.WriteLine("Error de sintaxis. No se reconoce la instruccion: " + getContenido());
                //nextToken();
            }
        }
        private Variable.TipoDato evaluaNumero(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if (resultado <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if (resultado <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }
        private bool evaluaSemantica(string variable, float resultado)
        {
            if (evaluaNumero(resultado) <= getType(variable))
                return true;
            return false;
        }
        // Asignacion -> identificador = cadena | Expresion ;
        private void Asignacion(bool evaluacion, bool ejecutado)
        {
            if (!existeVariable(getContenido()))
                throw new Error("\nError de sintaxis en linea " + linea + ". No existe la variable \"" + getContenido() + "\"", Log);
            string name = getContenido();
            match(tipos.Identificador);
            if (getClasificacion() == tipos.IncrementoTermino || getClasificacion() == tipos.IncrementoFactor)
            {
                Incremento(name, evaluacion, ejecutado);
                match(";");
                //Requerimiento 1.b
                //Requetimiento 1.c
            }
            else
            {
                //Log.WriteLine();
                //Log.Write(name + " = ");
                match("=");
                dominante = Variable.TipoDato.Char;
                Expresion(ejecutado);
                match(";");
                float resultado = stackOperandos.Pop();
                if (!ejecutado)
                    asm.WriteLine("POP AX");
                //Log.Write("= " + resultado);
                //Log.WriteLine();
                //Console.WriteLine(dominante);
                //Console.WriteLine(evaluaNumero(resultado));
                if (dominante < evaluaNumero(resultado))
                {
                    dominante = evaluaNumero(resultado);
                }
                if (dominante <= getType(name))
                {
                    if (evaluacion)
                    {
                        modValor(name, resultado);
                    }
                }
                else
                {
                    throw new Error("Error de semantica en la linea " + linea + ". No se puede asignar un <" + dominante + "> a un <" + getType(name) + ">", Log);
                }
                if (!ejecutado)
                    asm.WriteLine("MOV " + name + ", AX");
            }
        }
        // Printf -> printf (string | Expresion);
        private void Printf(bool evaluacion, bool ejecutado)
        {
            match("printf");
            match("(");
            if (getClasificacion() == tipos.Cadena)
            {
                string contenido = Regex.Unescape(getContenido().Remove(0, 1).Remove(getContenido().Length - 2));
                if (evaluacion)
                {
                    Console.Write(contenido);
                }
                if (!ejecutado)
                {
                    switch (contenido)
                    {
                        case "\n":
                            asm.WriteLine("PRINTN \"\"");
                            break;
                        default:
                            asm.WriteLine("PRINT \"" + contenido + "\"");
                            break;
                    }
                }
                match(tipos.Cadena);
            }
            else
            {
                Expresion(ejecutado);
                float resultado = stackOperandos.Pop();
                if (!ejecutado)
                {
                    // Codigo ensamblador para imprimir una variable
                    asm.WriteLine("POP AX");
                    asm.WriteLine("PRINT AX");
                }
                if (evaluacion)
                {
                    Console.Write(resultado);
                }
            }
            match(")");
            match(";");
        }
        // Scanf -> scanf (string, &Identificador);
        private void Scanf(bool evaluacion, bool ejecutado)
        {
            match("scanf");
            match("(");
            match(tipos.Cadena);
            match(",");
            match("&");
            string name = getContenido();
            match(tipos.Identificador);
            if (!existeVariable(name))
                throw new Error("\nError de sintaxis en linea " + linea + ". No existe la variable \"" + name + "\"", Log);
            if (evaluacion)
            {
                string value = "" + Console.ReadLine();
                try
                {
                    modValor(name, float.Parse(value));
                }
                catch (Exception)
                {
                    throw new Error("Error de semantica en linea " + linea + ". No se puede asignar \"" + value + "\" a un <" + getType(name) + ">", Log);
                }
            }
            match(")");
            match(";");
            if (!ejecutado)
            {
                asm.WriteLine("CALL SCAN_NUM");
                asm.WriteLine("MOV " + name + ", CX");
            }
        }
        // If -> if (Condicion) Bloque_Instrucciones (else Bloque_Instrucciones)?
        private void If(bool evaluacion, bool ejecutado)
        {
            string etiquetaIf = "if" + ++cIf;
            string etiquetaElse = "else" + ++cElse;
            match("if");
            match("(");
            bool validarIf = Condicion(etiquetaIf, ejecutado);
            match(")");
            if (getContenido() == "{")
                Bloque_Instrucciones(validarIf && evaluacion, ejecutado);
            else
                Instruccion(validarIf && evaluacion, ejecutado);
            if (getContenido() == "else")
            {
                if (!ejecutado)
                {
                    asm.WriteLine("JMP " + etiquetaElse);
                    asm.WriteLine(etiquetaIf + ":");
                }
                match("else");
                if (getContenido() == "{")
                    Bloque_Instrucciones(!validarIf && evaluacion, ejecutado);
                else
                    Instruccion(!validarIf && evaluacion, ejecutado);
                if (!ejecutado)
                    asm.WriteLine(etiquetaElse + ":");
            }
            else
            {
                if (!ejecutado)
                    asm.WriteLine(etiquetaIf + ":");
            }
        }
        // While -> while(Condicion) Bloque_Instrucciones | Instruccion
        private void While(bool evaluacion, bool ejecutado)
        {
            string inicioWhile = "while" + cWhile;
            string finWhile = "finWhile" + cWhile++;
            match("while");
            match("(");
            int posicionAct = position - 1;
            int lineaAct = linea;
            bool validarWhile;
            if (!ejecutado)
                asm.WriteLine(inicioWhile + ":");
            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(posicionAct, SeekOrigin.Begin);
                position = posicionAct;
                linea = lineaAct;
                nextToken();
                validarWhile = Condicion(finWhile, ejecutado);
                match(")");
                if (getContenido() == "{")
                    Bloque_Instrucciones(evaluacion && validarWhile, ejecutado);
                else
                    Instruccion(evaluacion && validarWhile, ejecutado);
                if (!ejecutado)
                {
                    asm.WriteLine("JMP " + inicioWhile);
                    asm.WriteLine(finWhile + ":");
                    ejecutado = true;
                }
            } while (evaluacion && validarWhile);
        }
        // Do -> do Bloque_Instrucciones | Instruccion while(Condicion);
        private void Do(bool evaluacion, bool ejecutado)
        {
            string inicioDo = "do" + cDo;
            string finDo = "finDo" + cDo++;
            match("do");
            int posicionAct = position - 1;
            int lineaAct = linea;
            bool validarDo = true;
            if (!ejecutado)
                asm.WriteLine(inicioDo + ":");
            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(posicionAct, SeekOrigin.Begin);
                position = posicionAct;
                linea = lineaAct;
                nextToken();
                if (getContenido() == "{")
                    Bloque_Instrucciones(evaluacion && validarDo, ejecutado);
                else
                    Instruccion(evaluacion && validarDo, ejecutado);
                match("while");
                match("(");
                validarDo = Condicion(finDo, ejecutado);
                match(")");
                match(";");
                if (!ejecutado)
                {
                    asm.WriteLine("JMP " + inicioDo);
                    asm.WriteLine(finDo + ":");
                    ejecutado = true;
                }
            } while (evaluacion && validarDo);
        }
        // For -> for (Asignacion Condición ; Incremento) Bloque_Instrucciones | Instruccion
        private void For(bool evaluacion, bool ejecutado)
        {
            string inicioFor = "for" + cFor;
            string finFor = "endfor" + cFor++;
            if (!ejecutado)
                asm.WriteLine(inicioFor + ":");
            match("for");
            match("(");
            Asignacion(evaluacion, ejecutado);
            int posicionAct = position - 1;
            int lineaAct = linea;
            string name;
            float cambio = 0;
            bool validarFor;
            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(posicionAct, SeekOrigin.Begin);
                position = posicionAct;
                linea = lineaAct;
                nextToken();
                validarFor = Condicion(finFor, ejecutado);
                match(";");
                name = getContenido();
                match(tipos.Identificador);
                if (!existeVariable(name))
                    throw new Error("\nError de sintaxis en linea " + linea + ". No existe la variable \"" + name + "\"", Log);
                cambio = Incremento(name, false, ejecutado);
                if (evaluacion && validarFor)
                {
                    if (!evaluaSemantica(name, cambio))
                        throw new Error("Error de semantica en linea " + linea + ". No se puede asignar un <" + evaluaNumero(cambio) + "> a un <" + getType(name) + ">", Log);
                }
                match(")");
                if (getContenido() == "{")
                    Bloque_Instrucciones(evaluacion && validarFor, ejecutado);
                else
                    Instruccion(evaluacion && validarFor, ejecutado);
                if (evaluacion && validarFor)
                    modValor(name, cambio);
                // Requerimiento 1.d:
                if (!ejecutado)
                {
                    asm.WriteLine("JMP " + inicioFor);
                    asm.WriteLine(finFor + ":");
                    ejecutado = true;
                }
            } while (evaluacion && validarFor);
        }
        // Incremento -> identificador ++ | --
        private float Incremento(string variable, bool evaluacion, bool ejecutado)
        {
            float cambio = 0;
            if (getClasificacion() == tipos.IncrementoTermino)
            {
                if (getContenido() == "++")
                {
                    match("++");
                    cambio = getValor(variable) + 1;
                }
                else if (getContenido() == "+=")
                {
                    match("+=");
                    Expresion(ejecutado);
                    float resultado = stackOperandos.Pop();
                    cambio = getValor(variable) + resultado;
                }
                else if (getContenido() == "--")
                {
                    match("--");
                    cambio = getValor(variable) - 1;
                }
                else
                {
                    match("-=");
                    Expresion(ejecutado);
                    float resultado = stackOperandos.Pop();
                    cambio = getValor(variable) - resultado;
                }
            }
            else if (getClasificacion() == tipos.IncrementoFactor)
            {
                if (getContenido() == "*=")
                {
                    match("*=");
                    Expresion(ejecutado);
                    float resultado = stackOperandos.Pop();
                    cambio = getValor(variable) * resultado;
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    Expresion(ejecutado);
                    float resultado = stackOperandos.Pop();
                    cambio = getValor(variable) / resultado;
                }
                else
                {
                    match("%=");
                    Expresion(ejecutado);
                    float resultado = stackOperandos.Pop();
                    cambio = getValor(variable) % resultado;
                }
            }
            else
                throw new Error("\nError de sintaxis en linea " + linea + ". Se esperaba un " + tipos.IncrementoFactor + " o " + tipos.IncrementoTermino, Log);
            if (evaluacion)
            {
                if (!evaluaSemantica(variable, cambio))
                    throw new Error("Error de semantica en linea " + linea + ". No se puede asignar un <" + evaluaNumero(cambio) + "> a un <" + getType(variable) + ">", Log);
                modValor(variable, cambio);
            }
            return cambio;
        }
        // Switch -> switch (Expresion) { Lista_Casos (default: (Lista_Instrucciones_Case | Bloque_Instrucciones)? (break;)? )? }
        private void Switch(bool evaluacion, bool ejecutado)
        {
            match("switch");
            match("(");
            Expresion(ejecutado);
            stackOperandos.Pop();
            if (!ejecutado)
                asm.WriteLine("POP AX");
            match(")");
            match("{");
            Lista_Casos(evaluacion, ejecutado);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() != "}" && getContenido() != "{")
                    Lista_Instrucciones_Case(evaluacion, ejecutado);
                else if (getContenido() == "{")
                    Bloque_Instrucciones(evaluacion, ejecutado);
                if (getContenido() == "break")
                {
                    match("break");
                    match(";");
                }
            }
            match("}");
        }
        // Lista_Casos -> case Expresion: (Lista_Instrucciones_Case | Bloque_Instrucciones)? (break;)? (Lista_Casos)?
        private void Lista_Casos(bool evaluacion, bool ejecutado)
        {
            if (getContenido() != "}" && getContenido() != "default")
            {
                match("case");
                Expresion(ejecutado);
                stackOperandos.Pop();
                if (!ejecutado)
                    asm.WriteLine("POP AX");
                match(":");
                if (getContenido() != "case" && getContenido() != "{")
                    Lista_Instrucciones_Case(evaluacion, ejecutado);
                else if (getContenido() == "{")
                    Bloque_Instrucciones(evaluacion, ejecutado);
                if (getContenido() == "break")
                {
                    match("break");
                    match(";");
                }
                Lista_Casos(evaluacion, ejecutado);
            }
        }
        // Lista_Instrucciones_Case -> Instruccion Lista_Instrucciones_Case?
        private void Lista_Instrucciones_Case(bool evaluacion, bool ejecutado)
        {
            Instruccion(evaluacion, ejecutado);
            if (getContenido() != "break" && getContenido() != "case" && getContenido() != "default" && getContenido() != "}")
                Lista_Instrucciones_Case(evaluacion, ejecutado);
        }
        // Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion(string etiquetaIf, bool ejecutado)
        {
            Expresion(ejecutado);
            string operador = getContenido();
            match(tipos.OperadorRelacional);
            Expresion(ejecutado);
            float e2 = stackOperandos.Pop();
            if (!ejecutado)
                asm.WriteLine("POP BX");
            float e1 = stackOperandos.Pop();
            if (!ejecutado)
            {
                asm.WriteLine("POP AX");
                asm.WriteLine("CMP AX, BX");
            }
            switch (operador)
            {
                case "==":
                    if (!ejecutado)
                        asm.WriteLine("JNE " + etiquetaIf);
                    return e1 == e2;
                case ">":
                    if (!ejecutado)
                        asm.WriteLine("JLE " + etiquetaIf);
                    return e1 > e2;
                case ">=":
                    if (!ejecutado)
                        asm.WriteLine("JL " + etiquetaIf);
                    return e1 >= e2;
                case "<":
                    if (!ejecutado)
                        asm.WriteLine("JGE " + etiquetaIf);
                    return e1 < e2;
                case "<=":
                    if (!ejecutado)
                        asm.WriteLine("JG " + etiquetaIf);
                    return e1 <= e2;
                default:
                    if (!ejecutado)
                        asm.WriteLine("JE " + etiquetaIf);
                    return e1 != e2;
            }
        }
        // Expresion -> Termino MasTermino
        private void Expresion(bool ejecutado)
        {
            Termino(ejecutado);
            MasTermino(ejecutado);
        }
        // MasTermino -> (operadorTermino Termino)?
        private void MasTermino(bool ejecutado)
        {
            if (getClasificacion() == tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(tipos.OperadorTermino);
                Termino(ejecutado);
                // Log.Write(operador + " ");
                float n1 = stackOperandos.Pop();
                if (!ejecutado)
                    asm.WriteLine("POP BX");
                float n2 = stackOperandos.Pop();
                if (!ejecutado)
                    asm.WriteLine("POP AX");
                switch (operador)
                {
                    case "+":
                        stackOperandos.Push(n2 + n1);
                        if (!ejecutado)
                        {
                            asm.WriteLine("ADD AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "-":
                        stackOperandos.Push(n2 - n1);
                        if (!ejecutado)
                        {
                            asm.WriteLine("SUB AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                }
            }
        }
        // Termino -> Factor PorFactor
        private void Termino(bool ejecutado)
        {
            Factor(ejecutado);
            PorFactor(ejecutado);
        }
        // PorFactor -> (operadorFactor Factor)?
        private void PorFactor(bool ejecutado)
        {
            if (getClasificacion() == tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(tipos.OperadorFactor);
                Factor(ejecutado);
                //Log.Write(operador + " ");
                float n1 = stackOperandos.Pop();
                if (!ejecutado)
                    asm.WriteLine("POP BX");
                float n2 = stackOperandos.Pop();
                if (!ejecutado)
                    asm.WriteLine("POP AX");
                // Requerimiento 1.a
                switch (operador)
                {
                    case "*":
                        stackOperandos.Push(n2 * n1);
                        if (!ejecutado)
                        {
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "/":
                        stackOperandos.Push(n2 / n1);
                        if (!ejecutado)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "%":
                        stackOperandos.Push(n2 % n1);
                        if (!ejecutado)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH DX");
                        }
                        break;
                }
            }
        }
        // Factor -> numero | identificador | (Expresion)
        private void Factor(bool ejecutado)
        {
            if (getClasificacion() == tipos.Numero)
            {
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                //Log.Write(getContenido() + " ");
                stackOperandos.Push(float.Parse(getContenido()));
                if (!ejecutado)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }
                match(tipos.Numero);
            }
            else if (getClasificacion() == tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                    throw new Error("\nError de sintaxis en linea " + linea + ". No existe la variable \"" + getContenido() + "\"", Log);
                if (dominante < getType(getContenido()))
                {
                    dominante = getType(getContenido());
                }
                //Log.Write(getContenido() + " ");
                stackOperandos.Push(getValor(getContenido()));
                // Requerimiento 3.a
                if (!ejecutado)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }
                match(tipos.Identificador);
            }
            else
            {
                bool huboCast = false;
                Variable.TipoDato cast = Variable.TipoDato.Char;
                match("(");
                if (getClasificacion() == tipos.TipoDato)
                {
                    huboCast = true;
                    switch (getContenido())
                    {
                        case "char":
                            cast = Variable.TipoDato.Char;
                            break;
                        case "int":
                            cast = Variable.TipoDato.Int;
                            break;
                        case "float":
                            cast = Variable.TipoDato.Float;
                            break;
                    }
                    match(tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion(ejecutado);
                match(")");
                if (huboCast)
                {
                    dominante = cast;
                    float value = convert(stackOperandos.Pop(), cast);
                    if (!ejecutado)
                    {
                        asm.WriteLine("POP AX");
                        asm.WriteLine("PUSH " + value);
                    }
                    stackOperandos.Push(value);
                }
                // Requerimiento 3.a
            }
        }
    }
}