namespace CNX.ToolWeb.Services;

/// <summary>M?t tab ?ang m? trên thanh tab.</summary>
public sealed class OpenTab
{
    public required string Path { get; init; }
    public required string Title { get; init; }
    public required string Icon { get; init; }
}

/// <summary>
/// Qu?n lý danh sách các tab ?ang m? c?a siêu Tool qu?n tr?.
/// M?i khi ?i?u h??ng t?i m?t menu, m?t tab t??ng ?ng ???c m? (ho?c kích ho?t n?u ?ã m?).
/// </summary>
public sealed class TabService
{
    private readonly List<OpenTab> _tabs = new();

    /// <summary>Danh sách tab ?ang m?.</summary>
    public IReadOnlyList<OpenTab> Tabs => _tabs;

    /// <summary>???ng d?n c?a tab ?ang ???c kích ho?t.</summary>
    public string? ActivePath { get; private set; }

    /// <summary>S? ki?n phát ra khi danh sách tab thay ??i.</summary>
    public event Action? OnChange;

    /// <summary>M? tab m?i ho?c kích ho?t tab ?ã t?n t?i theo ???ng d?n.</summary>
    public void Open(string path, string title, string icon)
    {
        var existing = _tabs.FirstOrDefault(t => t.Path == path);
        if (existing is null)
        {
            _tabs.Add(new OpenTab { Path = path, Title = title, Icon = icon });
        }

        ActivePath = path;
        OnChange?.Invoke();
    }

    /// <summary>?óng m?t tab. Tr? v? ???ng d?n c?n ?i?u h??ng t?i sau khi ?óng (null n?u không c?n).</summary>
    public string? Close(string path)
    {
        var index = _tabs.FindIndex(t => t.Path == path);
        if (index < 0)
        {
            return null;
        }

        _tabs.RemoveAt(index);

        string? navigateTo = null;
        if (ActivePath == path)
        {
            // Chuy?n sang tab k? bên (?u tiên tab tr??c ?ó).
            if (_tabs.Count > 0)
            {
                var newIndex = Math.Max(0, index - 1);
                navigateTo = _tabs[newIndex].Path;
                ActivePath = navigateTo;
            }
            else
            {
                ActivePath = null;
                navigateTo = "/";
            }
        }

        OnChange?.Invoke();
        return navigateTo;
    }

    /// <summary>?óng t?t c? các tab.</summary>
    public void CloseAll()
    {
        _tabs.Clear();
        ActivePath = null;
        OnChange?.Invoke();
    }
}
