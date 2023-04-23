using System.Collections;

namespace MiniORM;

public class DbSet<TEntity> : ICollection<TEntity>
    where TEntity : class, new()
{
    internal DbSet(IEnumerable<TEntity> entities)
    {
        this.Entities = entities.ToList();
        this.ChangeTracker = new ChangeTracker<TEntity>(entities);
    }

    internal ChangeTracker<TEntity> ChangeTracker { get; set; }

    internal IList<TEntity> Entities { get; set; }

    public int Count => this.Entities.Count;

    public bool IsReadOnly => this.Entities.IsReadOnly;

    public void Add(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
        }

        this.Entities.Add(entity);
        this.ChangeTracker.Add(entity);
    }

    public void Clear()
    {
        while (this.Entities.Any())
        {
            TEntity entity = this.Entities.First();
            this.Remove(entity);
        }
    }

    public bool Contains(TEntity entity)
        => this.Entities.Contains(entity);

    public void CopyTo(TEntity[] array, int arrayIndex)
        => this.Entities.CopyTo(array, arrayIndex);

    public bool Remove(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
        }

        bool removedSuccessfully = this.Entities.Remove(entity);

        if (removedSuccessfully)
        {
            this.ChangeTracker.Remove(entity);
        }

        return removedSuccessfully;
    }

    public IEnumerator<TEntity> GetEnumerator() => this.Entities.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        foreach (TEntity entity in entities.ToArray())
        {
            this.Remove(entity);
        }
    }
}
