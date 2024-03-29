using System.Text;
using Extensions.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RMS.Models;
using RMS.Services;

namespace RMS.Controllers
{
    [Authorize(Roles = "Admin,Cashier")]
    public class PosController : Controller
    {
        private readonly ILogger<PosController> _logger;
        private readonly IProductionService _productionService;
        private readonly ISalesService _salesService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PosController(ILogger<PosController> logger,
        IProductionService productionService,
        ISalesService salesService,
        UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _productionService = productionService;
            _salesService = salesService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var order = HttpContext.Session.GetObjectFromJson<Order>("Order");
            if (order == null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return BadRequest("Error: Couldn't get the user.");

                if (user.StaffId == null)
                    return BadRequest("Error: Users that aren't staff members can't add orders.");

                var staff = _salesService.GetStaffById(user.StaffId ?? 0);

                order = new Order()
                {
                    StaffId = staff.StaffId,
                    StoreId = staff.StoreId
                };
            }

            var products = _productionService.GetProducts().ToList();

            ViewData["Products"] = new SelectList(products, "ProductId", "ProductName");
            ViewData["Prices"] = new SelectList(products, "ProductId", "ListPrice");

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Index(int ProductId, int Quantity, Decimal Discount)
        {
            var order = HttpContext.Session.GetObjectFromJson<Order>("Order");
            if (order == null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return BadRequest("Error: Couldn't get the user.");

                if (user.StaffId == null)
                    return BadRequest("Error: Users that aren't staff members can't add orders.");

                var staff = _salesService.GetStaffById(user.StaffId ?? 0);

                order = new Order()
                {
                    StaffId = staff.StaffId,
                    StoreId = staff.StoreId
                };
            }

            var orderItem = new OrderItem()
            {
                ItemId = order.OrderItems.Count(),
                ProductId = ProductId,
                Quantity = Quantity,
                Discount = Discount
            };

            order.OrderItems.Add(orderItem);
            HttpContext.Session.SetObjectASJson("Order", order);

            var products = _productionService.GetProducts().ToList();

            ViewData["Products"] = new SelectList(products, "ProductId", "ProductName");
            ViewData["Prices"] = new SelectList(products, "ProductId", "ListPrice");

            return View(order);
        }

        public IActionResult Edit(int Id)
        {
            var order = HttpContext.Session.GetObjectFromJson<Order>("Order");
            if (order == null)
                return RedirectToAction(nameof(Index));

            var item = order.OrderItems.Where(i => i.ItemId == Id).First();
            if (item == null)
                return RedirectToAction(nameof(Index));

            ViewData["ProductName"] = _productionService.GetProductById(item.ProductId).ProductName;
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(int Id, int Quantity, Decimal Discount)
        {
            var order = HttpContext.Session.GetObjectFromJson<Order>("Order");
            if (order == null)
                return RedirectToAction(nameof(Index));

            var item = order.OrderItems.Where(i => i.ItemId == Id).First();
            if (item == null)
                return RedirectToAction(nameof(Index));

            item.Quantity = Quantity;
            item.Discount = Discount;

            HttpContext.Session.SetObjectASJson("Order", order);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int Id)
        {
            var order = HttpContext.Session.GetObjectFromJson<Order>("Order");
            if (order == null)
                return RedirectToAction(nameof(Index));

            var item = order.OrderItems.Where(i => i.ItemId == Id).First();
            if (item == null)
                return RedirectToAction(nameof(Index));

            order.OrderItems.Remove(item);
            HttpContext.Session.SetObjectASJson("Order", order);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Discard()
        {
            var order = HttpContext.Session.GetObjectFromJson<Order>("Order");
            if (order == null)
                return RedirectToAction(nameof(Index));

            return View(order);
        }

        [HttpPost, ActionName("Discard")]
        public IActionResult DiscardComfirmed()
        {
            HttpContext.Session.SetObjectASJson("Order", null);

            return RedirectToAction(nameof(Index));
        }
    }
}