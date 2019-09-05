using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.Odbc;
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
        /// <param name="pathFile">Шлях до БД</param>
        /// <returns></returns>
        public static void CreateTable(string pathFile)
        {
            // створення з'єднання
            using (OleDbConnection conn = new OleDbConnection(pathFile))
            {
                // відкриття
                conn.Open();

                // створення об'єкта керування
                OleDbCommand comm = conn.CreateCommand();

                // створення таблиці Drink
                try
                {
                    comm.CommandText = "Create table Drinks(ID int not null, Name varchar(50), Price real, SizeOf real, TypeOf bit)";
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
                    comm.CommandText = "Create table Additivs(ID int not null, Name varchar(50), Price real, SizeOf real, TypeOf bit)";
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
        /// <param name="size">розмів виміру мл/г</param>
        /// <param name="type">тип виміру вага/об'єм</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void AddDrink(int id, string name, double price,
            double size, TypeValue type, string pathFile)
            => AddDrink(new SProduct(id, name, price, size, type), pathFile);

        /// <summary>
        /// Додавання добавки
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">назва</param>
        /// <param name="price">ціна</param>
        /// <param name="size">розмів виміру мл/г</param>
        /// <param name="type">тип виміру вага/об'єм</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void AddAdditiv(int id, string name, double price,
            double size, TypeValue type, string pathFile)
            => AddAdditiv(new SProduct(id, name, price, size, type), pathFile);

        /// <summary>
        /// Додавання напою
        /// </summary>
        /// <param name="product">Напій</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void AddDrink(IProduct product, string pathFile)
            => AddRangeDrink(new List<IProduct>() { product }, pathFile);

        /// <summary>
        /// Додавання добавки
        /// </summary>
        /// <param name="product">Напій</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void AddAdditiv(IProduct product, string pathFile)
            => AddRangeAdditiv(new List<IProduct>() { product }, pathFile);

        /// <summary>
        /// Додавання колекії продуктів
        /// </summary>
        /// <param name="array">Масив даних</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void AddRangeDrink(List<IProduct> products, string pathFile)
            => AddProduct(pathFile, "Drinks", products);

        /// <summary>
        /// Додавання колекії добавок
        /// </summary>
        /// <param name="array">Масив даних</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void AddRangeAdditiv(List<IProduct> products, string pathFile)
            => AddProduct(pathFile, "Additivs", products);

        /// <summary>
        /// Додавання продукту
        /// </summary>
        /// <param name="pathFile">Шлях до БД</param>
        /// <param name="table">Назва таблиці</param>
        /// <param name="products">Масив продуктів</param>
        private static void AddProduct(string pathFile, string table, List<IProduct> products)
        {
            // створення з'єднання
            using (OleDbConnection conn = new OleDbConnection(pathFile))
            {
                // відкриття
                conn.Open();

                // створення об'єкта керування
                OleDbCommand comm = conn.CreateCommand();

                foreach (var i in products)
                {
                    // перевірка ID (унікальний номер)
                    comm.CommandText = $"Select * From {table} Where ID in ({i.ID})";
                    // виконання sql запиту
                    comm.ExecuteNonQuery();

                    // отримання даних
                    using (OleDbDataReader reader = comm.ExecuteReader())
                    {
                        // переврка на явності даних, якщо є то продовжуємо аналізувати інші дані
                        if (reader.HasRows)
                        {
                            continue;
                        }
                    }

                    // задання команди
                    comm.CommandText = $"Insert Into {table}([ID], Name, Price, SizeOf, TypeOf)" +
                        $"Values('{i.ID}', '{i.Name}', '{i.Price}', '{i.Size}', {i.TypeOfValue.ConvertToBool()})";
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
        /// <param name="size">розмів виміру мл/г</param>
        /// <param name="type">тип виміру вага/об'єм</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void ChangeDrink(int id, string name, double price,
            double size, TypeValue type, string pathFile)
            => ChangeDrink(new SProduct(id, name, price, size, type), pathFile);

        /// <summary>
        /// Зміна напою по id
        /// </summary>
        /// <param name="product">Напій</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void ChangeDrink(IProduct product, string pathFile)
            => ChangeProduct(pathFile, "Drinks", product);

        /// <summary>
        /// Додавання добавки по id
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">назва</param>
        /// <param name="price">ціна</param>
        /// <param name="size">розмів виміру мл/г</param>
        /// <param name="type">тип виміру вага/об'єм</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void ChangeAdditiv(int id, string name, double price,
            double size, TypeValue type, string pathFile)
            => ChangeAdditiv(new SProduct(id, name, price, size, type), pathFile);

        /// <summary>
        /// Додавання добавки по id
        /// </summary>
        /// <param name="product">Напій</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void ChangeAdditiv(IProduct product, string pathFile)
            => ChangeProduct(pathFile, "Additivs", product);

        /// <summary>
        /// Зміна продукту по id
        /// </summary>
        /// <param name="pathFile">Шлях до БД</param>
        /// <param name="table">Назва таблиці</param>
        /// <param name="product">Продукт</param>
        private static void ChangeProduct(string pathFile, string table, IProduct product)
        {
            // створення з'єднання
            using (OleDbConnection conn = new OleDbConnection(pathFile))
            {
                // відкриття
                conn.Open();

                // створення об'єкта керування
                OleDbCommand comm = conn.CreateCommand();

                // задання команди
                comm.CommandText = $"Update {table} Set Name = '{product.Name}'," +
                    $" Price = '{product.Price}', SizeOf = '{product.Size}'," +
                    $" TypeOf = {product.TypeOfValue.ConvertToBool()} Where [ID] = {product.ID}";
                // виконання sql запиту
                comm.ExecuteNonQuery();
            }

        }
        #endregion

        #region Delete
        /// <summary>
        /// Видалення напою по id
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void DeleteDrink(int id, string pathFile)
            => DeleteProduct(pathFile, "Drinks", id);

        /// <summary>
        /// Видалення добавки по id
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void DeleteAdditiv(int id, string pathFile)
            => DeleteProduct(pathFile, "Additivs", id);

        /// <summary>
        /// Видалення продукту по id
        /// </summary>
        /// <param name="pathFile">Шлях до БД</param>
        /// <param name="table">Назва таблиці</param>
        private static void DeleteProduct(string pathFile, string table, int id)
        {
            // створення з'єднання
            using (OleDbConnection conn = new OleDbConnection(pathFile))
            {
                // відкриття
                conn.Open();

                // створення об'єкта керування
                OleDbCommand comm = conn.CreateCommand();

                // задання команди
                comm.CommandText = $"Delete From {table} Where [ID] = {id}";
                // виконання sql запиту
                comm.ExecuteNonQuery();
            }

        }

        /// <summary>
        /// Очищення таблиці напоїв
        /// </summary>
        /// <param name="pathFile">Шлях до БД</param>
        public static void DeleteAllDrink(string pathFile)
            => DeleteAllProduct(pathFile, "Drinks");

        /// <summary>
        /// Очищення таблиці добавок
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="pathFile">Шлях до БД</param>
        public static void DeleteAllAdditiv(string pathFile)
            => DeleteAllProduct(pathFile, "Additivs");

        /// <summary>
        /// Очищення таблиці продуктів
        /// </summary>
        /// <param name="pathFile">Шлях до БД</param>
        /// <param name="table">Назва таблиці</param>
        private static void DeleteAllProduct(string pathFile, string table)
        {
            // створення з'єднання
            using (OleDbConnection conn = new OleDbConnection(pathFile))
            {
                // відкриття
                conn.Open();

                // створення об'єкта керування
                OleDbCommand comm = conn.CreateCommand();

                // задання команди
                comm.CommandText = $"Delete From {table}";
                // виконання sql запиту
                comm.ExecuteNonQuery();
            }

        }
        #endregion

    }
}
