﻿/*
    Morales Almeida David
*/
using System;

namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lenguaje a = new Lenguaje())
                a.Programa();
                /*while(!a.findArchivo())
                {
                    a.nextToken();
                }*/
                //a.close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //Console.WriteLine("Fin de compilacion.");
            }
        }
    }
}