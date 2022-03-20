using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Builders
{
    /// <summary>
    /// C#で型安全なBuilderパターン
    /// https://yuchiki1000yen.hatenablog.com/entry/2018/07/31/112317
    /// </summary>
    internal class BuildSample
    {
        internal void Creator()
        {
            var person = PersonBuilder.Create().Id(1).Name("ss").Build();
            var person2 = PersonBuilder.Create().Id(1).Name("test").Age(4).Build();
        }
    }

    internal static class PersonBuilder
    {
        public static Builder<None, None, None> Create() =>
            new Builder<None, None, None>(default(int), default(string), default(int?));

        public static Builder<Some, TName, TAge> Id<TName, TAge>(this Builder<None, TName, TAge> builder, int id)
            where TName : IOpt where TAge : IOpt =>
            new Builder<Some, TName, TAge>(id, builder.Name, builder.Age);

        public static Builder<TId, Some, TAge> Name<TId, TAge>(this Builder<TId, None, TAge> builder, string name)
            where TId : IOpt where TAge : IOpt =>
            new Builder<TId, Some, TAge>(builder.Id, name, builder.Age);

        public static Builder<TId, TName, Some> Age<TId, TName>(this Builder<TId, TName, None> builder, int age)
            where TId : IOpt where TName : IOpt =>
            new Builder<TId, TName, Some>(builder.Id, builder.Name, age);

        public static Person Build<TAge>(this Builder<Some, Some, TAge> builder) where TAge : IOpt =>
            new Person(builder.Id, builder.Name, builder.Age);
    }

    internal class Builder<TId, TName, TAge> where TId : IOpt where TName : IOpt where TAge : IOpt
    {
        internal readonly int Id;
        internal readonly string? Name;
        internal readonly int? Age;

        internal Builder(int id, string name, int? age) => (Id, Name, Age) = (id, name, age);
    }

    internal interface IOpt { }

    internal abstract class None : IOpt { }

    internal abstract class Some : IOpt { }

    internal class Person
    {
        private readonly int _id;
        private readonly string _name;
        private readonly int? _age;

        public Person(int id, string name, int? age) => (_id, _name, _age) = (id, name, age);

    }
}
