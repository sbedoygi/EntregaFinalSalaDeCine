using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace Cine_Nauta.DAL.Entities
{
    public class Reservation : EntityCine
    {


        [Display(Name = "Fecha de Reserva")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime ReservationDate { get; set; }

        [Display(Name = "Cantidad de Asientos")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int SeatCount { get; set; }

        [Display(Name = "Función")]
        [JsonIgnore]
        public int FunctionId { get; set; }
        public Function Function { get; set; } // Relación con la función

        [Display(Name = "Precio Total")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public decimal TotalPrice { get; set; } // Precio total de la reserva

        // Relación con el usuario (una reserva pertenece a un usuario)
        [Display(Name = "Usuarios")]
        public User User { get; set; }


    }
}

