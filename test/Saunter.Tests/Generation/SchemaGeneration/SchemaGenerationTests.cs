using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Saunter.AsyncApiSchema.v2;
using Saunter.AsyncApiSchema.v2.Extensions;
using Saunter.Attributes;
using Saunter.Generation.SchemaGeneration;
using Shouldly;
using Xunit;

namespace Saunter.Tests.Generation.SchemaGeneration
{
    public class SchemaGenerationTests
    {
        private readonly ISchemaRepository _schemaRepository;
        private readonly SchemaGenerator _schemaGenerator;

        public SchemaGenerationTests()
        {
            _schemaRepository = new SchemaRepository();
            var options = new AsyncApiOptions();

            _schemaGenerator = new SchemaGenerator(Options.Create(options));
        }

        [Fact]
        public void GenerateSchema_GenerateSchemaFromTypeWithProperties_GeneratesSchemaCorrectly()
        {
            // Arrange
            var type = typeof(Foo);

            // Act
            var schema = _schemaGenerator.GenerateSchema(type, _schemaRepository);

            // Arrange
            schema.ShouldNotBeNull();
            _schemaRepository.Schemas.ShouldNotBeNull();

            _schemaRepository.Schemas.ContainsKey("foo").ShouldBeTrue();
            _schemaRepository.Schemas["foo"].GetType().ShouldBe(typeof(Schema));
            var fooSchema = _schemaRepository.Schemas["foo"] as Schema;
            fooSchema.Required.Count.ShouldBe(1);
            fooSchema.Required.Contains("id").ShouldBeTrue();
            fooSchema.Properties.Count.ShouldBe(5);
            fooSchema.Properties.ContainsKey("id").ShouldBeTrue();
            fooSchema.Properties.ContainsKey("bar").ShouldBeTrue();
            fooSchema.Properties.ContainsKey("fooType").ShouldBeTrue();
            fooSchema.Properties.ContainsKey("hello").ShouldBeTrue();
            fooSchema.Properties.ContainsKey("world").ShouldBeTrue();

            _schemaRepository.Schemas.ContainsKey("bar").ShouldBeTrue();
            _schemaRepository.Schemas["bar"].GetType().ShouldBe(typeof(Schema));
            var barSchema = _schemaRepository.Schemas["bar"] as Schema;
            barSchema.Properties.Count.ShouldBe(2);
            barSchema.Properties.ContainsKey("name").ShouldBeTrue();
            barSchema.Properties.ContainsKey("cost").ShouldBeTrue();
        }

        [Fact]
        public void GenerateSchema_GenerateSchemaFromTypeWithFields_GeneratesSchemaCorrectly()
        {
            // Arrange
            var type = typeof(Book);

            // Act
            var schema = _schemaGenerator.GenerateSchema(type, _schemaRepository);

            // Arrange
            schema.ShouldNotBeNull();
            _schemaRepository.Schemas.ShouldNotBeNull();

            _schemaRepository.Schemas.ContainsKey("book").ShouldBeTrue();
            _schemaRepository.Schemas["book"].GetType().ShouldBe(typeof(Schema));
            var bookSchema = _schemaRepository.Schemas["book"] as Schema;
            bookSchema.Properties.Count.ShouldBe(4);

            _schemaRepository.Schemas.ContainsKey("foo").ShouldBeTrue();
            _schemaRepository.Schemas["foo"].GetType().ShouldBe(typeof(Schema));
            var fooSchema = _schemaRepository.Schemas["foo"] as Schema;
            fooSchema.Required.Count.ShouldBe(1);
            fooSchema.Required.Contains("id").ShouldBeTrue();
            fooSchema.Properties.Count.ShouldBe(5);
            fooSchema.Properties.ContainsKey("id").ShouldBeTrue();
            fooSchema.Properties.ContainsKey("bar").ShouldBeTrue();
            fooSchema.Properties.ContainsKey("fooType").ShouldBeTrue();
            fooSchema.Properties.ContainsKey("hello").ShouldBeTrue();
            fooSchema.Properties.ContainsKey("world").ShouldBeTrue();
        }

        [Fact]
        public void GenerateSchema_GenerateSchemaFromClassWithDiscriminator_GeneratesSchemaCorrectly()
        {
            // Arrange
            var type = typeof(Pet);

            // Act
            var schema = _schemaGenerator.GenerateSchema(type, _schemaRepository);

            // Assert
            schema.ShouldNotBeNull();
            _schemaRepository.Schemas.ShouldNotBeNull();

            _schemaRepository.Schemas.ContainsKey("pet").ShouldBeTrue();
            _schemaRepository.Schemas["pet"].GetType().ShouldBe(typeof(Schema));
            var petSchema = _schemaRepository.Schemas["pet"] as Schema;
            petSchema.Discriminator.ShouldBe("petType");
            petSchema.OneOf.Count().ShouldBe(2);

            _schemaRepository.Schemas.ContainsKey("cat").ShouldBeTrue();
            _schemaRepository.Schemas["cat"].GetType().ShouldBe(typeof(Schema));
            var catSchema = _schemaRepository.Schemas["cat"] as Schema;
            catSchema.Properties.Count.ShouldBe(3);
            catSchema.Properties.ContainsKey("petType").ShouldBeTrue();
            catSchema.Properties.ContainsKey("name").ShouldBeTrue();
            catSchema.Properties.ContainsKey("huntingSkill").ShouldBeTrue();

            _schemaRepository.Schemas.ContainsKey("dog").ShouldBeTrue();
            _schemaRepository.Schemas["dog"].GetType().ShouldBe(typeof(Schema));
            var dogSchema = _schemaRepository.Schemas["dog"] as Schema;
            dogSchema.Properties.Count.ShouldBe(3);
            dogSchema.Properties.ContainsKey("petType").ShouldBeTrue();
            dogSchema.Properties.ContainsKey("name").ShouldBeTrue();
            dogSchema.Properties.ContainsKey("packSize").ShouldBeTrue();
        }

        [Fact]
        public void GenerateSchema_GenerateSchemaFromInterfaceWithDiscriminator_GeneratesSchemaCorrectly()
        {
            // Arrange
            var type = typeof(IPet);

            // Act
            var schema = _schemaGenerator.GenerateSchema(type, _schemaRepository);

            // Assert
            schema.ShouldNotBeNull();
            _schemaRepository.Schemas.ShouldNotBeNull();

            _schemaRepository.Schemas.ContainsKey("iPet").ShouldBeTrue();
            _schemaRepository.Schemas["iPet"].GetType().ShouldBe(typeof(Schema));
            var petSchema = _schemaRepository.Schemas["iPet"] as Schema;
            petSchema.Discriminator.ShouldBe("petType");
            petSchema.OneOf.Count().ShouldBe(2);

            _schemaRepository.Schemas.ContainsKey("cat").ShouldBeTrue();
            _schemaRepository.Schemas["cat"].GetType().ShouldBe(typeof(Schema));
            var catSchema = _schemaRepository.Schemas["cat"] as Schema;
            catSchema.Properties.Count.ShouldBe(3);
            catSchema.Properties.ContainsKey("petType").ShouldBeTrue();
            catSchema.Properties.ContainsKey("name").ShouldBeTrue();
            catSchema.Properties.ContainsKey("huntingSkill").ShouldBeTrue();

            _schemaRepository.Schemas.ContainsKey("dog").ShouldBeTrue();
            _schemaRepository.Schemas["dog"].GetType().ShouldBe(typeof(Schema));
            var dogSchema = _schemaRepository.Schemas["dog"] as Schema;
            dogSchema.Properties.Count.ShouldBe(3);
            dogSchema.Properties.ContainsKey("petType").ShouldBeTrue();
            dogSchema.Properties.ContainsKey("name").ShouldBeTrue();
            dogSchema.Properties.ContainsKey("packSize").ShouldBeTrue();
        }

        [Fact]
        public void GenerateSchema_GenerateSchemaWithDiscriminatorMappings_GeneratesSchemaCorrectly()
        {
            // Arrange
            var type = typeof(Pet);
            var options = new AsyncApiOptions { EnableDiscriminatorMappings = true };
            var schemaGenerator = new SchemaGenerator(Options.Create(options));

            // Act
            var schema = schemaGenerator.GenerateSchema(type, _schemaRepository);

            // Assert
            schema.ShouldNotBeNull();
            _schemaRepository.Schemas.ShouldNotBeNull();

            _schemaRepository.Schemas.ContainsKey("pet").ShouldBeTrue();
            _schemaRepository.Schemas["pet"].GetType().ShouldBe(typeof(ExtendedSchema));
            var petSchema = _schemaRepository.Schemas["pet"] as ExtendedSchema;
            petSchema.Discriminator.PropertyName.ShouldBe("petType");
            petSchema.Discriminator.Mapping.Count.ShouldBe(2);
            petSchema.Discriminator.Mapping.ShouldContainKey("Cat");
            petSchema.Discriminator.Mapping.ShouldContainKey("Dog");
            petSchema.OneOf.Count().ShouldBe(2);

            _schemaRepository.Schemas.ContainsKey("cat").ShouldBeTrue();
            _schemaRepository.Schemas["cat"].GetType().ShouldBe(typeof(Schema));
            var catSchema = _schemaRepository.Schemas["cat"] as Schema;
            catSchema.Properties.Count.ShouldBe(3);
            catSchema.Properties.ContainsKey("petType").ShouldBeTrue();
            catSchema.Properties.ContainsKey("name").ShouldBeTrue();
            catSchema.Properties.ContainsKey("huntingSkill").ShouldBeTrue();

            _schemaRepository.Schemas.ContainsKey("dog").ShouldBeTrue();
            _schemaRepository.Schemas["dog"].GetType().ShouldBe(typeof(Schema));
            var dogSchema = _schemaRepository.Schemas["dog"] as Schema;
            dogSchema.Properties.Count.ShouldBe(3);
            dogSchema.Properties.ContainsKey("petType").ShouldBeTrue();
            dogSchema.Properties.ContainsKey("name").ShouldBeTrue();
            dogSchema.Properties.ContainsKey("packSize").ShouldBeTrue();
        }
    }

    public class Foo
    {
        [Required]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string Ignore { get; set; }

        public Bar Bar { get; set; }

        [JsonPropertyName("hello")]
        public string HelloWorld { get; set; }

        [DataMember(Name = "myworld")]
        public string World { get; set; }

        public FooType FooType { get; set; }
    }

    public enum FooType
    {
        Foo,
        Bar
    }

    public class Bar
    {
        public string Name { get; set; }

        public decimal? Cost { get; set; }
    }

    public class Book
    {
        public readonly string Name;

        public readonly string Author;

        public readonly int NumberOfPages;

        public readonly Foo Foo;

        public Book(string name, string author, int numberOfPages, Foo foo)
        {
            Author = author;
            Name = name;
            NumberOfPages = numberOfPages;
            Foo = foo;
        }
    }

    [Discriminator("petType")]
    [DiscriminatorSubType(typeof(Cat))]
    [DiscriminatorSubType(typeof(Dog))]
    public interface IPet
    {
        string PetType { get; }

        string Name { get; }
    }

    [Discriminator("petType")]
    [DiscriminatorSubType(typeof(Cat), DiscriminatorValue = nameof(Cat))]
    [DiscriminatorSubType(typeof(Dog), DiscriminatorValue = nameof(Dog))]
    public abstract class Pet : IPet
    {
        public string PetType { get; set; }

        public string Name { get; set; }
    }

    public class Cat : Pet
    {
        public string HuntingSkill { get; set; }
    }

    public class Dog : Pet
    {
        public string PackSize { get; set; }
    }
}