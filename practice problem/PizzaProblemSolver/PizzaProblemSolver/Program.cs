using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaProblemSolver
{
    class Program
    {
        private const string Filename = "big";

        private static int Width;
        private static int Height;
        private static int MinIngredients;
        private static int MaxSize;
        private static char[][] Pizza;

        private static List<int[]> Shapes;
        private static int[][] BestSlicedPizza;
        private static int MaxScore = 0;

        private const int Popsize = 100;
        private const double MutationRate = 0.1;
        private const double SurvivalRate = 0.15;

        private static bool StopAlgorithm = false; 

        static void Main(string[] args)
        {
            ReadFile();
            Shapes = DetermineShapes();
            var population = GeneratePopulation(Shapes.Count);
            
            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable && MaxScore < Width * Height)
                {
                    population = Mutate(population);
                    var scores = Score(population);
                    population = Kill(population, scores);
                    population = Crossover(population);
                    MaxScore = Math.Max(MaxScore, scores.Last());
                    Console.WriteLine(MaxScore + "/" + Width * Height
                        + "\tAverage = " + scores.Average()
                        + "\t Max = " + scores.Max()
                        + "\tMin = " + scores.Min());
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            Console.WriteLine("Writing submission...");
            WriteSubmission();
            Console.WriteLine("Submission written. Press Enter to exit.");
            Console.ReadLine();
        }

        private static void ReadFile()
        {
            var text = File.ReadAllLines("../../../../data/" + Filename + ".in");

            var metadata = text[0].Split(' ').Select(x => Convert.ToInt32(x)).ToArray();

            Height = metadata[0];
            Width = metadata[1];
            MinIngredients = metadata[2];
            MaxSize = metadata[3];

            Pizza = new char[Height][];
            for (int i = 0; i < Height; i++)
            {
                Pizza[i] = text[i + 1].ToCharArray();
            }
        }

        private static List<int[]> DetermineShapes()
        {
            var shapes = new List<int[]>();
            for (int i = 2*MinIngredients; i <= MaxSize; i++)
            {
                for (int j = 1; j <= i; j++)
                {
                    var shape = new int[] {j, (int) i/j};

                    if (shape[0]*shape[1] == i)
                    {
                        shapes.Add(shape);
                    }
                }
            }

            return shapes;
        }

        private static int[][] GeneratePopulation(int numberOfShapes)
        {
            var genes = Width*Height/(2*MinIngredients);
            var population = new int[Popsize][];
            
            var random = new Random();
            for (int i = 0; i < Popsize; i++)
            {
                var individual = new int[genes];
                for (int j = 0; j < genes; j++)
                {
                    individual[j] = random.Next(0, numberOfShapes-1);
                }

                population[i] = individual;
            }

            return population;
        }

        private static int[][] Mutate(int[][] population)
        {
            var random = new Random();

            Parallel.For(0, Popsize, i =>
            {
                for (int j = 0; j < population[0].Length; j++)
                {
                    if (random.NextDouble() < MutationRate)
                    {
                        population[i][j] = random.Next(0, Shapes.Count - 1);
                    }
                }
            });
            
            return population;
        }

        private static int[] Score(int[][] population)
        {
            var scores = new int[Popsize];

            Parallel.For(0, Popsize, i =>
            {
                var slicedPizza = new int[Height][];
                for (int j = 0; j < Height; j++)
                {
                    slicedPizza[j] = Enumerable.Repeat(-1, Width).ToArray();
                }
                
                var individual = population[i];
                var currentGene = 0;
                var currentShape = Shapes[individual[currentGene]];
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (SliceFits(currentShape, slicedPizza, y, x))
                        {
                            AddSlice(currentShape, slicedPizza, y, x, currentGene);
                            x += currentShape[1];
                            currentGene++;
                            scores[i]+= currentShape[0]*currentShape[1];
                            if (currentGene >= individual.Length)
                            {
                                break;
                            }
                            currentShape = Shapes[individual[currentGene]];
                        }
                    }
                    if (currentGene >= individual.Length)
                    {
                        break;
                    }
                }
                if (scores[i] > MaxScore)
                {
                    MaxScore = scores[i];
                    BestSlicedPizza = slicedPizza;
                }
            });
            
            return scores;
        }

        private static bool SliceFits(int[] shape, int[][] slicedPizza, int y, int x)
        {
            if (y + shape[0] >= Height || x + shape[1] >= Width)
            {
                return false;
            }

            var tomatoes = 0;
            var mushrooms = 0;
            for (int h = y; h < y+shape[0]; h++)
            {
                for (int w = x; w < x+shape[1]; w++)
                {
                    if (slicedPizza[h][w] != -1)
                    {
                        return false;
                    }
                    if (Pizza[h][w] == 'M')
                    {
                        mushrooms++;
                    }
                    else
                    {
                        tomatoes++;
                    }
                }
            }

            return mushrooms >= MinIngredients && tomatoes >= MinIngredients;
        }

        private static void AddSlice(int[] shape, int[][] slicedPizza, int y, int x, int gene)
        {
            for (int h = y; h < y + shape[0]; h++)
            {
                for (int w = x; w < x + shape[1]; w++)
                {
                    slicedPizza[h][w] = gene;
                }
            }
        }

        private static int[][] Kill(int[][] population, int[] scores)
        {
            Array.Sort(scores, population);
            return population.Skip((int)(Popsize*(1 - SurvivalRate))).ToArray();
        }

        private static int[][] Crossover(int[][] population)
        {
            var random = new Random();

            var newpop = new int[Popsize][];
            population.CopyTo(newpop, 0);
            Parallel.For(population.Length, Popsize, i =>
            {
                var individual1 = population[random.Next(population.Length)];
                var individual2 = population[random.Next(population.Length)];
                newpop[i] = Enumerable.Repeat(0, individual1.Length).ToArray();
                var cut = random.Next(1, population[0].Length - 2);
                for (int j = 0; j < cut; j++)
                {
                    newpop[i][j] = individual1[j];
                }
                for (int j = cut; j < individual1.Length; j++)
                {
                    newpop[i][j] = individual2[j];
                }
            });

            return newpop;
        }

        private static void WriteSubmission()
        {
            var unfoldedPizza = BestSlicedPizza
                .SelectMany(x => x)
                .ToArray();

            var slices = unfoldedPizza
                .Distinct()
                .Where(x => x > -1)
                .ToArray();

            var file = slices.Length + "\n";

            Parallel.ForEach(slices, slice =>
            {
                var i1 = Array.IndexOf(unfoldedPizza, slice);
                var i2 = Array.LastIndexOf(unfoldedPizza, slice);

                var r1 = i1/Width;
                var r2 = i2/Width;

                var c1 = i1%Width;
                var c2 = i2%Width;
                file += r1 + " " + c1 + " " + r2 + " " + c2 + "\n";
            });

            File.WriteAllText("../../../../submissions/" + Filename + MaxScore + ".txt", file);
        }
    }
}
