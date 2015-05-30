using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Fun.BAL
{
    public class Category
    {
        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression("([0-9]+)",ErrorMessage="CategoryID請輸入數字!")]
        [DisplayName("CategoryID")]      
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("CategoryName")]
        [StringLength(15,ErrorMessage="CategoryName長度限制:{2}~{1}個字",MinimumLength=2)]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Description")]
        [StringLength(10,ErrorMessage="Description長度限制:{2}~{1}個字",MinimumLength=2)]
        public string Description { get; set; }

        [DisplayName("Picture")]
        public Image Picture { get; set; }
    }
}