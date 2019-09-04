using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App0.Product
{
    /// <summary>
    /// Структура напою
    /// </summary>
    struct SDrink : IDrink, IProduct
    {
        /// <summary>
        /// ID - напою
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// Назва напою
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Ціна напою
        /// </summary>
        public double Price { get; private set; }
        /// <summary>
        /// Тип в якому вимірюється напій
        /// </summary>
        public double Size { get; private set; }
        /// <summary>
        /// Величина виміру
        /// </summary>
        public TypeValue TypeOfValue { get; private set; }

        /// <summary>
        /// Додавання напою
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">назва</param>
        /// <param name="price">ціна</param>
        /// <param name="size">розмів виміру мг/мл</param>
        /// <param name="type">тип виміру вага/об'єм</param>
        public SDrink(int id, string name, double price,
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
