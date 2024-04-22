// Andrea Aparicio López.
// José Tomás Gómez Becerra.

using System;

namespace FP2P2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{"",8}PACMAN");
            Console.Write($"Pulsa S para comenzar");
            char c = ' ';
            bool start;
            do
            {
                LeeInput(ref c);
                start = c == 's';
            }
            while (!start);

            while(start)
            {
                // Nivel inicial.
                int num = 0;
                bool capturado = false;

                while (num < 10 && start)
                {
                    // Cargado de nivel y renderizado inicial
                    string file = $"levels/level0{num}.dat";
                    Tablero t = new Tablero(file);
                    t.Render();

                    int lap = 200; // retardo para bucle ppal
                    c = ' ';
                    bool pausa = false;

                    while (!capturado && !t.FinNivel())
                    {
                        // input de usuario
                        LeeInput(ref c);
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

                    Console.Clear();
                    if (capturado)
                    {
                        Console.WriteLine("¡Te han comido!");
                        Console.Write("¿Quieres volver a empezar? S/N");
                        start = Console.ReadLine().ToLower() == "s";
                        num = 0;
                    }
                    else
                    {
                        Console.Write("¡Victoria!");
                        Console.Write("¿Quieres avanzar al siguiente nivel? S/N");
                        if (Console.ReadLine().ToLower() == "s") num++;
                        
                    }
                    capturado = false;
                }
                if(num >= 10) {

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
                    case "Q": dir = 'q'; break; // Salir.
                    case "S": dir = 's'; break; // Start
                }
            }
            while (Console.KeyAvailable) Console.ReadKey().Key.ToString();
        }
    }
}