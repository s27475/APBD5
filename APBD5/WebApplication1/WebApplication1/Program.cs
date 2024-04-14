using WebApplication1.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var animals = new List<Animal>()
{
    new Animal{Id = 1, Name = "Bydlak", Category = Category.Dog, Weight = 50.1, Color = "black"},
    new Animal{Id = 2, Name = "Mieczyslaw", Category = Category.Cat, Weight = 6.39 , Color = "grey"},
    new Animal{Id = 3, Name = "Gruchot", Category = Category.Bird, Weight = 1.2, Color = "white"},
    new Animal{Id = 4, Name = "Borowik", Category = Category.Turtle, Weight = 5.9, Color = "green"}
    
};

var visits = new List<Visit>()
{
    new Visit { Date = new DateTime(2020, 3, 1), AnimalId = 1, Description = "szlifowanie zebow", Price = 100 },
    new Visit { Date = new DateTime(2020, 3, 2), AnimalId = 2, Description = "odrobaczenie", Price = 50 },
    new Visit { Date = new DateTime(2020, 3, 2), AnimalId = 3, Description = "leczenie skrzydla", Price = 200 },
    new Visit { Date = new DateTime(2020, 3, 3), AnimalId = 4, Description = "leczenie skorupy", Price = 149.99 }
};

app.MapGet("/api/animals", () => Results.Ok(animals))
    .WithName("GetAnimals")
    .WithOpenApi();

app.MapGet("/api/animals/{id:int}", (int id) =>
    {
        Animal animal = animals.FirstOrDefault(a => a.Id == id);
        return animal == null ? Results.NotFound($"Animal with id {id} was not found") : Results.Ok(animal);
    })
    .WithName("GetAnimal")
    .WithOpenApi();

app.MapPost("/api/animals", (Animal animal) =>
    {
        animals.Add(animal);
        return Results.StatusCode(StatusCodes.Status201Created);
    })
    .WithName("AddAnimal")
    .WithOpenApi();

app.MapPut("/api/animals/{id:int}", (int id, Animal update) =>
    {
        var animal = animals.FirstOrDefault(a => a.Id == id);
        if (animal == null)
        {
            return Results.NotFound($"Animal with id {id} was not found");
        }

        animals.Remove(animal);
        animals.Add(update);
        return Results.StatusCode(StatusCodes.Status200OK);
    })
    .WithName("UpdateAnimal")
    .WithOpenApi();

app.MapDelete("/api/animals/{id:int}", (int id) =>
    {
        Animal deletedAnimal = animals.FirstOrDefault(a => a.Id == id);
        if (deletedAnimal == null)
        {
            return Results.NotFound($"Animal with id {id} was not found");
        }

        visits.RemoveAll(v => v.AnimalId == id);
        animals.Remove(deletedAnimal);
        return Results.StatusCode(StatusCodes.Status200OK);
    })
    .WithName("DeleteAnimal")
    .WithOpenApi();

app.MapGet("/api/animals/{id:int}/visits", (int id) =>
    {
        var animalVisits = visits.Where(v => v.AnimalId == id).ToList();
        if (!animalVisits.Any())
        {
            return Results.NotFound($"Animal visits with id {id} were not found");
        }

        return Results.Ok(animalVisits);
    })
    .WithName("GetVisit")
    .WithOpenApi();

app.MapPost("/api/animals/{id}/visits", (int id, Visit visit) =>
    {
        var animal = animals.FirstOrDefault(a => a.Id == id);
        if (animal == null)
        {
            return Results.NotFound($"Animal with id {id} was not found");
        }

        visit.AnimalId = id;
        visits.Add(visit);
        return Results.StatusCode(StatusCodes.Status201Created);
    })
    .WithName("AddVisit")
    .WithOpenApi();

app.Run();

