using System;
using System.IO;
using System.Collections.Generic;

namespace SalesTracker
{
    public class Sale
    {
        private string employee;
        private decimal salesAmount;
        private readonly decimal commissionRate;

        public Sale(string name, decimal salesAmount, decimal commissionRate)
        {
            this.employee = name;
            this.salesAmount = salesAmount;
            this.commissionRate = commissionRate;
        }

        public Sale(string name, decimal salesAmount)
        {
            this.employee = name;
            this.salesAmount = salesAmount;
            this.commissionRate = 0.03M;
        }

        public Sale(string csv)
        {
            string[] data = csv.Split(",");
            string employee = data[0];
            decimal salesAmount;
            decimal commissionRate;
            if (data.Length != 3)
            {
                throw new ArgumentException($"CSV line does not contain correct data. Should include employee, sale amount, and commission rate.");
            }
            else if (!Decimal.TryParse(data[1], out salesAmount))
            {
                throw new ArgumentException($"Sales amount, {data[1]}, not a valid number.");
            }
            else if (!Decimal.TryParse(data[2], out commissionRate))
            {
                throw new ArgumentException($"Commission rate, {data[2]}, not a valid number.");
            }
            else
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = data[i].Trim();
                }

                this.employee = employee;
                this.salesAmount = salesAmount;
                this.commissionRate = commissionRate;
            }
        }

        public string Employee
        {
            get
            {
                return employee;
            }
            set
            {
                employee = value;
            }
        }

        public decimal SalesAmount
        {
            get
            {
                return salesAmount;
            }
            set
            {
                salesAmount = value;
            }
        }

        public decimal Commission
        {
            get
            {
                return salesAmount * commissionRate;
            }
        }

        public static Sale operator +(Sale a, Sale b)
        {
            if (a.Employee != b.Employee)
            {
                throw new ArgumentException($"Can only add Sale objects if the employee is the same. In this case left='{a.Employee}' and right='{b.Employee}'");
            }
            else
            {
                decimal rate;
                decimal totalComm = a.Commission + b.Commission;
                decimal totalSales = a.SalesAmount + b.SalesAmount;
                rate = totalComm / totalSales;
                return new Sale(a.Employee, totalSales, rate);
            }
        }

        public override string ToString()
        {
            return string.Format("Employee: {0, 10} Sales: {1,15} Rate: {2, 10} Commission: {3}",
                                employee, Math.Round(salesAmount, 2), Math.Round(commissionRate, 3), Math.Round(Commission, 2));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"./sales.csv";
            try
            {
                //Sale sale1 = new("One", 1000, 0.10M);
                //Sale sale2 = new("One", 500, 0.05M);
                //Sale sale3 = new("Two", 2500, 0.10M);
                //Sale sale4 = new("Two", 3000);
                //Console.WriteLine(sale2);
                //Console.WriteLine(sale3);
                //Console.WriteLine(sale4);
                //Console.WriteLine(sale1 + sale2);
                //Console.WriteLine(sale3 + sale4);
                //Console.WriteLine(sale2 + sale3);

                List<Sale> sales = GetSales(filePath);
                PrintSales(sales);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                SalesConsole(filePath);
            }
        }

        static List<Sale> GetSales(string filePath)
        {
            StreamReader reader;
            List<Sale> sales = new();
            if (File.Exists(filePath))
            {
                reader = new StreamReader(File.OpenRead(filePath));
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    Sale sale = new(line);
                    sales.Add(sale);
                }
                reader.Close();
            }
            else
            {
                throw new ArgumentException($"File not found at '{filePath}'");
            }
            return sales;
        }

        static void PrintSales(List<Sale> sales)
        {
            for (int i = 0; i < sales.Count; i++)
            {
                Console.Write(i + ":");
                Console.WriteLine($" {sales[i]}");
            }
        }

        static void SalesConsole(string filePath)
        {
            bool cont = true;
            while (cont)
            {
                string answer;
                Console.Write("Type in two numbers separted by a space from the list above to add together the sales (or hit enter to exit): ");
                answer = Console.ReadLine();
                try
                {
                    List<Sale> sales = GetSales(filePath);
                    string[] indices = answer.Split(' ');

                    int index1 = Convert.ToInt32(indices[0]);
                    int index2 = Convert.ToInt32(indices[1]);

                    if (index1 < 0 || index1 > sales.Count)
                    {
                        throw new ArgumentException($"'{index1}' invalid. Must be 0-{sales.Count}.");
                    }
                    else if (index2 < 0 || index2 > sales.Count)
                    {
                        throw new ArgumentException($"'{index2}' invalid. Must be 0-{sales.Count}.");
                    }
                    else if (indices.Length > 2)
                    {
                        throw new ArgumentException($"Enter only 2 numbers.");
                    }
                    else
                    {
                        Console.WriteLine(sales[index1] + sales[index2]);
                    }
                }
                catch (Exception e)
                {
                    if (answer == "")
                    {
                        cont = false;
                    }
                    else
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
