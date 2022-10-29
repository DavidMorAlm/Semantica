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
    public class Lenguaje : Sintaxis
    {
        List<Variable> listaVariables = new List<Variable>();
        Stack<float> stackOperandos = new Stack<float>();
        Variable.TipoDato dominante;
        int cIf, cFor;
        public Lenguaje()
        {
            cIf = cFor = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = cFor = 0;
        }
        ~Lenguaje()
        {
            Console.WriteLine("Destructor");
            close();
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
            asm.WriteLine("\n; Variables:\n");
            foreach (Variable v in listaVariables)
            {
                asm.WriteLine("\t" + v.getNombre() + " dw ?");
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
            asm.WriteLine("ORG 1000h");
            Librerias();
            Variables();
            variablesAsm();
            Main();
            displayVariables();
            asm.WriteLine("RET");
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
            Bloque_Instrucciones(true);
        }
        // Bloque_Instrucciones -> {Lista_Instrucciones?}
        private void Bloque_Instrucciones(bool evaluacion)
        {
            match("{");
            if (getContenido() != "}")
            {
                Lista_Instrucciones(evaluacion);
            }
            match("}");
        }
        // Lista_Instrucciones -> Instruccion Lista_Instrucciones?
        private void Lista_Instrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "}")
            {
                Lista_Instrucciones(evaluacion);
            }
        }
        // Instruccion -> Printf | Scanf | If | While | Do | For | Switch | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "printf")
                Printf(evaluacion);
            else if (getContenido() == "scanf")
                Scanf(evaluacion);
            else if (getContenido() == "if")
                If(evaluacion);
            else if (getContenido() == "while")
                While(evaluacion);
            else if (getContenido() == "do")
                Do(evaluacion);
            else if (getContenido() == "for")
                For(evaluacion);
            else if (getContenido() == "switch")
                Switch(evaluacion);
            else
            {
                Asignacion(evaluacion);
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
            Variable.TipoDato type = getType(variable);
            return false;
        }
        // Asignacion -> identificador = cadena | Expresion ;
        private void Asignacion(bool evaluacion)
        {
            if (!existeVariable(getContenido()))
                throw new Error("\nError de sintaxis en linea " + linea + ". No existe la variable \"" + getContenido() + "\"", Log);
            string name = getContenido();
            match(tipos.Identificador);
            if (getClasificacion() == tipos.IncrementoTermino || getClasificacion() == tipos.IncrementoFactor)
            {
                Incremento(name, evaluacion);
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
                Expresion();
                match(";");
                float resultado = stackOperandos.Pop();
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
                asm.WriteLine("MOV " + name + ", AX");
            }
        }
        // Printf -> printf (string | Expresion);
        private void Printf(bool evaluacion)
        {
            match("printf");
            match("(");
            if (getClasificacion() == tipos.Cadena)
            {
                string contenido = getContenido();
                if (evaluacion)
                {
                    contenido = Regex.Unescape(contenido.Remove(0, 1).Remove(contenido.Length - 2));
                    Console.Write(contenido);
                }
                asm.WriteLine("PRINTN \"" + contenido + "\"");
                match(tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stackOperandos.Pop();
                asm.WriteLine("POP AX");
                if (evaluacion)
                {
                    // Codigo ensamblador para imprimir una variable
                    Console.Write(resultado);
                }
            }
            match(")");
            match(";");
        }
        // Scanf -> scanf (string, &Identificador);
        private void Scanf(bool evaluacion)
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
                    throw new Error("Error de semantica en linea " + linea + ". No se puede asignar \"" + value + "\" a un <" + getType(getContenido()) + ">", Log);
                }
            }
            match(")");
            match(";");
            asm.WriteLine("CALL SCAN_NUM");
            asm.WriteLine("MOV " + name + ", CX");
        }
        // If -> if (Condicion) Bloque_Instrucciones (else Bloque_Instrucciones)?
        private void If(bool evaluacion)
        {
            string etiquetaIf = "if" + ++cIf;
            match("if");
            match("(");
            bool validarIf = Condicion(etiquetaIf);
            match(")");
            if (getContenido() == "{")
                Bloque_Instrucciones(validarIf && evaluacion);
            else
                Instruccion(validarIf && evaluacion);
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                    Bloque_Instrucciones(!validarIf && evaluacion);
                else
                    Instruccion(!validarIf && evaluacion);
            }
            asm.WriteLine(etiquetaIf + ":");
        }
        // While -> while(Condicion) Bloque_Instrucciones | Instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            bool validarWhile = Condicion("");
            match(")");
            if (getContenido() == "{")
                Bloque_Instrucciones(evaluacion && validarWhile);
            else
                Instruccion(evaluacion && validarWhile);
        }
        // Do -> do Bloque_Instrucciones | Instruccion while(Condicion);
        private void Do(bool evaluacion)
        {
            match("do");
            if (getContenido() == "{")
                Bloque_Instrucciones(evaluacion);
            else
                Instruccion(evaluacion);
            match("while");
            match("(");
            bool validarDo = Condicion("");
            match(")");
            match(";");
        }
        // For -> for (Asignacion Condición ; Incremento) Bloque_Instrucciones | Instruccion
        private void For(bool evaluacion)
        {
            string inicioFor = "for" + cFor;
            string finFor = "endfor" + cFor++;
            asm.WriteLine(inicioFor + ":");
            match("for");
            match("(");
            Asignacion(evaluacion);
            int posicionAct = position - 1;
            int lineaAct = linea;
            string name;
            int cambio = 0;
            bool validarFor = Condicion("");
            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(posicionAct, SeekOrigin.Begin);
                position = posicionAct;
                linea = lineaAct;
                nextToken();
                validarFor = Condicion("");
                match(";");
                name = getContenido();
                cambio = Incremento();
                match(")");
                if (getContenido() == "{")
                    Bloque_Instrucciones(evaluacion && validarFor);
                else
                    Instruccion(evaluacion && validarFor);
                if (evaluacion && validarFor)
                    modValor(name, getValor(name) + cambio);
                // Requerimiento 1.d:
            } while (evaluacion && validarFor);
            asm.WriteLine(finFor + ":");
        }
        // Incremento -> identificador ++ | --
        private int Incremento()
        {
            //if (!existeVariable(getContenido()))
            //    throw new Error("\nError de sintaxis en linea " + linea + ". No existe la variable \"" + getContenido() + "\"", Log);
            //string variable = getContenido();
            int cambio = 0;
            match(tipos.Identificador);
            if (getClasificacion() == tipos.IncrementoTermino)
            {
                if (getContenido()[0] == '+')
                {
                    match("++"); cambio = 1;
                    //if (evaluacion)
                    //    modValor(variable, getValor(variable) + 1);
                }
                else
                {
                    match("--"); cambio = -1;
                    //if (evaluacion)
                    //    modValor(variable, getValor(variable) - 1);
                }
            }
            else
                match(tipos.IncrementoTermino);
            return cambio;
        }
        private void Incremento(string variable, bool evaluacion)
        {
            if (getClasificacion() == tipos.IncrementoTermino)
            {
                if (getContenido()[0] == '+')
                {
                    match("++");
                    if (evaluacion)
                        modValor(variable, getValor(variable) + 1);
                }
                else
                {
                    match("--");
                    if (evaluacion)
                        modValor(variable, getValor(variable) - 1);
                }
            }
            else
                match(tipos.IncrementoTermino);
        }
        // Switch -> switch (Expresion) { Lista_Casos (default: (Lista_Instrucciones_Case | Bloque_Instrucciones)? (break;)? )? }
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stackOperandos.Pop();
            asm.WriteLine("POP AX");
            match(")");
            match("{");
            Lista_Casos(evaluacion);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() != "}" && getContenido() != "{")
                    Lista_Instrucciones_Case(evaluacion);
                else if (getContenido() == "{")
                    Bloque_Instrucciones(evaluacion);
                if (getContenido() == "break")
                {
                    match("break");
                    match(";");
                }
            }
            match("}");
        }
        // Lista_Casos -> case Expresion: (Lista_Instrucciones_Case | Bloque_Instrucciones)? (break;)? (Lista_Casos)?
        private void Lista_Casos(bool evaluacion)
        {
            if (getContenido() != "}" && getContenido() != "default")
            {
                match("case");
                Expresion();
                stackOperandos.Pop();
                asm.WriteLine("POP AX");
                match(":");
                if (getContenido() != "case" && getContenido() != "{")
                    Lista_Instrucciones_Case(evaluacion);
                else if (getContenido() == "{")
                    Bloque_Instrucciones(evaluacion);
                if (getContenido() == "break")
                {
                    match("break");
                    match(";");
                }
                Lista_Casos(evaluacion);
            }
        }
        // Lista_Instrucciones_Case -> Instruccion Lista_Instrucciones_Case?
        private void Lista_Instrucciones_Case(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "break" && getContenido() != "case" && getContenido() != "default" && getContenido() != "}")
                Lista_Instrucciones_Case(evaluacion);
        }
        // Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion(string etiquetaIf)
        {
            Expresion();
            string operador = getContenido();
            match(tipos.OperadorRelacional);
            Expresion();
            float e2 = stackOperandos.Pop();
            asm.WriteLine("POP BX");
            float e1 = stackOperandos.Pop();
            asm.WriteLine("POP AX");
            asm.WriteLine("CMP AX, BX");
            switch (operador)
            {
                case "==":
                    asm.WriteLine("JNE " + etiquetaIf);
                    return e1 == e2;
                case ">":
                    asm.WriteLine("JLE " + etiquetaIf);
                    return e1 > e2;
                case ">=":
                    asm.WriteLine("JL " + etiquetaIf);
                    return e1 >= e2;
                case "<":
                    asm.WriteLine("JGE " + etiquetaIf);
                    return e1 < e2;
                case "<=":
                    asm.WriteLine("JG " + etiquetaIf);
                    return e1 <= e2;
                default:
                    asm.WriteLine("JE " + etiquetaIf);
                    return e1 != e2;
            }
        }
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (operadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(tipos.OperadorTermino);
                Termino();
                // Log.Write(operador + " ");
                float n1 = stackOperandos.Pop();
                asm.WriteLine("POP BX");
                float n2 = stackOperandos.Pop();
                asm.WriteLine("POP AX");
                switch (operador)
                {
                    case "+":
                        stackOperandos.Push(n2 + n1);
                        asm.WriteLine("ADD AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "-":
                        stackOperandos.Push(n2 - n1);
                        asm.WriteLine("SUB AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                }
            }
        }
        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (operadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(tipos.OperadorFactor);
                Factor();
                //Log.Write(operador + " ");
                float n1 = stackOperandos.Pop();
                asm.WriteLine("POP BX");
                float n2 = stackOperandos.Pop();
                asm.WriteLine("POP AX");
                // Requerimiento 1.a
                switch (operador)
                {
                    case "*":
                        stackOperandos.Push(n2 * n1);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "/":
                        stackOperandos.Push(n2 / n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "%":
                        stackOperandos.Push(n2 % n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH DX");
                        break;
                }
            }
        }
        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == tipos.Numero)
            {
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                //Log.Write(getContenido() + " ");
                stackOperandos.Push(float.Parse(getContenido()));
                asm.WriteLine("MOV AX, " + getContenido());
                asm.WriteLine("PUSH AX");
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
                Expresion();
                match(")");
                if (huboCast)
                {
                    dominante = cast;
                    float value = convert(stackOperandos.Pop(), cast);
                    asm.WriteLine("POP AX");
                    stackOperandos.Push(value);
                }
                // Requerimiento 3.a
            }
        }
    }
}