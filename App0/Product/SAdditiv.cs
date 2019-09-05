using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App0.Product
{
    /// <summary>
    /// Структура добавки
    /// </summary>
    struct SAdditiv : IAdditiv
    {
        /// <summary>
        /// ID - добавки
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// Назва добавки
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Ціна добавки
        /// </summary>
        public double Price { get; private set; }
        /// <summary>
        /// Тип в якому вимірюється добавка
        /// </summary>
        public double Size { get; private set; }
        /// <summary>
        /// Величина виміру
        /// </summary>
        public TypeValue TypeOfValue { get; private set; }

        /// <summary>
        /// Додавання добавки
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">назва</param>
        /// <param name="price">ціна</param>
        /// <param name="size">розмів виміру мл/</param>
        /// <param name="type">тип виміру вага/об'єм</param>
        public SAdditiv(int id, string name, double price,
            double size, TypeValue type)
        {
            this.ID = id;
            this.Name = name;
            this.Price = price;
            this.Size = size;
            this.TypeOfValue = type;
        }
    }
}
