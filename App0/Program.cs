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
#if false

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
            Console.Clear();

            // тестування
            SOrder order = new SOrder();
            order.Glass = TypeOfGlass.ml110;

            Console.WriteLine(order.IsGlass);
            Console.WriteLine(order.Capacity);
            Console.WriteLine(order.Free);

            order.Drinks = new List<IDrink>()
            {
                new SDrink(1, "Dr", 5.5, 10, TypeValue.Volume),
                new SDrink(2, "Ad", 6.6, 11, TypeValue.Weight),
                new SDrink(3, "Ad", 6.6, 11, TypeValue.Weight),
                new SDrink(4, "Ad", 6.6, 11, TypeValue.Weight),
                new SDrink(5, "Ad", 6.6, 11, TypeValue.Weight),
                new SDrink(6, "Ad", 6.6, 11, TypeValue.Weight),

            };

            Console.WriteLine(order.Drinks?.Count);
            order.Additivs.Add(new SAdditiv(8, "Dr", 5.5, 10, TypeValue.Volume));

            order.Clear();

            order.Drinks.Add(new SDrink(7, "Ad", 5.5, 10, TypeValue.Volume));
            order.Drinks.Add(new SDrink(7, "Ad", 5.5, 10, TypeValue.Volume));
            order.Drinks.Add(new SDrink(8, "Dr", 5.5, 10, TypeValue.Volume));

            order.Additivs.Add(new SAdditiv(8, "Dr", 5.5, 10, TypeValue.Volume));
            order.Additivs.Add(new SAdditiv(8, "Dr", 5.5, 10, TypeValue.Volume));

            Console.WriteLine(order.Drinks?.Count);
            Console.WriteLine(order.Additivs?.Count);

            // delay
            Console.ReadKey(true);
        }
    }
}
