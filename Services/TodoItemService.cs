using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

public class TodoItemService
{
    private readonly TodoContext _context;

    public TodoItemService(TodoContext context)
    {
        _context = context;
    }

    public async Task<List<TodoItem>> GetTodoItemsAsync()
    {
        return await _context.TodoItems.ToListAsync();
    }

    public async Task<TodoItem> GetTodoItemAsync(long id)
    {
        return await _context.TodoItems.FindAsync(id);
    }

    public async Task<bool> UpdateTodoItemAsync(long id, TodoItem todoItem)
    {
        if (id != todoItem.Id)
        {
            return false;
        }

        _context.Entry(todoItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return !_context.TodoItems.Any(e => e.Id == id);
        }

        return true;
    }

    public async Task<TodoItem> AddTodoItemAsync(TodoItem todoItem)
    {
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();
        return todoItem;
    }

    public async Task<bool> DeleteTodoItemAsync(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return false;
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return true;
    }
}