using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RolnikowePole.Models
{
    public class Wiadomosc
    {
        public int WiadomoscId { get; set; }

        // AspNetUsers
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }

        // SenderId of the message
        [Required]
        [ForeignKey("Sender")]
        [Display(Name = "Sender")]
        //  [InverseProperty("MessageSenderId")]
        public string SenderId { get; set; }

        // ReceiverId of this message
        [Required]
        [ForeignKey("Receiver")]
        [Display(Name = "Receiver")]
        //   [InverseProperty("MessageReceiverId")]
        public string ReceiverId { get; set; }

        [Required]
        [StringLength(150)]
        public string Body { get; set; }

        public DateTime DateAndTimeOfSend { get; set; }

        public bool Read { get; set; }

}
}