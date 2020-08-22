using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Taste.Models.ViewModels
{
    public class MenuItemVM 
    {
        public MenuItem MenuItem { get; set; }
        public IEnumerable<SelectListItem> CategoryType { get; set; }
        public IEnumerable<SelectListItem> FoodType { get; set; }
    }
}
