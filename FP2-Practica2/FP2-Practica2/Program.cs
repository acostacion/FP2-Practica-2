// Andrea Aparicio López.
// José Tomás Gómez Becerra.

using System;

namespace FP2P2
{
    internal class Program
    {
        const int nivelInicial = 1;
        const int nivelFinal = 9;
        static void Main(string[] args)
        {
            Console.WriteLine($"{"",8}PACMAN");
            Console.Write($"Pulsa Enter para comenzar");
  
            bool start;
            do
            {
                start = Console.ReadLine().ToLower() == "";
            }
            while (!start);

            while(start)
            {
                // Nivel inicial.
                int num = nivelInicial;
                bool capturado = false;

                while (num < 10 && start)
                {
                    // Cargado de nivel y renderizado inicial
                    string file = $"levels/level0{num}.dat";
                    Tablero t = new Tablero(file);
                    t.Render();

                    int lap = 200; // retardo para bucle ppal
                    char c = ' ';
                    bool pausa = false;
                    bool abortar = false;

                    while (!capturado && !t.FinNivel() && !abortar)
                    {
                        // input de usuario
                        LeeInput(ref c);
                        abortar = c == 'q';
                        if(!abortar)
                        {
                            Pausa(ref pausa, ref c);
                            if (!pausa)
                            {
                                // procesamiento del input
                                if (c != ' ' && t.CambiaDir(c)) c = ' ';

                                t.MuevePacman();
                                capturado = t.Captura();

                                if (!capturado)
                                {
                                    // IA de los fantasmas
                                    t.MueveFantasmas(ref t.lapFantasmas);
                                    capturado = t.Captura();
                                }

                                // rederizado
                                t.Render();

                                // retardo
                                System.Threading.Thread.Sleep(lap);
                            }
                        }
                        
                    }

                    Console.Clear();
                    if (capturado) // Si te han comido
                    {
                        Console.WriteLine("¡Te han comido!");
                        Console.Write("¿Quieres volver a empezar? S/N");
                        start = Console.ReadLine().ToLower() == "s";
                        num = nivelInicial;
                    }
                    else if (abortar) // Si has abortado
                    {
                        start = false;
                        Console.WriteLine("Partida abortada");
                    }
                    else // Si has ganado
                    {
                        Console.Write("¡Victoria!");
                        if (num < nivelFinal)
                        {
                            Console.Write("Pulsa Enter para avanzar al siguiente nivel");
                            if (Console.ReadLine().ToLower() == "") num++;
                        }
                    }
                    capturado = false;
                }
                if(num >= nivelFinal && start) {

                    Console.WriteLine("¡Has terminado todos los niveles!");
                    Console.Write("¿Quieres volver a empezar? S/N");
                    start = Console.ReadLine().ToLower() == "s";
                }
            }

            Console.Write("¡Gracias por jugar!\n\n");
        }

        static void LeeInput(ref char dir)
        {
            if (Console.KeyAvailable)
            {
                string tecla = Console.ReadKey(true).Key.ToString();
                switch (tecla)
                {
                    case "LeftArrow": dir = 'l'; break;
                    case "UpArrow": dir = 'u'; break;
                    case "RightArrow": dir = 'r'; break;
                    case "DownArrow": dir = 'd'; break;
                    case "P": dir = 'p'; break; // Pausa.
                    case "Q": dir = 'q'; break; // Salir
                }
            }
            while (Console.KeyAvailable) Console.ReadKey().Key.ToString();
        }

        static void Pausa(ref bool pausa, ref char c)
        {
            if (!pausa && c == 'p')
            {
                pausa = true;
                c = ' ';
            }
            else if (pausa && c == 'p')
            {
                pausa = false;
                c = ' ';
            }
        }
    }
}