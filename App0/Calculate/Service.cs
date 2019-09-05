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
        /// Делегат на відслідковування зміни параметрів консолі і БД
        /// </summary>
        private delegate void ChangeData();
        /// <summary>
        /// Подія яка відбувається при зміні розмірів вікна консолі
        /// </summary>
        private event ChangeData ChangeSize;
        /// <summary>
        /// Подія яка відбувається при зміні БД
        /// </summary>
        private event ChangeData ChangeDB;

        /// <summary>
        /// для синхронного доступу до консолі
        /// </summary>
        private readonly object block = new object();
        /// <summary>
        /// Список доступних напоїв
        /// </summary>
        private List<IDrink> drinks = new List<IDrink>();
        /// <summary>
        /// Список доступних добавок
        /// </summary>
        private List<IAdditiv> aditivs = new List<IAdditiv>();
        /// <summary>
        /// Установка культури
        /// </summary>
        private readonly RegionInfo region;
        /// <summary>
        /// Загальне замовлення
        /// </summary>
        private SOrder data;

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

            // підписи на події
            ChangeSize += Menu;         // оновлення меню
            ChangeDB += DownloadData;   // оновлення БД

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

            // масив потоків
            Thread[] thread = new Thread[]
            {
                // рамка
                new Thread(Frame),
                // стаканчик
                new Thread(Glass),
                // напої
                new Thread(Drink),
                // добавки
                new Thread(Aditiv),
            };

            // запуск виконання
            for (int i = 0; i < thread.Length; i++)
            {
                thread[i].Start();
            }

            // очікуємо їх виконання
            for (int i = 0; i < thread.Length; i++)
            {
                thread[i].Join();
            }

            // запуск обробки введення даних
            new Thread(Entry).Start();
        }

        /// <summary>
        /// Відображення рамок
        /// </summary>
        private void Frame()
        {
            // Примітка. Блокування потрібно, щоб не з'являтися артефакти 
            // при масштабуванні вікна консолі

            // Блокуємо доступ іншим потокам
            lock (block)
            {
                // зміна кольору
                //Console.ForegroundColor = ConsoleColor.White;

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

                // висота на яку треба далі спускатись
                int height = 6 + Math.Max(drinks.Count, aditivs.Count);

                // між напоями і добавками
                for (int i = 0; i < height; i++)
                {
                    Console.SetCursorPosition(Console.WindowWidth / 2, i + 4);
                    Console.Write('#');
                }

                height += 3;
                // після продуктів
                Console.SetCursorPosition(0, height);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write('#');
                }

                height += 6;
                // після відображення замовлення
                Console.SetCursorPosition(0, height);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write('#');
                }

                // скидання налаштувань
                Console.ResetColor();
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
                    .Append("Емкость стаканчика: ");

                // Вивід
                Console.Write(glass.ToString());

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Yellow;

                // очищення
                glass.Clear();

                {
                    int j = 1;
                    foreach (var i in array)
                    {
                        glass.Append($" {j++}) " + (int)i + ", ");
                    }
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
                    .Append("Напитки: (70 млм)");

                // Вивід
                Console.Write(drink.ToString());

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Yellow;

                foreach (var i in drinks)
                {
                    // установка курсора
                    Console.SetCursorPosition(2, 7 + i.ID);
                    Console.Write($"  {i.ID}. {i.Name,-4} - " +
                        $"{i.Price.ToString("C2", new CultureInfo(region.Name))}");
                }

                // скидання налаштувань
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Відображення набору добавок
        /// </summary>
        private void Aditiv()
        {
            // Блокуємо доступ іншим потокам
            lock (block)
            {
                // установка курсора
                Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 6);

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Magenta;

                StringBuilder aditiv = new StringBuilder()
                    .Append("Добавки: (10 мл/10 г)");

                // Вивід
                Console.Write(aditiv.ToString());

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Yellow;

                foreach (var i in aditivs)
                {
                    // установка курсора
                    Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 7 + i.ID);
                    Console.Write($"  {i.ID}. {i.Name,-6} - " +
                        $"{i.Price.ToString("C2", new CultureInfo(region.Name))}");
                }

                // скидання налаштувань
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Виведення замовлення клієнта
        /// </summary>
        private void Order()
        {
            // Блокуємо доступ іншим потокам
            lock (block)
            {
                #region Продукти замовлення
                // установка курсора
                Console.SetCursorPosition(2, 11 + Math.Max(drinks.Count, aditivs.Count));
                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 4));

                // установка курсора
                Console.SetCursorPosition(2, 11 + Math.Max(drinks.Count, aditivs.Count));

                //TODO: удалить тестові дані
                /*data.Glass = TypeOfGlass.ml110;
                data.AddDrink(drinks[1]);
                data.AddAdditiv(aditivs[1]);
                data.AddAdditiv(aditivs[0]);
                data.AddAdditiv(aditivs[0]);
                data.AddAdditiv(aditivs[1]);
                data.AddAdditiv(aditivs[3]);*/

                // якщо стаканчик вибрано то можна відображати замовлення
                if (data.IsGlass)
                {
                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Вивід
                    Console.Write("Ваш заказ: ");

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Green;
                    // Вивід
                    Console.Write($"G - {Enum.GetValues(typeof(TypeOfGlass)).Cast<TypeOfGlass>().ToList().IndexOf(data.Glass) + 1};");

                    // Перевірка чи вибнано напій
                    if (data.GetDrinks().Count > 0)
                    {
                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        // Вивід
                        Console.Write($" D - {data.GetDrinks()[0].ID}x{data.GetDrinks().Count};");
                    }

                    // Перевірка чи вибнано добавки
                    if (data.GetAdditivs().Count > 0)
                    {
                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.Magenta;

                        // Вивід
                        var s = new StringBuilder($" A -");

                        foreach (var i in aditivs)
                        {
                            if (data.GetAdditivs().Contains(i))
                            {
                                s.Append($" {i.ID}x{data.GetAdditivs().Where(t => t.ID == i.ID).Select(t => t).ToList().Count},");
                            }
                        }

                        // Вивід
                        Console.Write(s.ToString().TrimEnd(',') + ";");
                    }

                    // Можна зробити умову, що можливість оплати відбувається лише при умові вибору напою
                    if (data.GetDrinks().Count > 0)
                    {
                        // установка курсора
                        Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 11 + Math.Max(drinks.Count, aditivs.Count));

                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.White;
                        // Вивід
                        Console.Write("P - оптала;");
                    }
                }
                #endregion

                // установка курсора
                Console.SetCursorPosition(2, 13 + Math.Max(drinks.Count, aditivs.Count));
                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 4));

                // установка курсора
                Console.SetCursorPosition(2, 13 + Math.Max(drinks.Count, aditivs.Count));

                #region Ціна
                if (data.IsGlass)
                {
                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Вивід
                    Console.Write("Стоимость: ");

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Green;
                    // Вивід
                    Console.Write($"{data.Price.ToString("C2", new CultureInfo(region.Name))}");
                }
                #endregion

                #region Об'єм
                if (data.IsGlass)
                {
                    // установка курсора
                    Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 13 + Math.Max(drinks.Count, aditivs.Count));

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Вивід
                    Console.Write("Объем: ");

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Green;
                    // Вивід
                    Console.Write($"{data.Volume()}");
                }
                #endregion

                // скидання налаштувань
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Взаємодія з клієнтом
        /// </summary>
        private void Entry()
        {
            // Примітка. Блокувати не потрібно там де э очыкування курсора, 
            // так як не буде можливості оновити меню коректно без артефактів

            // Обробка натискань
            while (true)
            {
                // Результат замовлення
                Order();

                // Головне меню вибору
                MenuChange();

                // введення клавіши
                ConsoleKey key = Console.ReadKey(true).Key;

                // дія згідно вибору
                switch (key)
                {
                    case ConsoleKey.N:  // очистить
                        data.Clear();
                        break;
                    case ConsoleKey.G:  // стаканчик
                        SizeGlass();
                        break;
                        //TODO: додать вибір/зміну стаканчика
                    case ConsoleKey.D:  // напиток
                        if (data.IsGlass)
                        {

                        }
                        break;
                        //TODO: додать вибір напою
                    case ConsoleKey.A:  // добавка
                        if (data.IsGlass)
                        {

                        }
                        break;
                        //TODO: додать вибір добавок
                    case ConsoleKey.P:  // оплата
                        if (data.GetDrinks().Count > 0)
                        {
                            Pay();
                            goto case ConsoleKey.N; // перехід на очистку
                        }
                        break;
                    //TODO: додать оплату (запис в лог файл і очистка)
                    case ConsoleKey.Q:  // виход
                        Environment.Exit(0);
                        break;

                }
            }
        }

        /// <summary>
        /// Вибір розмірів стаканчика
        /// </summary>
        /// <returns></returns>
        private void SizeGlass()
        {
            // значення наявні для введення відповідно до кількості стаканчиків
            int[] value = Enumerable.Range(1, Enum.GetValues(typeof(TypeOfGlass)).Length).ToArray();

            // перевірка введеного значення
            do
            {
                // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));
                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 4));
                // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Green;
                
                // Вивід
                Console.Write("Введите размер стаканчика: ");

                // скидання налаштувань
                Console.ResetColor();

                // введення клавіши
                string key = Console.ReadLine();

                // при натисканні виходу 
                if (key.ToLower() == ConsoleKey.Q.ToString().ToLower())
                {
                    #region Обробка виходу
                    do
                    {

                        // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 4));

                // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.Red;

                        // Вивід
                        Console.Write("Выйти с програмы [y/n]: ");

                        // введення клавіши
                        key = Console.ReadLine();

                        if (key.ToLower() == ConsoleKey.Y.ToString().ToLower())
                        {
                            Environment.Exit(0);
                        }
                        else if (key.ToLower() == ConsoleKey.N.ToString().ToLower())
                        {
                            break;
                        }

                        // скидання налаштувань
                        Console.ResetColor();
                    } while (true);

                    // повторення запиту
                    continue;
                    #endregion
                }

                // введене число
                int num = 0;
                // при натисканні однієї із доступних клавіш
                if (int.TryParse(key, out num))
                {
                    if (0 < num && num <= value.Length)
                    {
                        break;
                    }
                }

            } while (true);
        }

        /// <summary>
        /// Оплата замовлення
        /// </summary>
        private void Pay()
        {
            lock (block)
            {
                // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 4));

                // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                // Вивід
                Print("Спасибо за оплату. Нажмите клавишу для продолжения...", ConsoleColor.Green);
            }

            // очікування натискання клавіші
            Console.ReadKey(true);
        }

        /// <summary>
        /// Головне меню вибору
        /// </summary>
        private void MenuChange()
        {
            // Блокуємо доступ іншим потокам
            lock (block)
            {
                // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 4));

                // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(drinks.Count, aditivs.Count));

                #region Menu of change
                Print("Выберите: ", ConsoleColor.White);
                Print("N - очистить, ", ConsoleColor.Yellow);
                Print("G - стаканчик, ", ConsoleColor.Green);
                Print("D - напиток, ", ConsoleColor.Cyan);
                Print("A - добавка, ", ConsoleColor.Magenta);
                Print("Q - выход.", ConsoleColor.Red);

                // скидання налаштувань
                Console.ResetColor();
                #endregion
            }
        }

        /// <summary>
        /// Швидке виведеня в консоль деяких частин меню
        /// </summary>
        /// <param name="s">рядок</param>
        /// <param name="color">колір</param>
        private void Print(string s, ConsoleColor color)
        {
            // зміна кольору
            Console.ForegroundColor = color;
            // Вивід
            Console.Write(s);
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

                // якщо оновилася БД
                if (LoadDataBase.IsUpdateDB)
                {
                    // оновлюємо БД
                    ChangeDB.Invoke();
                    // оновлюємо меню
                    ChangeSize.Invoke();
                    // ставимо, що БД "застаріла"
                    LoadDataBase.IsUpdateDB = false;
                }
            }
        }

        /// <summary>
        /// Завантаження даних (таблиць продуктів)
        /// </summary>
        private void DownloadData()
        {
            // 
            lock (LoadDataBase.Block)
            {
                // завантаження даних з таблиць
                ExstractingData(LoadDataBase.Products.Tables["Drinks"], drinks);
                ExstractingData(LoadDataBase.Products.Tables["Additivs"], aditivs);
            }
        }

        /// <summary>
        /// Витягування даних
        /// </summary>
        /// <typeparam name="T">Тип вихідних даних</typeparam>
        /// <param name="table">Таблиця з даними</param>
        /// <returns></returns>
        private void ExstractingData<T>(DataTable table, List<T> products)
            where T : IProduct
        {
            // очищення списку
            products.Clear();

            try
            {
                // тимчасова змінна
                IProduct temp;

                // перебираємо всі рядки в таблиці
                foreach (DataRow row in table.Rows)
                {
                    // заносимо дані в колекції
                    if (typeof(T).Equals(typeof(IDrink)))
                    {
                        temp = new SDrink((int)row["ID"], (string)row["Name"],
                            Math.Abs((float)row["Price"]), Math.Abs((float)row["SizeOf"]),
                            ((bool)row["TypeOf"]).ConvertToTypeValue());

                    }
                    else if (typeof(T).Equals(typeof(IAdditiv)))
                    {
                        temp = new SAdditiv((int)row["ID"], (string)row["Name"],
                            Math.Abs((float)row["Price"]), Math.Abs((float)row["SizeOf"]),
                            ((bool)row["TypeOf"]).ConvertToTypeValue());
                    }
                    else
                    {
                        throw new Exception("This type is not valid");
                    }

                    products.Add((T)temp);
                }
            }
            catch (ArgumentException ex)
            {
                // сохранить в лог-файл
                Console.WriteLine(ex.Message);
            }
        }

    }
}
