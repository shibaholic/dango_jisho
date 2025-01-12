using System.Diagnostics;
using Application;
using Application.Mappings.EntityDtos;
using Application.Mappings.EntityDtos.Tracking;
using Application.Response;
using Application.Services;
using Application.UseCaseCommands;
using Domain.Entities;
using Domain.Entities.JMDict;
using Domain.Entities.Tracking;
using Infrastructure;
using Infrastructure.DbContext;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Presentation;
using Presentation.ExceptionHandler;

namespace Program;

public class Program
{
    public static async Task Main(string[] args)
    {
        // BUILD
        var builder = WebApplication.CreateBuilder(args);
        
        builder.ConfigurePresentationServices();
        builder.Services.ConfigureApplicationServices();
        builder.Services.ConfigureInfrastructureServices(builder.Configuration);

        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                
                // var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                string? traceId = System.Diagnostics.Activity.Current?.TraceId.ToString();
                context.ProblemDetails.Extensions.TryAdd("traceId", traceId);
            };
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder => builder
                .WithOrigins(["http://192.168.86.100:5173", "http://localhost:5173"])
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
        });

        builder.Services.AddExceptionHandler<ProblemExceptionHandler>();
        
        // APP
        var app = builder.Build();
    
        app.UseCors();
        
        app.RunPresentationServices();
        app.CheckIfDatabaseCreated();
        await app.CheckEnsureCreated();

        app.UseExceptionHandler();
        
        Console.WriteLine("app.Run()");
        app.Run();
    }
}

public static class ApplicationExtensions
{
    public static void CheckIfDatabaseCreated(this WebApplication app)
    {
        var serviceScope = app.Services.CreateScope();
        var dataContext = serviceScope.ServiceProvider.GetService<MyDbContext>();
    
        if (dataContext!.CheckConnection())
        {
            Console.WriteLine("Database connected.");
        }
        else
        {
            Console.Error.WriteLine("Could not connect to database.");
            System.Environment.Exit(1);
        }
    }

    public static async Task CheckEnsureCreated(this WebApplication app)
    {
        var serviceScope = app.Services.CreateScope();
        var dataContext = serviceScope.ServiceProvider.GetService<MyDbContext>();
        
        if (app.Environment.IsDevelopment())
        {
            var deleteCreate = (string)app.Configuration.GetValue(typeof(string), "EFCORE_DELETECREATE");
            if (deleteCreate == "true")
            {
                Console.WriteLine("Database ensuring deleted and created...");
                dataContext.Database.EnsureDeleted();
                dataContext.Database.EnsureCreated();

                await SeedJMDictData(serviceScope);
                await SeedUserData(serviceScope);
                await SeedTrackingData(serviceScope);
            }
            else
            {
                Console.WriteLine("Database ensuring created...");
                dataContext.Database.EnsureCreated();
            }
            
        }
    }

    private static async Task SeedJMDictData(IServiceScope serviceScope)
    {
        var mediator = serviceScope.ServiceProvider.GetService<IMediator>();
        
        Console.WriteLine("Seeding with JMDict data...");
        string filePath = Path.Combine(AppContext.BaseDirectory, "SeedData", "JMdict_e.xml");
        if (!File.Exists(filePath))
        {
            Console.WriteLine("  Could not find seed data.");
            System.Environment.Exit(1);
        }
        var stopwatch = Stopwatch.StartNew();
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(filePath)));
        var request = new ImportJMdictRequest { Content = stream.ToArray() };
        stopwatch.Stop();
        Console.WriteLine($"  Loading JMdict from {filePath} took {stopwatch.ElapsedMilliseconds} ms.");
        stopwatch.Restart();
        var result = await mediator.Send(request, CancellationToken.None);
                
        if (!result.Successful)
        {
            Console.WriteLine("  Error while importing JMdict_e.xml.");
            System.Environment.Exit(1);
        }
        stopwatch.Stop();
        Console.WriteLine($"JMDict seeding took {stopwatch.ElapsedMilliseconds} ms.\n");
    }

    private static async Task SeedUserData(IServiceScope serviceScope)
    {
        Console.WriteLine("Seeding with User data...");
        
        var userCrud = serviceScope.ServiceProvider.GetService<ICrudService<User,User>>();

        if (userCrud == null)
        {
            Console.WriteLine("  Error while getting CrudService<User,User>");
            System.Environment.Exit(1);
        }

        var user = new User
        {
            Id = new Guid("faeb2480-fbdc-4921-868b-83bd93324099"), Username = "myuser", Password = "password",
            Email = "myuser@mail.com", IsAdmin = false
        };

        var result = await userCrud.CreateAsync(user);
        if (!result.Successful)
        {
            Console.WriteLine("  Error while seeding user.");
            System.Environment.Exit(1);
        }
        
        Console.WriteLine("Successfully seeded user.\n");
    }
    
    private static async Task SeedTrackingData(IServiceScope serviceScope)
    {
        Console.WriteLine("Seeding with Tracking data...");
        
        var tagCrud =     serviceScope.ServiceProvider.GetService<ICrudService<Tag,TagDto>>();
        var eitCrud =     serviceScope.ServiceProvider.GetService<ICrudService<EntryIsTagged,EntryIsTagged>>();
        var trackedCrud = serviceScope.ServiceProvider.GetService<ICrudService<TrackedEntry,TrackedEntry>>();
        var tissCrud =    serviceScope.ServiceProvider.GetService<ICrudService<TagInStudySet,TagInStudySet>>();
        var studyCrud =   serviceScope.ServiceProvider.GetService<ICrudService<StudySet,StudySetDto>>();

        if (tagCrud == null || eitCrud == null || trackedCrud == null || tissCrud == null || studyCrud == null)
        {
            Console.WriteLine("  Error while getting CrudService<,>(s)");
            System.Environment.Exit(1);
        }
        
        // Tag
        
        var tag = new Tag
        {
            Id = new Guid("f57c7dff-1d29-4e50-b18c-f0471694262d"), 
            UserId = new Guid("faeb2480-fbdc-4921-868b-83bd93324099"),
            Name = "MyTag"
        };
        var tag2 = new Tag
        {
            Id = new Guid("f57c7dff-1d29-4e50-b18c-f0471694262e"),
            UserId = new Guid("faeb2480-fbdc-4921-868b-83bd93324099"),
            Name = "MyTag2"
        };
        
        var tagResult = await tagCrud.CreateAsync(tag);
        var tag2Result = await tagCrud.CreateAsync(tag2);

        // list of ent_seq
        var ent_seqs_New = new List<string>
        {
            "1471130",
            "1317830",
            "1467550",
            "1511500",
            "1474050",
            "1309550",
            "1294540",
            "1786080",
            "1326600",
            "1386350",
            "1592380",
            "1470020",
            "1433840",
            "1481670"
        };
        
        var ent_seqs_Learning = new List<string>
        {
            "2740680",
            "2859672",
            "2172220",
            "1579310"
        };

        var ent_seqs_Review = new List<string>
        {
            "1421540",
            "1192700",
            "1502790",
            "1186760",
            "1379430"
        };

        var ent_seqs_Know = new List<string>
        {
            "1775870",
            "1412640",
            "1630050",
            "1792970",
            "1500100",
        };
        
        // TrackedEntry
        
        var trackedEntriesResults = new List<Response<TrackedEntry>>();
        
        foreach (var ent_seq in ent_seqs_New)
        {
            var trackedEntry = new TrackedEntry
            {
                ent_seq = ent_seq, UserId = tag.UserId, LevelStateType = LevelStateType.New
            };
            trackedEntriesResults.Add(await trackedCrud.CreateAsync(trackedEntry));
        }
        
        foreach (var ent_seq in ent_seqs_Learning)
        {
            var trackedEntry = new TrackedEntry
            {
                ent_seq = ent_seq, UserId = tag.UserId, LevelStateType = LevelStateType.Learning
            };
            trackedEntriesResults.Add(await trackedCrud.CreateAsync(trackedEntry));
        }
        
        var lastReviewDate = new DateTime(2025, 1, 1, 0, 0, 0);
        var nextReviewDays = 9;
        
        foreach (var ent_seq in ent_seqs_Review)
        {
            var trackedEntry = new TrackedEntry
            {
                ent_seq = ent_seq, UserId = tag.UserId, LevelStateType = LevelStateType.Reviewing, 
                LastReviewDate = lastReviewDate, NextReviewDays = nextReviewDays
            };
            trackedEntriesResults.Add(await trackedCrud.CreateAsync(trackedEntry));
        }

        foreach (var ent_seq in ent_seqs_Know)
        {
            var trackedEntry = new TrackedEntry
            {
                ent_seq = ent_seq, UserId = tag.UserId, LevelStateType = LevelStateType.Known, 
                LastReviewDate = lastReviewDate, NextReviewDays = nextReviewDays
            };
            trackedEntriesResults.Add(await trackedCrud.CreateAsync(trackedEntry));
        }
        
        // EntryIsTagged

        var entryIsTaggeds = new List<EntryIsTagged>();
        for (int i = 1; i <= 7; i++)
        {
            entryIsTaggeds.Add(new EntryIsTagged { TagId = tag.Id, ent_seq = ent_seqs_New[i - 1], UserId = tag.UserId, UserOrder = i });
        }
        for (int i = 1; i <= 7; i++)
        {
            entryIsTaggeds.Add(new EntryIsTagged { TagId = tag2.Id, ent_seq = ent_seqs_New[i + 6], UserId = tag.UserId, UserOrder = i });
        }

        int j = 8;
        foreach (var ent_seq in ent_seqs_Learning)
        {
            entryIsTaggeds.Add(new EntryIsTagged {TagId = tag.Id, ent_seq = ent_seq, UserId = tag.UserId, UserOrder = j++});
        }
        foreach (var ent_seq in ent_seqs_Review)
        {
            entryIsTaggeds.Add(new EntryIsTagged {TagId = tag.Id, ent_seq = ent_seq, UserId = tag.UserId, UserOrder = j++});
        }
        foreach (var ent_seq in ent_seqs_Know)
        {
            entryIsTaggeds.Add(new EntryIsTagged {TagId = tag.Id, ent_seq = ent_seq, UserId = tag.UserId, UserOrder = j++});
        }

        var entryIsTaggedResults = new List<Response<EntryIsTagged>>();

        foreach (var entryIsTagged in entryIsTaggeds)
        {
            entryIsTaggedResults.Add(await eitCrud.CreateAsync(entryIsTagged));
        }
        
        // StudySet

        var studySet = new StudySet
        {
            Id = new Guid("8fac6386-976e-4eb4-bd8e-ab0f9db9270a"), UserId = tag.UserId, LastStartDate = null,
            NewEntryGoal = 10, NewEntryCount = 0
        };
        
        var studySetResult = await studyCrud.CreateAsync(studySet);
        
        // TagInStudySet

        var tagInStudySets = new List<TagInStudySet>
        {
            new TagInStudySet { TagId = tag.Id, StudySetId = studySet.Id, Order = 1 },
            new TagInStudySet { TagId = tag2.Id, StudySetId = studySet.Id, Order = 2 }
        };
        
        var tagInStudySet1Result = await tissCrud.CreateAsync(tagInStudySets[0]);
        var tagInStudySet2Result = await tissCrud.CreateAsync(tagInStudySets[1]);
        
        if (!(tagResult.Successful && studySetResult.Successful && tagInStudySet1Result.Successful && tagInStudySet2Result.Successful) || 
            (entryIsTaggedResults.Any(eit => !eit.Successful) ||
                trackedEntriesResults.Any(te => !te.Successful)))
        {
            Console.WriteLine("  Error while seeding tracking data.");
            System.Environment.Exit(1);
        }
        
        Console.WriteLine("Successfully seeded tracking data.\n");
    }
}