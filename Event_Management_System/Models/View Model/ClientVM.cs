using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Event_Management_System.Models.View_Model
{
    public class ClientVM
    {
        public int ClientId { get; set; }
        [Display(Name = "Client Name"), Required]
        public string ClientName { get; set; }
        [Display(Name = "Date of Birth"), Required, Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public string Picture { get; set; }
        public HttpPostedFileBase PictureFile { get; set; }
        [Display(Name = "Marital Status")]
        public bool MaritalStatus { get; set; }
        public List<int> EventList { get; set; } = new List<int>();
    }
}