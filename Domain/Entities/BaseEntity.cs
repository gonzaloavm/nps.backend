using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public abstract class BaseEntity<T>
    {
        public T Id { get; set; }
        public DateTime CreatedAt { get; protected set; } = DateTime.Now;
        public DateTime UpdatedAt { get; protected set; } = DateTime.Now;

        // Método para actualizar la fecha de modificación automáticamente
        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.Now;
        }
    }
}
