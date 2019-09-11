using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using App0.M.Product;
using App0.P;

namespace App0.V
{
    /// <summary>
    /// Вид, відображення
    /// </summary>
    class View
    {
        /// <summary>
        /// Менеджер
        /// </summary>
        private readonly Presenter presenter;

        /// <summary>
        /// Делегат на відслідковування зміни параметрів консолі
        /// </summary>
        public delegate void ChangeData();
        /// <summary>
        /// Подія яка відбувається при зміні розмірів вікна консолі
        /// </summary>
        private event ChangeData ChangeSize;
        /// <summary>
        /// При натисканні клавіші N
        /// </summary>
        public event ChangeData PressedN;
        /// <summary>
        /// При натисканні клавіші G
        /// </summary>
        public event ChangeData PressedG;
        /// <summary>
        /// При натисканні клавіші D
        /// </summary>
        public event ChangeData PressedD;
        /// <summary>
        /// При натисканні клавіші A
        /// </summary>
        public event ChangeData PressedA;
        /// <summary>
        /// При натисканні клавіші P
        /// </summary>
        public event ChangeData PressedP;

        /// <summary>
        /// Блокування для синхронного доступу до консолі
        /// </summary>
        public object Block { get; } = new object();
        /// <summary>
        /// Установка культури
        /// </summary>
        private RegionInfo Region
            => presenter.Region;
        /// <summary>
        /// Дані по замовленню, необхідні для лог-файла
        /// </summary>
        public string InfoOrder { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="presenter">Керівник</param>
        public View(Presenter presenter)
        {
            // підключення моделі і виду
            this.presenter = presenter;

            // запуск відображення меню
            new Thread(Menu).Start();

            // зв'язування подій
            ChangeSize += Menu; // оновлення меню

            // запуск відслідковування змін розмірів вікна
            new Thread(CheckerChangeWindow).Start();
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
            lock (Block)
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
                int height = 6 + Math.Max(presenter.Drinks.Count, presenter.Aditivs.Count);

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
            lock (Block)
            {
                // установка курсора
                Console.SetCursorPosition(2, 2);

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Green;

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
                    foreach (var i in presenter.Glasses)
                    {
                        glass.Append($" {j++}) " + i + ", ");
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
            lock (Block)
            {
                // установка курсора
                Console.SetCursorPosition(2, 6);

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Cyan;

                StringBuilder drink = new StringBuilder()
                    .Append("Напитки:");

                // Вивід
                Console.Write(drink.ToString());

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Yellow;

                foreach (var i in presenter.Drinks)
                {
                    // установка курсора
                    Console.SetCursorPosition(2, 7 + i.ID);
                    Console.Write($"  {i.ID}. {i.Name,-4} - " +
                        $"{i.Price.ToString("C2", new CultureInfo(Region.Name))} " +
                        $": {i.Size,3} {((i.TypeOfValue == TypeValue.Volume) ? "мл" : "г")}");
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
            lock (Block)
            {
                // установка курсора
                Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 6);

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Magenta;

                StringBuilder aditiv = new StringBuilder()
                    .Append("Добавки:");

                // Вивід
                Console.Write(aditiv.ToString());

                // зміна кольору
                Console.ForegroundColor = ConsoleColor.Yellow;

                foreach (var i in presenter.Aditivs)
                {
                    // установка курсора
                    Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 7 + i.ID);
                    Console.Write($"  {i.ID}. {i.Name,-6} - " +
                        $"{i.Price.ToString("C2", new CultureInfo(Region.Name))} " +
                        $": {i.Size,3} {((i.TypeOfValue == TypeValue.Volume) ? "мл" : "г")}");
                }

                // скидання налаштувань
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Головне меню вибору
        /// </summary>
        private void MenuChange()
        {
            // Блокуємо доступ іншим потокам
            lock (Block)
            {
                // установка курсора
                Console.SetCursorPosition(1, 17 + Math.Max(presenter.Drinks.Count, presenter.Aditivs.Count));

                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 2));

                // установка курсора
                Console.SetCursorPosition(2, 17 + Math.Max(presenter.Drinks.Count, presenter.Aditivs.Count));

                #region Menu of change
                Print("Меню: ", ConsoleColor.White);
                Print("N - очистить, ", ConsoleColor.Yellow);
                Print("G - стаканчик, ", ConsoleColor.Green);
                Print("D - напиток, ", ConsoleColor.Cyan);
                Print("A - добавка, ", ConsoleColor.Magenta);
                Print("Esc - выход. ", ConsoleColor.Red);

                // скидання налаштувань
                Console.ResetColor();
                #endregion
            }
        }

        /// <summary>
        /// Виведення замовлення клієнта
        /// </summary>
        public void Order()
        {
            // Примітка. В даному випадку, розташування "чека/замовлення" із вказаними
            // даними зручне/сприятливе. Якщо ж буде необхідно розширювати асортимент 
            // продуктів/замовлення, то необхідно або враховувати величину рядка
            // замовлення і нижню частину зміщувати, або ж перенести цей пункт після
            // блоку упралінням замовлення.

            // Блокуємо доступ іншим потокам
            lock (Block)
            {
                #region Продукти замовлення
                // установка курсора
                Console.SetCursorPosition(2, 11 + Math.Max(presenter.Drinks.Count, presenter.Aditivs.Count));
                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 4));

                // установка курсора
                Console.SetCursorPosition(2, 11 + Math.Max(presenter.Drinks.Count, presenter.Aditivs.Count));

                // якщо стаканчик вибрано то можна відображати замовлення
                if (presenter.Data.IsGlass)
                {
                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Вивід
                    Console.Write("Ваш заказ: ");

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Green;
                    // Вивід
                    {
                        int numGlass = Enum.GetValues(typeof(TypeOfGlass)).Cast<TypeOfGlass>()
                            .ToList().IndexOf(presenter.Data.Glass) + 1;
                        string s = $"G - {numGlass};";
                        // збереження даних для лог-файлу
                        InfoOrder = s;
                        Console.Write(s);
                    }

                    // Перевірка чи вибнано напій
                    if (presenter.Data.GetDrinks().Count > 0)
                    {
                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        // Вивід
                        {
                            string s = $" D - {presenter.Data.GetDrinks()[0].ID}x{presenter.Data.GetDrinks().Count};";
                            // збереження даних для лог-файлу
                            InfoOrder += s;
                            Console.Write(s);
                        }
                    }

                    // Перевірка чи вибнано добавки
                    if (presenter.Data.GetAdditivs().Count > 0)
                    {
                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.Magenta;

                        // Вивід
                        var s = new StringBuilder($" A -");

                        foreach (var i in presenter.Aditivs)
                        {
                            if (presenter.Data.GetAdditivs().Contains(i))
                            {
                                int num = presenter.Data.GetAdditivs().Where(t => t.ID == i.ID)
                                    .Select(t => t).ToList().Count;
                                s.Append($" {i.ID}x{num},");
                            }
                        }

                        // Вивід
                        {
                            string s1 = s.ToString().TrimEnd(',') + "; ";
                            // збереження даних для лог-файлу
                            InfoOrder += s1;
                            Console.Write(s1);
                        }
                    }

                    #region For MS Visual Studio 2019 and Windows 10
#if false
                    // Можна зробити умову, що можливість оплати відбувається лише при умові вибору напою
                    if (presenter.Data.GetDrinks().Count > 0)
                    {
                        // установка курсора
                        Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 11 + 
                            Math.Max(presenter.Drinks.Count, presenter.Aditivs.Count));

                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.White;
                        // Вивід
                        Console.Write("P - оптала;");
                    } 
#endif 
                    #endregion
                }
                #endregion

                // установка курсора
                Console.SetCursorPosition(2, 13 + Math.Max(presenter.Drinks.Count, presenter.Aditivs.Count));
                // очистка
                Console.Write(new string(' ', Console.WindowWidth - 4));

                // установка курсора
                Console.SetCursorPosition(2, 13 + Math.Max(presenter.Drinks.Count, presenter.Aditivs.Count));

                #region Ціна
                if (presenter.Data.IsGlass)
                {
                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Вивід
                    Console.Write("Стоимость: ");

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Green;
                    // Вивід
                    Console.Write($"{presenter.Data.Price.ToString("C2", new CultureInfo(Region.Name))}; ");

                    #region For MS Visual Studio 2015 and Windows 7
#if true
                    // Можна зробити умову, що можливість оплати відбувається лише при умові вибору напою
                    if (presenter.Data.GetDrinks().Count > 0)
                    {
                        // установка курсора (початку це було добре відображено на MS visual srudio v. 2019)
                        // але на v. 2015 - затираються дані на консолі
                        //Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 11 + Math.Max(drinks.Count, aditivs.Count));

                        // зміна кольору
                        Console.ForegroundColor = ConsoleColor.White;
                        // Вивід
                        Console.Write("P - оптала;");
                    }
#endif
                    #endregion
                }
                #endregion

                #region Об'єм
                if (presenter.Data.IsGlass)
                {
                    // установка курсора
                    Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 13 + 
                        Math.Max(presenter.Drinks.Count, presenter.Aditivs.Count));

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Вивід
                    Console.Write("Объем: ");

                    // зміна кольору
                    Console.ForegroundColor = ConsoleColor.Green;
                    // Вивід
                    Console.Write($"{presenter.Data.Volume()} мл");
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
            // Примітка. Блокувати не потрібно там де є очікування курсора, 
            // так як не буде можливості оновити меню коректно без артефактів

            // Обробка натискань
            while (true)
            {
                // Результат замовлення
                Order();

                // Головне меню вибору
                MenuChange();

                // клавіша вводу
                ConsoleKey? key = Console.ReadKey(true).Key;

                // дія згідно вибору
                switch (key)
                {
                    case ConsoleKey.N:  // очистить
                        if (presenter.Data.IsGlass)
                        {
                            PressedN.Invoke();
                        }
                        break;
                    case ConsoleKey.G:  // стаканчик
                        PressedG.Invoke();
                        break;
                    case ConsoleKey.D:  // напиток
                        if (presenter.Data.IsGlass)
                        {
                            PressedD.Invoke();
                        }
                        break;
                    case ConsoleKey.A:  // добавка
                        if (presenter.Data.IsGlass)
                        {
                            PressedA.Invoke();
                        }
                        break;
                    case ConsoleKey.P:  // оплата
                        if (presenter.Data.GetDrinks().Count  > 0)
                        {
                            PressedP.Invoke();
                        }
                        break;
                    case ConsoleKey.Escape:  // виход
                        Environment.Exit(0);
                        break;
                }
            }
        }

        /// <summary>
        /// Перевірка зміни розмірів консолі
        /// </summary>
        private void CheckerChangeWindow()
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
        /// Швидке виведеня в консоль деяких частин меню
        /// </summary>
        /// <param name="s">рядок</param>
        /// <param name="color">колір</param>
        public void Print(string s, ConsoleColor color)
        {
            // зміна кольору
            Console.ForegroundColor = color;
            // Вивід
            Console.Write(s);
        }

    }
}
