using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _1._2laba
{
    public class Program
    {
        private struct Coefficients
        {
            public double a, b;
        };

        private struct ToSave
        {
            public double[] X { get; set; }
            public double[] Y { get; set; }
            public double[] ApproxValues { get; set; }
        };

        static void Main()
        {
            const int n = 1000;
            const double a = 1;
            const double b = 2;

            double[,] matrix = generate(a, b, n);

            Console.WriteLine("Исходная табличная функция");
            print(matrix);

            Coefficients coefficients = calculateCoefficients(matrix, n);

            Console.WriteLine($"Полученные коэффиценты\na = {coefficients.a}\nb = {coefficients.b}");

            double[] approxValues = new double[n];

            for (int i = 0; i < n; ++i)
            {
                approxValues[i] = fApprox(coefficients, matrix[0, i]);
            }

            Console.WriteLine($"\nМера отклонения: {getPrecision(matrix, approxValues, n)}");
            //Console.WriteLine($"\nУвеличение разбиения на основе предыдущих вычислений, новая мера отклонения: {increasingSplitAndCountError(matrix, approxValues, n, coefficients)}");
            drawingGraph(matrix, approxValues, n);

            Console.Read();
        }

        static void drawingGraph(double[,] matrix, double[] approxValues, int size)
        {
            try
            {
                ToSave data = new ToSave();
                double[] x = new double[size];
                double[] y = new double[size];
                for (int i = 0; i < size; ++i)
                {
                    x[i] = matrix[0, i];
                    y[i] = matrix[1, i];
                }
                data.X = x;
                data.Y = y;
                data.ApproxValues = approxValues;

                string json = JsonSerializer.Serialize(data);
                File.WriteAllText(@"D:\лабы\6 семестр\ЧМ\1.2laba\Save\temp.json", json);

                Process p = Process.Start(@"D:\лабы\6 семестр\ЧМ\1.2laba\visualizationGraphs\visualizationGraphs.py");
                p.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static double increasingSplitAndCountError(double[,] accurateValues, double[] approxValues, int size, Coefficients coefficients)
        {
            double[] newApproxValues = new double[size * 2 - 1];
            double[,] newAccurateValues = new double[2, size * 2 - 1];
            for(int i = 0, j = 0; i < size * 2 - 1; i += 2, ++j)
            {
                newAccurateValues[0, i] = accurateValues[0, j];
                newAccurateValues[1, i] = accurateValues[1, j];
                newApproxValues[i] = approxValues[j];
            }

            for(int i = 1; i < size * 2 - 1; i += 2)
            {
                newAccurateValues[0, i] = newAccurateValues[0, i - 1] + (newAccurateValues[0, i + 1] - newAccurateValues[0, i - 1]) / 2;
                newAccurateValues[1, i] = f(newAccurateValues[0, i]);
                newApproxValues[i] = fApprox(coefficients, newAccurateValues[0, i]);
            }

            return getPrecision(newAccurateValues, newApproxValues, size * 2 - 1);
        }

        static double getPrecision(double[,] accurateValues, double[] approxValues, int size)
        {
            double precision = 0;
            for (var i = 0; i < size; ++i)
            {
                precision += (accurateValues[1, i] - approxValues[i]) * (accurateValues[1, i] - approxValues[i]);
            }

            return Math.Sqrt(precision / (size + 1));
        }

        static Coefficients calculateCoefficients(double[,] data, int size)
        {
            Coefficients result;

            double ySumm = data[1, 0];
            for (int i = 1; i < size; ++i)
            {
                ySumm += data[1, i];
            }

            double xSumm = Math.Log(data[0, 0]);
            for (int i = 1; i < size; ++i)
            {
                xSumm += Math.Log(data[0, i]);
            }

            double xSquareSumm = Math.Pow(Math.Log(data[0, 0]), 2);
            for(int i = 1; i < size; ++i)
            {
                xSquareSumm += Math.Pow(Math.Log(data[0, i]), 2);
            }

            double xSummY = 0;
            for (int i = 0; i < size; ++i)
            {
                xSummY += Math.Log(data[0, i]) * data[1, i];
            }

            result.b = (size * xSummY - ySumm * xSumm) / (size * xSquareSumm - xSumm * xSumm);
            result.a = (ySumm - result.b * xSumm) / size;

            return result;
        }

        private static double fApprox(Coefficients coefficients, double x)
        {
            return coefficients.a + coefficients.b * Math.Log(x);
        }

        private static void print(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < m; ++j)
                {
                    Console.Write(Math.Round(matrix[i, j], 3) + " ");
                }
                Console.WriteLine();
            }
        }

        private static double[,] generate(double a, double b, int n)
        {
            double[,] result = new double[2, n];

            double step = (b - a) / n;

            fillingMatrix(result, a, b, step, n);
            double fmax = findMax(result, n);
            addingRandomNumbers(result, n, fmax);

            return result;
        }

        private static void fillingMatrix(double[,] matrix, double a, double b, double step, int size)
        {
            for (int i = 0; i < size; ++i)
            {
                matrix[0, i] = a + i * step;
                matrix[1, i] = f(matrix[0, i]);
            }
        }

        private static double findMax(double[,] matrix, double size)
        {
            double max = matrix[1, 0];
            for (int i = 1; i < size; ++i)
            {
                max = Math.Max(max, matrix[1, i]);
            }
            return max;
        }

        private static void addingRandomNumbers(double[,] matrix, int size, double max)
        {
            Random rnd = new Random();

            double d = 0.2 * max;
            for (int i = 0; i < size; ++i)
            {
                double delta = rnd.NextDouble() * d - d / 2;
                matrix[1, i] += delta;
            }
        }

        //private static double f(double x)
        //{
        //    return 0.6 + 0.5 * Math.Log(x);
        //}

        private static double f(double x)
        {
            return 12 + 1.23 / x;
        }
    }
}
