using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using App0.Product;
using System.Data;
using System.Globalization;

namespace App0.Calculate
{
    /// <summary>
    /// Стакани для наливання
    /// </summary>
    enum TypeOfGlass
    {
        /// <summary>
        /// стакан на 110 мл
        /// </summary>
        ml110 = 110,
        /// <summary>
        /// стакан на 175 мл
        /// </summary>
        ml175 = 175,
        /// <summary>
        /// стакан на 250 мл
        /// </summary>
        ml250 = 250,
        /// <summary>
        /// стакан на 400 мл
        /// </summary>
        ml400 = 400,
        /// <summary>
        /// стакан на 500 мл
        /// </summary>
        ml500 = 500,
    }
    // Примітка. Дані об'єму взяті з інтернету. Також вважатимемо,
    // що ціна стаканчика уже врахована в сам напій. В подальшому 
    // при необхідності програму можна вдосконалити і дані по стаканчиках
    // винести в файл БД.

    /// <summary>
    /// Обслуговування запитів клієнта
    /// </summary>
    class Service
    {
        /// <summary>
        /// Делегат на відслідковування зміни розмірів консолі
        /// </summary>
        private delegate void ChangeSizeConcole();
        /// <summary>
        /// Подія яка відбувається при зміні розмірів вікна консолі
        /// </summary>
        private event ChangeSizeConcole ChangeSize;
        /// <summary>
        /// для синхронного доступу до консолі
        /// </summary>
        private readonly object block = new object();
        /// <summary>
        /// Список напоїв
        /// </summary>
        private List<IProduct> drinks = new List<IProduct>();
        /// <summary>
        /// Список добавок
        /// </summary>
        private List<IProduct> aditivs = new List<IProduct>();
        /// <summary>
        /// Установка культури
        /// </summary>
        private RegionInfo region;


        /// <summary>
        /// Конструктор діалогу
        /// </summary>
        public Service(RegionInfo region)
        {
            // Установка культури
            this.region = region;

            // завантаження даних з БД, лише після того як наявні 
            // дані можна запускати побудову інтерфейсу
            DownloadData();

            // запуск відображення меню
            new Thread(Menu).Start();

            // підпис на подію
            ChangeSize += Menu;
            // запуск відслідковування змін
            new Thread(CheckerChange).Start();

        }

        /// <summary>
        /// Меню для користувача
        /// </summary>
        public void Menu()
        {
            // Очищення консолі
            Console.Clear();

            // рамка
            new Thread(Frame).Start();

            // стаканчик
            new Thread(Glass).Start();

            // напої
            new Thread(Drink).Start();
        }

        /// <summary>
        /// Відображення рамок
        /// </summary>
        private void Frame()
        {
            // Блокуємо доступ іншим потокам
            lock (block)
            {
                // верх і низ
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write('#');
                }
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write('#');
                }

                // ліво і право
                for (int i = 0; i < Console.WindowHeight - 1; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write('#');
                }
                for (int i = 0; i < Console.WindowHeight - 1; i++)
                {
                    Console.SetCursorPosition(Console.WindowWidth - 1, i);
                    Console.Write('#');
                }

                // після стаканчиків
                Console.SetCursorPosition(0, 4);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write('#');
                }
            }
        }

        /// <summary>
        /// Відображення набору стаканчиків
        /// </summary>
        private void Glass()
        {
            // Блокуємо доступ іншим потокам
            lock (block)
            {
                // установка курсора
                Console.SetCursorPosition(2, 2);

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Green;

                // Створення масиву даних стаканів
                TypeOfGlass[] array = Enum.GetValues(typeof(TypeOfGlass))
                    .Cast<TypeOfGlass>().ToArray();

                StringBuilder glass = new StringBuilder()
                    .Append("Имеются стаканчики такой емкости: ");

                // Вивід
                Console.Write(glass.ToString());

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Yellow;

                // очищення
                glass.Clear();

                foreach (var i in array)
                {
                    glass.Append(" " + (int)i + ", ");
                }

                // вивід
                Console.Write(glass.ToString().Trim().TrimEnd(',') + " мл");

                // скидання налаштувань
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Відображення набору напоїв
        /// </summary>
        private void Drink()
        {
            // Блокуємо доступ іншим потокам
            lock (block)
            {
                // установка курсора
                Console.SetCursorPosition(2, 6);

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Cyan;

                StringBuilder drink = new StringBuilder()
                    .Append("Напиток: ");

                // Вивід
                Console.Write(drink.ToString());

                foreach (var i in drinks)
                {
                    // установка курсора
                    Console.SetCursorPosition(2, 7 + i.ID);
                    Console.Write($"  {i.ID}. {i.Name} - {i.Price.ToString("C")}");
                }


                // скидання налаштувань
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Перевірка зміни розмірів консолі
        /// </summary>
        private void CheckerChange()
        {
            // висота і ширина вікна консолі
            int hight = Console.WindowHeight,
                weigth = Console.WindowWidth;

            while (true)
            {
                // якщо ширина і висота консолі змінилася
                // то оновлюємо внутрішнє меню
                if (hight != Console.WindowHeight ||
                    weigth != Console.WindowWidth)
                {
                    // оновлюємо дані
                    hight = Console.WindowHeight;
                    weigth = Console.WindowWidth;

                    // Запускаємо подію оновлення меню
                    ChangeSize.Invoke();
                }
            }
        }

        /// <summary>
        /// Завантаження даних (таблиць продуктів)
        /// </summary>
        private void DownloadData()
        {
            // завантаження даних з таблиць
            ExstractingData<SDrink>(LoadDataBase.Products.Tables["Drinks"], drinks);
            ExstractingData<SAdditiv>(LoadDataBase.Products.Tables["Additivs"], aditivs);
        }

        /// <summary>
        /// Витягування даних
        /// </summary>
        /// <typeparam name="T">Тип вихідних даних</typeparam>
        /// <param name="table">Таблиця з даними</param>
        /// <returns></returns>
        private void ExstractingData<T>(DataTable table, List<IProduct> products)
            where T : IProduct
        {
            // очищення списку
            products.Clear();

            // перебираємо всі рядки в таблиці
            foreach (DataRow row in table.Rows)
            {
                // заносимо дані в колекції
                if (typeof(T).Equals(typeof(SDrink)))
                {
                    products.Add(new SDrink((int)row["ID"], (string)row["Name"],
                        (float)row["Price"], (float)row["SizeOf"],
                        ((bool)row["TypeOf"]).ConvertToTypeValue()));
                }
                else if (typeof(T).Equals(typeof(SAdditiv)))
                {
                    products.Add(new SAdditiv((int)row["ID"], (string)row["Name"],
                        (float)row["Price"], (float)row["SizeOf"],
                        ((bool)row["TypeOf"]).ConvertToTypeValue()));
                }
                else
                {
                    throw new Exception("This type is not valid");
                }
            }
        }

    }
}
