﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.API.Controllers
{

    [Microsoft.AspNetCore.Mvc.Route("api/controller")]
    [ApiController]
    public class TodoController : Controller
    {
        private readonly TodoDbContext _todoDbContext;
        
        public TodoController(TodoDbContext todoDbContext)
        {
            _todoDbContext = todoDbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllTodos()
        {
            var todos = await _todoDbContext.Todos
                .Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();


            return Ok(todos);
        }


        [HttpGet]
        [Route("get-deleted-todos")]

        public async Task<IActionResult> GetDeletedTodos()
        {
            var todos = await _todoDbContext.Todos
                .Where (x => x.IsDeleted == true)
                .OrderByDescending (x => x.CreatedDate)
                .ToListAsync();

            return Ok(todos);

        }

        [HttpPost]

        public async Task<IActionResult> AddTodo(Todo todo)
        {
            todo.Id = Guid.NewGuid();

            _todoDbContext.Todos.Add(todo);
            await _todoDbContext.SaveChangesAsync();

            return Ok(todo);

        }
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateTodo([FromRoute] Guid id, Todo todoUpdateRequest)
        {
            var todo = await _todoDbContext.Todos.FindAsync(id);

            if (todo == null)
                return NotFound();

            // Convert local DateTime to UTC (if not null)
            if (todoUpdateRequest.CompletedDate.HasValue)
            {
                todo.IsCompleted = todoUpdateRequest.IsCompleted;
                todo.CompletedDate = todoUpdateRequest.CompletedDate.Value.ToUniversalTime();
            }

            await _todoDbContext.SaveChangesAsync();

            return Ok(todo);
        }


        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteTodo([FromRoute] Guid id)
        {
            var todo = await _todoDbContext.Todos.FindAsync(id);

            if (todo == null) return NotFound();

            todo.IsDeleted = true;
            todo.DeletedDate = DateTime.UtcNow; // Use UtcNow instead of Now

            await _todoDbContext.SaveChangesAsync();

            return Ok(todo);
        }

        [HttpPut]
        [Route("undo-deleted-todo/{id:Guid}")]
        public async Task<IActionResult> undoDeletedTodo([FromRoute] Guid id, Todo undoDeleteTodoRequest)
        {
            var todo = await _todoDbContext.Todos.FindAsync(id);

            if (todo == null) return NotFound();

            todo.DeletedDate=null;
            todo.IsDeleted=false;

            await _todoDbContext.SaveChangesAsync();    

            return Ok(todo);
        }

    }
}
