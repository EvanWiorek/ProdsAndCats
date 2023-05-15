using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProdsAndCats.Models;
using Microsoft.EntityFrameworkCore;
namespace ProdsAndCats.Controllers;

public class HomeController : Controller
{
  private readonly ILogger<HomeController> _logger;
  private MyContext _context;
  public HomeController(ILogger<HomeController> logger, MyContext context)
  {
    _logger = logger;
    _context = context;
  }

  // * Product Routes
  [HttpGet("")]
  public IActionResult Products()
  {
    ViewBag.AllProducts = _context.Products.ToList();
    return View();
  }

  [HttpPost("products/create")]
  public IActionResult CreateProduct(Product newProduct)
  {
    if (ModelState.IsValid)
    {
      _context.Add(newProduct);
      _context.SaveChanges();
      return RedirectToAction("Products");
    }
    ViewBag.AllProducts = _context.Products.ToList();
    return View("Products");
  }

  [HttpGet("products/{ProductId}")]
  public IActionResult AddToProduct(int ProductId)
  {
    ViewBag.OneProduct = _context.Products.Include(assoc => assoc.AssoCategories).ThenInclude(assoc => assoc.Category).FirstOrDefault(prod => prod.ProductId == ProductId);

    ViewBag.FilteredCategories = _context.Categories.Include(cat => cat.AssoProducts).Where(cat => cat.AssoProducts.All(assoc => assoc.ProductId != ProductId)).ToList();
    return View();
  }

  // * Category Routes
  [HttpGet("categories")]
  public IActionResult Categories()
  {
    ViewBag.AllCategories = _context.Categories.ToList();
    return View();
  }

  [HttpPost("Categories/create")]
  public IActionResult CreateCategory(Category newCategory)
  {
    if (ModelState.IsValid)
    {
      _context.Add(newCategory);
      _context.SaveChanges();
      return RedirectToAction("Categories");
    }
    ViewBag.AllCategories = _context.Categories.ToList();
    return View("Categories");
  }

  [HttpGet("categories/{CategoryId}")]
  public IActionResult AddToCategory(int CategoryId)
  {
    ViewBag.OneCategory = _context.Categories.Include(assoc => assoc.AssoProducts).ThenInclude(assoc => assoc.Product).FirstOrDefault(cat => cat.CategoryId == CategoryId);

    ViewBag.FilteredProducts = _context.Products.Include(prod => prod.AssoCategories).Where(prod => prod.AssoCategories.All(assoc => assoc.CategoryId != CategoryId)).ToList();
    return View();
  }

  // * Association Routes
  [HttpPost("association/create/toproduct")]
  public IActionResult CreateAssociationToProduct(Association newAssoc)
  {
    if (newAssoc?.ProductId != null && newAssoc?.CategoryId != null)
    {
      _context.Add(newAssoc);
      _context.SaveChanges();
      ViewBag.AllProducts = _context.Products.ToList();
      return RedirectToAction("AddToProduct", new { newAssoc.ProductId });
    }
    return View("AddToProduct");
  }

  [HttpPost("association/create/tocategory")]
  public IActionResult CreateAssociationToCategory(Association newAssoc)
  {
    if (newAssoc?.ProductId != null && newAssoc?.CategoryId != null)
    {
      _context.Add(newAssoc);
      _context.SaveChanges();
      ViewBag.AllCategories = _context.Categories.ToList();
      return RedirectToAction("AddToCategory", new { newAssoc.CategoryId });
    }
    return View("AddToCategory");
  }

  [HttpPost("assocciation/{AssocId}/destroy")]
  public IActionResult DeleteAssociation(int AssocId, string navRoute)
  {
    Association? AssocToDelete = _context.Associations.SingleOrDefault(assoc => assoc.AssociationId == AssocId);
    _context.Associations.Remove(AssocToDelete);
    _context.SaveChanges();
    if (navRoute == "ToProduct")
    {
      return RedirectToAction("AddToProduct", new { AssocToDelete.ProductId });
    }
    else
    {
      return RedirectToAction("AddToCategory", new { AssocToDelete.CategoryId });
    }
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }
}