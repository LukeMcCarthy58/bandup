using BandUp.Domain.Common;

namespace BandUp.UnitTests.Domain;

public class EntityTests
{
    private sealed class TestEntity : Entity;

    [Fact]
    public void New_entity_gets_a_time_ordered_uuid_v7()
    {
        var entity = new TestEntity();

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.Equal(7, entity.Id.Version);
    }

    [Fact]
    public void Entities_get_unique_ids()
    {
        Assert.NotEqual(new TestEntity().Id, new TestEntity().Id);
    }
}
