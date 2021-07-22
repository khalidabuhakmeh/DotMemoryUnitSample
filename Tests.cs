using System.Linq;
using ConsoleTables;
using DotMemoryUnitSample.Models;
using JetBrains.dotMemoryUnit;
using JetBrains.dotMemoryUnit.Client.Interface;
using JetBrains.dotMemoryUnit.Properties;
using Xunit;
using Xunit.Abstractions;

namespace DotMemoryUnitSample
{
    public class Tests
    {
        private readonly ITestOutputHelper output;

        public Tests(ITestOutputHelper output)
        {
            this.output = output;
            DotMemoryUnitTestOutput.SetOutputMethod(output.WriteLine);
        }
        
        [Fact]
        public void Find_allocated_objects()
        {
            var things = new object[] { new Foo(), new Hoo(), new Goo() };

            dotMemory.Check(memory =>
            {
                var result = memory
                    .GetObjects(where => 
                        where.Type.IsInNamespace<Foo>());

                output.WriteLine(
                    ConsoleTable.From(
                        result
                        .GroupByType()
                        .Select(t => new
                        {
                            t.Type.Name,
                            t.ObjectsCount,
                            t.SizeInBytes
                        })
                    ).ToMarkDownString()); 
                
                Assert.Equal(3, result.ObjectsCount);
            });
        }
    }
    
    public static class TypePropertyExtensions
    {
        public static Query IsInNamespace<TType>(this TypeProperty where)
        {
            var type = typeof(TType);
            var types = type.Assembly.GetTypes()
                .Where(t => t.Namespace?.StartsWith(type.Namespace) == true)
                .ToArray();

            return where.Is(types);
        }
    }
}

namespace DotMemoryUnitSample.Models
{
    public class Foo
    {
    }

    public class Goo
    {
        
    }

    public class Hoo
    {
        
    }
}