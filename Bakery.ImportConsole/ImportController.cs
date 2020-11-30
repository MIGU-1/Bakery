using Bakery.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Bakery.ImportConsole
{
    public class ImportController
    {
        public static (IEnumerable<Order>, IEnumerable<Product>) ReadFromCsv()
        {
            string fileName2 = "OrderItems.csv";
            string[][] matrix2 = MyFile.ReadStringMatrixFromCsv(fileName2, true);

            var products = GetAllProductsFromCsv();

            var customers = matrix2
                .GroupBy(line => $"{line[2]};{line[3]};{line[4]}")
                .Select(grp => new Customer()
                {
                    CustomerNr = grp.Key.Split(';')[0],
                    LastName = grp.Key.Split(';')[1],
                    FirstName = grp.Key.Split(';')[2]
                })
                .ToArray();

            var orders = matrix2
                .GroupBy(line => $"{line[0]};{line[1]};{line[2]}")
                .Select(grp => new Order()
                {
                    OrderNr = grp.Key.Split(';')[0],
                    Date = ParseDate(grp.Key.Split(';')[1]),
                    Customer = customers
                        .Where(c => c.CustomerNr
                        .Equals(grp.Key.Split(';')[2]))
                        .SingleOrDefault(),
                    OrderItems = new List<OrderItem>()
                })
                .ToArray();

            var orderItems = matrix2
                .Select(line => new OrderItem()
                {
                    Order = orders
                        .Where(o => o.OrderNr
                        .Equals(line[0]))
                        .SingleOrDefault(),
                    Product = products
                        .Where(p => p.ProductNr
                        .Equals(line[5]))
                        .SingleOrDefault(),
                    Amount = Convert.ToInt32(line[6])
                })
                .ToArray();

            List<Product> productsInUse = new List<Product>();

            foreach (var order in orders)
            {
                foreach (var orderItem in orderItems)
                {
                    if (orderItem.Order.OrderNr.Equals(order.OrderNr))
                    {
                        order.OrderItems.Add(orderItem);
                        productsInUse.Add(orderItem.Product);
                    }

                }
            }

            return (orders, products.Except(productsInUse));
        }

        public static IEnumerable<Product> GetAllProductsFromCsv()
        {
            string fileName1 = "Products.csv";
            string[][] matrix1 = MyFile.ReadStringMatrixFromCsv(fileName1, true);

            return matrix1
                .Select(line => new Product()
                {
                    ProductNr = line[0],
                    Name = line[1],
                    Price = Convert.ToDouble(line[2])
                })
                .ToArray();
        }

        private static DateTime ParseDate(string date)
        {
            if (!String.IsNullOrEmpty(date))
            {
                int day = Convert.ToInt32(date.Split('.')[0]);
                int month = Convert.ToInt32(date.Split('.')[1]);
                int year = Convert.ToInt32(date.Split('.')[2]);

                return new DateTime(year, month, day, 0, 0, 0);
            }
            else
            {
                return DateTime.MinValue;
            }
        }
    }
}
