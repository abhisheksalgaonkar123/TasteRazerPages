using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Taste.Pages
{
    public class IndexModel : PageModel
    {
       
        public IActionResult OnGet()
        {
           
            return RedirectToPage("./Customer/Home/Index");
        }
    }
}
