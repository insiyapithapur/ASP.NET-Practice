using Microsoft.AspNetCore.Mvc;
using EmployeeMgt.Models;
using EmployeeMgt.Data;

namespace EmployeeMgt.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _repo;

        public EmployeeController(IEmployeeRepository repo)
        {
            _repo = repo;
        }

        // READ: List all employees
        public IActionResult Index()
        {
            var employees = _repo.GetAll();
            return View(employees);
        }

        // CREATE: Show Create Employee form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE: Save Employee data from the form
        [HttpPost]
        public IActionResult Create(Employee employee, IFormFile PhotoFile)
        {
            // Check if photo was uploaded
            if (PhotoFile == null || PhotoFile.Length == 0)
            {
                ModelState.AddModelError("PhotoFile", "Please upload a photo");
            }
            else
            {
                // Process the photo file
                var uploadsFolder = Path.Combine("wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Use a temp unique file name for now
                var tempFileName = Guid.NewGuid().ToString() + Path.GetExtension(PhotoFile.FileName);
                var tempFilePath = Path.Combine(uploadsFolder, tempFileName);

                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    PhotoFile.CopyTo(stream);
                }

                // Store the temp path initially
                employee.Photo = "/images/" + tempFileName;
                ModelState.Remove("Photo");
            }

            foreach (var state in ModelState)
            {
                Console.WriteLine($"Key: {state.Key}, Errors: {state.Value.Errors.Count}");
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"-- Error: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                // Save the employee to get the ID
                _repo.Add(employee);

                // Now that we have the ID, rename the photo file as Employee_ID
                if (!string.IsNullOrEmpty(employee.Photo))
                {
                    var uploadsFolder = Path.Combine("wwwroot", "images");
                    var oldFilePath = Path.Combine(uploadsFolder, Path.GetFileName(employee.Photo));
                    var fileExtension = Path.GetExtension(oldFilePath);
                    var newFileName = $"Employee_{employee.Id:D2}{fileExtension}";
                    var newFilePath = Path.Combine(uploadsFolder, newFileName);

                    // Rename the file
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Move(oldFilePath, newFilePath, true); // true to overwrite if exists

                        // Update the path in the employee record
                        employee.Photo = $"/images/{newFileName}";
                        _repo.Update(employee); // Update the record with the new path
                    }
                }

                return RedirectToAction("Index");
            }

            return View(employee);
        }


        // READ: Show Employee details
        public IActionResult Details(int id)
        {
            var employee = _repo.GetById(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // UPDATE: Show edit form to edit Employee details
        public IActionResult Edit(int id)
        {
            var employee = _repo.GetById(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // UPDATE: Save changes from edit form
        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _repo.Update(employee);
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // DELETE: Show Employee Details to Confirm delete
        public IActionResult Delete(int id)
        {
            var employee = _repo.GetById(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // DELETE: Perform deletion for employee
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {

            // First get the employee to access their photo path
            var employee = _repo.GetById(id);

            if (employee != null && !string.IsNullOrEmpty(employee.Photo))
            {
                // Get the physical file path from the stored photo path
                // employee.Photo is something like "/images/Employee_ID.jpg"
                string fileName = Path.GetFileName(employee.Photo);
                string filePath = Path.Combine("wwwroot", "images", fileName);

                // Delete the physical file if it exists
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                        Console.WriteLine($"Deleted photo file: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue with the deletion
                        Console.WriteLine($"Error deleting photo file: {ex.Message}");
                    }
                }
            }

            // Now delete the employee record from the database
            _repo.Delete(id);
            Console.WriteLine($"Deleted employee record");

            return RedirectToAction("Index");
        }
    }
}
