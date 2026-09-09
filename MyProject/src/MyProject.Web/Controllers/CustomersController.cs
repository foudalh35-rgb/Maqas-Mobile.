using Microsoft.AspNetCore.Mvc;
using MyProject.Domain.Interfaces;

namespace MyProject.Web.Controllers;

public class CustomersController : Controller
{
    private readonly ICustomerRepository _customerRepository;

    public CustomersController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IActionResult> Index()
    {
        var customers = await _customerRepository.GetAllAsync();
        return View(customers);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return NotFound();
        return View(customer);
    }
}
