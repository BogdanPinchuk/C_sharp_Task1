﻿using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using App1.Product;

namespace App1
{
    /// <summary>
    /// Створення файла БД
    /// </summary>
    static class CreateDB
    {
        /// <summary>
        /// Створення файла БД
        /// </summary>
        /// <param name="name">назва/шлях для файлу БД</param>
        /// <returns></returns>
        public static string CreateFile(string name)
        {
            // створення каталогу
            var catalog = new ADOX.Catalog();
            // провайдер і шлях до файлу БД
            string connect = "Provider=Microsoft.Jet." +
                    $@"OLEDB.4.0; Data Source = {name}.mdb";

            try
            {
                // створення бази данних
                catalog.Create(connect);
                Console.WriteLine("File of data base was create.");
            }
            catch (COMException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                catalog = null;
            }

            return connect;
        }

        /// <summary>
        /// Створення таблиць згідно завданню
        /// </summary>
        /// <param name="puthFile">Шлях до БД</param>
        /// <returns></returns>
        public static void CreateTable(string puthFile)
        {
            // створення з'єднання
            using (OleDbConnection conn = new OleDbConnection(puthFile))
            {
                // відкриття
                conn.Open();

                // створення об'єкта керування
                OleDbCommand comm = conn.CreateCommand();

                // створення таблиці Drink
                try
                {
                    comm.CommandText = "Create table Drinks(ID int not null, Name varchar(50), Price real)";
                    // виконання sql запиту
                    comm.ExecuteNonQuery();
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // створення таблиці Additiv
                try
                {
                    comm.CommandText = "Create table Additivs(ID int not null, Name varchar(50), Price real)";
                    // виконання sql запиту
                    comm.ExecuteNonQuery();
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        #region Add Product
        /// <summary>
        /// Додавання напою
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">назва</param>
        /// <param name="price">ціна</param>
        /// <param name="puthFile">Шлях до БД</param>
        public static void AddDrink(int id, string name, double price, string puthFile)
            => AddDrink(new SProduct(id, name, price) as IProduct, puthFile);

        /// <summary>
        /// Додавання добавки
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">назва</param>
        /// <param name="price">ціна</param>
        /// <param name="puthFile">Шлях до БД</param>
        public static void AddAdditiv(int id, string name, double price, string puthFile)
            => AddAdditiv(new SProduct(id, name, price) as IProduct, puthFile);

        /// <summary>
        /// Додавання напою
        /// </summary>
        /// <param name="product">Напій</param>
        /// <param name="puthFile">Шлях до БД</param>
        public static void AddDrink(IProduct product, string puthFile)
            => AddRangeDrink(new List<IProduct>() { product }, puthFile);

        /// <summary>
        /// Додавання добавки
        /// </summary>
        /// <param name="product">Напій</param>
        /// <param name="puthFile">Шлях до БД</param>
        public static void AddAdditiv(IProduct product, string puthFile)
            => AddRangeAdditiv(new List<IProduct>() { product }, puthFile);

        /// <summary>
        /// Додавання колекії продуктів
        /// </summary>
        /// <param name="array">Масив даних</param>
        /// <param name="puthFile">Шлях до БД</param>
        public static void AddRangeDrink(List<IProduct> products, string puthFile)
            => AddProduct(puthFile, "Drinks", products);

        /// <summary>
        /// Додавання колекії добавок
        /// </summary>
        /// <param name="array">Масив даних</param>
        /// <param name="puthFile">Шлях до БД</param>
        public static void AddRangeAdditiv(List<IProduct> products, string puthFile)
            => AddProduct(puthFile, "Additivs", products);

        /// <summary>
        /// Додавання продукту
        /// </summary>
        /// <param name="puthFile">Шлях до БД</param>
        /// <param name="table">Назва таблиці</param>
        private static void AddProduct(string puthFile, string table, List<IProduct> products)
        {
            // створення з'єднання
            using (OleDbConnection conn = new OleDbConnection(puthFile))
            {
                // відкриття
                conn.Open();

                // створення об'єкта керування
                OleDbCommand comm = conn.CreateCommand();

                foreach (var i in products)
                {
                    // перевірка ID (унікальний номер)
                    comm.CommandText = $"Select * From {table} Where 'ID == {i.ID}'";
                    // отримання даних
                    OleDbDataReader reader = comm.ExecuteReader();
                    // переврка на явності даних, якщо є то продовжуємо аналізувати інші дані
                    if (reader.HasRows)
                    {
                        continue;
                    }

                    // задання команди
                    comm.CommandText = $"Insert Into {table}([ID], Name, Price)" +
                        $"Values({i.ID}, '{i.Name}', '{i.Price}')";
                    // виконання sql запиту
                    comm.ExecuteNonQuery();
                }
            }

        }
        #endregion

        #region Change
        /// <summary>
        /// Зміна напою по id
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">назва</param>
        /// <param name="price">ціна</param>
        /// <param name="puthFile">Шлях до БД</param>
        public static void ChangeDrink(int id, string name, double price, string puthFile)
            => ChangeDrink(new SProduct(id, name, price) as IProduct, puthFile);

        /// <summary>
        /// Зміна напою по id
        /// </summary>
        /// <param name="product">Напій</param>
        /// <param name="puthFile">Шлях до БД</param>
        public static void ChangeDrink(IProduct product, string puthFile)
            => ChangeProduct(puthFile, "Drinks", product);

        /// <summary>
        /// Додавання добавки по id
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">назва</param>
        /// <param name="price">ціна</param>
        /// <param name="puthFile">Шлях до БД</param>
        public static void ChangeAdditiv(int id, string name, double price, string puthFile)
            => ChangeAdditiv(new SProduct(id, name, price) as IProduct, puthFile);

        /// <summary>
        /// Додавання добавки по id
        /// </summary>
        /// <param name="product">Напій</param>
        /// <param name="puthFile">Шлях до БД</param>
        public static void ChangeAdditiv(IProduct product, string puthFile)
            => ChangeProduct(puthFile, "Additivs", product);

        /// <summary>
        /// Зміна продукту по id
        /// </summary>
        /// <param name="puthFile">Шлях до БД</param>
        /// <param name="table">Назва таблиці</param>
        private static void ChangeProduct(string puthFile, string table, IProduct products)
        {
            // створення з'єднання
            using (OleDbConnection conn = new OleDbConnection(puthFile))
            {
                // відкриття
                conn.Open();

                // створення об'єкта керування
                OleDbCommand comm = conn.CreateCommand();

                // задання команди
                comm.CommandText = $"Update {table} Set Name = '{products.Name}'," +
                    $" Price = '{products.Price}' Where [ID] = {products.ID}";
                // виконання sql запиту
                comm.ExecuteNonQuery();
            }

        }
        #endregion

    }
}
