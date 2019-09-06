using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using App0.Product;
using App0.Calculate;

/// <summary>
/// Базовий додаток
/// </summary>
namespace App0
{
    class Program
    {
        static void Main()
        {
            // Join Unicode
            Console.OutputEncoding = Encoding.Unicode;

            // Заголовок програми
            Console.Title = "Fresh drinks";
#if true

            #region Установка культури
            // В умові вказані $ але можливо 
            // треба використовувати регіональну валюту - гривні
            RegionInfo region = RegionInfo.CurrentRegion;
            // встановити культури USA для вткористання доларів?
            if (true)
            {
                region = new RegionInfo("en-US");
            }
            #endregion

            #region Temp hided
            // назва файлу БД з можливим повним шляхом, але без розширення
            string path = string.Empty, // шлях до папки
                file = "Product";       // назва файлу

            // завантаження БД
            LoadDataBase.UpLoadDataBase(path + file);

            // якщо БД завантажена успошно запускаємо обробку запитів користувача
            if (LoadDataBase.Succesfull)
            {
#if true
                // Запуск інформатора який оновлюватиме БД
                Checker checker = new Checker(file);
#endif
                // запуск для тестування
                new Service(region).Menu();
            }
            #endregion

#endif

#if false
            Console.Clear();

            // тестування
            SOrder order = new SOrder
            {
                Glass = TypeOfGlass.ml110
            };

            Console.WriteLine(order.IsGlass);
            Console.WriteLine(order.Capacity);
            Console.WriteLine(order.Free);
            Console.WriteLine();

            for (int i = 0; i < 10; i++)
            {
                order.AddDrink(new SDrink(i, "Ad", 10, 10, TypeValue.Weight));
                order.AddAdditiv(new SAdditiv(8, "Dr", 1, 10, TypeValue.Volume));
                //Console.WriteLine(order.Volume());
                Console.WriteLine(order.Price);
            }

            order.Clear(); 
#endif
            //List<int> list = new List<int>();
            //list.Add(125);
            //list.Add(126);
            //list.Add(127);
            //list.Add(128);
            //list.Remove(124);

            // delay
            Console.ReadKey(true);
        }
    }
}
