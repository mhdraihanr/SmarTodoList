using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Models;

namespace TodoApp.Api.Services;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAllAsync();
    IEnumerable<TodoItem> GetAll(); // Keep for backward compatibility
    Task<TodoItem?> GetAsync(int id);
    TodoItem? Get(int id); // Keep for backward compatibility
    Task<TodoItem> AddAsync(TodoItem item);
    TodoItem Add(TodoItem item); // Keep for backward compatibility
    Task<bool> UpdateAsync(TodoItem item);
    bool Update(TodoItem item); // Keep for backward compatibility
    Task<bool> DeleteAsync(int id);
    bool Delete(int id); // Keep for backward compatibility
    Task ClearAllAsync();
    void ClearAll(); // Keep for backward compatibility
}

public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<TodoItem> _items = new();
    private int _seq = 1;

    // Async methods
    public Task<IEnumerable<TodoItem>> GetAllAsync() => Task.FromResult(GetAll());
    public Task<TodoItem?> GetAsync(int id) => Task.FromResult(Get(id));
    public Task<TodoItem> AddAsync(TodoItem item) => Task.FromResult(Add(item));
    public Task<bool> UpdateAsync(TodoItem item) => Task.FromResult(Update(item));
    public Task<bool> DeleteAsync(int id) => Task.FromResult(Delete(id));
    public Task ClearAllAsync() { ClearAll(); return Task.CompletedTask; }

    // Sync methods (backward compatibility)
    public IEnumerable<TodoItem> GetAll() => _items.OrderByDescending(x => x.CreatedAt);
    public TodoItem? Get(int id) => _items.FirstOrDefault(x => x.Id == id);

    public TodoItem Add(TodoItem item)
    {
        item.Id = _seq++;
        _items.Add(item);
        return item;
    }

    public bool Update(TodoItem item)
    {
        var idx = _items.FindIndex(x => x.Id == item.Id);
        if (idx < 0) return false;
        _items[idx] = item;
        return true;
    }

    public bool Delete(int id) => _items.RemoveAll(x => x.Id == id) > 0;

    public void ClearAll() => _items.Clear();
}

public class DatabaseTodoRepository : ITodoRepository
{
    private readonly TodoDbContext _context;
    private readonly ILogger<DatabaseTodoRepository> _logger;

    public DatabaseTodoRepository(TodoDbContext context, ILogger<DatabaseTodoRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Async methods (primary)
    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        try
        {
            return await _context.Todos
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all todos");
            throw;
        }
    }

    public async Task<TodoItem?> GetAsync(int id)
    {
        try
        {
            return await _context.Todos.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting todo by id: {Id}", id);
            throw;
        }
    }

    public async Task<TodoItem> AddAsync(TodoItem item)
    {
        try
        {
            item.CreatedAt = DateTime.UtcNow;
            _context.Todos.Add(item);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created todo with id: {Id}", item.Id);
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo: {Title}", item.Title);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(TodoItem item)
    {
        try
        {
            var existingTodo = await _context.Todos.FindAsync(item.Id);
            if (existingTodo == null) return false;

            existingTodo.Title = item.Title;
            existingTodo.IsDone = item.IsDone;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated todo with id: {Id}", item.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo: {Id}", item.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) return false;

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted todo with id: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo: {Id}", id);
            throw;
        }
    }

    public async Task ClearAllAsync()
    {
        try
        {
            var allTodos = await _context.Todos.ToListAsync();
            _context.Todos.RemoveRange(allTodos);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleared all todos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing all todos");
            throw;
        }
    }

    // Sync methods (backward compatibility - runs async methods synchronously)
    public IEnumerable<TodoItem> GetAll() => GetAllAsync().GetAwaiter().GetResult();
    public TodoItem? Get(int id) => GetAsync(id).GetAwaiter().GetResult();
    public TodoItem Add(TodoItem item) => AddAsync(item).GetAwaiter().GetResult();
    public bool Update(TodoItem item) => UpdateAsync(item).GetAwaiter().GetResult();
    public bool Delete(int id) => DeleteAsync(id).GetAwaiter().GetResult();
    public void ClearAll() => ClearAllAsync().GetAwaiter().GetResult();
}
