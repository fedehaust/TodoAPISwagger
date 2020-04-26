using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    /// <summary>
    /// Un item a completar con id, nombre y estado
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// Id del item
        /// </summary>
        [Required]
        public long Id { get; set; }
        /// <summary>
        /// El nombre que va a posser nuestro item
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        /// <summary>
        /// Indicador de si la tarea esta compleatada o no
        /// </summary>
        [Required]
        public bool IsComplete { get; set; }
    }
}