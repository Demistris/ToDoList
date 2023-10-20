using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ToDoList.Controllers
{
    public class ToDoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToDoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ToDoes
        public IActionResult Index()
        {
            //return _context.ToDos != null ? 
            //            View(await _context.ToDos.ToListAsync()) :
            //            Problem("Entity set 'ApplicationDbContext.ToDos'  is null.");

            return View();
        }

        public async Task<IEnumerable<ToDo>> GetMyToDoesAsync()
        {
            IEnumerable<ToDo> myToDoes = await _context.ToDos.ToListAsync();

            int completeCount = 0;

            foreach (ToDo toDo in myToDoes)
            {
                if (toDo.IsChecked)
                {
                    completeCount++;
                }
            }

            if(myToDoes.Count() > 0)
            {
                ViewBag.Percent = Math.Round(100f * ((float)completeCount / (float)myToDoes.Count()));
            }
            else
            {
                ViewBag.Percent = 0f;
            }

            var sortedTodoItems = myToDoes.OrderBy(item => item.IsChecked).ToList();

            return sortedTodoItems;
        }

        public async Task<IActionResult> BuildToDoTable()
        {
            return _context.ToDos != null ?
                        PartialView("_ToDoTable", await GetMyToDoesAsync()) :
                        Problem("Entity set 'ApplicationDbContext.ToDos'  is null.");
        }

        // GET: ToDoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ToDos == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDo == null)
            {
                return NotFound();
            }

            return View(toDo);
        }

        // GET: ToDoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ToDoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,IsChecked")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(toDo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(toDo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AJAXCreate([Bind("Id,Description")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                toDo.IsChecked = false;
                toDo.Order = toDo.Id;
                _context.Add(toDo);
                await _context.SaveChangesAsync();
            }

            return _context.ToDos != null ?
                        PartialView("_ToDoTable", await GetMyToDoesAsync()) :
                        Problem("Entity set 'ApplicationDbContext.ToDos'  is null.");
        }

        // GET: ToDoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ToDos == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }
            return View(toDo);
        }

        // POST: ToDoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,IsChecked")] ToDo toDo)
        {
            if (id != toDo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoExists(toDo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(toDo);
        }

        [HttpPost]
        public async Task<IActionResult> AJAXEdit(int? id, bool value)
        {
            if (id == null || _context.ToDos == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos.FindAsync(id);
            
            if (toDo == null || id != toDo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    toDo.IsChecked = value;
                    _context.Update(toDo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoExists(toDo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return _context.ToDos != null ?
                        PartialView("_ToDoTable", await GetMyToDoesAsync()) :
                        Problem("Entity set 'ApplicationDbContext.ToDos'  is null.");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDescription(int? id, string description)
        {
            if (id == null || _context.ToDos == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos.FindAsync(id);

            if (toDo == null || id != toDo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    toDo.Description = description;
                    _context.Update(toDo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoExists(toDo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return _context.ToDos != null ?
                        PartialView("_ToDoTable", await GetMyToDoesAsync()) :
                        Problem("Entity set 'ApplicationDbContext.ToDos'  is null.");
        }

        // GET: ToDoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ToDos == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDo == null)
            {
                return NotFound();
            }

            return View(toDo);
        }

        // POST: ToDoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ToDos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ToDos'  is null.");
            }
            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo != null)
            {
                _context.ToDos.Remove(toDo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoExists(int id)
        {
          return (_context.ToDos?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // POST: ToDoes/AJAXDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AJAXDelete(int id)
        {
            if (_context.ToDos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ToDos'  is null.");
            }

            var toDo = await _context.ToDos.FindAsync(id);

            if (toDo != null)
            {
                _context.ToDos.Remove(toDo);
                await _context.SaveChangesAsync();
            }

            return _context.ToDos != null ?
                        PartialView("_ToDoTable", await GetMyToDoesAsync()) :
                        Problem("Entity set 'ApplicationDbContext.ToDos'  is null.");
        }
    }
}