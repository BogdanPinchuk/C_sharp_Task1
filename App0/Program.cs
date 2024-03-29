﻿// Програма «App0» реалізована згідно із умовою ТЗ з деякими розширеннями, 
// які пробують симулювати/змоделювати роботу Кофе-автомата. Програма 
// запуститься якщо їй буде надано доступ до файла бази даних MS Access *.mdb.
// Який можна створити в напів-ручному режимі за допомогою «App0». Програма 
// виконує все, що вказано в умові ТЗ, а також додатково:
// - враховує, що користувач/замовник захоче використовувати не «$» грошову одиницю, 
// а валюту свого регіону – тобто враховано відображення у національній валюті.
// - для зручності клієнта, реалізовано консольний інтерфейс меню, в якому 
// відображається вся базова інформація, а в низу іде обробка запитів клієнта. 
// Також, в меню замовлення відображається об’єм – який вказує скільки вільного 
// місця лишилося в стаканчику відносно всього об’єму стаканчика.
// - в головному меню можна масштабувати (розтягувати) консольне вікно, 
// при цьому пропорції(до певних моментів) меню зберігаються.
// - реалізовані стаканчики для прикладу в перерахунках «enum», якщо вони 
// наприклад мають відповідати деякому стандарту, хоча це можна було 
// реалізувати в БД, але в умові ТЗ цього не було сказано.Також, якщо клієнт 
// зробив замовлення і вирішив додати ще щось а місця в стаканчику не достатньо, 
// то він може змінити стаканчик на більший із збереженням всіх «покупок», 
// аналогічно він може і змінити на стаканчик меншого розміру. Єдина умова 
// очищення всіх покупок, якщо об’єм «покупок» більший від стаканчика який 
// обирається (щоб не було переливу), в інших випадках дані зберігаються. 
// Реалізовано, що додавати покупки так щоб переповнити місце в стаканчику не можна.
// - реалізована «автономна» БД через DataSet, яка дозволяє скопіювати дані з 
// файлу в цей об’єкт.При цьому зникає необхідність постійно бути підключеним 
// до БД і постійно при кожному запиті звертатися до файла, що зменшує 
// швидкодію програми.Також завдяки цьому, реалізовано слідкування за зміною 
// файла БД і у випадку корегування самим користувачем чи його заміною на новий, 
// не потрібно зупиняти роботу самої програми, так як БД автоматично оновлюється 
// в іншому потоці. І у випадку оновлення встановлює прапор, який про це сигналізує. 
// І коли дані занесені в автономну БД і подано сигнал що вона оновилася, 
// інший потік автоматично оновлює меню яке бачить клієнт. Також завдяки цьому, 
// якщо програма вже запущена і нею користується клієнт, на роботу програми не 
// вплине видалення файла БД, до моменту доки не буде потрібно перезапустити програму.
// - щоб забезпечити умовно «безпеку перед законом», та можна було слідкувати 
// скільки і яких «продуктів» було замовлено або коли відбувалося оновлення БД – 
// реалізовано запис операцій оновлення і покупки в текстовий лог-файл, збереження 
// даних в якому відбувається автоматично.
// - через проблеми із самою консоллю, лише в головному меню реалізовано обробка 
// клавіш в підменю необхідно робити ввід і натискати Enter.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using App0.Calculate;
using App0.P;

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

            // назва файлу БД з можливим повним шляхом, але без розширення
            string path = string.Empty, // шлях до папки
                file = "Product";       // назва файлу

            // створення і запуск керівника
            new Presenter(path, file);

            // delay
            Console.ReadKey(true);
        }
    }
}
