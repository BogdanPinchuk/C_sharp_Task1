using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using App0.M;
using App0.V;

namespace App0.P
{
    /// <summary>
    /// Менеджер
    /// </summary>
    class Presenter
    {
        /// <summary>
        /// Дані
        /// </summary>
        private readonly Model model;
        /// <summary>
        /// Вигляд
        /// </summary>
        private readonly View view;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">Адреса директорії в якій знаходиться БД</param>
        /// <param name="file">Назва файла БД без розширення</param>
        public Presenter(string path, string file)
        {
            // підключення моделі і виду
            this.model = new Model(path, file, Model.Currency.Dollar);
            this.view = new View();


        }

    }
}
