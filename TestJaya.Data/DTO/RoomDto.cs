using System;
using System.ComponentModel.DataAnnotations;

namespace TestJaya.Data.DTO
{
    [Serializable]
    public class RoomDto : IDto<Guid>
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El nombre es requerido"), StringLength(10, ErrorMessage = "El nombre no puede tener una longitud mayor a 10")]
        public string Name { get; set; }
        [Required(ErrorMessage = "La descripción es requerida"), StringLength(20, ErrorMessage = "La description no puede tener una longitud mayor a 20")]
        public string Description { get; set; }
    }
}