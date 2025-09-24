using TodoApp.Api.Models;

namespace TodoApp.Api.Services;

public interface ITodoRepository
{
    IEnumerable<TodoItem> GetAll();
    TodoItem? Get(int id);
    TodoItem Add(TodoItem item);
    bool Update(TodoItem item);
    bool Delete(int id);
    void ClearAll();
}

public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<TodoItem> _items = new();
    private int _seq = 1;

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
