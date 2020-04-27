using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    #region TodoController
    [Route("api/[controller]")]
    [Produces("application/json","application/xml")]
    [Consumes("application/json","application/xml")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }
        #endregion

        /// <summary>
        /// Obtener todos los TodoItems
        /// </summary>
        /// <returns>Lista de Todo Items</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        #region snippet_GetByID
        /// <summary>
        /// Obtener un TodoItem por su id
        /// </summary>
        /// <param name="id">El id del item que se quiere obtener</param>
        /// <returns>Una instancia de TodoItem</returns>
        /// <response code="200">Retorna el item solicitado</response>
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(TodoItem))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(TodoItem))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TodoItem))]
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }
        #endregion

        #region snippet_Update
        /// <summary>
        /// Actualizar un item
        /// </summary>
        /// <param name="id">El id del item que se quiere actualizar</param>
        /// <param name="todoItem">Item que se quiere actualizar</param>
        /// <remarks>Ejemplo de **body** del request para modificar item  
        /// 
        /// PUT /api/TodoItems/id
        /// { 
        ///     "id": 1, 
        ///     "name": "Bañar al perro", 
        ///     "isComplete": false 
        /// } 
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        #endregion

        #region snippet_Create
        /// <summary>
        /// Agregar un item
        /// </summary>
        /// <param name="todoItem">Item que se quiere insertar</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }
        #endregion

        #region snippet_Delete
        /// <summary>
        /// Borrar un item
        /// </summary>
        /// <param name="id">El id del item que se quiere obtener borrar</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return todoItem;
        }
        #endregion

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
